using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class approved_changes : System.Web.UI.Page
{
    //string savedFilePath = "";
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pcdInit();

            if (Session["User"] != null)
            {
                string userId = Session["login_id"].ToString();
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                string ipAddress = Request.UserHostAddress;

                lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;

                if (new[] { "1", "23345", "17101" }.Contains(userId))
                {
                    btnExport.Enabled = true;
                }
                else
                {
                    btnExport.Enabled = false;
                }
            }

            LoadBTKNumbers();
            HasFormAccess();
        }
    }

    private void HasFormAccess()
    {
        int rows = 0;
        int userId = 0;
        int mAllowed = 0;
        string formName = "approved_changes";

        if (Session["User"] != null)
        {
            int.TryParse(Session["login_id"].ToString(), out userId);
        }

        if (userId == 1 || userId == 23345)
        {
            rows = 0;
        }
        else
        {
            string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            string sql = @"SELECT ALLOWED FROM LOGIN_ACCESS WHERE LOGIN_ID = :userId AND FORM_NAME = :formName";

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add("formName", OracleDbType.Varchar2).Value = formName;
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    mAllowed = Convert.ToInt32(result);
                    rows = 1;
                }
                else
                {
                    rows = 0;
                    mAllowed = 0;
                }

                if (rows == 0 || mAllowed == 0)
                {
                    ddlBTKNo.Enabled = false;

                    lblStatus.Text = "You are not authorized to access this form. Please go back to the menu.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }

    private void LoadBTKNumbers()
    {
        int userId = 0;

        if (Session["User"] != null)
        {
            int.TryParse(Session["login_id"].ToString(), out userId);
        }

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string query = @"
            SELECT O.REFCODE
            FROM BIL_ELEC_MODIFYED O
            WHERE NVL(LENGTH(O.APPROVEDBY),0)=0
              AND O.VERSION = (
                    SELECT MAX(I.VERSION)
                    FROM BIL_ELEC_MODIFYED I
                    WHERE I.REFCODE = O.REFCODE
              )
              AND NVL(O.APPROVEDBY,0)=0 
              AND O.DEPT_ID = (
                    SELECT L.DEPT_ID 
                    FROM LOGIN_INFO L 
                    WHERE L.ID=:userId
              )
              AND O.BGID = (
                    SELECT A.BG_ID
                    FROM BILL_GENERATE A 
                    WHERE A.IS_LOCKED='N'
              )
            ORDER BY O.REFCODE";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(query, con))
        {
            cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlBTKNo.DataSource = dt;
                ddlBTKNo.DataTextField = "REFCODE";   // display
                ddlBTKNo.DataValueField = "REFCODE";  // value (same column)
                ddlBTKNo.DataBind();
            }
        }

        ddlBTKNo.Items.Insert(0, new ListItem("-- Select BTK Number --", ""));
    }

    protected void pcdInit()
    {
        lblStatus.Text = "";
        txtK1From_Left.Text = "";
        txtK1To_Left.Text = "";
        txtElDiff_Left.Text = "";
        txtMFactor_Left.Text = "";
        txtUnitsEl_Left.Text = "";
        txtUnitRate_Left.Text = "";
        txtBillCost_Left.Text = "";
        txtK2From_Left.Text = "";
        txtK2To_Left.Text = "";
        txtSlDiff_Left.Text = "";
        txtUnitAdj_Left.Text = "";
        txtAdjRate_Left.Text = "";
        txtK2Amt_Left.Text = "";
        txtFixedRate_Left.Text = "";
        txtNetUnit_Left.Text = "";
        txtUnitPurchase_Left.Text = "";
        txtK3Amt_Left.Text = "";
        txtTltAmt_Left.Text = "";
        txtPrevBal_Left.Text = "";
        txtQtr1_Left.Text = "";
        txtQtr2_Left.Text = "";
        txtQtr3_Left.Text = "";
        txtQtr4_Left.Text = "";
        txtTotAmt_Left.Text = "";
        txtDcCharges_Left.Text = "";
        txtAdvance_Left.Text = "";
        txtArrear_Left.Text = "";
        txtInstalment_Left.Text = "";
        txtFine_Left.Text = "";
        txtONMCharges_Left.Text = "";
        txtBillAmt_Left.Text = "";
        txtLatePayment_Left.Text = "";
        txtAfterDue_Left.Text = "";

        txtK1From_Right.Text = "";
        txtK1To_Right.Text = "";
        txtElDiff_Right.Text = "";
        txtMFactor_Right.Text = "";
        txtUnitsEl_Right.Text = "";
        txtUnitRate_Right.Text = "";
        txtBillCost_Right.Text = "";
        txtK2From_Right.Text = "";
        txtK2To_Right.Text = "";
        txtSlDiff_Right.Text = "";
        txtUnitAdj_Right.Text = "";
        txtAdjRate_Right.Text = "";
        txtK2Amt_Right.Text = "";
        txtFixedRate_Right.Text = "";
        txtNetUnit_Right.Text = "";
        txtUnitPurchase_Right.Text = "";
        txtK3Amt_Right.Text = "";
        txtTltAmt_Right.Text = "";
        txtPrevBal_Right.Text = "";
        txtQtr1_Right.Text = "";
        txtQtr2_Right.Text = "";
        txtQtr3_Right.Text = "";
        txtQtr4_Right.Text = "";
        txtTotAmt_Right.Text = "";
        txtDcCharges_Right.Text = "";
        txtAdvance_Right.Text = "";
        txtArrear_Right.Text = "";
        txtInstalment_Right.Text = "";
        txtFine_Right.Text = "";
        txtONMCharges_Right.Text = "";
        txtBillAmt_Right.Text = "";
        txtLatePayment_Right.Text = "";
        txtAfterDue_Right.Text = "";
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        string btkNo = ddlBTKNo.SelectedValue;

        if (string.IsNullOrEmpty(btkNo))
            return;

        string connStr = WebConfigurationManager
                            .ConnectionStrings["MyDbConnection"]
                            .ConnectionString;

        string updateSql = @"
        UPDATE BIL_ELEC T
        SET
            ( 
             T.READPRV
             ,T.READCURR
             ,T.READDIFF
             ,T.MFACTRATE
             ,T.UNITS
             ,T.UNIT_RATE
             ,T. BILLCOSTNET
             ,T.READ_PRV_SLR
             ,T.READ_CURR_SLR
             ,T.READ_DIFF_SLR
             ,T.UNITS_SLR
             ,T.UNIT_RATE_SLR
             ,T.UNIT_AMOUNT_SLR
             ,T.NET_UNITS_SLR_NET
             ,T.FIXED_RATES_NET
             ,T.NET_AMNT_SLR_NET
             ,T.ALL_K_TOT_NET
             ,T.PRE_AMT
             ,T.Q1
             ,T.Q2
             ,T.Q3
             ,T.Q4
             ,T.TOT_PAYABLE_AMT_NET
             ,T.ADVPAYMNTNET
             ,T.ARREARSNET
             ,T.INSTAMTNET
             ,T.FINECHRGSNET
             ,T.DCCHRGSNET
             ,T.ONMCHRDSNET
             ,T.BILAMNTBDDT
             ,T.BILAMNTLP
             ,T.BILAMNTADDT
             ,T.NETAMT
             ,T.GROSAMT
            ) =
            (
                SELECT
                    NVL(O.READPRV_TOBE,0)
                    ,NVL(O.READCURR_TOBE,0)
                    ,NVL(O.READDIFF_TOBE,0)
                    ,NVL(O.MFACTRATE_TOBE,0)
                    ,NVL(O.UNITS_TOBE,0)
                    ,NVL(O.UNIT_RATE_TOBE,0)
                    ,NVL(O.BILLCOST_TOBE,0)
                    ,NVL(O.READ_PRV_SLR_TOBE,0)
                    ,NVL(O.READ_CURR_SLR_TOBE,0)
                    ,NVL(O.READ_DIFF_SLR_TOBE,0)
                    ,NVL(O.UNITS_SLR_TOBE,0)
                    ,NVL(O.UNIT_RATE_SLR_TOBE,0)
                    ,NVL(O.UNIT_AMOUNT_SLR_TOBE,0)
                    ,NVL(O.NET_UNITS_SLR_NET_TOBE,0)
                    ,NVL(O.FIXED_RATES_NET_TOBE,0)
                    ,NVL(O.NET_AMNT_SLR_NET_TOBE,0)
                    ,NVL(O.ALL_K_TOT_NET_TOBE,0)
                    ,NVL(O.PRE_AMT_TOBE,0)
                    ,NVL(O.Q1_TOBE,0)
                    ,NVL(O.Q2_TOBE,0)
                    ,NVL(O.Q3_TOBE,0)
                    ,NVL(O.Q4_TOBE,0)
                    ,NVL(O.TOT_PAYABLE_AMT_NET_TOBE,0)
                    ,NVL(O.ADVPAYMNT_TOBE,0)
                    ,NVL(O.ARREARS_TOBE,0)
                    ,NVL(O.INSTAMT_TOBE,0)
                    ,NVL(O.FINECHRGS_TOBE,0)
                    ,NVL(O.DCCHRGS_TOBE,0)
                    ,NVL(O.ONMCHRDS_TOBE,0)
                    ,NVL(O.BILAMNTBDDT_TOBE,0)
                    ,NVL(O.BILAMNTLP_TOBE,0)
                    ,NVL(O.BILAMNTADDT_TOBE,0)
                    ,NVL(O.BILAMNTBDDT_TOBE,0)
                    ,NVL(O.BILAMNTADDT_TOBE,0)
                FROM BIL_ELEC_MODIFYED O
                WHERE O.REFCODE = T.REFCODE
                  AND O.VERSION = (
                        SELECT MAX(I.VERSION)
                        FROM BIL_ELEC_MODIFYED I
                        WHERE I.REFCODE = O.REFCODE
                  ) 
                  AND O.BGID=(
                        SELECT A.BG_ID 
                        FROM BILL_GENERATE A 
                        WHERE A.IS_LOCKED='N'
                  )
            )
        WHERE T.REFCODE = :btkNo AND T.BGID=(SELECT B.BG_ID FROM BILL_GENERATE B WHERE B.IS_LOCKED='N')";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(updateSql, con))
        {
            cmd.Parameters.Add(":btkNo", OracleDbType.Varchar2).Value = btkNo;

            con.Open();
            int rows = cmd.ExecuteNonQuery();

            if (rows > 0)
            {
                lblStatus.Text = "Bill updated successfully";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblStatus.Text = "No record updated. Please verify data.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        int userId = 0;
        if (Session["User"] != null)
            {
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                string ipAddress = Request.UserHostAddress;
                int.TryParse(Session["login_id"].ToString(), out userId);
                string updateSql1 = @"
                    UPDATE BIL_ELEC_MODIFYED O SET
                        O.APPROVEDBY=:userId
                        , O.APPROVEDDT=:currentDate
                        , O.APPROVEDIP=:ipAddress 
                    WHERE 
                        O.REFCODE = :btkNo 
                        AND O.VERSION = (SELECT MAX(I.VERSION) FROM BIL_ELEC_MODIFYED I WHERE I.REFCODE = O.REFCODE) 
                        AND O.BGID=(SELECT B.BG_ID FROM BILL_GENERATE B WHERE B.IS_LOCKED='N')";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
                    cmd.Parameters.Add(":currentDate", OracleDbType.Varchar2).Value = currentDate;
                    cmd.Parameters.Add(":ipAddress", OracleDbType.Varchar2).Value = ipAddress;
                    cmd.Parameters.Add(":btkNo", OracleDbType.Varchar2).Value = btkNo;

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();
                }
            }

        LoadBTKNumbers(); 
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pcdInit();
        LoadBTKNumbers();
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                string sql = "SELECT VERSION, BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT, READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST, READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET, FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET, ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT, BLANKCOL, READPRV_TOBE, READCURR_TOBE, READDIFF_TOBE, MFACTRATE_TOBE, UNITS_TOBE, UNIT_RATE_TOBE, BILLCOST_TOBE, READ_PRV_SLR_TOBE, READ_CURR_SLR_TOBE, READ_DIFF_SLR_TOBE, UNITS_SLR_TOBE, UNIT_RATE_SLR_TOBE, UNIT_AMOUNT_SLR_TOBE, NET_UNITS_SLR_NET_TOBE, FIXED_RATES_NET_TOBE, NET_AMNT_SLR_NET_TOBE, ALL_K_TOT_NET_TOBE, PRE_AMT_TOBE, Q1_TOBE, Q2_TOBE, Q3_TOBE, Q4_TOBE, TOT_PAYABLE_AMT_NET_TOBE, ADVPAYMNT_TOBE, ARREARS_TOBE, INSTAMT_TOBE, FINECHRGS_TOBE, DCCHRGS_TOBE, ONMCHRDS_TOBE, BILAMNTBDDT_TOBE, BILAMNTLP_TOBE, BILAMNTADDT_TOBE, REMARKS, MODIFYBY, MODIFYDT, MODIFYIP, APPROVEDBY, APPROVEDDT, APPROVEDIP, DEPT_ID FROM BIL_ELEC_MODIFYED WHERE BGID=(SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N') ORDER BY DEPT_ID DESC, MODIFYBY ASC, BARCODE ASC, VERSION ASC";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    // ---- HEADER ----
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        sb.Append("\"" + dr.GetName(i) + "\"");
                        if (i < dr.FieldCount - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();

                    // ---- DATA ----
                    while (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string value = dr[i] == DBNull.Value
                                ? ""
                                : dr[i].ToString();

                            value = value.Replace("\"", "\"\"");
                            sb.Append("\"" + value + "\"");

                            if (i < dr.FieldCount - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    // ---- DOWNLOAD ----
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "text/csv";
                    Response.AddHeader(
                        "Content-Disposition",
                        "attachment; filename=BIL_ELEC_MODIFYED.csv"
                    );
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.Write(sb.ToString());
                    Response.Flush();
                    Response.End(); // ✔ SAFE HERE
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Export Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string btkNo = ddlBTKNo.SelectedValue;

        pcdInit();

        if (string.IsNullOrEmpty(btkNo))
            return;

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        string sql = @"SELECT 
                                READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOSTNET, 
                                READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, 
                                NET_UNITS_SLR_NET, FIXED_RATES_NET, NET_AMNT_SLR_NET, 
                                ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET, 
                                DCCHRGSNET, ADVPAYMNTNET, ARREARSNET, INSTAMTNET, FINECHRGSNET, ONMCHRDSNET, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT 
                        FROM BIL_ELEC
                        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N') AND REFCODE = :btkNo";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add(new OracleParameter("btkNo", btkNo));

            con.Open();
            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    txtK1From_Left.Text = dr["READPRV"].ToString();
                    txtK1To_Left.Text = dr["READCURR"].ToString();
                    txtElDiff_Left.Text = dr["READDIFF"].ToString();
                    txtMFactor_Left.Text = dr["MFACTRATE"].ToString();
                    txtUnitsEl_Left.Text = dr["UNITS"].ToString();
                    txtUnitRate_Left.Text = dr["UNIT_RATE"].ToString();
                    txtBillCost_Left.Text = Convert.ToDecimal(dr["BILLCOSTNET"]).ToString("N0");
                    txtK2From_Left.Text = dr["READ_PRV_SLR"].ToString();
                    txtK2To_Left.Text = dr["READ_CURR_SLR"].ToString();
                    txtSlDiff_Left.Text = dr["READ_DIFF_SLR"].ToString();
                    txtUnitAdj_Left.Text = dr["UNITS_SLR"].ToString();
                    txtAdjRate_Left.Text = dr["UNIT_RATE_SLR"].ToString();
                    txtK2Amt_Left.Text = dr["UNIT_AMOUNT_SLR"].ToString();
                    txtFixedRate_Left.Text = dr["FIXED_RATES_NET"].ToString();
                    txtNetUnit_Left.Text = dr["NET_AMNT_SLR_NET"].ToString();
                    txtUnitPurchase_Left.Text = dr["FIXED_RATES_NET"].ToString();
                    txtK3Amt_Left.Text = dr["NET_AMNT_SLR_NET"].ToString();
                    txtTltAmt_Left.Text = dr["ALL_K_TOT_NET"].ToString();
                    txtPrevBal_Left.Text = dr["PRE_AMT"].ToString();
                    txtQtr1_Left.Text = dr["Q1"].ToString();
                    txtQtr2_Left.Text = dr["Q2"].ToString();
                    txtQtr3_Left.Text = dr["Q3"].ToString();
                    txtQtr4_Left.Text = dr["Q4"].ToString();
                    txtTotAmt_Left.Text = dr["TOT_PAYABLE_AMT_NET"].ToString();
                    txtDcCharges_Left.Text = dr["DCCHRGSNET"].ToString();
                    txtAdvance_Left.Text = dr["ADVPAYMNTNET"].ToString();
                    txtArrear_Left.Text = dr["ARREARSNET"].ToString();
                    txtInstalment_Left.Text = dr["INSTAMTNET"].ToString();
                    txtFine_Left.Text = dr["FINECHRGSNET"].ToString();
                    txtONMCharges_Left.Text = dr["ONMCHRDSNET"].ToString();
                    txtBillAmt_Left.Text = Convert.ToDecimal(dr["BILAMNTBDDT"]).ToString("N0");
                    txtLatePayment_Left.Text = Convert.ToDecimal(dr["BILAMNTLP"]).ToString("N0");
                    txtAfterDue_Left.Text = Convert.ToDecimal(dr["BILAMNTADDT"]).ToString("N0");

                    btnSearch_Click1();
                    btnPost.Focus();
                }
            }
        }
    }

    protected void btnSearch_Click1()
    {
        string btkNo = ddlBTKNo.SelectedValue;

        if (string.IsNullOrEmpty(btkNo))
            return;
        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        string sql = @"SELECT 
                            O.READPRV_TOBE, O.READCURR_TOBE, O.READDIFF_TOBE, O.MFACTRATE_TOBE, O.UNITS_TOBE, 
                            O.UNIT_RATE_TOBE, O.BILLCOST_TOBE, O.READ_PRV_SLR_TOBE, O.READ_CURR_SLR_TOBE, O.READ_DIFF_SLR_TOBE, 
                            O.UNITS_SLR_TOBE, O.UNIT_RATE_SLR_TOBE, O.UNIT_AMOUNT_SLR_TOBE, O.NET_UNITS_SLR_NET_TOBE, 
                            O.FIXED_RATES_NET_TOBE, O.NET_AMNT_SLR_NET_TOBE, O.ALL_K_TOT_NET_TOBE, O.PRE_AMT_TOBE, 
                            O.Q1_TOBE, O.Q2_TOBE, O.Q3_TOBE, O.Q4_TOBE, O.TOT_PAYABLE_AMT_NET_TOBE, 
                            O.DCCHRGS_TOBE, O.ADVPAYMNT_TOBE, O.ARREARS_TOBE, O.INSTAMT_TOBE, O.FINECHRGS_TOBE, 
                            O.ONMCHRDS_TOBE, O.BILAMNTBDDT_TOBE, O.BILAMNTLP_TOBE, O.BILAMNTADDT_TOBE 
                        FROM BIL_ELEC_MODIFYED O 
                        WHERE O.REFCODE=:btkNo AND O.VERSION=(SELECT MAX(I.VERSION) FROM BIL_ELEC_MODIFYED I WHERE I.REFCODE=O.REFCODE)";
        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add(new OracleParameter("btkNo", btkNo));

            con.Open();
            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    txtK1From_Right.Text = dr["READPRV_TOBE"].ToString();
                    txtK1To_Right.Text = dr["READCURR_TOBE"].ToString();
                    txtElDiff_Right.Text = dr["READDIFF_TOBE"].ToString();
                    txtMFactor_Right.Text = dr["MFACTRATE_TOBE"].ToString();
                    txtUnitsEl_Right.Text = dr["UNITS_TOBE"].ToString();
                    txtUnitRate_Right.Text = dr["UNIT_RATE_TOBE"].ToString();
                    txtBillCost_Right.Text = dr["BILLCOST_TOBE"].ToString();
//                    txtBillCost_Right.Text = Convert.ToDecimal(dr["BILLCOST_TOBE"]).ToString("N0");
                    txtK2From_Right.Text = dr["READ_PRV_SLR_TOBE"].ToString();
                    txtK2To_Right.Text = dr["READ_CURR_SLR_TOBE"].ToString();
                    txtSlDiff_Right.Text = dr["READ_DIFF_SLR_TOBE"].ToString();
                    txtUnitAdj_Right.Text = dr["UNITS_SLR_TOBE"].ToString();
                    txtAdjRate_Right.Text = dr["UNIT_RATE_SLR_TOBE"].ToString();
                    txtK2Amt_Right.Text = dr["UNIT_AMOUNT_SLR_TOBE"].ToString();
//                    txtK2Amt_Right.Text = Convert.ToDecimal(dr["UNIT_AMOUNT_SLR_TOBE"]).ToString("N0");
                    txtFixedRate_Right.Text = dr["FIXED_RATES_NET_TOBE"].ToString();
                    txtNetUnit_Right.Text = dr["NET_AMNT_SLR_NET_TOBE"].ToString();
                    txtUnitPurchase_Right.Text = dr["FIXED_RATES_NET_TOBE"].ToString();
                    txtK3Amt_Right.Text = dr["NET_AMNT_SLR_NET_TOBE"].ToString();
//                    txtK3Amt_Right.Text = Convert.ToDecimal(dr["NET_AMNT_SLR_NET_TOBE"]).ToString("N0");
                    txtTltAmt_Right.Text = dr["ALL_K_TOT_NET_TOBE"].ToString();
                    txtPrevBal_Right.Text = dr["PRE_AMT_TOBE"].ToString();
                    txtQtr1_Right.Text = dr["Q1_TOBE"].ToString();
                    txtQtr2_Right.Text = dr["Q2_TOBE"].ToString();
                    txtQtr3_Right.Text = dr["Q3_TOBE"].ToString();
                    txtQtr4_Right.Text = dr["Q4_TOBE"].ToString();
                    txtTotAmt_Right.Text = dr["TOT_PAYABLE_AMT_NET_TOBE"].ToString();
//                    txtTotAmt_Right.Text = Convert.ToDecimal(dr["TOT_PAYABLE_AMT_NET_TOBE"]).ToString("N0");
                    txtDcCharges_Right.Text = dr["DCCHRGS_TOBE"].ToString();
                    txtAdvance_Right.Text = dr["ADVPAYMNT_TOBE"].ToString();
                    txtArrear_Right.Text = dr["ARREARS_TOBE"].ToString();
                    txtInstalment_Right.Text = dr["INSTAMT_TOBE"].ToString();
                    txtFine_Right.Text = dr["FINECHRGS_TOBE"].ToString();
                    txtONMCharges_Right.Text = dr["ONMCHRDS_TOBE"].ToString();
                    txtBillAmt_Right.Text = dr["BILAMNTBDDT_TOBE"].ToString();
                    txtLatePayment_Right.Text = dr["BILAMNTLP_TOBE"].ToString();
                    txtAfterDue_Right.Text = dr["BILAMNTADDT_TOBE"].ToString();
                }
            }
        }
    }

