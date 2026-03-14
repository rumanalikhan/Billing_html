using System;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace billing_html.queries
{
    public class DbHelper
    {
        private readonly string connStr;

        public DbHelper()
        {
            var cs = ConfigurationManager.ConnectionStrings["MyDbConnection"];
            if (cs == null)
                throw new Exception("Database connection 'MyDbConnection' is not configured in web.config.");
            connStr = cs.ConnectionString;
        }

        public object ExecuteScalar(string sql, OracleParameter[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            using (var conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }

        public int ExecuteNonQuery(string sql, OracleParameter[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentException("SQL query cannot be null or empty.", nameof(sql));

            using (var conn = new OracleConnection(connStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
