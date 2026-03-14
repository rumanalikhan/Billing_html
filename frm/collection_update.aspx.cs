using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;


public partial class frm_billing_Default : System.Web.UI.Page
{
    /* CONNECTION STRING */
    private readonly string connStrMAINT =
        WebConfigurationManager.ConnectionStrings["MyDbConnectionMNT"].ConnectionString;

    private readonly string connStrELEC =
    WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

    private readonly string connStrWATER =
    WebConfigurationManager.ConnectionStrings["MyDbConnectionWTR"].ConnectionString;


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

        rbBillType.SelectedValue = "1"; // default = Electric
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

            lblStatus.ForeColor = System.Drawing.Color.Green;
            lblStatus.Text = "Upload successful.";
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

    private void pcdProcessCsvAndUpload()
    {
        int insertCount = 0;
        int updateCount = 0;
        switch (rbBillType.SelectedValue)
        {
            case "1": // Maintenance
                pcdMaintUpdate();
                break; 
            case "2": // Electric
                pcdElectricUpdate();
                break;
           case "3": // Water
                pcdWaterUpdate();
               break;

           case "4": // Gas
               pcdGasUpdate();
               break;

           case "5": // Rent
               pcdRentUpdate();
               break;

           case "6": // BNB
               pcdBnbUpdate();
               break;
            default:
                throw new Exception("Invalid bill type selected.");
        }
        // =====================================================
        // 🔹 SUCCESS MESSAGE
        // =====================================================
        lblStatus.Text = "Process Completed Successfully! " +
                            "Inserted: " + insertCount + " | " +
                            "Updated: " + updateCount;
    }