//    protected void btnFixBill_Click(object sender, EventArgs e)
//    {
//        pcdReCalculateBill();
//    }

//    private decimal ToDecimal(string val)
//    {
//        decimal d;
//        decimal.TryParse(val, out d);
//        return d;
//    }

//    protected void btnPost_Click(object sender, EventArgs e)
//    {
//        string btkNo = txtBTKNo.Text.Trim();
//        if (string.IsNullOrEmpty(btkNo))
//            return;

//        int mTrancode = GetTranCode(btkNo);

//        if (mTrancode <= 0)
//            return;

//        try
//        {
//            InsertIntoModified(mTrancode);
//            UpdateModified(mTrancode);

//            lblStatus.Text = "Electric Bill has been updated successfully...";
//            lblStatus.ForeColor = System.Drawing.Color.Green;
//        }
//        catch (Exception ex)
//        {
//            lblStatus.Text = "Bill Updating has failed...";
//            lblStatus.ForeColor = System.Drawing.Color.Red;
//        }
//    }

//    private int GetTranCode(string btkNo)
//    {
//        int mTrancode = 0;
//        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

//        string sql = @"
//        SELECT TRANCODE
//        FROM BIL_ELEC
//        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')
//          AND REFCODE = :btkNo";

//        using (OracleConnection con = new OracleConnection(connStr))
//        using (OracleCommand cmd = new OracleCommand(sql, con))
//        {
//            cmd.Parameters.Add("btkNo", OracleDbType.Varchar2).Value = btkNo;
//            con.Open();

