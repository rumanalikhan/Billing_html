using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text;
using System.Web.UI;

public partial class general_ledger_report : System.Web.UI.Page
{
    private readonly string connStr = WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultDates();
            LoadReport();
            lblReportDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
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

    // Excel Export Method
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string fromAccount = txtFromAccount.Text.Trim();
            string toAccount = txtToAccount.Text.Trim();

            string dbFromAccount = ConvertToDbFormat(fromAccount);
            string dbToAccount = ConvertToDbFormat(toAccount);

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

                // Get Pakistan time
                TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename=GeneralLedger_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
                Response.Charset = "";

                System.IO.StringWriter sw = new System.IO.StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                // Write HTML header
                hw.Write("<html><head><meta charset='UTF-8'><title>General Ledger Report</title>");
                hw.Write("<style>");
                hw.Write("td { font-family: 'Segoe UI', Arial, sans-serif; }");
                hw.Write(".total-row { background-color: #dcdcdc; font-weight: bold; }");
                hw.Write(".text-right { text-align: right; }");
                hw.Write("th { background-color: #0f7c57; color: white; }");
                hw.Write("table { border-collapse: collapse; width: 100%; }");
                hw.Write("th, td { border: 1px solid #000; padding: 5px; }");
                hw.Write("</style></head><body>");

                // Company Header
                hw.Write("<div style='text-align:center; margin-bottom:20px;'>");
                hw.Write("<h2>BAHRIA TOWN KARACHI</h2>");
                hw.Write("<p>GL ACCOUNTING SYSTEM</p>");
                hw.Write("<h3>GENERAL LEDGER REPORT</h3>");
                hw.Write("<p>Printed on: " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt") + "</p>");
                hw.Write("<p>FROM: " + fromDate.ToString("dd-MM-yyyy") + " TO: " + toDate.ToString("dd-MM-yyyy") + "</p>");
                if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
                {
                    hw.Write("<p>ACCOUNT: " + fromAccount + " TO " + toAccount + "</p>");
                }
                hw.Write("</div>");

                // Build table
                hw.Write("<table>");
                hw.Write("<thead>");
                hw.Write("<tr>");
                hw.Write("<th>GL CODE</th>");
                hw.Write("<th>GL DESCRIPTION</th>");
                hw.Write("<th>BOOK TYPE</th>");
                hw.Write("<th>GL FORM</th>");
                hw.Write("<th>VOUCHER DATE</th>");
                hw.Write("<th>NARATION</th>");
                hw.Write("<th>CHEQUE NO</th>");
                hw.Write("<th>BILL NO</th>");
                hw.Write("<th>OPENING</th>");
                hw.Write("<th>DEBIT</th>");
                hw.Write("<th>CREDIT</th>");
                hw.Write("<th>RUNNING BALANCE</th>");
                hw.Write("</tr>");
                hw.Write("</thead>");
                hw.Write("<tbody>");

                foreach (DataRow row in dt.Rows)
                {
                    string glCode = row["GL_CODE"].ToString();
                    string glDesc = row["GL_DESCRP"].ToString();
                    string bookType = row["BOOK_TYPE"].ToString();
                    string glForm = row["GL_FORM_NUMBER"].ToString();
                    DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
                    string narration = row["NARATION"].ToString();
                    string chequeNo = row["CHEQUE_NUMBER"].ToString();
                    string billNo = row["BILL_NUMBER"].ToString();

                    decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
                    string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
                    string openingText = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;

                    decimal debit = Convert.ToDecimal(row["DEBIT"]);
                    decimal credit = Convert.ToDecimal(row["CREDIT"]);

                    decimal runningBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                    string runningDrCr = runningBalance >= 0 ? "DR" : "CR";
                    string runningText = Math.Abs(runningBalance).ToString("N2") + " " + runningDrCr;

                    hw.Write("<tr>");
                    hw.Write("<td>" + glCode + "</td>");
                    hw.Write("<td>" + glDesc + "</td>");
                    hw.Write("<td>" + bookType + "</td>");
                    hw.Write("<td>" + glForm + "</td>");
                    hw.Write("<td>" + voucherDate.ToString("dd-MM-yyyy") + "</td>");
                    hw.Write("<td>" + narration + "</td>");
                    hw.Write("<td>" + chequeNo + "</td>");
                    hw.Write("<td>" + billNo + "</td>");
                    hw.Write("<td style='text-align:right;'>" + openingText + "</td>");
                    hw.Write("<td style='text-align:right;'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
                    hw.Write("<td style='text-align:right;'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</td>");

                    if (runningDrCr == "CR")
                    {
                        hw.Write("<td style='text-align:right;color:red;font-weight:bold;'>" + runningText + "</td>");
                    }
                    else
                    {
                        hw.Write("<td style='text-align:right;'>" + runningText + "</td>");
                    }
                    hw.Write("</tr>");
                }

                // Add Total Row
                hw.Write("<tr style='background-color:#dcdcdc;font-weight:bold;'>");
                hw.Write("<td colspan='9' style='text-align:left;'>TOTAL</td>");
                hw.Write("<td style='text-align:right;'>" + totalDebit.ToString("N2") + "</td>");
                hw.Write("<td style='text-align:right;'>" + totalCredit.ToString("N2") + "</td>");

                if (finalDrCr == "CR")
                {
                    hw.Write("<td style='text-align:right;color:red;'>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</td>");
                }
                else
                {
                    hw.Write("<td style='text-align:right;'>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</td>");
                }
                hw.Write("</tr>");

                hw.Write("</tbody>");
                hw.Write("</table>");

                hw.Write("<div style='margin-top:30px; text-align:center; font-size:10px; color:#999;'>");
                hw.Write("This is a computer generated document - No signature required");
                hw.Write("</div>");
                hw.Write("</body></html>");

                Response.Write(sw.ToString());
                Response.End();
            }
            else
            {
                lblStatus.Text = "No data to export.";
                lblStatus.ForeColor = Color.Red;
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error exporting to Excel: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
        }
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
                decimal totalDebit = 0;
                decimal totalCredit = 0;

                foreach (DataRow row in dt.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["DEBIT"]);
                    totalCredit += Convert.ToDecimal(row["CREDIT"]);
                }

