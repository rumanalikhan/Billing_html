using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class trial_balance : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["CurrentCompId"] == null)
                Session["CurrentCompId"] = 1;

            SetDefaultDates();
            lblReportDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            LoadReport();
        }
    }

    private void SetDefaultDates()
    {
        int year = DateTime.Now.Year;
        if (DateTime.Now.Month < 7) year--;

        txtOpeningDate.Text = new DateTime(year, 6, 30).ToString("yyyy-MM-dd");
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

    private void LoadReport()
    {
        try
        {
            DateTime openingDate = DateTime.Parse(txtOpeningDate.Text);
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string postingStatus = ddlPostingStatus.SelectedValue;
            bool showZeroOpening = chkShowZeroOpening.Checked;

            lblOpeningDate.Text = openingDate.ToString("dd-MM-yyyy");
            lblFromDate.Text = fromDate.ToString("dd-MM-yyyy");
            lblToDate.Text = toDate.ToString("dd-MM-yyyy");

            DataTable dt = GetTrialBalanceData(openingDate, fromDate, toDate, postingStatus, showZeroOpening);

            if (dt.Rows.Count > 0)
            {
                AddIndentation(dt);
                rptReport.DataSource = dt;
                rptReport.DataBind();
                lblStatus.Text = "Report loaded successfully. Total accounts: " + dt.Rows.Count;
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "No data found for selected period.";
                lblStatus.ForeColor = Color.Red;
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
        }
    }

    private DataTable GetTrialBalanceData(DateTime openingDate, DateTime fromDate, DateTime toDate, string postingStatus, bool showZeroOpening)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();

            string postCondition = "";
            if (postingStatus == "Posted")
                postCondition = " AND f.POST = 1";
            else if (postingStatus == "Unposted")
                postCondition = " AND f.POST = 0";

            string query = @"
            SELECT 
                g.GL_CODE,
                g.GL_DESCRP,
                g.LEVELL,
                NVL(ob.OPENING_BALANCE, 0) AS OPENING_BALANCE,
                NVL((
                    SELECT SUM(v.AMOUNT)
                    FROM GL_VOUCHERS v
                    INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
                    WHERE v.GL_CODE = g.GL_CODE
                    AND f.VOUCHER_DATE BETWEEN :FromDate AND :ToDate
                    " + postCondition + @"
                    AND v.DR_CR IN ('2', 'D')
                    AND v.COMP_ID = :CompId
                ), 0) AS PERIOD_DEBIT,
                NVL((
                    SELECT SUM(v.AMOUNT)
                    FROM GL_VOUCHERS v
                    INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
                    WHERE v.GL_CODE = g.GL_CODE
                    AND f.VOUCHER_DATE BETWEEN :FromDate AND :ToDate
                    " + postCondition + @"
                    AND v.DR_CR IN ('1', 'C')
                    AND v.COMP_ID = :CompId
                ), 0) AS PERIOD_CREDIT
            FROM GL_GLMF g
            LEFT JOIN GL_GLMF_OPENING_BALANCE ob ON g.GL_CODE = ob.GL_CODE AND ob.COMP_ID = :CompId
            WHERE g.COMP_ID = :CompId
            AND g.ACTIVE = '1' ";

            // If NOT showing zero opening balances, exclude rows where OPENING_BALANCE = 0
            if (!showZeroOpening)
            {
                query += " AND NVL(ob.OPENING_BALANCE, 0) != 0";
            }

            query += " ORDER BY g.GL_CODE";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("FromDate", OracleDbType.Date).Value = fromDate;
            cmd.Parameters.Add("ToDate", OracleDbType.Date).Value = toDate;
            cmd.Parameters.Add("CompId", OracleDbType.Int32).Value = GetCurrentCompId();

            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
        }

        // Add columns for split Debit/Credit
        dt.Columns.Add("OPENING_DEBIT", typeof(decimal));
        dt.Columns.Add("OPENING_CREDIT", typeof(decimal));
        dt.Columns.Add("CLOSING_DEBIT", typeof(decimal));
        dt.Columns.Add("CLOSING_CREDIT", typeof(decimal));

        foreach (DataRow row in dt.Rows)
        {
            decimal opening = Convert.ToDecimal(row["OPENING_BALANCE"]);
            decimal periodDebit = Convert.ToDecimal(row["PERIOD_DEBIT"]);
            decimal periodCredit = Convert.ToDecimal(row["PERIOD_CREDIT"]);
            decimal closing = opening + periodDebit - periodCredit;

            if (opening >= 0)
            {
                row["OPENING_DEBIT"] = opening;
                row["OPENING_CREDIT"] = 0;
            }
            else
            {
                row["OPENING_DEBIT"] = 0;
                row["OPENING_CREDIT"] = Math.Abs(opening);
            }

            if (closing >= 0)
            {
                row["CLOSING_DEBIT"] = closing;
                row["CLOSING_CREDIT"] = 0;
            }
            else
            {
                row["CLOSING_DEBIT"] = 0;
                row["CLOSING_CREDIT"] = Math.Abs(closing);
            }
        }

        return dt;
    }

    private void AddIndentation(DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            int level = Convert.ToInt32(row["LEVELL"]);
            string description = row["GL_DESCRP"].ToString();

            if (level == 2)
                row["GL_DESCRP"] = "    " + description;
            else if (level == 3)
                row["GL_DESCRP"] = "        " + description;
            else if (level == 4)
                row["GL_DESCRP"] = "            " + description;
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime openingDate = DateTime.Parse(txtOpeningDate.Text);
            DateTime fromDate = DateTime.Parse(txtFromDate.Text);
            DateTime toDate = DateTime.Parse(txtToDate.Text);
            string postingStatus = ddlPostingStatus.SelectedValue;
            bool showZeroOpening = chkShowZeroOpening.Checked;

            DataTable dt = GetTrialBalanceData(openingDate, fromDate, toDate, postingStatus, showZeroOpening);

            // Apply indentation for Excel using spaces
            foreach (DataRow row in dt.Rows)
            {
                int level = Convert.ToInt32(row["LEVELL"]);
                string description = row["GL_DESCRP"].ToString();

                if (level == 2)
                    row["GL_DESCRP"] = "    " + description;
                else if (level == 3)
                    row["GL_DESCRP"] = "        " + description;
                else if (level == 4)
                    row["GL_DESCRP"] = "            " + description;
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=TrialBalance_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            Response.Charset = "";

            StringBuilder html = new StringBuilder();

            html.Append("<html><head><meta charset='UTF-8'><title>Trial Balance Report</title>");
            html.Append("<style>");
            html.Append("td { font-family: 'Segoe UI', Arial, sans-serif; font-size: 10px; }");
            html.Append(".text-right { text-align: right; }");
            html.Append(".credit-balance { color: red; font-weight: bold; }");
            html.Append(".indent-2 { padding-left: 20px; }");
            html.Append(".indent-3 { padding-left: 40px; }");
            html.Append(".indent-4 { padding-left: 60px; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div style='text-align:center; margin-bottom:20px;'>");
            html.Append("<h3>BAHRIA TOWN KARACHI</h3>");
            html.Append("<h4>TRIAL BALANCE REPORT</h4>");
            html.Append("<p>Opening As On: " + openingDate.ToString("dd-MM-yyyy") + "</p>");
            html.Append("<p>Period: " + fromDate.ToString("dd-MM-yyyy") + " To " + toDate.ToString("dd-MM-yyyy") + "</p>");
            html.Append("<p>Posting Status: " + ddlPostingStatus.SelectedItem.Text + "</p>");
            html.Append("<p>Include zero opening balance: " + (showZeroOpening ? "Yes" : "No") + "</p>");
            html.Append("</div>");

            html.Append("<table border='1' cellpadding='4' cellspacing='0' style='border-collapse:collapse; width:100%;'>");

            // Header Row 1
            html.Append("<tr style='background-color:#0f7c57; color:white;'>");
            html.Append("<th rowspan='2'>CODE</th><th rowspan='2'>TITLE</th>");
            html.Append("<th colspan='2'>OPENING BALANCE</th><th colspan='2'>PERIOD</th><th colspan='2'>CLOSING BALANCE</th>");
            html.Append("</tr><tr style='background-color:#0f7c57; color:white;'>");
            html.Append("<th>DEBIT</th><th>CREDIT</th><th>DEBIT</th><th>CREDIT</th><th>DEBIT</th><th>CREDIT</th></tr>");

            foreach (DataRow row in dt.Rows)
            {
                int level = Convert.ToInt32(row["LEVELL"]);
                string title = row["GL_DESCRP"].ToString();
                string indentClass = level == 2 ? "indent-2" : (level == 3 ? "indent-3" : (level == 4 ? "indent-4" : ""));

                decimal openingDebit = Convert.ToDecimal(row["OPENING_DEBIT"]);
                decimal openingCredit = Convert.ToDecimal(row["OPENING_CREDIT"]);
                string openingCreditClass = openingCredit > 0 ? "credit-balance" : "";

                decimal closingDebit = Convert.ToDecimal(row["CLOSING_DEBIT"]);
                decimal closingCredit = Convert.ToDecimal(row["CLOSING_CREDIT"]);
                string closingCreditClass = closingCredit > 0 ? "credit-balance" : "";

                html.Append("<tr>");
                html.Append("<td>" + row["GL_CODE"].ToString() + "</td>");
                html.Append("<td class='" + indentClass + "'>" + title + "</td>");
                html.Append("<td class='text-right'>" + openingDebit.ToString("N2") + "</td>");
                html.Append("<td class='text-right " + openingCreditClass + "'>" + openingCredit.ToString("N2") + "</td>");
                html.Append("<td class='text-right'>" + Convert.ToDecimal(row["PERIOD_DEBIT"]).ToString("N2") + "</td>");
                html.Append("<td class='text-right'>" + Convert.ToDecimal(row["PERIOD_CREDIT"]).ToString("N2") + "</td>");
                html.Append("<td class='text-right'>" + closingDebit.ToString("N2") + "</td>");
                html.Append("<td class='text-right " + closingCreditClass + "'>" + closingCredit.ToString("N2") + "</td>");
                html.Append("</tr>");
            }

            html.Append("</table>");
            html.Append("<div style='margin-top:20px; text-align:center; font-size:9px; color:#999;'>");
            html.Append("This is a computer generated document - No signature required");
            html.Append("</div></body></html>");

            Response.Write(html.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Excel Error: " + ex.Message;
            lblStatus.ForeColor = Color.Red;
        }
    }

    protected string GetRowClass(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;
        int level = Convert.ToInt32(row["LEVELL"]);
        if (level == 1) return "level-1";
        if (level == 2) return "level-2";
        if (level == 3) return "level-3";
        if (level == 4) return "level-4";
        return "";
    }

    protected void rptReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView row = (DataRowView)e.Item.DataItem;

            Label lblOpeningCredit = (Label)e.Item.FindControl("lblOpeningCredit");
            if (lblOpeningCredit != null && Convert.ToDecimal(row["OPENING_CREDIT"]) > 0)
                lblOpeningCredit.CssClass = "credit-balance";

            Label lblClosingCredit = (Label)e.Item.FindControl("lblClosingCredit");
            if (lblClosingCredit != null && Convert.ToDecimal(row["CLOSING_CREDIT"]) > 0)
                lblClosingCredit.CssClass = "credit-balance";
        }
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // Required for Excel export
    }
}