//            object result = cmd.ExecuteScalar();
//            if (result != null && result != DBNull.Value)
//                mTrancode = Convert.ToInt32(result);
//        }

//        return mTrancode;
//    }

//    private void InsertIntoModified(int mTrancode)
//    {
//        int rows=0;

//        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

//        string sql = @"
//        INSERT INTO BIL_ELEC_MODIFYED (
//            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
//            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
//            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
//            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
//            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT
//        )
//        SELECT
//            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
//            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
//            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
//            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
//            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT
//        FROM BIL_ELEC
//        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')
//          AND TRANCODE = :mTrancode";

//        using (OracleConnection con = new OracleConnection(connStr))
//        using (OracleCommand cmd = new OracleCommand(sql, con))
//        {
//            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;
//            con.Open();
//            cmd.ExecuteNonQuery();
//            if (rows == 0)
//            {
//                rows = 0;
//            }
//        }
//    }

//    private void UpdateModified(int mTrancode)
//    {
//        int userId = 0;
//        string userName = "";
//        string currentDate = "";
//        string ipAddress = "";

//        if (Session["User"] != null)
//        {
//            int.TryParse(Session["login_id"].ToString(), out userId);
//            userName = Session["login_name"].ToString();
//            currentDate = DateTime.Now.ToString("dd-MMM-yy");
//            ipAddress = Request.UserHostAddress;

