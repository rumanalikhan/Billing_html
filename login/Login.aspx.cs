using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Web;
using System.Net;
using System.Net.Sockets;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // no-op; you can log or show messages here if needed
    }

    private string GetIPv4Address()
    {
        string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length > 0)
                return addresses[0];
        }

        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        if (ipAddress == "::1")
            return "127.0.0.1"; // Convert IPv6 localhost to IPv4

        return ipAddress;
    }

    private string GetLocalIPAddress()
    {
        try
        {
            // Get the local machine's IP address (from ipconfig)
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                // Look for IPv4 addresses that are not loopback (127.0.0.1)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    // Skip automatic private IP addressing if needed
                    if (!ip.ToString().StartsWith("169.254"))
                        return ip.ToString();
                }
            }

            // Fallback to first IPv4 found
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }

            return "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
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
                            // USER_IP: The IP from which the request came (network IP)
                            string userIp = GetIPv4Address();

                            // WNDO_ID: The local system IP (from ipconfig)
                            string systemIp = GetLocalIPAddress();

                            //session values
                            Session["login_id"] = dr["ID"].ToString();
                            Session["login_name"] = dr["USER_NAME"].ToString();
                            Session["system_date"] = DateTime.Now;
                            Session["system_ip"] = userIp;
                            Session["User"] = username;

                            // Create log entry using LogHelper class
                            int logId = LogHelper.CreateLogEntry(
                                dr["ID"].ToString(),
                                1, // compId - default to 1
                                userIp,                     // USER_IP (network IP)
                                Environment.MachineName,    // HOST_NAME (computer name)
                                systemIp                    // WNDO_ID (system IP from ipconfig)
                            );

                            Session["CurrentLogId"] = logId;
                            Session["CurrentCompId"] = 1;

                            Response.Redirect("~/main_menu/main_menu.aspx");
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