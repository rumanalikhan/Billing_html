using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;

public partial class sub_ledger_report : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["CurrentCompId"] == null)
                Session["CurrentCompId"] = 1;
            if (Session["CurrentLogId"] == null)
                Session["CurrentLogId"] = 0;

            SetDefaultDates();
            lblReportDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
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
        string slCode = txtSLCode.Text.Trim();

        if (string.IsNullOrEmpty(slCode))
        {
            ShowStatus("Please enter Sub Ledger Code", "error");
            return;
        }

        LoadReport(slCode);
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSLCode.Text = "";
        txtFromDate.Text = new DateTime(DateTime.Now.Year, 7, 1).ToString("yyyy-MM-dd");
        txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        ddlPostingStatus.SelectedValue = "Posted";  // Reset to default
        slInfo.Visible = false;
        periodInfo.Visible = false;
        gvReport.DataSource = null;
        gvReport.DataBind();
        ShowStatus("Form cleared", "info");
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }

    private void LoadReport(string slCode)
    {
        try
        {
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string postingStatus = ddlPostingStatus.SelectedValue;

            lblFromDate.Text = fromDate.ToString("dd-MM-yyyy");
            lblToDate.Text = toDate.ToString("dd-MM-yyyy");
            periodInfo.Visible = true;

            string slName = GetSubLedgerName(slCode);

            if (string.IsNullOrEmpty(slName))
            {
                ShowStatus("Sub Ledger Code not found: " + slCode, "error");
                return;
            }

            lblSLCode.Text = slCode;
            lblSLName.Text = slName;
            lblPrintDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            slInfo.Visible = true;

            DataTable dt = GetSubLedgerData(slCode, fromDate, toDate, postingStatus);

            if (dt.Rows.Count == 0)
            {
                ShowStatus("No transactions found for Sub Ledger: " + slCode, "info");
                gvReport.DataSource = null;
                gvReport.DataBind();
                return;
            }

            DataTable processedDt = ProcessData(dt, slCode);

            gvReport.DataSource = processedDt;
            gvReport.DataBind();

            ShowStatus("Report loaded successfully. Total transactions: " + (processedDt.Rows.Count - 1), "success");
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    private string GetSubLedgerName(string slCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT DESCRIP FROM GL_SL_GLMF WHERE SL_CODE = :slCode";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private DataTable GetSubLedgerData(string slCode, DateTime fromDate, DateTime toDate, string postingStatus)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
                SELECT 
                    v.SL_CODE,
                    s.DESCRIP AS SL_NAME,
                    f.VOUCHER_DATE,
                    f.VOUCHER_KEY,
                    v.GL_CODE,
                    NVL(g.GL_DESCRP, '') AS GL_DESCRIPTION,
                    NVL(v.NARATION, '') AS PARTICULARS,
                    NVL(v.CHEQUE_NUMBER, '') AS CHEQUE_NUMBER,
                    v.AMOUNT,
                    v.DR_CR
                FROM GL_VOUCHERS v
                INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
                INNER JOIN GL_SL_GLMF s ON v.SL_CODE = s.SL_CODE
                LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
                WHERE v.SL_CODE = :slCode
                AND f.VOUCHER_DATE BETWEEN :FromDate AND :ToDate
            ";

            // Apply posting status filter using gl_forms.POST
            if (postingStatus == "Posted")
                query += " AND f.POST = 1";
            else if (postingStatus == "Unposted")
                query += " AND f.POST = 0";
            // "All" adds no extra condition

            query += " ORDER BY f.VOUCHER_DATE, f.VOUCHER_KEY";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
            cmd.Parameters.Add("FromDate", OracleDbType.Date).Value = fromDate;
            cmd.Parameters.Add("ToDate", OracleDbType.Date).Value = toDate;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
        }

        return dt;
    }

    private decimal GetOpeningBalance(string slCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT NVL(OPENING_BALANCE, 0) FROM GL_SL_OPENING_BALANCE WHERE SL_CODE = :slCode AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }

    private DataTable ProcessData(DataTable dt, string slCode)
    {
        DataTable result = new DataTable();
        result.Columns.Add("VOUCHER_KEY", typeof(string));
        result.Columns.Add("TRANS_DATE", typeof(string));
        result.Columns.Add("PARTICULARS", typeof(string));
        result.Columns.Add("CHEQUE_NUMBER", typeof(string));
        result.Columns.Add("DEBIT", typeof(decimal));
        result.Columns.Add("CREDIT", typeof(decimal));
        result.Columns.Add("RUNNING_BALANCE", typeof(decimal));

        decimal openingBalance = GetOpeningBalance(slCode);

        DataRow openingRow = result.NewRow();
        openingRow["VOUCHER_KEY"] = "";
        openingRow["TRANS_DATE"] = "";
        openingRow["PARTICULARS"] = "OPENING BALANCE";
        openingRow["CHEQUE_NUMBER"] = "";
        openingRow["DEBIT"] = 0;
        openingRow["CREDIT"] = 0;
        openingRow["RUNNING_BALANCE"] = Math.Abs(openingBalance);
        result.Rows.Add(openingRow);

        decimal runningBalance = openingBalance;
        decimal totalDebit = openingBalance >= 0 ? openingBalance : 0;
        decimal totalCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0;

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = result.NewRow();

            newRow["VOUCHER_KEY"] = row["VOUCHER_KEY"];
            newRow["TRANS_DATE"] = Convert.ToDateTime(row["VOUCHER_DATE"]).ToString("dd-MM-yyyy");

            string glCode = row["GL_CODE"].ToString();
            string glDesc = row["GL_DESCRIPTION"].ToString();
            string particulars = row["PARTICULARS"].ToString();

            if (!string.IsNullOrEmpty(glDesc))
            {
                newRow["PARTICULARS"] = glDesc + " - " + particulars;
            }
            else
            {
                newRow["PARTICULARS"] = particulars;
            }

            newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"];

            string drcr = row["DR_CR"].ToString().ToUpper();
            decimal amount = Convert.ToDecimal(row["AMOUNT"]);

            if (drcr == "2" || drcr == "D")
            {
                newRow["DEBIT"] = amount;
                newRow["CREDIT"] = 0;
                totalDebit += amount;
                runningBalance += amount;
            }
            else
            {
                newRow["DEBIT"] = 0;
                newRow["CREDIT"] = amount;
                totalCredit += amount;
                runningBalance -= amount;
            }

            newRow["RUNNING_BALANCE"] = runningBalance;
            result.Rows.Add(newRow);
        }

        DataRow totalRow = result.NewRow();
        totalRow["VOUCHER_KEY"] = "";
        totalRow["TRANS_DATE"] = "";
        totalRow["PARTICULARS"] = "TOTAL";
        totalRow["CHEQUE_NUMBER"] = "";
        totalRow["DEBIT"] = totalDebit;
        totalRow["CREDIT"] = totalCredit;
        totalRow["RUNNING_BALANCE"] = runningBalance;
        result.Rows.Add(totalRow);

        return result;
    }

    protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView row = (DataRowView)e.Row.DataItem;

            string particulars = row["PARTICULARS"].ToString();
            bool isOpeningRow = (particulars == "OPENING BALANCE");
            bool isTotalRow = (particulars == "TOTAL");

            if (isOpeningRow)
            {
                e.Row.CssClass = "total-row";
                e.Row.Cells[4].Text = "";
                e.Row.Cells[5].Text = "";
                decimal balance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                e.Row.Cells[6].Text = balance.ToString("N2");
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                return;
            }

            if (isTotalRow)
            {
                e.Row.CssClass = "total-row";
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i != 2)
                        e.Row.Cells[i].Text = "";
                }
                e.Row.Cells[2].Text = "TOTAL";
                e.Row.Cells[2].Font.Bold = true;
                e.Row.Cells[4].Text = Convert.ToDecimal(row["DEBIT"]).ToString("N2");
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[5].Text = Convert.ToDecimal(row["CREDIT"]).ToString("N2");
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[6].Text = Convert.ToDecimal(row["RUNNING_BALANCE"]).ToString("N2");
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Right;
                return;
            }

            Label lblBalance = (Label)e.Row.FindControl("lblBalance");
            if (lblBalance != null)
            {
                decimal balance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                string drCr = balance >= 0 ? "DR" : "CR";
                lblBalance.Text = Math.Abs(balance).ToString("N2") + " " + drCr;

                if (drCr == "CR")
                {
                    lblBalance.ForeColor = Color.Red;
                    lblBalance.Font.Bold = true;
                }
            }
        }
    }

    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string slCode = txtSLCode.Text.Trim();

            if (string.IsNullOrEmpty(slCode))
            {
                ShowStatus("Please enter Sub Ledger Code", "error");
                return;
            }

            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string postingStatus = ddlPostingStatus.SelectedValue;
            string slName = GetSubLedgerName(slCode);

            if (string.IsNullOrEmpty(slName))
            {
                ShowStatus("Sub Ledger Code not found: " + slCode, "error");
                return;
            }

            DataTable dt = GetSubLedgerData(slCode, fromDate, toDate, postingStatus);

            if (dt.Rows.Count == 0)
            {
                ShowStatus("No transactions found for Sub Ledger: " + slCode, "info");
                return;
            }

            DataTable processedDt = ProcessDataForExcel(dt, slCode);

            TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=SubLedger_" + slCode + "_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
            Response.Charset = "";

            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            hw.Write("<html><head><meta charset='UTF-8'><title>Sub Ledger Report</title>");
            hw.Write("<style>");
            hw.Write("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
            hw.Write(".company-header { text-align: center; margin-bottom: 20px; }");
            hw.Write(".company-name { font-size: 20px; font-weight: bold; color: #0f7c57; }");
            hw.Write(".company-sub { font-size: 12px; color: #555; }");
            hw.Write(".report-title { text-align: center; font-size: 16px; font-weight: bold; text-decoration: underline; margin: 15px 0; }");
            hw.Write(".period-info { text-align: center; margin-bottom: 15px; padding: 8px; background: #dcdcdc; }");
            hw.Write(".sl-info { background: #e8f0fe; padding: 10px; margin-bottom: 15px; border-left: 3px solid #0f7c57; }");
            hw.Write(".report-table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            hw.Write(".report-table th { background: #0f7c57; color: white; padding: 10px; border: 1px solid #0a5e42; text-align: center; }");
            hw.Write(".report-table td { padding: 8px; border: 1px solid #ddd; }");
            hw.Write(".amount-column { text-align: right; }");
            hw.Write(".total-row { background: #dcdcdc; font-weight: bold; }");
            hw.Write(".credit-balance { color: red; font-weight: bold; }");
            hw.Write(".footer { margin-top: 20px; text-align: center; font-size: 10px; color: #999; border-top: 1px solid #ddd; padding-top: 10px; }");
            hw.Write("</style></head><body>");

            hw.Write("<div class='company-header'>");
            hw.Write("<div class='company-name'>BAHRIA TOWN KARACHI</div>");
            hw.Write("<div class='company-sub'>GL ACCOUNTING SYSTEM</div>");
            hw.Write("</div>");

            hw.Write("<div class='report-title'>SUB LEDGER REPORT</div>");

            hw.Write("<div class='period-info'>");
            hw.Write("FROM: " + fromDate.ToString("dd-MM-yyyy") + " TO: " + toDate.ToString("dd-MM-yyyy"));
            hw.Write("</div>");

            hw.Write("<div class='sl-info'>");
            hw.Write("<strong>Sub Ledger:</strong> " + slCode + " - " + slName + "<br />");
            hw.Write("<strong>Posting Status:</strong> " + ddlPostingStatus.SelectedItem.Text + "<br />");
            hw.Write("<strong>Printed On:</strong> " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt"));
            hw.Write("</div>");

            hw.Write("<table class='report-table' cellspacing='0' cellpadding='4' border='1'>");
            hw.Write("<thead><tr style='background-color:#0f7c57; color:white;'>");
            hw.Write("<th>VOUCHER #</th><th>DATE</th><th>PARTICULARS</th><th>CHQ/SLIP</th>");
            hw.Write("<th>DEBIT</th><th>CREDIT</th><th>BALANCE</th>");
            hw.Write("</tr></thead><tbody>");

            foreach (DataRow row in processedDt.Rows)
            {
                string voucherKey = row["VOUCHER_KEY"].ToString();
                string transDate = row["TRANS_DATE"].ToString();
                string particulars = row["PARTICULARS"].ToString();
                string chequeNumber = row["CHEQUE_NUMBER"].ToString();
                decimal debit = Convert.ToDecimal(row["DEBIT"]);
                decimal credit = Convert.ToDecimal(row["CREDIT"]);
                decimal runningBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                string runningBalanceText = "";

                if (particulars == "OPENING BALANCE" || particulars == "TOTAL")
                {
                    runningBalanceText = Math.Abs(runningBalance).ToString("N2");
                }
                else
                {
                    string drCr = runningBalance >= 0 ? "DR" : "CR";
                    runningBalanceText = Math.Abs(runningBalance).ToString("N2") + " " + drCr;
                }

                hw.Write("<tr>");
                hw.Write("<td>" + voucherKey + "</td>");
                hw.Write("<td>" + transDate + "</td>");
                hw.Write("<td>" + particulars + "</td>");
                hw.Write("<td>" + chequeNumber + "</td>");
                hw.Write("<td align='right'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
                hw.Write("<td align='right'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</td>");

                if (particulars == "OPENING BALANCE" || particulars == "TOTAL")
                {
                    hw.Write("<td align='right'>" + runningBalanceText + "</td>");
                }
                else if (runningBalance < 0)
                {
                    hw.Write("<td align='right' style='color:red;font-weight:bold;'>" + runningBalanceText + "</td>");
                }
                else
                {
                    hw.Write("<td align='right'>" + runningBalanceText + "</td>");
                }
                hw.Write("</tr>");
            }

            hw.Write("</tbody>");
            hw.Write("</table>");
            hw.Write("<div class='footer'>");
            hw.Write("This is a computer generated document - No signature required");
            hw.Write("</div>");
            hw.Write("</body></html>");

            Response.Write(sw.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            ShowStatus("Error exporting to Excel: " + ex.Message, "error");
        }
    }

    private DataTable ProcessDataForExcel(DataTable dt, string slCode)
    {
        DataTable result = new DataTable();
        result.Columns.Add("VOUCHER_KEY", typeof(string));
        result.Columns.Add("TRANS_DATE", typeof(string));
        result.Columns.Add("PARTICULARS", typeof(string));
        result.Columns.Add("CHEQUE_NUMBER", typeof(string));
        result.Columns.Add("DEBIT", typeof(decimal));
        result.Columns.Add("CREDIT", typeof(decimal));
        result.Columns.Add("RUNNING_BALANCE", typeof(decimal));

        decimal openingBalance = GetOpeningBalance(slCode);

        DataRow openingRow = result.NewRow();
        openingRow["VOUCHER_KEY"] = "";
        openingRow["TRANS_DATE"] = "";
        openingRow["PARTICULARS"] = "OPENING BALANCE";
        openingRow["CHEQUE_NUMBER"] = "";
        openingRow["DEBIT"] = 0;
        openingRow["CREDIT"] = 0;
        openingRow["RUNNING_BALANCE"] = openingBalance;
        result.Rows.Add(openingRow);

        decimal runningBalance = openingBalance;
        decimal totalDebit = openingBalance >= 0 ? openingBalance : 0;
        decimal totalCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0;

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = result.NewRow();

            newRow["VOUCHER_KEY"] = row["VOUCHER_KEY"].ToString();
            newRow["TRANS_DATE"] = Convert.ToDateTime(row["VOUCHER_DATE"]).ToString("dd-MM-yyyy");

            string glDesc = row["GL_DESCRIPTION"].ToString();
            string particulars = row["PARTICULARS"].ToString();

            if (!string.IsNullOrEmpty(glDesc))
            {
                newRow["PARTICULARS"] = glDesc + " - " + particulars;
            }
            else
            {
                newRow["PARTICULARS"] = particulars;
            }

            newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"].ToString();

            string drcr = row["DR_CR"].ToString().ToUpper();
            decimal amount = Convert.ToDecimal(row["AMOUNT"]);

            if (drcr == "2" || drcr == "D")
            {
                newRow["DEBIT"] = amount;
                newRow["CREDIT"] = 0;
                totalDebit += amount;
                runningBalance += amount;
            }
            else
            {
                newRow["DEBIT"] = 0;
                newRow["CREDIT"] = amount;
                totalCredit += amount;
                runningBalance -= amount;
            }

            newRow["RUNNING_BALANCE"] = runningBalance;
            result.Rows.Add(newRow);
        }

        DataRow totalRow = result.NewRow();
        totalRow["VOUCHER_KEY"] = "";
        totalRow["TRANS_DATE"] = "";
        totalRow["PARTICULARS"] = "TOTAL";
        totalRow["CHEQUE_NUMBER"] = "";
        totalRow["DEBIT"] = totalDebit;
        totalRow["CREDIT"] = totalCredit;
        totalRow["RUNNING_BALANCE"] = runningBalance;
        result.Rows.Add(totalRow);

        return result;
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    private void ShowStatus(string message, string type)
    {
        lblStatus.Text = message;
        if (type == "success")
            lblStatus.ForeColor = Color.Green;
        else if (type == "error")
            lblStatus.ForeColor = Color.Red;
        else
            lblStatus.ForeColor = Color.Blue;
    }
}