//            lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
//        }

//        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
//        string sql = @"
//        UPDATE BIL_ELEC_MODIFYED O SET
//            O.READPRV_TOBE     = :K1From,
//            O.READCURR_TOBE    = :K1To,
//            O.READDIFF_TOBE    = :ElDiff,
//            O.MFACTRATE_TOBE   = :MFactor,
//            O.UNITS_TOBE       = :UnitsEl,
//            O.UNIT_RATE_TOBE   = :UnitRate,
//            O.BILLCOST_TOBE    = :BillCost,
//
//            O.READ_PRV_SLR_TOBE = :K2From, 
//            O.READ_CURR_SLR_TOBE = :K2To, 
//            O.READ_DIFF_SLR_TOBE = :SlDiff, 
//            O.UNITS_SLR_TOBE = :UnitAdj, 
//            O.UNIT_RATE_SLR_TOBE = :AdjRate, 
//            O.UNIT_AMOUNT_SLR_TOBE = :K2Amt, 
//
//            O.NET_UNITS_SLR_NET_TOBE = :NetUnit, 
//            O.FIXED_RATES_NET_TOBE = 22.42, 
//            O.NET_AMNT_SLR_NET_TOBE = :K3Amt, 
//            O.ALL_K_TOT_NET_TOBE = :TltAmt, 
//
//            O.PRE_AMT_TOBE = :PrevBal, 
//            O.Q1_TOBE = :Qtr1, 
//            O.Q2_TOBE = :Qtr2, 
//            O.Q3_TOBE = :Qtr3, 
//            O.Q4_TOBE = :Qtr4, 
//            O.TOT_PAYABLE_AMT_NET_TOBE = :TotAmt, 
//
//            O.DCCHRGS_TOBE     = :DcCharges,
//            O.ADVPAYMNT_TOBE   = :Advance,
//            O.ARREARS_TOBE     = :Arrear,
//            O.INSTAMT_TOBE     = :Instalment,
//            O.FINECHRGS_TOBE   = :Fine,
//
//            O.ONMCHRDS_TOBE    = :ONMCharges,
//            O.BILAMNTBDDT_TOBE = :BillAmt,
//            O.BILAMNTLP_TOBE   = :LatePayment,
//            O.BILAMNTADDT_TOBE = :AfterDue,
//
//            O.REMARKS=:Remarks,
//            O.MODIFYBY=:userId,
//            O.MODIFYDT=:currentDate,
//            O.MODIFYIP=:ipAddress
//        WHERE O.TRANCODE = :mTrancode AND O.VERSION=(SELECT MAX(I.VERSION) FROM BIL_ELEC_MODIFYED I WHERE I.TRANCODE=O.TRANCODE)";

