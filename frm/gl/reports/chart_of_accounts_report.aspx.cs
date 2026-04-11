using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Text;

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

            // Add indentation to the description based on level
            AddIndentationToDescription(dt);

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
}