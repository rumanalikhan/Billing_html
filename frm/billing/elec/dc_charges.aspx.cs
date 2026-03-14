using System;
using System.IO;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class dc_charges_upload : System.Web.UI.Page
{
    string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

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
                        new OracleCommand("TRUNCATE TABLE DC_CHARGES", con))
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
                                new OracleCommand(@"INSERT INTO DC_CHARGES (RESID, DCAMNT) VALUES (:RESID, :DCAMNT)", con))
                            {
                                cmdIns.Parameters.Add(":RESID", cols[0].Trim());
                                cmdIns.Parameters.Add(":DCAMNT", cols[1].Trim());

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