//        using (OracleConnection con = new OracleConnection(connStr))
//        using (OracleCommand cmd = new OracleCommand(sql, con))
//        {
//            cmd.Parameters.Add("K1From", OracleDbType.Decimal).Value = ToDecimal(txtK1From.Text);
//            cmd.Parameters.Add("K1To", OracleDbType.Decimal).Value = ToDecimal(txtK1To.Text);
//            //cmd.Parameters.Add("ElDiff", OracleDbType.Decimal).Value = ToDecimal(txtElDiff.Text);
//            //cmd.Parameters.Add("MFactor", OracleDbType.Decimal).Value = ToDecimal(txtMFactor.Text);
//            //cmd.Parameters.Add("UnitsEl", OracleDbType.Decimal).Value = ToDecimal(txtUnitsEl.Text);
//            //cmd.Parameters.Add("UnitRate", OracleDbType.Decimal).Value = ToDecimal(txtUnitRate.Text);
//            //cmd.Parameters.Add("BillCost", OracleDbType.Decimal).Value = ToDecimal(txtBillCost.Text);

//            cmd.Parameters.Add("K2From", OracleDbType.Decimal).Value = ToDecimal(txtK2From.Text);
//            cmd.Parameters.Add("K2To", OracleDbType.Decimal).Value = ToDecimal(txtK2To.Text);
//            cmd.Parameters.Add("SlDiff", OracleDbType.Decimal).Value = ToDecimal(txtSlDiff.Text);
//            cmd.Parameters.Add("UnitAdj", OracleDbType.Decimal).Value = ToDecimal(txtUnitAdj.Text);
//            cmd.Parameters.Add("AdjRate", OracleDbType.Decimal).Value = ToDecimal(txtAdjRate.Text);
//            cmd.Parameters.Add("K2Amt", OracleDbType.Decimal).Value = ToDecimal(txtK2Amt.Text);

