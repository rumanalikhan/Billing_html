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
        if (!IsPostBack)
        {
            Session.Clear();
        }
    }

    private string GetClientIPAddress()
    {
        string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length > 0)
                return addresses[0].Trim();
        }

        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        if (ipAddress == "::1")
            return "127.0.0.1";

        return ipAddress;
    }

    private string GetServerIPAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !ip.ToString().StartsWith("169.254"))
                    return ip.ToString();
            }
            return "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }

    private string GetClientHostName(string clientIp)
    {
        if (clientIp == "127.0.0.1")
        {
            return Environment.MachineName;
        }

        if (clientIp.StartsWith("192.168.") || clientIp.StartsWith("10.") ||
            clientIp.StartsWith("172.16.") || clientIp.StartsWith("172.17.") || clientIp.StartsWith("172.18.") ||
            clientIp.StartsWith("172.19.") || clientIp.StartsWith("172.20.") || clientIp.StartsWith("172.21.") ||
            clientIp.StartsWith("172.22.") || clientIp.StartsWith("172.23.") || clientIp.StartsWith("172.24.") ||
            clientIp.StartsWith("172.25.") || clientIp.StartsWith("172.26.") || clientIp.StartsWith("172.27.") ||
            clientIp.StartsWith("172.28.") || clientIp.StartsWith("172.29.") || clientIp.StartsWith("172.30.") ||
            clientIp.StartsWith("172.31."))
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(clientIp);
                if (entry != null && !string.IsNullOrEmpty(entry.HostName))
                {
                    string pcName = entry.HostName.Split('.')[0];
                    if (!pcName.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
                    {
                        return pcName;
                    }
                }
            }
            catch { }
        }

        return clientIp;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";

        string username = txtUser.Text.Trim();
        string password = txtPass.Text;

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
                            // CORRECT MAPPING:
                            string clientIp = GetClientIPAddress();      // 172.16.10.63 (USER's network IP)
                            string serverIp = GetServerIPAddress();      // 172.16.40.56 (SERVER's IP)
                            string clientHostName = GetClientHostName(clientIp); 

                            Session["login_id"] = dr["ID"].ToString();
                            Session["login_name"] = dr["USER_NAME"].ToString();
                            Session["system_date"] = DateTime.Now;
                            Session["system_ip"] = clientIp;
                            Session["User"] = username;

                            // Insert with CORRECT column mapping
                            int logId = LogHelper.CreateLogEntry(
                                dr["ID"].ToString(),
                                1,
                                clientIp,        // USER_IP = Client network IP 
                                clientHostName,  // HOST_NAME = Client PC 
                                serverIp         // WNDO_ID = Server IP 
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