    private void pcdMaintUpdate()
    {
        string sql = "TRUNCATE TABLE MAINT_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrMAINT))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO MAINT_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                        MERGE INTO BILL_GENERATE_AMOUNT T
                        USING (SELECT S.AMT,S.REFCODE,R.RES_ID
                        FROM MAINT_COL_DMY S
                        JOIN RESIDENTIAL_BARCODE R
                        ON R.BARCODE = S.REFCODE
                        ) SRC ON (T.RES_ID = SRC.RES_ID
                        AND T.BILL_MONTH = :txtProcessMonth)
                        WHEN MATCHED THEN UPDATE SET T.AMNT_RECEIVE = SRC.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                        MERGE INTO BILL_GENERATE_AMOUNT T
                        USING (SELECT S.DT,S.REFCODE,R.RES_ID
                        FROM MAINT_COL_DMY S
                        JOIN RESIDENTIAL_BARCODE R
                        ON R.BARCODE = S.REFCODE
                        ) SRC ON (T.RES_ID = SRC.RES_ID
                        AND T.BILL_MONTH = :txtProcessMonth)
                        WHEN MATCHED THEN UPDATE SET T.AMNT_REC_DATE = SRC.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private void pcdElectricUpdate()
    {
        string sql = "TRUNCATE TABLE ELEC_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrELEC))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO ELEC_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BIL_ELEC T
                                    USING ELEC_COL_DMY S
                                    ON (T.REFCODE = S.REFCODE AND T.BILLMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMTRECEIVED = S.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BIL_ELEC T
                                    USING ELEC_COL_DMY S
                                    ON (T.REFCODE = S.REFCODE AND T.BILLMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMTRECDT = S.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private void pcdWaterUpdate()
    {
        string sql = "TRUNCATE TABLE WATER_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrWATER))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO WATER_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_WATER T
                                    USING WATER_COL_DMY S
                                    ON (T.BARCODE = S.REFCODE AND T.BILLING_MONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMNT_RECEIVE = S.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_WATER T
                                    USING WATER_COL_DMY S
                                    ON (T.BARCODE = S.REFCODE AND T.BILLING_MONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMNT_REC_DATE = S.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private void pcdGasUpdate()
    {
        string sql = "TRUNCATE TABLE GAS_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrMAINT))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO GAS_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_GAS T
                                    USING GAS_COL_DMY S
                                    ON (T.BARCODE = S.REFCODE AND T.BILLING_MONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMNT_RECEIVE = S.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_GAS T
                                    USING GAS_COL_DMY S
                                    ON (T.BARCODE = S.REFCODE AND T.BILLING_MONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.AMNT_REC_DATE = S.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private void pcdRentUpdate()
    {
        string sql = "TRUNCATE TABLE RENT_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrMAINT))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO RENT_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_RENT T
                                    USING RENT_COL_DMY S
                                    ON (T.REF_NO = S.REFCODE AND T.BILMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.BILL_AMT_REC = S.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILLS_RENT T
                                    USING RENT_COL_DMY S
                                    ON (T.REF_NO = S.REFCODE AND T.BILMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.REC_DATE = S.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private void pcdBnbUpdate()
    {
        string sql = "TRUNCATE TABLE BNB_COL_DMY";

        using (OracleConnection con = new OracleConnection(connStrMAINT))
        {
            con.Open();
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.ExecuteNonQuery();
            }

            try
            {
                List<string> refcodeList = new List<string>();
                List<int> amtList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();

                // 🔵 STEP 1: READ CSV INTO LISTS
                using (StreamReader sr = new StreamReader(fuCsv.FileContent))
                {
                    string line;
                    int amt;
                    bool isHeader = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        string[] cols = line.Split(',');

                        if (cols.Length < 3)
                            continue;

                        refcodeList.Add(cols[0].Trim());
                        if (!int.TryParse(cols[1].Trim(), out amt))
                            continue;

                        amtList.Add(amt);
                        dtList.Add(DateTime.ParseExact(
                            cols[2].Trim(),
                            "dd-MMM-yy",
                            System.Globalization.CultureInfo.InvariantCulture));
                    }
                }

                int rowCount = refcodeList.Count;

                if (rowCount > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "INSERT INTO BNB_COL_DMY (REFCODE, AMT, DT) VALUES (:1, :2, :3)",
                        con))
                    {
                        cmd.ArrayBindCount = rowCount;

                        OracleParameter p1 = new OracleParameter();
                        p1.OracleDbType = OracleDbType.Varchar2;
                        p1.Value = refcodeList.ToArray();
                        cmd.Parameters.Add(p1);

                        OracleParameter p2 = new OracleParameter();
                        p2.OracleDbType = OracleDbType.Varchar2;
                        p2.Value = amtList.ToArray();
                        cmd.Parameters.Add(p2);

                        OracleParameter p3 = new OracleParameter();
                        p3.OracleDbType = OracleDbType.Date;
                        p3.Value = dtList.ToArray();
                        cmd.Parameters.Add(p3);

                        cmd.ExecuteNonQuery();
                    }

                    // -------------------------------
                    // UPDATE 2 - AMTRECDT
                    // -------------------------------
                    OracleTransaction txnUpdate = con.BeginTransaction();
                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILL_BNB T
                                    USING BNB_COL_DMY S
                                    ON (T.REF_NO = S.REFCODE AND T.BILLMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.BILL_AMT_REC = S.AMT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand(@"
                                    MERGE INTO BILL_BNB T
                                    USING BNB_COL_DMY S
                                    ON (T.REF_NO = S.REFCODE AND T.BILLMONTH = :txtProcessMonth)
                                    WHEN MATCHED THEN
                                    UPDATE SET T.REC_DATE = S.DT", con))
                    {
                        cmd.Transaction = txnUpdate;
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":txtProcessMonth", OracleDbType.Varchar2)
                            .Value = txtProcessMonth.Text.Trim();

                        cmd.ExecuteNonQuery();
                    }

                    txnUpdate.Commit();  // ✅ Both updates saved together

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
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    /* =========================
       CSV HELPERS
       ========================= */
/*
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
*/

    /* =========================
       DATABASE OPERATIONS
       ========================= */
/*
    private void pcdTruncateByBillType(OracleConnection con)
    {
        switch (rbBillType.SelectedValue)
        {
            case "1": // Maintenance
                pcdTruncateMaintenance(con);
                break; 

            case "2": // Electric
                pcdTruncateElectric(con);
                break;

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
            default:
                throw new Exception("Invalid bill type selected.");
        }
    }
*/
/*
    private void pcdInsertByBillType(OracleConnection con, OracleTransaction txn, string[] cols)
//    private void pcdInsertByBillType(OracleConnection con, string[] cols)
    {
        switch (rbBillType.SelectedValue)
        {
            case "1": // Maintenance
                pcdInsertElectric(con, cols);
                break;
            
            case "2": // Electric
                pcdInsertElectric(con,txn, cols);
                break;
            
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
            default:
                throw new Exception("Invalid bill type selected.");
        }
    }
*/

    /* =========
       TRUNCATE
       ========= */
/*
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

*/
    /* =======
       INSERT
       ======= */
/*
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
*/

}
