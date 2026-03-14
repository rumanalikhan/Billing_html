using System;
using System.IO;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class water_reading_upload : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionWTR"]
                        .ConnectionString;

    protected void btnUpload_Click(object sender, EventArgs e)
    {
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
                    new OracleCommand("TRUNCATE TABLE MRC_DMY", con))
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

                        if (cols.Length < 3)
                            continue;

                        using (OracleCommand cmdIns =
                            new OracleCommand(
                                @"INSERT INTO MRC_DMY
                                  (REF_NO, METERNO, CR)
                                  VALUES (:REF_NO, :METERNO, :CR)", con))
                        {
                            cmdIns.Parameters.Add(":REF_NO", cols[0].Trim());
                            cmdIns.Parameters.Add(":METERNO", cols[1].Trim());
                            cmdIns.Parameters.Add(":CR", cols[2].Trim());

                            cmdIns.ExecuteNonQuery();
                            insertCount++;
                        }
                    }
                }

                lblStatus.Text =
                    "CSV Uploaded Successfully. Total Records Inserted: " + insertCount;
                lblStatus.ForeColor = System.Drawing.Color.Green;
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
