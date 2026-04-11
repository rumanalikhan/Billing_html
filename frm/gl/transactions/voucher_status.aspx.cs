using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class voucher_status : System.Web.UI.Page
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

            // Set current status label - handle NULL values
            Label lblCurrentStatus = (Label)e.Row.FindControl("lblCurrentStatus");
            object postValue = drv["POST"];
            
            int post = 0;
            if (postValue != DBNull.Value && postValue != null)
            {
                post = Convert.ToInt32(postValue);
            }

            if (post == 1)
            {
                lblCurrentStatus.Text = "Posted";
                lblCurrentStatus.CssClass = "current-status status-posted";
            }
            else
            {
                lblCurrentStatus.Text = "Unposted";
                lblCurrentStatus.CssClass = "current-status status-unposted";
            }

            // Set the dropdown to current value
            DropDownList ddlNewStatus = (DropDownList)e.Row.FindControl("ddlNewStatus");
            if (ddlNewStatus != null)
            {
                ddlNewStatus.SelectedValue = post.ToString();
            }
        }
    }

    protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "UpdateStatus")
        {
            string voucherKey = e.CommandArgument.ToString();
            
            // Find the row and get the selected status
            foreach (GridViewRow row in gvResults.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Find the voucher key in the row
                    string rowVoucherKey = row.Cells[0].Text;
                    if (rowVoucherKey == voucherKey)
                    {
                        DropDownList ddlNewStatus = (DropDownList)row.FindControl("ddlNewStatus");
                        if (ddlNewStatus != null && !string.IsNullOrEmpty(ddlNewStatus.SelectedValue))
                        {
                            int newStatus = Convert.ToInt32(ddlNewStatus.SelectedValue);
                            UpdateVoucherStatus(voucherKey, newStatus);
                        }
                        else
                        {
                            ShowStatus("Please select a status to update", "error");
                        }
                        break;
                    }
                }
            }
        }
    }

    private void UpdateVoucherStatus(string voucherKey, int newStatus)
    {
        try
        {
            int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    string query = @"UPDATE GL_FORMS 
                                    SET POST = :post, LOG_ID = :logId
                                    WHERE VOUCHER_KEY = :voucherKey AND COMP_ID = :compId";

                    OracleCommand cmd = new OracleCommand(query, conn);
                    cmd.Parameters.Add("post", OracleDbType.Int32).Value = newStatus;
                    cmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;
                    cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;
                    cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        Session["CurrentLogId"] = transactionLogId;
                        
                        string statusText = newStatus == 1 ? "Posted" : "Unposted";
                        ShowStatus("Voucher "+voucherKey+" status updated to "+statusText+" successfully!", "success");
                        
                        // Refresh the grid
                        SearchVouchers();
                    }
                    else
                    {
                        transaction.Rollback();
                        ShowStatus("No voucher found to update", "error");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowStatus("Database error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
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

        if (type != "error")
        {
            string script = "setTimeout(function() { var elem = document.getElementById('" + statusContainer.ClientID + "'); if(elem) { elem.style.display = 'none'; } }, 2000);";
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

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }
}