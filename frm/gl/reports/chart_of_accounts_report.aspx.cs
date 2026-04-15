using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Text;
using System.Web.UI;

public partial class chart_of_accounts_report : System.Web.UI.Page
{
    private readonly string connStr = WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set print date
            lblPrintDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            LoadReport();
        }
    }

    private void LoadReport()
    {
        try
        {
            DataTable dt = GetChartOfAccountsData();
            AddIndentationToDescription(dt, false);  // Pass false for normal view
            gvReport.DataSource = dt;
            gvReport.DataBind();
            lblStatus.Text = "Report loaded successfully. Total records: " + dt.Rows.Count;
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }
    private void AddIndentationToDescription(DataTable dt)
    {
        foreach (DataRow row in dt.Rows)
        {
            int level = Convert.ToInt32(row["LEVELL"]);
            string description = row["GL_DESCRP"].ToString();

            // Create indentation based on level (level 1 = 0 spaces, level 2 = 4 spaces, level 3 = 8 spaces, etc.)
            string indent = new string(' ', (level - 1) * 4);

            // Update the description with indentation
            row["GL_DESCRP"] = indent + description;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/gl/setup/chart_of_accounts.aspx", false);
    }

    private DataTable GetChartOfAccountsData()
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql = @"
                SELECT 
                    DECODE(gl.family, 
                        'A', 'ASSETS',
                        'L', 'LIABILITIES', 
                        'C', 'CAPITAL',
                        'R', 'REVENUE',
                        'E', 'EXPENSES'
                    ) AS FAMILY_NAME,
                    GL.LEVELL,
                    GL.GL_CODE,
                    GL.GL_DESCRP,
                    NVL(OB.OPENING_BALANCE, 0) AS OPENING_BALANCE
                FROM GL_GLMF GL
                LEFT JOIN GL_GLMF_OPENING_BALANCE OB ON OB.GL_CODE = GL.GL_CODE
                ORDER BY 
                    DECODE(gl.family, 'A', 1, 'L', 2, 'C', 3, 'R', 4, 'E', 5),
                    GL.GL_CODE";

            using (OracleCommand cmd = new OracleCommand(sql, conn))
            {
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    dt.Load(dr);
                }
            }
        }

        return dt;
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = GetChartOfAccountsData();
            AddIndentationToDescription(dt, true);  // Pass true for Excel

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=ChartOfAccounts_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls");
            Response.Charset = "";

            StringBuilder html = new StringBuilder();

            html.Append("<html><head><meta charset='UTF-8'><title>Chart of Accounts Report</title>");
            html.Append("<style>");
            html.Append("td { font-family: 'Segoe UI', Arial, sans-serif; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div style='text-align:center; margin-bottom:20px;'>");
            html.Append("<h2>BAHRIA TOWN KARACHI</h2>");
            html.Append("<p>GL ACCOUNTING SYSTEM</p>");
            html.Append("<h3>CHART OF ACCOUNTS REPORT</h3>");
            html.Append("<p>Printed on: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt") + "</p>");
            html.Append("</div>");

            html.Append("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse; width:100%;'>");
            html.Append("<thead><tr style='background-color:#0f7c57; color:white;'>");
            html.Append("<th>Family</th><th>Level</th><th>Account Code</th><th>Description</th><th>Opening Balance</th>");
            html.Append("</tr></thead><tbody>");

            foreach (DataRow row in dt.Rows)
            {
                html.Append("<tr>");
                html.Append("<td>" + row["FAMILY_NAME"].ToString() + "</td>");
                html.Append("<td style='text-align:center;'>" + row["LEVELL"].ToString() + "</td>");
                html.Append("<td>" + row["GL_CODE"].ToString() + "</td>");
                html.Append("<td>" + row["GL_DESCRP"].ToString() + "</td>");
                html.Append("<td style='text-align:right;'>" + Convert.ToDecimal(row["OPENING_BALANCE"]).ToString("N2") + "</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody></table>");
            html.Append("<div style='margin-top:30px; text-align:center; font-size:10px; color:#999;'>");
            html.Append("This is a computer generated document - No signature required");
            html.Append("</div>");
            html.Append("</body></html>");

            Response.Write(html.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error exporting to Excel: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        // Required for Excel export
    }
    private void AddIndentationToDescription(DataTable dt, bool forExcel = false)
    {
        foreach (DataRow row in dt.Rows)
        {
            int level = Convert.ToInt32(row["LEVELL"]);
            string description = row["GL_DESCRP"].ToString();

            string indent;
            if (forExcel)
            {
                // Use non-breaking spaces for Excel
                indent = new string('\u00A0', (level - 1) * 4);
            }
            else
            {
                // Use regular spaces for HTML/Print
                indent = new string(' ', (level - 1) * 4);
            }

            row["GL_DESCRP"] = indent + description;
        }
    }

}