                decimal finalBalance = totalDebit - totalCredit;

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

            string narration = row["NARATION"].ToString();
            bool isTotalRow = (narration == "TOTAL");

            if (isTotalRow)
            {
                e.Row.CssClass = "total-row";

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Text = "";
                }

                e.Row.Cells[0].Text = "TOTAL";
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;

                decimal totalDebitVal = Convert.ToDecimal(row["DEBIT"]);
                decimal totalCreditVal = Convert.ToDecimal(row["CREDIT"]);
                decimal totalBalanceVal = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                string totalDrCr = totalBalanceVal >= 0 ? "DR" : "CR";
                string totalBalanceText = Math.Abs(totalBalanceVal).ToString("N2") + " " + totalDrCr;

                e.Row.Cells[9].Text = totalDebitVal.ToString("N2");
                e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[10].Text = totalCreditVal.ToString("N2");
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;

                e.Row.Cells[11].Text = totalBalanceText;
                e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Right;

                if (totalDrCr == "CR")
                {
                    e.Row.Cells[11].ForeColor = Color.Red;
                }

                return;
            }

            DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
            e.Row.Cells[4].Text = voucherDate.ToString("dd-MM-yyyy");

            decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
            string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
            e.Row.Cells[8].Text = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;
            e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;

            decimal debitAmt = Convert.ToDecimal(row["DEBIT"]);
            e.Row.Cells[9].Text = debitAmt == 0 ? "" : debitAmt.ToString("N2");
            e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;

            decimal creditAmt = Convert.ToDecimal(row["CREDIT"]);
            e.Row.Cells[10].Text = creditAmt == 0 ? "" : creditAmt.ToString("N2");
            e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;

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

    public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    {
        // Required for Excel export
    }
}