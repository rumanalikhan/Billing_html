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
        int M_BM_ID = 0;
        int M_BM_ID_OLD = 0;
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

                // 🔹 STEP 1: Get M_BM_ID from WATER_DATES_TOBE
                try
                {
                    using (OracleCommand cmd = new OracleCommand("SELECT BM_ID FROM WATER_DATES_TOBE", con))
                    {
                        object result = cmd.ExecuteScalar();
                        M_BM_ID = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                }
                catch
                {
                    M_BM_ID = 0;
                }

                // 🔹 STEP 2: Get M_BM_ID_OLD from WATER_DATES WHERE ACTIVE=1
                try
                {
                    using (OracleCommand cmdOld = new OracleCommand("SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1", con))
                    {
                        object resultOld = cmdOld.ExecuteScalar();
                        M_BM_ID_OLD = (resultOld != null && resultOld != DBNull.Value) ? Convert.ToInt32(resultOld) : 0;
                    }
                }
                catch
                {
                    M_BM_ID_OLD = 0;
                }

                // 🔹 STEP 3: Get M_RECORD_COUNT FROM WATER_METER_READING WHERE BM_ID=M_BM_ID;
                try
                {
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT COUNT(*) FROM WATER_METER_READING WHERE BM_ID = :BM_ID",
                            con))
                    {
                        cmdCnt.Parameters.Add(":BM_ID", M_BM_ID);

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
                    using (OracleCommand cmdDel =
                        new OracleCommand(
                            "DELETE FROM WATER_METER_READING WHERE BM_ID = :BM_ID",
                            con))
                    {
                        cmdDel.Parameters.Add(":BM_ID", M_BM_ID);
                        cmdDel.ExecuteNonQuery();
                    }
                }
                // 🔹 Ab M_BM_ID aur M_BM_ID_OLD ke saath baqi CSV upload aur insert ka logic likh sakte hain
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
                                new OracleCommand(@"INSERT INTO MRC_DMY (REF_NO, METERNO, CR, READ_DT) VALUES (:REF_NO, :METERNO, :CR, :READ_DT)", con))
                            {
                                cmdIns.Parameters.Add(":REF_NO", cols[0].Trim());
                                cmdIns.Parameters.Add(":METERNO", cols[1].Trim());
                                cmdIns.Parameters.Add(":CR", cols[2].Trim());
                                cmdIns.Parameters.Add(":READ_DT", OracleDbType.Date).Value = DateTime.ParseExact(cols[3].Trim(),"dd-MMM-yy",System.Globalization.CultureInfo.InvariantCulture);

                                cmdIns.ExecuteNonQuery();
                                insertCount++;
                            }
                        }
                    }

                    // 🔵 STEP 3: CHECK COUNT AND SHOW MESSAGE
                    using (OracleCommand cmdCount = new OracleCommand("SELECT COUNT(*) FROM MRC_DMY", con))
                    {
                        object cntObj = cmdCount.ExecuteScalar();
                        int count = 0;

                        if (cntObj != null && cntObj != DBNull.Value)
                            count = Convert.ToInt32(cntObj);

                        if (count > 0)
                        {
                            string loginId = Session["LOGIN_ID"] != null ? Session["LOGIN_ID"].ToString() : "SYSTEM";

                            using (OracleCommand cmdInsert =
                                new OracleCommand(@"
                                    INSERT INTO WATER_METER_READING
                                    (BM_ID, REF_ID, METER_NO, READING_TO, READING_DATE, DT_CREATED, CREATED_BY)
                                    SELECT :BM_ID,
                                           REF_NO,
                                           METERNO,
                                           CR,
                                           READ_DT,
                                           SYSDATE,
                                           :CREATED_BY
                                      FROM MRC_DMY", con))
                            {
                                cmdInsert.Parameters.Add(":BM_ID", M_BM_ID);
                                cmdInsert.Parameters.Add(":CREATED_BY", loginId);

                                int rowsInserted = cmdInsert.ExecuteNonQuery();
                                lblStatus.Text = "Records Inserted: " + rowsInserted;
                                lblStatus.ForeColor = System.Drawing.Color.Green;
                            }
                            
                            using (OracleCommand cmdUpd = new OracleCommand(@"
                                MERGE INTO WATER_METER_READING T
                                USING (
                                    SELECT S.REF_NO,
                                           S.WATER_METER_TO
                                      FROM BILLS_WATER S
                                     WHERE S.BM_ID = :BM_ID_OLD
                                ) S
                                ON (
                                    T.REF_ID = S.REF_NO
                                    AND T.BM_ID = :BM_ID
                                )
                                WHEN MATCHED THEN
                                UPDATE SET T.READING_FROM = S.WATER_METER_TO", con))
                            {
                                cmdUpd.BindByName = true;

                                cmdUpd.Parameters.Add(":BM_ID_OLD", OracleDbType.Int32).Value = M_BM_ID_OLD;
                                cmdUpd.Parameters.Add(":BM_ID", OracleDbType.Int32).Value = M_BM_ID;

                                int rows = cmdUpd.ExecuteNonQuery();
                                lblStatus.Text = "Rows Updated: " + rows;
                            }

                            using (OracleCommand cmdUpd = new OracleCommand(@"
                                MERGE INTO WATER_METER_READING T
                                USING (
                                    SELECT RB.BARCODE, RB.RES_ID
                                      FROM BILLING.RESIDENTIAL_BARCODE RB
                                     WHERE RB.BTYPE_ID = 2
                                ) S
                                ON (
                                    T.REF_ID = S.BARCODE
                                    AND T.BM_ID = :BM_ID
                                )
                                WHEN MATCHED THEN
                                    UPDATE SET T.RES_ID = S.RES_ID
                            ", con))
                            {
                                cmdUpd.BindByName = true;   // 🔴 VERY IMPORTANT

                                cmdUpd.Parameters.Add(":BM_ID", OracleDbType.Int32).Value = M_BM_ID;

                                int rows = cmdUpd.ExecuteNonQuery();
                                lblStatus.Text = "RES_ID Updated Rows: " + rows;
                                lblStatus.ForeColor = System.Drawing.Color.Green;
                            }
                            
                            lblStatus.Text =
                                "CSV Uploaded Successfully. Total Records Inserted: " + insertCount;
                            lblStatus.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            lblStatus.Text =
                                "CSV failed to load data. Total Records Inserted: " + insertCount;
                            lblStatus.ForeColor = System.Drawing.Color.Red;
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
