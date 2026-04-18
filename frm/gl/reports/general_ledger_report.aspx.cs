using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Drawing;
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

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string fromAccount = txtFromAccount.Text.Trim();
            string toAccount = txtToAccount.Text.Trim();
            string postingStatus = ddlPostingStatus.SelectedValue;

            string dbFromAccount = ConvertToDbFormat(fromAccount);
            string dbToAccount = ConvertToDbFormat(toAccount);

            DataTable dt = GetLedgerData(fromDate, toDate, dbFromAccount, dbToAccount, postingStatus);

            if (dt.Rows.Count > 0)
            {
                decimal totalDebit = 0, totalCredit = 0;
                foreach (DataRow row in dt.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["DEBIT"]);
                    totalCredit += Convert.ToDecimal(row["CREDIT"]);
                }
                decimal finalBalance = totalDebit - totalCredit;
                string finalDrCr = finalBalance >= 0 ? "DR" : "CR";

                TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename=GeneralLedger_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
                Response.Charset = "";

                System.IO.StringWriter sw = new System.IO.StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                hw.Write("<html><head><meta charset='UTF-8'><title>General Ledger Report</title>");
                hw.Write("<style>td { font-family: 'Segoe UI', Arial, sans-serif; } .total-row { background-color: #dcdcdc; font-weight: bold; } .text-right { text-align: right; } th { background-color: #0f7c57; color: white; } table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #000; padding: 5px; }</style></head><body>");

                hw.Write("<div style='text-align:center; margin-bottom:20px;'>");
                hw.Write("<h2>BAHRIA TOWN KARACHI</h2><p>GL ACCOUNTING SYSTEM</p><h3>GENERAL LEDGER REPORT</h3>");
                hw.Write("<p>Printed on: " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt") + "</p>");
                hw.Write("<p>FROM: " + fromDate.ToString("dd-MM-yyyy") + " TO: " + toDate.ToString("dd-MM-yyyy") + "</p>");
                if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
                    hw.Write("<p>ACCOUNT: " + fromAccount + " TO " + toAccount + "</p>");
                hw.Write("<p>Posting Status: " + ddlPostingStatus.SelectedItem.Text + "</p></div>");

                hw.Write("<table><thead><tr><th>GL CODE</th><th>GL DESCRIPTION</th><th>BOOK TYPE</th><th>GL FORM</th><th>VOUCHER DATE</th><th>NARATION</th><th>CHEQUE NO</th><th>BILL NO</th><th>OPENING</th><th>DEBIT</th><th>CREDIT</th><th>RUNNING BALANCE</th></tr></thead><tbody>");

                foreach (DataRow row in dt.Rows)
                {
                    DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
                    decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
                    string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
                    string openingText = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;
                    decimal debit = Convert.ToDecimal(row["DEBIT"]);
                    decimal credit = Convert.ToDecimal(row["CREDIT"]);
                    decimal runningBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                    string runningDrCr = runningBalance >= 0 ? "DR" : "CR";
                    string runningText = Math.Abs(runningBalance).ToString("N2") + " " + runningDrCr;

                    hw.Write("<tr>");
                    hw.Write("<td>" + row["GL_CODE"] + "</td>");
                    hw.Write("<td>" + row["GL_DESCRP"] + "</td>");
                    hw.Write("<td>" + row["BOOK_TYPE"] + "</td>");
                    hw.Write("<td>" + row["GL_FORM_NUMBER"] + "</td>");
                    hw.Write("<td>" + voucherDate.ToString("dd-MM-yyyy") + "</td>");
                    hw.Write("<td>" + row["NARATION"] + "</td>");
                    hw.Write("<td>" + row["CHEQUE_NUMBER"] + "</td>");
                    hw.Write("<td>" + row["BILL_NUMBER"] + "</td>");
                    hw.Write("<td style='text-align:right;'>" + openingText + "</td>");
                    hw.Write("<td style='text-align:right;'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
                    hw.Write("<td style='text-align:right;'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</td>");
                    if (runningDrCr == "CR")
                        hw.Write("<td style='text-align:right;color:red;font-weight:bold;'>" + runningText + "</td>");
                    else
                        hw.Write("<td style='text-align:right;'>" + runningText + "</td>");
                    hw.Write("</tr>");
                }

                hw.Write("<tr style='background-color:#dcdcdc;font-weight:bold;'><td colspan='9' style='text-align:left;'>TOTAL</td>");
                hw.Write("<td style='text-align:right;'>" + totalDebit.ToString("N2") + "</td>");
                hw.Write("<td style='text-align:right;'>" + totalCredit.ToString("N2") + "</td>");
                if (finalDrCr == "CR")
                    hw.Write("<td style='text-align:right;color:red;'>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</td>");
                else
                    hw.Write("<td style='text-align:right;'>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</td>");
                hw.Write("</tr></tbody></table>");

                hw.Write("<div style='margin-top:30px; text-align:center; font-size:10px; color:#999;'>This is a computer generated document - No signature required</div></body></html>");
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
            string postingStatus = ddlPostingStatus.SelectedValue;

            string dbFromAccount = ConvertToDbFormat(fromAccount);
            string dbToAccount = ConvertToDbFormat(toAccount);

            if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
                lblAccountRange.Text = " | ACCOUNT: " + fromAccount + " TO " + toAccount;
            else
                lblAccountRange.Text = "";

            DataTable dt = GetLedgerData(fromDate, toDate, dbFromAccount, dbToAccount, postingStatus);

            if (dt.Rows.Count > 0)
            {
                decimal totalDebit = 0, totalCredit = 0;
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
        if (string.IsNullOrEmpty(account)) return account;
        if (account.StartsWith("111")) return "11" + account.Substring(3);
        return account;
    }

//    private DataTable GetLedgerData(DateTime fromDate, DateTime toDate, string fromAccount, string toAccount, string postingStatus)
//    {
//        string sql = @"
//            SELECT 
//                l.GL_CODE,
//                l.GL_DESCRP,
//                l.BOOK_TYPE,
//                l.GL_FORM_NUMBER,
//                l.VOUCHER_DATE,
//                l.NARATION,
//                l.CHEQUE_NUMBER,
//                l.BILL_NUMBER,
//                l.OPENING_BALANCE,
//                l.DEBIT,
//                l.CREDIT,
//                l.RUNNING_BALANCE
//            FROM TMP_RPT_GL_LEDGER l
//            INNER JOIN gl_forms f ON l.GL_FORM_NUMBER = f.GL_FORM_NUMBER
//            WHERE l.VOUCHER_DATE BETWEEN :FROM_DATE AND :TO_DATE
//        ";

//        if (postingStatus == "Posted")
//            sql += " AND f.POST = 1";
//        else if (postingStatus == "Unposted")
//            sql += " AND f.POST = 0";

//        if (!string.IsNullOrEmpty(fromAccount) && !string.IsNullOrEmpty(toAccount))
//            sql += " AND l.GL_CODE >= :FROM_ACCOUNT AND l.GL_CODE <= :TO_ACCOUNT";
//        else if (!string.IsNullOrEmpty(fromAccount))
//            sql += " AND l.GL_CODE >= :FROM_ACCOUNT";
//        else if (!string.IsNullOrEmpty(toAccount))
//            sql += " AND l.GL_CODE <= :TO_ACCOUNT";

//        sql += " ORDER BY l.GL_CODE, l.VOUCHER_DATE, l.GL_FORM_NUMBER";

//        DataTable dt = new DataTable();
//        using (OracleConnection conn = new OracleConnection(connStr))
//        using (OracleCommand cmd = new OracleCommand(sql, conn))
//        {
//            cmd.Parameters.Add(":FROM_DATE", OracleDbType.Date).Value = fromDate;
//            cmd.Parameters.Add(":TO_DATE", OracleDbType.Date).Value = toDate;
//            if (!string.IsNullOrEmpty(fromAccount))
//                cmd.Parameters.Add(":FROM_ACCOUNT", OracleDbType.Varchar2).Value = fromAccount;
//            if (!string.IsNullOrEmpty(toAccount))
//                cmd.Parameters.Add(":TO_ACCOUNT", OracleDbType.Varchar2).Value = toAccount;

//            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
//            {
//                da.Fill(dt);
//            }
//        }
//        return dt;
//    }

    private DataTable GetLedgerData(DateTime fromDate, DateTime toDate, string fromAccount, string toAccount, string postingStatus)
    {
        string sql = @"
        SELECT 
            v.GL_CODE,
            g.GL_DESCRP,
            f.BOOK_TYPE,
            f.GL_FORM_NUMBER,
            f.VOUCHER_DATE,
            v.NARATION,
            v.CHEQUE_NUMBER,
            v.BILL_NUMBER,
            0 AS OPENING_BALANCE,  -- Opening balance needs separate calculation
            CASE WHEN v.DR_CR IN ('2', 'D') THEN v.AMOUNT ELSE 0 END AS DEBIT,
            CASE WHEN v.DR_CR IN ('1', 'C') THEN v.AMOUNT ELSE 0 END AS CREDIT,
            0 AS RUNNING_BALANCE   -- Will calculate in code
        FROM GL_VOUCHERS v
        INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
        LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
        WHERE f.VOUCHER_DATE BETWEEN :FROM_DATE AND :TO_DATE
    ";

        // Posting status filter
        if (postingStatus == "Posted")
            sql += " AND f.POST = 1";
        else if (postingStatus == "Unposted")
            sql += " AND f.POST = 0";

        // Account range filters
        if (!string.IsNullOrEmpty(fromAccount) && !string.IsNullOrEmpty(toAccount))
            sql += " AND v.GL_CODE >= :FROM_ACCOUNT AND v.GL_CODE <= :TO_ACCOUNT";
        else if (!string.IsNullOrEmpty(fromAccount))
            sql += " AND v.GL_CODE >= :FROM_ACCOUNT";
        else if (!string.IsNullOrEmpty(toAccount))
            sql += " AND v.GL_CODE <= :TO_ACCOUNT";

        sql += " ORDER BY v.GL_CODE, f.VOUCHER_DATE, f.GL_FORM_NUMBER";

        DataTable dt = new DataTable();
        using (OracleConnection conn = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, conn))
        {
            cmd.Parameters.Add(":FROM_DATE", OracleDbType.Date).Value = fromDate;
            cmd.Parameters.Add(":TO_DATE", OracleDbType.Date).Value = toDate;
            if (!string.IsNullOrEmpty(fromAccount))
                cmd.Parameters.Add(":FROM_ACCOUNT", OracleDbType.Varchar2).Value = fromAccount;
            if (!string.IsNullOrEmpty(toAccount))
                cmd.Parameters.Add(":TO_ACCOUNT", OracleDbType.Varchar2).Value = toAccount;

            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }

        // Now calculate running balance per GL_CODE
        DataTable result = new DataTable();
        result = dt.Clone(); // same schema
        // Add OPENING_BALANCE column if needed (you already have it)

        decimal runningBalance = 0;
        string currentGlCode = "";

        foreach (DataRow row in dt.Rows)
        {
            string glCode = row["GL_CODE"].ToString();
            if (glCode != currentGlCode)
            {
                // Get opening balance for this GL_CODE as of fromDate-1 day
                runningBalance = GetOpeningBalanceForGL(glCode, fromDate.AddDays(-1));
                currentGlCode = glCode;
            }

            decimal debit = Convert.ToDecimal(row["DEBIT"]);
            decimal credit = Convert.ToDecimal(row["CREDIT"]);
            runningBalance += debit - credit;

            row["RUNNING_BALANCE"] = runningBalance;
            result.ImportRow(row);
        }

        return result;
    }

    // Helper method to get opening balance for a GL code up to a specific date
    private decimal GetOpeningBalanceForGL(string glCode, DateTime asOfDate)
    {
        string query = @"
        SELECT NVL(SUM(CASE WHEN v.DR_CR IN ('2','D') THEN v.AMOUNT ELSE 0 END) - 
                   SUM(CASE WHEN v.DR_CR IN ('1','C') THEN v.AMOUNT ELSE 0 END), 0)
        FROM GL_VOUCHERS v
        INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
        WHERE v.GL_CODE = :glCode
        AND f.VOUCHER_DATE <= :asOfDate
        AND f.POST = 1  -- Only posted vouchers affect opening balance
    ";
        using (OracleConnection conn = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(query, conn))
        {
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;
            cmd.Parameters.Add("asOfDate", OracleDbType.Date).Value = asOfDate;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0;
        }
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
                    e.Row.Cells[i].Text = "";

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
                    e.Row.Cells[11].ForeColor = Color.Red;
                return;
            }

            // Format regular rows
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
                    lblBalance.ForeColor = Color.Black;
            }
        }
    }

    public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    {
        // Required for Excel export
    }
}