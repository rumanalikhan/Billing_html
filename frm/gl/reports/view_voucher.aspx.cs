using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class view_voucher : System.Web.UI.Page
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
            if (Session["Username"] != null)
            {
                lblUser.Text = "Welcome, " + Session["Username"].ToString();
            }
        }
    }

    private void SearchVouchers()
    {
        try
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = @"
                SELECT 
                    f.VOUCHER_KEY,
                    f.BOOK_TYPE,
                    f.VOUCHER_NUMBER,
                    f.VOUCHER_DATE,
                    f.POST,
                    NVL(SUM(v.AMOUNT), 0) AS TOTAL_AMOUNT
                FROM GL_FORMS f
                LEFT JOIN GL_VOUCHERS v ON f.VOUCHER_KEY = v.VOUCHER_KEY
                WHERE 1=1";

                // EXACT MATCH for voucher key
                if (!string.IsNullOrEmpty(txtVoucherKey.Text))
                {
                    query += " AND UPPER(TRIM(f.VOUCHER_KEY)) = UPPER(TRIM(:voucherKey))";
                }

                // FILTER for voucher type
                if (!string.IsNullOrEmpty(ddlVoucherType.SelectedValue))
                {
                    query += " AND f.BOOK_TYPE = :bookType";
                }

                query += @" GROUP BY f.VOUCHER_KEY, f.BOOK_TYPE, f.VOUCHER_NUMBER, f.VOUCHER_DATE, f.POST
                        ORDER BY f.VOUCHER_NUMBER ASC";

                OracleCommand cmd = new OracleCommand(query, conn);

                if (!string.IsNullOrEmpty(txtVoucherKey.Text))
                {
                    cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = txtVoucherKey.Text.Trim();
                }

                if (!string.IsNullOrEmpty(ddlVoucherType.SelectedValue))
                {
                    cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = ddlVoucherType.SelectedValue;
                }

                conn.Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dt);
            }

            gvResults.DataSource = dt;
            gvResults.DataBind();

            if (dt.Rows.Count > 0)
            {
                ShowStatus(dt.Rows.Count + " voucher(s) found", "success");
            }
            else
            {
                ShowStatus("No vouchers found. Please try different search criteria.", "info");
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }
    
    protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView drv = (DataRowView)e.Row.DataItem;

            // Set status label
            Label lblStatus = (Label)e.Row.FindControl("lblStatus");
            int post = Convert.ToInt32(drv["POST"]);

            if (post == 1)
            {
                lblStatus.Text = "Posted";
                lblStatus.CssClass = "status-posted";
            }
            else
            {
                lblStatus.Text = "Unposted";
                lblStatus.CssClass = "status-unposted";
            }
        }
    }

    protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "PrintVoucher")
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            string voucherKey = args[0];
            string voucherType = args[1];

            string url = "~/frm/gl/reports/voucher_report.aspx?VoucherKey=" + Server.UrlEncode(voucherKey) + "&VoucherType=" + voucherType;
            string script = "window.open('" + ResolveUrl(url) + "', '_blank', 'width=900,height=700,scrollbars=yes,resizable=yes,toolbar=yes');";
            ClientScript.RegisterStartupScript(this.GetType(), "OpenVoucher", script, true);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SearchVouchers();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtVoucherKey.Text = "";
        ddlVoucherType.SelectedIndex = 0;
        gvResults.DataSource = null;
        gvResults.DataBind();
        ShowStatus("Form cleared", "info");
    }

    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx", false);
    }

    private void ShowStatus(string message, string type)
    {
        lblStatus.Text = message;
        statusContainer.Visible = true;

        if (type == "success")
            statusContainer.Attributes["class"] = "status-label status-success";
        else if (type == "error")
            statusContainer.Attributes["class"] = "status-label status-error";
        else
            statusContainer.Attributes["class"] = "status-label status-info";

        // Auto hide after 3 seconds for success/info
        if (type != "error")
        {
            string script = "setTimeout(function() { var elem = document.getElementById('" + statusContainer.ClientID + "'); if(elem) { elem.style.display = 'none'; } }, 3000);";
            ClientScript.RegisterStartupScript(this.GetType(), "HideStatus", script, true);
        }
    }

    private void ShowMessage(string message)
    {
        lblMessage.Text = message;
        mpeMessage.Show();
    }

    protected void btnMessageOk_Click(object sender, EventArgs e)
    {
        mpeMessage.Hide();
    }
}