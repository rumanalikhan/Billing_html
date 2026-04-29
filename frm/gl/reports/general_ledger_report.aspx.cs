using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI;
using System.IO;
using System.Text;

public partial class general_ledger_report : System.Web.UI.Page
{
    private readonly string connStr = WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetDefaultDates();
            lblReportDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            gvReport.DataSource = null;
            gvReport.DataBind();
            lblStatus.Text = "Please select criteria and click Search to load report.";
            lblStatus.ForeColor = Color.Blue;
        }
    }

    private void SetDefaultDates()
    {
        int year = DateTime.Now.Year;
        if (DateTime.Now.Month < 7) year--;

        txtFromDate.Text = new DateTime(year, 7, 1).ToString("yyyy-MM-dd");
        txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");  
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadReport();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }

    protected void gvReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (ViewState["ReportData"] != null)
        {
            DataTable dt = (DataTable)ViewState["ReportData"];
            gvReport.DataSource = dt;
            gvReport.PageIndex = e.NewPageIndex;
            gvReport.DataBind();

            int startRow = e.NewPageIndex * 30 + 1;
            int endRow = Math.Min(startRow + 29, dt.Rows.Count - 1);
            int totalRecords = dt.Rows.Count - 1;
            int currentPage = e.NewPageIndex + 1;
            int totalPages = (int)Math.Ceiling((double)totalRecords / 30);

            lblStatus.Text = "Showing rows " + startRow + " to " + endRow + " of " + totalRecords + " total records. Page " + currentPage + " of " + totalPages + ".";
            lblStatus.ForeColor = Color.Green;
        }
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
                ExportToExcel(dt, fromDate, toDate, fromAccount, toAccount, postingStatus);
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

    //private void ExportToExcel(DataTable dt, DateTime fromDate, DateTime toDate, string fromAccount, string toAccount, string postingStatus)
    //{
    //    // Create a new DataTable for Excel export (without the total row)
    //    DataTable excelDt = new DataTable();

    //    // Copy structure
    //    foreach (DataColumn col in dt.Columns)
    //    {
    //        excelDt.Columns.Add(col.ColumnName, col.DataType);
    //    }

    //    // Copy rows EXCLUDING the last row (which is TOTAL)
    //    for (int i = 0; i < dt.Rows.Count - 1; i++)
    //    {
    //        excelDt.ImportRow(dt.Rows[i]);
    //    }

    //    // Calculate totals from original data (excluding total row)
    //    decimal totalDebit = 0, totalCredit = 0;
    //    foreach (DataRow row in excelDt.Rows)
    //    {
    //        totalDebit += Convert.ToDecimal(row["DEBIT"]);
    //        totalCredit += Convert.ToDecimal(row["CREDIT"]);
    //    }
    //    decimal finalBalance = totalDebit - totalCredit;
    //    string finalDrCr = finalBalance >= 0 ? "DR" : "CR";

    //    TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
    //    DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

    //    Response.Clear();
    //    Response.Buffer = true;
    //    Response.ContentType = "application/vnd.ms-excel";
    //    Response.AddHeader("content-disposition", "attachment;filename=GeneralLedger_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
    //    Response.Charset = "";
    //    Response.ContentEncoding = Encoding.UTF8;

    //    using (StringWriter sw = new StringWriter())
    //    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
    //    {
    //        // Write HTML document
    //        hw.Write("<html>");
    //        hw.Write("<head>");
    //        hw.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
    //        hw.Write("<title>General Ledger Report</title>");
    //        hw.Write("<style>");
    //        hw.Write("td { mso-number-format: \\@; font-family: 'Segoe UI', Arial, sans-serif; font-size: 9pt; }");
    //        hw.Write("th { background-color: #0f7c57; color: white; font-family: 'Segoe UI', Arial, sans-serif; font-size: 9pt; font-weight: bold; }");
    //        hw.Write(".text-right { text-align: right; }");
    //        hw.Write(".total-row { background-color: #dcdcdc; font-weight: bold; }");
    //        hw.Write("table { border-collapse: collapse; width: 100%; margin-top: 10px; }");
    //        hw.Write("th, td { border: 1px solid #000000; padding: 6px; }");
    //        hw.Write("</style>");
    //        hw.Write("</head>");
    //        hw.Write("<body>");

    //        // Header section
    //        hw.Write("<div style='text-align:center; margin-bottom:20px;'>");
    //        hw.Write("<h2>BAHRIA TOWN KARACHI</h2>");
    //        hw.Write("<h3>GL ACCOUNTING SYSTEM</h3>");
    //        hw.Write("<h4>GENERAL LEDGER REPORT</h4>");
    //        hw.Write("<p>Printed on: " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt") + "</p>");
    //        hw.Write("<p>FROM: " + fromDate.ToString("dd-MM-yyyy") + " TO: " + toDate.ToString("dd-MM-yyyy") + "</p>");
    //        if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
    //            hw.Write("<p>ACCOUNT: " + fromAccount + " TO " + toAccount + "</p>");
    //        hw.Write("<p>Posting Status: " + (postingStatus == "Posted" ? "Posted Only" : postingStatus == "Unposted" ? "Unposted Only" : "All Vouchers") + "</p>");
    //        hw.Write("</div>");

    //        // Build table with proper structure
    //        hw.Write("<table cellspacing='0' cellpadding='4' border='1'>");

    //        // Header row
    //        hw.Write("<thead>");
    //        hw.Write("<tr>");
    //        hw.Write("<th>GL CODE</th>");
    //        hw.Write("<th>GL DESCRIPTION</th>");
    //        hw.Write("<th>BOOK TYPE</th>");
    //        hw.Write("<th>GL FORM</th>");
    //        hw.Write("<th>VOUCHER DATE</th>");
    //        hw.Write("<th>NARATION</th>");
    //        hw.Write("<th>CHEQUE NO</th>");
    //        hw.Write("<th>BILL NO</th>");
    //        hw.Write("<th>OPENING</th>");
    //        hw.Write("<th>DEBIT</th>");
    //        hw.Write("<th>CREDIT</th>");
    //        hw.Write("<th>RUNNING BALANCE</th>");
    //        hw.Write("</tr>");
    //        hw.Write("</thead>");

    //        hw.Write("<tbody>");

    //        // Data rows - use the excelDt (without total row)
    //        foreach (DataRow row in excelDt.Rows)
    //        {
    //            DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
    //            decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
    //            string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
    //            string openingText = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;
    //            decimal debit = Convert.ToDecimal(row["DEBIT"]);
    //            decimal credit = Convert.ToDecimal(row["CREDIT"]);
    //            decimal runningBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
    //            string runningDrCr = runningBalance >= 0 ? "DR" : "CR";
    //            string runningText = Math.Abs(runningBalance).ToString("N2") + " " + runningDrCr;

    //            hw.Write("<tr>");
    //            hw.Write("<td>" + row["GL_CODE"].ToString() + "</td>");
    //            hw.Write("<td>" + row["GL_DESCRP"].ToString() + "</td>");
    //            hw.Write("<td>" + row["BOOK_TYPE"].ToString() + "</td>");
    //            hw.Write("<td>" + row["GL_FORM_NUMBER"].ToString() + "</td>");
    //            hw.Write("<td>" + voucherDate.ToString("dd-MM-yyyy") + "</td>");
    //            hw.Write("<td>" + row["NARATION"].ToString() + "</td>");
    //            hw.Write("<td>" + row["CHEQUE_NUMBER"].ToString() + "</td>");
    //            hw.Write("<td>" + row["BILL_NUMBER"].ToString() + "</td>");
    //            hw.Write("<td class='text-right'>" + openingText + "</td>");
    //            hw.Write("<td class='text-right'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
    //            hw.Write("<td class='text-right'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</tr>");
    //            if (runningDrCr == "CR")
    //                hw.Write("<td class='text-right' style='color:red;'>" + runningText + "</td>");
    //            else
    //                hw.Write("<td class='text-right'>" + runningText + "</td>");
    //            hw.Write("</tr>");
    //        }

    //        // Total row
    //        hw.Write("<tr class='total-row'>");
    //        hw.Write("<td colspan='9'><strong>TOTAL</strong></td>");
    //        hw.Write("<td class='text-right'><strong>" + totalDebit.ToString("N2") + "</strong></td>");
    //        hw.Write("<td class='text-right'><strong>" + totalCredit.ToString("N2") + "</strong></td>");
    //        if (finalDrCr == "CR")
    //            hw.Write("<td class='text-right' style='color:red;'><strong>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</strong></td>");
    //        else
    //            hw.Write("<td class='text-right'><strong>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</strong></td>");
    //        hw.Write("</tr>");

    //        hw.Write("</tbody>");
    //        hw.Write("</table>");

    //        hw.Write("<div style='margin-top:30px; text-align:center; font-size:10px; color:#999;'>");
    //        hw.Write("This is a computer generated document - No signature required");
    //        hw.Write("</div>");
    //        hw.Write("</body></html>");

    //        Response.Write(sw.ToString());
    //        Response.End();
    //    }
    //}

    private void ExportToExcel(DataTable dt, DateTime fromDate, DateTime toDate, string fromAccount, string toAccount, string postingStatus)
    {
        TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("content-disposition", "attachment;filename=GeneralLedger_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
        Response.Charset = "";
        Response.ContentEncoding = Encoding.UTF8;

        using (StringWriter sw = new StringWriter())
        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
        {
            // Write HTML document
            hw.Write("<html>");
            hw.Write("<head>");
            hw.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            hw.Write("<title>General Ledger Report</title>");
            hw.Write("<style>");
            hw.Write("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
            hw.Write("td { mso-number-format: \\@; font-family: 'Segoe UI', Arial, sans-serif; font-size: 10pt; }");
            hw.Write("th { background-color: #0f7c57; color: white; font-family: 'Segoe UI', Arial, sans-serif; font-size: 10pt; font-weight: bold; padding: 8px; }");
            hw.Write(".text-right { text-align: right; }");
            hw.Write(".total-row { background-color: #dcdcdc; font-weight: bold; }");
            hw.Write("table { border-collapse: collapse; width: 100%; margin-top: 10px; }");
            hw.Write("th, td { border: 1px solid #000000; padding: 6px; }");
            hw.Write("</style>");
            hw.Write("</head>");
            hw.Write("<body>");

            // Header section
            hw.Write("<div style='text-align:center; margin-bottom:20px;'>");
            hw.Write("<h2>BAHRIA TOWN KARACHI</h2>");
            hw.Write("<h3>GL ACCOUNTING SYSTEM</h3>");
            hw.Write("<h4>GENERAL LEDGER REPORT</h4>");
            hw.Write("<p><strong>Printed on:</strong> " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt") + "</p>");
            hw.Write("<p><strong>FROM:</strong> " + fromDate.ToString("dd-MM-yyyy") + " <strong>TO:</strong> " + toDate.ToString("dd-MM-yyyy") + "</p>");
            if (!string.IsNullOrEmpty(fromAccount) || !string.IsNullOrEmpty(toAccount))
                hw.Write("<p><strong>ACCOUNT:</strong> " + fromAccount + " TO " + toAccount + "</p>");
            hw.Write("<p><strong>Posting Status:</strong> " + (postingStatus == "Posted" ? "Posted Only" : postingStatus == "Unposted" ? "Unposted Only" : "All Vouchers") + "</p>");
            hw.Write("</div>");

            // Build table
            hw.Write("<table cellspacing='0' cellpadding='4' border='1'>");

            // Header row
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

            // Calculate totals
            decimal totalDebit = 0, totalCredit = 0;
            foreach (DataRow row in dt.Rows)
            {
                totalDebit += Convert.ToDecimal(row["DEBIT"]);
                totalCredit += Convert.ToDecimal(row["CREDIT"]);

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
                hw.Write("<td style='white-space: nowrap;'>" + row["GL_CODE"].ToString() + "</td>");
                hw.Write("<td style='white-space: nowrap;'>" + row["GL_DESCRP"].ToString() + "</td>");
                hw.Write("<td style='text-align:center;'>" + row["BOOK_TYPE"].ToString() + "</td>");
                hw.Write("<td style='text-align:center;'>" + row["GL_FORM_NUMBER"].ToString() + "</td>");
                hw.Write("<td style='text-align:center; white-space: nowrap;'>" + voucherDate.ToString("dd-MM-yyyy") + "</td>");
                hw.Write("<td>" + row["NARATION"].ToString() + "</td>");
                hw.Write("<td style='white-space: nowrap;'>" + row["CHEQUE_NUMBER"].ToString() + "</td>");
                hw.Write("<td style='white-space: nowrap;'>" + row["BILL_NUMBER"].ToString() + "</td>");
                hw.Write("<td class='text-right' style='white-space: nowrap;'>" + openingText + "</td>");
                hw.Write("<td class='text-right'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
                hw.Write("<td class='text-right'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</td>");
                if (runningDrCr == "CR")
                    hw.Write("<td class='text-right' style='color:red; white-space: nowrap;'>" + runningText + "</td>");
                else
                    hw.Write("<td class='text-right' style='white-space: nowrap;'>" + runningText + "</td>");
                hw.Write("</tr>");
            }

            decimal finalBalance = totalDebit - totalCredit;
            string finalDrCr = finalBalance >= 0 ? "DR" : "CR";

            // Total row
            hw.Write("<tr class='total-row' style='background-color:#dcdcdc; font-weight:bold;'>");
            hw.Write("<td colspan='9' style='text-align:left;'><strong>TOTAL</strong></td>");
            hw.Write("<td class='text-right'><strong>" + totalDebit.ToString("N2") + "</strong></td>");
            hw.Write("<td class='text-right'><strong>" + totalCredit.ToString("N2") + "</strong></td>");
            if (finalDrCr == "CR")
                hw.Write("<td class='text-right' style='color:red;'><strong>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</strong></td>");
            else
                hw.Write("<td class='text-right'><strong>" + Math.Abs(finalBalance).ToString("N2") + " " + finalDrCr + "</strong></td>");
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

            lblStatus.Text = "Loading report... Please wait.";
            lblStatus.ForeColor = Color.Blue;
            gvReport.DataSource = null;
            gvReport.DataBind();

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

                ViewState["ReportData"] = dt;
                gvReport.PageIndex = 0;
                gvReport.DataSource = dt;
                gvReport.DataBind();

                int totalRecords = dt.Rows.Count - 1;
                int pageSize = 30;
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                int showing = Math.Min(pageSize, totalRecords);

                lblStatus.Text = "Report loaded successfully. Total rows: " + totalRecords + ". Page 1 of " + totalPages + " (showing " + showing + " records per page).";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "No data found for selected criteria.";
                lblStatus.ForeColor = Color.Red;
                gvReport.DataSource = null;
                gvReport.DataBind();
                ViewState["ReportData"] = null;
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
            ViewState["ReportData"] = null;
        }
    }

    private string ConvertToDbFormat(string account)
    {
        if (string.IsNullOrEmpty(account)) return account;
        if (account.StartsWith("111")) return "11" + account.Substring(3);
        return account;
    }

    private DataTable GetLedgerData(DateTime fromDate, DateTime toDate, string fromAccount, string toAccount, string postingStatus)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string query = @"
                SELECT 
                    v.GL_CODE,
                    NVL(g.GL_DESCRP, v.GL_CODE) AS GL_DESCRP,
                    f.BOOK_TYPE,
                    f.GL_FORM_NUMBER,
                    f.VOUCHER_DATE,
                    NVL(v.NARATION, '') AS NARATION,
                    NVL(v.CHEQUE_NUMBER, '') AS CHEQUE_NUMBER,
                    NVL(v.BILL_NUMBER, '') AS BILL_NUMBER,
                    CASE WHEN v.DR_CR IN ('2', 'D') THEN v.AMOUNT ELSE 0 END AS DEBIT,
                    CASE WHEN v.DR_CR IN ('1', 'C') THEN v.AMOUNT ELSE 0 END AS CREDIT
                FROM GL_VOUCHERS v
                INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
                LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
                WHERE f.VOUCHER_DATE BETWEEN :FROM_DATE AND :TO_DATE
            ";

            if (postingStatus == "Posted")
                query += " AND f.POST = 1";
            else if (postingStatus == "Unposted")
                query += " AND f.POST = 0";

            if (!string.IsNullOrEmpty(fromAccount))
                query += " AND v.GL_CODE >= :FROM_ACCOUNT";
            if (!string.IsNullOrEmpty(toAccount))
                query += " AND v.GL_CODE <= :TO_ACCOUNT";

            query += " ORDER BY v.GL_CODE, f.VOUCHER_DATE, f.GL_FORM_NUMBER";

            OracleCommand cmd = new OracleCommand(query, conn);
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

            conn.Close();
        }

        // Add calculated columns
        dt.Columns.Add("OPENING_BALANCE", typeof(decimal));
        dt.Columns.Add("RUNNING_BALANCE", typeof(decimal));

        decimal runningBalance = 0;
        string currentGlCode = "";
        decimal openingBalance = 0;

        foreach (DataRow row in dt.Rows)
        {
            string glCode = row["GL_CODE"].ToString();

            if (glCode != currentGlCode)
            {
                openingBalance = GetOpeningBalanceForGL(glCode, fromDate.AddDays(-1));
                runningBalance = openingBalance;
                currentGlCode = glCode;
            }

            row["OPENING_BALANCE"] = openingBalance;

            decimal debit = Convert.ToDecimal(row["DEBIT"]);
            decimal credit = Convert.ToDecimal(row["CREDIT"]);
            runningBalance += debit - credit;

            row["RUNNING_BALANCE"] = runningBalance;
        }

        return dt;
    }

    private decimal GetOpeningBalanceForGL(string glCode, DateTime asOfDate)
    {
        string query = @"
            SELECT NVL(SUM(CASE WHEN v.DR_CR IN ('2','D') THEN v.AMOUNT ELSE 0 END) - 
                       SUM(CASE WHEN v.DR_CR IN ('1','C') THEN v.AMOUNT ELSE 0 END), 0)
            FROM GL_VOUCHERS v
            INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
            WHERE v.GL_CODE = :glCode
            AND f.VOUCHER_DATE <= :asOfDate
            AND f.POST = 1
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
            if (e.Row.Cells.Count > 4 && row["VOUCHER_DATE"] != DBNull.Value)
            {
                DateTime voucherDate = Convert.ToDateTime(row["VOUCHER_DATE"]);
                e.Row.Cells[4].Text = voucherDate.ToString("dd-MM-yyyy");
            }

            // Opening Balance
            if (e.Row.Cells.Count > 8 && row["OPENING_BALANCE"] != DBNull.Value)
            {
                decimal openingBalance = Convert.ToDecimal(row["OPENING_BALANCE"]);
                string openingDrCr = openingBalance >= 0 ? "DR" : "CR";
                e.Row.Cells[8].Text = Math.Abs(openingBalance).ToString("N2") + " " + openingDrCr;
                e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;
            }

            // Debit
            if (e.Row.Cells.Count > 9 && row["DEBIT"] != DBNull.Value)
            {
                decimal debitAmt = Convert.ToDecimal(row["DEBIT"]);
                e.Row.Cells[9].Text = debitAmt == 0 ? "" : debitAmt.ToString("N2");
                e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;
            }

            // Credit
            if (e.Row.Cells.Count > 10 && row["CREDIT"] != DBNull.Value)
            {
                decimal creditAmt = Convert.ToDecimal(row["CREDIT"]);
                e.Row.Cells[10].Text = creditAmt == 0 ? "" : creditAmt.ToString("N2");
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;
            }

            // Running Balance
            if (e.Row.Cells.Count > 11 && row["RUNNING_BALANCE"] != DBNull.Value)
            {
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
    }

    public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
    {
        // Required for Excel export
    }
}