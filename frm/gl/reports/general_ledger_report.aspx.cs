using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Drawing;

public partial class general_ledger_report : System.Web.UI.Page
{
    private readonly string connStr = WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultDates();
            LoadReport();
        }
    }

    private void SetDefaultDates()
    {
        int year = DateTime.Now.Year;
        if (DateTime.Now.Month < 7) year--;

        txtFromDate.Text = new DateTime(year, 7, 1).ToString("yyyy-MM-dd");
        txtToDate.Text = new DateTime(year + 1, 6, 30).ToString("yyyy-MM-dd");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadReport();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }


    private void LoadReport()
    {
        try
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);

            lblFromDate.Text = fromDate.ToString("dd-MM-yyyy");
            lblToDate.Text = toDate.ToString("dd-MM-yyyy");

            string fromAccount = txtFromAccount.Text.Trim();
            string toAccount = txtToAccount.Text.Trim();

            string dbFromAccount = ConvertToDbFormat(fromAccount);
            string dbToAccount = ConvertToDbFormat(toAccount);

            if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
            {
                lblAccountRange.Text = " | ACCOUNT: " + fromAccount + " TO " + toAccount;
            }
            else
            {
                lblAccountRange.Text = "";
            }

            DataTable dt = GetLedgerData(fromDate, toDate, dbFromAccount, dbToAccount);

            if (dt.Rows.Count > 0)
            {
                // Calculate totals
                decimal totalDebit = 0;
                decimal totalCredit = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["DEBIT"]);
                    totalCredit += Convert.ToDecimal(row["CREDIT"]);
                }

                decimal finalBalance = totalDebit - totalCredit;
                string finalDrCr = finalBalance >= 0 ? "DR" : "CR";
                string finalBalanceText = Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr;

                // Add Total Row to DataTable
                DataRow totalRow = dt.NewRow();
                totalRow["GL_CODE"] = "";
                totalRow["GL_DESCRP"] = "";
                totalRow["BOOK_TYPE"] = "";
                totalRow["GL_FORM_NUMBER"] = 0;
                totalRow["VOUCHER_DATE"] = DBNull.Value;
                totalRow["NARATION"] = "TOTAL";
                totalRow["CHEQUE_NUMBER"] = "";
                totalRow["BILL_NUMBER"] = "";
                totalRow["OPENING_BALANCE"] = 0;
                totalRow["DEBIT"] = totalDebit;
                totalRow["CREDIT"] = totalCredit;
                totalRow["RUNNING_BALANCE"] = finalBalance;

                dt.Rows.Add(totalRow);

                gvReport.DataSource = dt;
                gvReport.DataBind();
                lblStatus.Text = "Report loaded successfully. Total rows: " + (dt.Rows.Count - 1);
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "No data found for selected criteria.";
                lblStatus.ForeColor = Color.Red;
                gvReport.DataSource = null;
                gvReport.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
        }
    }

    private string ConvertToDbFormat(string account)
    {
        if (string.IsNullOrEmpty(account))
            return account;

        if (account.StartsWith("111"))
        {
            return "11" + account.Substring(3);
        }

        return account;
    }

    private DataTable GetLedgerData(DateTime fromDate, DateTime toDate, string fromAccount, string toAccount)
    {
        string sql = @"
            SELECT 
                GL_CODE,
                GL_DESCRP,
                BOOK_TYPE,
                GL_FORM_NUMBER,
                VOUCHER_DATE,
                NARATION,
                CHEQUE_NUMBER,
                BILL_NUMBER,
                OPENING_BALANCE,
                DEBIT,
                CREDIT,
                RUNNING_BALANCE
            FROM TMP_RPT_GL_LEDGER
            WHERE VOUCHER_DATE BETWEEN :FROM_DATE AND :TO_DATE
        ";

        if (!string.IsNullOrEmpty(fromAccount) && !string.IsNullOrEmpty(toAccount))
        {
            sql += " AND GL_CODE >= :FROM_ACCOUNT AND GL_CODE <= :TO_ACCOUNT";
        }
        else if (!string.IsNullOrEmpty(fromAccount))
        {
            sql += " AND GL_CODE >= :FROM_ACCOUNT";
        }
        else if (!string.IsNullOrEmpty(toAccount))
        {
            sql += " AND GL_CODE <= :TO_ACCOUNT";
        }

        sql += " ORDER BY GL_CODE, VOUCHER_DATE, GL_FORM_NUMBER";

        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, conn))
        {
            cmd.Parameters.Add(":FROM_DATE", OracleDbType.Date).Value = fromDate;
            cmd.Parameters.Add(":TO_DATE", OracleDbType.Date).Value = toDate;

            if (!string.IsNullOrEmpty(fromAccount))
            {
                cmd.Parameters.Add(":FROM_ACCOUNT", OracleDbType.Varchar2).Value = fromAccount;
            }
            if (!string.IsNullOrEmpty(toAccount))
            {
                cmd.Parameters.Add(":TO_ACCOUNT", OracleDbType.Varchar2).Value = toAccount;
            }

            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        return dt;
    }

    protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView row = (DataRowView)e.Row.DataItem;

            // Check if this is the TOTAL row
            string narration = row["NARATION"].ToString();
            bool isTotalRow = (narration == "TOTAL");

            if (isTotalRow)
            {
                e.Row.CssClass = "total-row";

                // Clear ALL cells first
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = "";
                }

                // Set TOTAL text in the first cell (VOUCHER # column)
                e.Row.Cells[0].Text = "TOTAL";
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;

                // Get values
                decimal totalDebitVal = Convert.ToDecimal(row["DEBIT"]);
                decimal totalCreditVal = Convert.ToDecimal(row["CREDIT"]);
                decimal totalBalanceVal = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                string totalDrCr = totalBalanceVal >= 0 ? "DR" : "CR";
                string totalBalanceText = Math.Abs(totalBalanceVal).ToString("N2") + " " + totalDrCr;

                // Set Debit in the DEBIT column (index 9)
                e.Row.Cells[9].Text = totalDebitVal.ToString("N2");
                e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;

                // Set Credit in the CREDIT column (index 10)
                e.Row.Cells[10].Text = totalCreditVal.ToString("N2");
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;

                // Set Balance in the RUNNING BALANCE column (index 11)
                e.Row.Cells[11].Text = totalBalanceText;
                e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Right;

                if (totalDrCr == "CR")
                {
                    e.Row.Cells[11].ForeColor = Color.Red;
                }

                return;
            }

            // Format regular rows
            // Format Date (column index 4)
            DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
            e.Row.Cells[4].Text = voucherDate.ToString("dd-MM-yyyy");

            // Format Opening Balance (column index 8)
            decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
            string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
            e.Row.Cells[8].Text = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;
            e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;

            // Format Debit (column index 9)
            decimal debitAmt = Convert.ToDecimal(row["DEBIT"]);
            e.Row.Cells[9].Text = debitAmt == 0 ? "" : debitAmt.ToString("N2");
            e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;

            // Format Credit (column index 10)
            decimal creditAmt = Convert.ToDecimal(row["CREDIT"]);
            e.Row.Cells[10].Text = creditAmt == 0 ? "" : creditAmt.ToString("N2");
            e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;

            // Format Running Balance (column index 11)
            decimal runningBalanceAmt = Convert.ToDecimal(row["RUNNING_BALANCE"]);
            string runningDrCr = runningBalanceAmt >= 0 ? "DR" : "CR";
            string runningBalanceText = Math.Abs(runningBalanceAmt).ToString("N2") + " " + runningDrCr;

            Label lblBalance = (Label)e.Row.FindControl("lblRunningBalance");
            if (lblBalance != null)
            {
                lblBalance.Text = runningBalanceText;
                if (runningDrCr == "CR")
                {
                    lblBalance.ForeColor = Color.Red;
                    lblBalance.Font.Bold = true;
                }
                else
                {
                    lblBalance.ForeColor = Color.Black;
                }
            }
        }
    }
}