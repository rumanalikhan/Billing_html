using System;
using System.IO;
using System.Globalization;
using System.Web.Configuration;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

public partial class elec_reading_upload : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        DateTime mrDate;
        int M_BG_ID = 0;
        int M_BG_ID_OLD = 0;
        int M_RECORD_COUNT = 0;
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

                // 🔹 STEP 1: Get M_BG_ID from BILL_GENERATE_TOBE
                try
                {
                    using (OracleCommand cmd = new OracleCommand("SELECT BG_ID FROM BILL_GENERATE_TOBE", con))
                    {
                        object result = cmd.ExecuteScalar();
                        M_BG_ID = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                }
                catch
                {
                    M_BG_ID = 0;
                }

                // 🔹 STEP 2: Get M_BG_ID_OLD from BILL_GENERATE WHERE ACTIVE=1
                try
                {
                    using (OracleCommand cmdOld = new OracleCommand("SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N'", con))
                    {
                        object resultOld = cmdOld.ExecuteScalar();
                        M_BG_ID_OLD = (resultOld != null && resultOld != DBNull.Value) ? Convert.ToInt32(resultOld) : 0;
                    }
                }
                catch
                {
                    M_BG_ID_OLD = 0;
                }

                // 🔹 STEP 3: Get M_RECORD_COUNT FROM WATER_METER_READING WHERE BG_ID=M_BG_ID;
                try
                {
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT COUNT(*) FROM MRC_DMY",
                            con))
                    {
                        //                        cmdCnt.Parameters.Add(":BG_ID", M_BG_ID);

                        object result = cmdCnt.ExecuteScalar();

                        M_RECORD_COUNT =
                            (result != null && result != DBNull.Value)
                                ? Convert.ToInt32(result)
                                : 0;
                    }
                }
                catch
                {
                    M_RECORD_COUNT = 0;
                }

                if (M_RECORD_COUNT > 0)
                {
                    using (OracleCommand cmdTrn =
                        new OracleCommand("TRUNCATE TABLE MRC_DMY", con))
                    {
                        cmdTrn.ExecuteNonQuery();
                    }
                }
                // 🔹 Ab M_BG_ID aur M_BG_ID_OLD ke saath baqi CSV upload aur insert ka logic likh sakte hain
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }

        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                List<string> barcodeList = new List<string>();
                List<string> readPrvList = new List<string>();
                List<string> readCrntList = new List<string>();
                List<DateTime> dateList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 4)
                            continue;

                        barcodeList.Add(cols[0].Trim());
                        readPrvList.Add(cols[1].Trim());
                        readCrntList.Add(cols[2].Trim());
                        dateList.Add(DateTime.ParseExact(
                            cols[3].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = barcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO MRC_DMY (BARCODE, READPRV, READCRNT, MRDT) VALUES (:1, :2, :3, :4)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = barcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = readPrvList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Varchar2;
                        p3.Value = readCrntList.ToArray();
                        cmd.Parameters.Add(p3);

                        OracleParameter p4 = new OracleParameter();
                        p4.OracleDbType = OracleDbType.Date;
                        p4.Value = dateList.ToArray();
                        cmd.Parameters.Add(p4);

                        cmd.ExecuteNonQuery();
                    }

                    lblStatus.Text =
                        "CSV Uploaded Successfully. Total Records Inserted: " + rowCount;
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text =
                        "CSV failed to load data. No valid records found.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
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
