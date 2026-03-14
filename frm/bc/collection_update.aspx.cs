using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI.WebControls;

public partial class frm_billing_Default : System.Web.UI.Page
{
    /* CONNECTION STRING */
    private readonly string connStrELEC =
        WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;


    /* =========================
       PAGE LOAD
       ========================= */

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pcdInit();
        }
    }


    /* =========================
       INITIALIZATION
       ========================= */

    protected void pcdInit()
    {
        lblStatus.Text = "";

        rbBillType.Items.Clear();

        rbBillType.Items.Add(new ListItem("Maintenance", "1"));
        rbBillType.Items.Add(new ListItem("Electric", "2"));
        rbBillType.Items.Add(new ListItem("Water", "3"));
        rbBillType.Items.Add(new ListItem("Gas", "4"));
        rbBillType.Items.Add(new ListItem("Rent", "5"));
        rbBillType.Items.Add(new ListItem("BNB", "6"));

        rbBillType.SelectedValue = "2"; // default = Electric
    }


    /* =========================
       BUTTON EVENTS
       ========================= */

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
        lblStatus.ForeColor = System.Drawing.Color.Red;

        try
        {
            pcdValidateInputs();
            pcdProcessCsvAndUpload();

//            lblStatus.ForeColor = System.Drawing.Color.Green;
//            lblStatus.Text = "Upload successful.";
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Text = "Upload failed: " + ex.Message;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
        txtProcessMonth.Text = "";
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_bs.aspx");
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx");
    }


    /* =========================
       VALIDATION
       ========================= */

    private void pcdValidateInputs()
    {
        if (!fuCsv.HasFile)
            throw new Exception("Please select a CSV file.");

        pcdValidateProcessMonth();

        switch (rbBillType.SelectedValue)
        {
            case "1": // Maintenance
            case "2": // Electric
            case "3": // Water
            case "4": // Gas
            case "5": // Rent
            case "6": // BNB
                break;

            default:
                throw new Exception("Invalid bill type selected.");
        }
    }

    private void pcdValidateProcessMonth()
    {
        string month = txtProcessMonth.Text.Trim();

        if (string.IsNullOrEmpty(month))
            throw new Exception("Process month is required (YYYYMM).");

        if (month.Length != 6 || !month.All(char.IsDigit))
            throw new Exception("Invalid process month format.");

        int mm = int.Parse(month.Substring(4, 2));
        if (mm < 1 || mm > 12)
            throw new Exception("Invalid month in process month.");
    }


    /* =========================
       CORE PROCESS
       ========================= */

    //private void pcdProcessCsvAndUpload()
    //{
    //    using (StreamReader sr = new StreamReader(fuCsv.FileContent))
    //    {
    //        string header = sr.ReadLine();
    //        if (header == null)
    //            throw new Exception("CSV file is empty.");

    //        pcdValidateHeader(header);

    //        using (OracleConnection con = new OracleConnection(connStrELEC))
    //        {
    //            con.Open();

    //            using (OracleTransaction txn = con.BeginTransaction())
    //            {
    //                try
    //                {
    //                    pcdTruncateByBillType(con);

    //                    string line;
    //                    int rowNo = 1;

    //                    while ((line = sr.ReadLine()) != null)
    //                    {
    //                        rowNo++;

    //                        if (string.IsNullOrWhiteSpace(line))
    //                            continue;

    //                        string[] cols = line.Split(',');

    //                        if (cols.Length != 3)
    //                            throw new Exception("Invalid column count at row " + rowNo);

    //                        pcdInsertByBillType(con, cols);
    //                    }

    //                    txn.Commit();

                    
                    
                    
                    
                    
    //                }
    //                catch
    //                {
    //                    txn.Rollback();
    //                    throw;
    //                }
    //            }
    //        }
    //    }
    //}

    private void pcdProcessCsvAndUpload()
    {
        int insertCount = 0;
        int updateCount = 0;

        using (StreamReader sr = new StreamReader(fuCsv.FileContent))
        {
            string header = sr.ReadLine();
            if (header == null)
                throw new Exception("CSV file is empty.");

            pcdValidateHeader(header);

            using (OracleConnection con = new OracleConnection(connStrELEC))
            {
                con.Open();

                // =====================================================
                // 🔹 FIRST TRANSACTION - INSERT
                // =====================================================
                using (OracleTransaction txnInsert = con.BeginTransaction())
                {
                    try
                    {
                        pcdTruncateByBillType(con);

                        string line;
                        int rowNo = 1;

                        while ((line = sr.ReadLine()) != null)
                        {
                            rowNo++;

                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            string[] cols = line.Split(',');

                            if (cols.Length != 3)
                                throw new Exception("Invalid column count at row " + rowNo);

                            pcdInsertByBillType(con, txnInsert, cols);
                            insertCount++;
                        }

                        txnInsert.Commit();
                    }
                    catch
                    {
                        txnInsert.Rollback();
                        throw;
                    }
                }

                // =====================================================
                // 🔹 SECOND TRANSACTION - BOTH UPDATES TOGETHER
                // =====================================================
                using (OracleCommand disableCmd = new OracleCommand(
                    "ALTER TRIGGER TRG_UPDT_DEL_BIL_ELEC DISABLE", con))
                {
                    disableCmd.ExecuteNonQuery();
                } 
                
                using (OracleTransaction txnUpdate = con.BeginTransaction())
                {
                    try
                    {
                        // -------------------------------
                        // UPDATE 1 - AMTRECEIVED
                        // -------------------------------
                        using (OracleCommand cmd1 = new OracleCommand(@"
                            MERGE INTO BIL_ELEC T
                                USING COLLECTION_DMY S
                                ON (T.REFCODE = S.BARCODE AND T.BILLMONTH = :txtProcessMonth)
                                WHEN MATCHED THEN
                                UPDATE SET T.AMTRECEIVED = S.AMT", con))
                        {
                            cmd1.Transaction = txnUpdate;
                            cmd1.BindByName = true;

                            cmd1.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                                .Value = txtProcessMonth.Text.Trim();

                            updateCount = cmd1.ExecuteNonQuery();    // ✅ Capture count here
                        }

                        // -------------------------------
                        // UPDATE 2 - AMTRECDT
                        // -------------------------------
                        using (OracleCommand cmd2 = new OracleCommand(@"
                            MERGE INTO BIL_ELEC T
                            USING COLLECTION_DMY S
                            ON (T.REFCODE = S.BARCODE AND T.BILLMONTH = :txtProcessMonth)
                            WHEN MATCHED THEN
                            UPDATE SET T.AMTRECDT = S.DT", con))
                        {
                            cmd2.Transaction = txnUpdate;
                            cmd2.BindByName = true;

                            cmd2.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                                .Value = txtProcessMonth.Text.Trim();

                            cmd2.ExecuteNonQuery();
                        }

                        txnUpdate.Commit();  // ✅ Both updates saved together
                    }
                    catch
                    {
                        txnUpdate.Rollback();  // ❌ If either fails, both rolled back
                        throw;
                    }
                    finally
                    {
                        // Always Enable Trigger Again
                        using (OracleCommand enableCmd = new OracleCommand(
                            "ALTER TRIGGER TRG_UPDT_DEL_BIL_ELEC ENABLE", con))
                        {
                            enableCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        // =====================================================
        // 🔹 SUCCESS MESSAGE
        // =====================================================
        lblStatus.Text = "Process Completed Successfully! " +
                            "Inserted: " + insertCount + " | " +
                            "Updated: " + updateCount;
    }

    /* =========================
       CSV HELPERS
       ========================= */

    private void pcdValidateHeader(string headerLine)
    {
        string[] expectedELEC = { "REFCODE", "AMTRECEIVED", "AMTRECDT" };

        string[] actual = headerLine
            .Split(',')
            .Select(h => h.Trim().Trim('"').ToUpper())
            .ToArray();

        if (actual.Length != expectedELEC.Length)
            throw new Exception("CSV header column count mismatch.");

        for (int i = 0; i < expectedELEC.Length; i++)
        {
            if (actual[i] != expectedELEC[i])
                throw new Exception("CSV header mismatch at column " + (i + 1));
        }
    }


    /* =========================
       DATABASE OPERATIONS
       ========================= */

    private void pcdTruncateByBillType(OracleConnection con)
    {
        switch (rbBillType.SelectedValue)
        {
            /*
            case "1": // Maintenance
                pcdTruncateMaintenance(con);
                break; 
             */

            case "2": // Electric
                pcdTruncateElectric(con);
                break;

             /*
            case "3": // Water
                pcdTruncateWater(con);
                break;

            case "4": // Gas
                pcdTruncateGas(con);
                break;

            case "5": // Rent
                pcdTruncateRent(con);
                break;

            case "6": // BNB
                pcdTruncateBNB(con);
                break;
            */
            default:
                throw new Exception("Invalid bill type selected.");
        }
    }

    private void pcdInsertByBillType(OracleConnection con, OracleTransaction txn, string[] cols)
//    private void pcdInsertByBillType(OracleConnection con, string[] cols)
    {
        switch (rbBillType.SelectedValue)
        {
            /*
            case "1": // Maintenance
                pcdInsertElectric(con, cols);
                break;
            */
            
            case "2": // Electric
                pcdInsertElectric(con,txn, cols);
                break;
            
            /*
            case "3": // Water
                pcdInsertWater(con, cols);
                break;

            case "4": // Gas
                pcdInsertGas(con, cols);
                break;

            case "5": // Rent
                pcdInsertRent(con, cols);
                break;

            case "6": // BNB
                pcdInsertBNB(con, cols);
                break;
            */
            default:
                throw new Exception("Invalid bill type selected.");
        }
    }


    /* =========
       TRUNCATE
       ========= */

    private void pcdTruncateMaintenance(OracleConnection con)
    {
        string sql = "TRUNCATE TABLE COLLECTION_DMY";

        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private void pcdTruncateElectric(OracleConnection con)
    {
        string sql = "TRUNCATE TABLE COLLECTION_DMY";

        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private void pcdTruncateWater(OracleConnection con)
    {
        using (OracleCommand cmd =
            new OracleCommand("TRUNCATE TABLE COLLECTION_DMY", con))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private void pcdTruncateGas(OracleConnection con)
    {
        using (OracleCommand cmd =
            new OracleCommand("TRUNCATE TABLE COLLECTION_DMY", con))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private void pcdTruncateRent(OracleConnection con)
    {
        using (OracleCommand cmd =
            new OracleCommand("TRUNCATE TABLE COLLECTION_DMY", con))
        {
            cmd.ExecuteNonQuery();
        }
    }

    private void pcdTruncateBNB(OracleConnection con)
    {
        using (OracleCommand cmd =
            new OracleCommand("TRUNCATE TABLE COLLECTION_DMY", con))
        {
            cmd.ExecuteNonQuery();
        }
    }


    /* =======
       INSERT
       ======= */

    private void pcdInsertMaintenance(OracleConnection con, string[] cols)
    {
        string sql = @"
        INSERT INTO 
        ()
        VALUES ()";

        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.BindByName = true;

            cmd.Parameters.Add("", OracleDbType.Varchar2).Value = cols[0].Trim();
            cmd.Parameters.Add("", OracleDbType.Decimal).Value = Convert.ToDecimal(cols[1].Trim());
            cmd.Parameters.Add("", OracleDbType.Date).Value =
                DateTime.ParseExact(cols[2].Trim(), "dd-MMM-yy", CultureInfo.InvariantCulture);

            cmd.ExecuteNonQuery();
        }
    }

    private void pcdInsertElectric(OracleConnection con, OracleTransaction txn, string[] cols)
//    private void pcdInsertElectric(OracleConnection con, string[] cols)
    {
        string sql = @"
            INSERT INTO COLLECTION_DMY
            (BARCODE, AMT, DT)
            VALUES (:REFCODE, :AMTRECEIVED, :AMTRECDT)";

        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Transaction = txn;   // VERY IMPORTANT
            cmd.BindByName = true;

            cmd.Parameters.Add("REFCODE", OracleDbType.Varchar2)
                .Value = cols[0].Trim();

            cmd.Parameters.Add("AMTRECEIVED", OracleDbType.Decimal)
                .Value = Convert.ToDecimal(cols[1].Trim());

            cmd.Parameters.Add("AMTRECDT", OracleDbType.Date)
                .Value = DateTime.ParseExact(
                    cols[2].Trim(),
                    "dd-MMM-yy",
                    CultureInfo.InvariantCulture
                );

            cmd.ExecuteNonQuery();
        }
    }

    private void pcdInsertWater(OracleConnection con, string[] cols)
    {
        string sql = @"
        INSERT INTO 
        ()
        VALUES ()";

        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.BindByName = true;

            cmd.Parameters.Add("", OracleDbType.Varchar2).Value = cols[0].Trim();
            cmd.Parameters.Add("", OracleDbType.Decimal).Value = Convert.ToDecimal(cols[1].Trim());
            cmd.Parameters.Add("", OracleDbType.Date).Value =
                DateTime.ParseExact(cols[2].Trim(), "dd-MMM-yy", CultureInfo.InvariantCulture);

            cmd.ExecuteNonQuery();
        }
    }


}