//            cmd.Parameters.Add("NetUnit", OracleDbType.Decimal).Value = ToDecimal(txtNetUnit.Text);
//            cmd.Parameters.Add("K3Amt", OracleDbType.Decimal).Value = ToDecimal(txtK3Amt.Text);

//            cmd.Parameters.Add("TltAmt", OracleDbType.Decimal).Value = ToDecimal(txtTltAmt.Text);
//            cmd.Parameters.Add("PrevBal", OracleDbType.Decimal).Value = ToDecimal(txtPrevBal.Text);
//            cmd.Parameters.Add("Qtr1", OracleDbType.Decimal).Value = ToDecimal(txtQtr1.Text);
//            cmd.Parameters.Add("Qtr2", OracleDbType.Decimal).Value = ToDecimal(txtQtr2.Text);
//            cmd.Parameters.Add("Qtr3", OracleDbType.Decimal).Value = ToDecimal(txtQtr3.Text);
//            cmd.Parameters.Add("Qtr4", OracleDbType.Decimal).Value = ToDecimal(txtQtr4.Text);
//            cmd.Parameters.Add("TotAmt", OracleDbType.Decimal).Value = ToDecimal(txtTotAmt.Text);

//            cmd.Parameters.Add("DcCharges", OracleDbType.Decimal).Value = ToDecimal(txtDcCharges.Text);
//            cmd.Parameters.Add("Advance", OracleDbType.Decimal).Value = ToDecimal(txtAdvance.Text);
//            cmd.Parameters.Add("Arrear", OracleDbType.Decimal).Value = ToDecimal(txtArrear.Text);
//            cmd.Parameters.Add("Instalment", OracleDbType.Decimal).Value = ToDecimal(txtInstalment.Text);
//            cmd.Parameters.Add("Fine", OracleDbType.Decimal).Value = ToDecimal(txtFine.Text);

//            cmd.Parameters.Add("ONMCharges", OracleDbType.Decimal).Value = ToDecimal(txtONMCharges.Text);
//            cmd.Parameters.Add("BillAmt", OracleDbType.Decimal).Value = ToDecimal(txtBillAmt.Text);
//            cmd.Parameters.Add("LatePayment", OracleDbType.Decimal).Value = ToDecimal(txtLatePayment.Text);
//            cmd.Parameters.Add("AfterDue", OracleDbType.Decimal).Value = ToDecimal(txtAfterDue.Text);

//            cmd.Parameters.Add("Remarks", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();

//            cmd.Parameters.Add("userId", OracleDbType.Decimal).Value = Convert.ToDecimal(userId);
//            cmd.Parameters.Add("currentDate", OracleDbType.Date).Value = DateTime.Now;
//            cmd.Parameters.Add("ipAddress", OracleDbType.Varchar2).Value = ipAddress;

//            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;

//            con.Open();
//            cmd.ExecuteNonQuery();
//        }
//    }

}
