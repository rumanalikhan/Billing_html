using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class water_bill_posting : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionWTR"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadCounts();
        }
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                using (OracleCommand cmd =
                    new OracleCommand("SP_POST_BIL_WATER", con))
                {
                    cmd.CommandType =
                        System.Data.CommandType.StoredProcedure;

                    cmd.ExecuteNonQuery();

                    lblStatus.Text =
                        "Water bills posted successfully.";
                    lblStatus.ForeColor =
                        System.Drawing.Color.Green;
                }

                LoadCounts();
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    private void LoadCounts()
    {
        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();

            // Posted
            using (OracleCommand cmd =
                new OracleCommand(
                    "SELECT 'Bill Count='|| TO_CHAR(COUNT(*), 'FM999G999G999G999')|| ' and Bill Cost='|| TO_CHAR(NVL(ROUND(SUM(NET_PAYBLE), 0), 0),'FM999G999G999G999') FROM BILLS_WATER B WHERE B.BM_ID=(SELECT G.BM_ID FROM WATER_DATES G WHERE G.ACTIVE=1)",
                    con))
            {
                object val = cmd.ExecuteScalar();
                txtPosted.Text =
                    val == null ? "0" : val.ToString();
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
    }
}
