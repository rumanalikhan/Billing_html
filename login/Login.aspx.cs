using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // no-op; you can log or show messages here if needed
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        lblMessage.Text = ""; // clear previous

        string username = txtUser.Text.Trim();
        string password = txtPass.Text; // do not trim password

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Please enter both username and password.";
            return;
        }

        string connStr = null;
        try
        {
            var cs = ConfigurationManager.ConnectionStrings["MyDbConnection"];
            if (cs == null)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Database connection is not configured (MyDbConnection).";
                return;
            }
            connStr = cs.ConnectionString;
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Config error: " + ex.Message;
            return;
        }

        try
        {
            using (var conn = new OracleConnection(connStr))
            {
                conn.Open();

                string sql = @"SELECT ID, USER_NAME 
                               FROM LOGIN_INFO 
                               WHERE ID = :id AND PASSWORD = :PASSWORD";

                using (var cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("id", username));
                    cmd.Parameters.Add(new OracleParameter("PASSWORD", password));

                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // ===============================
                            // >>> ADDED SESSION VALUES START
                            // ===============================
                            Session["login_id"] = dr["ID"].ToString();
                            Session["login_name"] = dr["USER_NAME"].ToString();
                            Session["system_date"] = DateTime.Now;
                            Session["system_ip"] = Request.UserHostAddress;
                            // ===============================
                            // >>> ADDED SESSION VALUES END
                            // ===============================

                            // OLD logic kept
                            Session["User"] = username;

                            Response.Redirect("~/main_menu/main_menu.aspx");
                            return;
                        }
                        else
                        {
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Text = "Invalid username or password.";
                        }
                    }
                }
            }
        }
        catch (OracleException oex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Database error: " + oex.Message;
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Error: " + ex.Message;
        }
    }
}
