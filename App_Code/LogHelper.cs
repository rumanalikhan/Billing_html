using System;
using System.Web;
using System.Web.SessionState;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Sockets;
public static class LogHelper
{
    private static string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    /// <summary>
    /// Creates a new log entry and returns the LOG_ID
    /// </summary>
    public static int CreateLogEntry(string loginId, int compId, string userIp, string hostName, string wndoId)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"INSERT INTO USER_OP_LOG 
                        (LOG_ID, USER_IP, LOG_DATE, HOST_NAME, WNDO_ID, COMP_ID, LOGIN_ID)
                        VALUES 
                        (SEQ_USER_OP_LOG.NEXTVAL, :userIp, SYSDATE, :hostName, :wndoId, :compId, :loginId)
                        RETURNING LOG_ID INTO :logId";

            OracleCommand cmd = new OracleCommand(query, conn);

            // Handle nulls with explicit checks
            if (string.IsNullOrEmpty(userIp))
                cmd.Parameters.Add("userIp", OracleDbType.Varchar2, 15).Value = "0.0.0.0";
            else
                cmd.Parameters.Add("userIp", OracleDbType.Varchar2, 15).Value = userIp;

            if (string.IsNullOrEmpty(hostName))
                cmd.Parameters.Add("hostName", OracleDbType.Varchar2, 30).Value = "UNKNOWN";
            else
                cmd.Parameters.Add("hostName", OracleDbType.Varchar2, 30).Value = hostName;

            if (string.IsNullOrEmpty(wndoId))
                cmd.Parameters.Add("wndoId", OracleDbType.Varchar2, 30).Value = userIp ?? "UNKNOWN";
            else
                cmd.Parameters.Add("wndoId", OracleDbType.Varchar2, 30).Value = wndoId;

            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = compId;

            if (string.IsNullOrEmpty(loginId))
                cmd.Parameters.Add("loginId", OracleDbType.Varchar2, 30).Value = "SYSTEM";
            else
                cmd.Parameters.Add("loginId", OracleDbType.Varchar2, 30).Value = loginId;

            OracleParameter logIdParam = new OracleParameter("logId", OracleDbType.Int32);
            logIdParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(logIdParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            // Convert OracleDecimal to string then to int
            return int.Parse(logIdParam.Value.ToString());
        }
    }

    /// <summary>
    /// Gets the current log ID from session, or creates a new one if not found
    /// </summary>
    public static int GetCurrentLogId(HttpSessionState session, HttpRequest request)
    {
        if (session != null && session["CurrentLogId"] != null)
        {
            return Convert.ToInt32(session["CurrentLogId"]);
        }

        // Create a new log entry if session doesn't have one
        string loginId = "SYSTEM";
        if (session != null && session["login_id"] != null)
        {
            loginId = session["login_id"].ToString();
        }

        int compId = 1;
        if (session != null && session["CurrentCompId"] != null)
        {
            compId = Convert.ToInt32(session["CurrentCompId"]);
        }

        string userIp = "0.0.0.0";
        if (request != null && request.UserHostAddress != null)
        {
            userIp = request.UserHostAddress;
        }

        string hostName = Environment.MachineName;
        if (string.IsNullOrEmpty(hostName))
        {
            hostName = "UNKNOWN";
        }

        string wndoId = userIp;
        if (session != null && session.SessionID != null)
        {
            wndoId = session.SessionID;
        }

        int logId = CreateLogEntry(loginId, compId, userIp, hostName, wndoId);

        if (session != null)
        {
            session["CurrentLogId"] = logId;
        }

        return logId;
    }

    /// <summary>
    /// Creates a new log entry for each transaction
    /// </summary>
    public static int CreateTransactionLog(HttpSessionState session, HttpRequest request)
    {
        string loginId = "SYSTEM";
        if (session != null && session["login_id"] != null)
        {
            loginId = session["login_id"].ToString();
        }

        int compId = 1;
        if (session != null && session["CurrentCompId"] != null)
        {
            compId = Convert.ToInt32(session["CurrentCompId"]);
        }

        // Get proper IP addresses
        string userIp = GetIPv4Address(request);
        string hostName = Environment.MachineName;
        string systemIp = GetLocalIPAddress();

        return CreateLogEntry(loginId, compId, userIp, hostName, systemIp);
    }

    // Helper method to get IPv4 from request
    private static string GetIPv4Address(HttpRequest request)
    {
        if (request == null) return "0.0.0.0";

        string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length > 0)
                return addresses[0];
        }

        ipAddress = request.ServerVariables["REMOTE_ADDR"];
        if (ipAddress == "::1")
            return "127.0.0.1";

        return ipAddress ?? "0.0.0.0";
    }

    // Helper method to get local system IP
    private static string GetLocalIPAddress()
    {
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!ip.ToString().StartsWith("169.254"))
                        return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }
}