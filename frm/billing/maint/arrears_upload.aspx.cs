using System;
using System.IO;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class arrears_upload : System.Web.UI.Page
{
    string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnectionMNT"].ConnectionString;

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        //int M_BM_ID = 0;
        //int M_BM_ID_OLD = 0;
        //int M_RECORD_COUNT = 0;
        lblStatus.Text = "";

        if (!fuCsv.HasFile)
        {
            lblStatus.Text = "Please select a CSV file.";
            lblStatus.ForeColor = System.Drawing.Color.Red;
            return;
        }

        try
            {
                using (OracleConnection con = new OracleConnection(connStr))
                {
                    con.Open();

                    // 🔴 STEP 1: TRUNCATE TABLE
                    using (OracleCommand cmdTrn =
                        new OracleCommand("TRUNCATE TABLE ARR_TABLE", con))
                    {
                        cmdTrn.ExecuteNonQuery();
                    }

                    int insertCount = 0;

                    // 🔵 STEP 2: READ CSV
                    using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                    {
                        string line;
                        bool isHeader = true;

                        while ((line = sr.ReadLine()) != null)
                        {
                            // skip header
                            if (isHeader)
                            {
                                isHeader = false;
                                continue;
                            }

                            string[] cols = line.Split(',');

                            if (cols.Length < 2)
                                continue;

                            using (OracleCommand cmdIns =
                                new OracleCommand(@"INSERT INTO ARR_TABLE (RES_ID, AMT, UPDATE_TYPE) VALUES (:RES_ID, :AMT, 'P')", con))
                            {
                                cmdIns.Parameters.Add(":RES_ID", cols[0].Trim());
                                cmdIns.Parameters.Add(":AMT", cols[1].Trim());

                                cmdIns.ExecuteNonQuery();
                                insertCount++;
                            }

                            lblStatus.Text =
                                "CSV Uploaded Successfully. Total Records Inserted: " + insertCount;
                            lblStatus.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
    }
}
