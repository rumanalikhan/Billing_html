using System;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;

public partial class update_elec : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null)
            {
                string userId = Session["login_id"].ToString();
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                string ipAddress = Request.UserHostAddress;

                lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
            }

            pcdInit();
            txtBTKNo.Text = "";
            txtBTKNo.Focus();
        }
    }

    protected void pcdInit()
    {
        txtBarcode.Text = "";
        txtResID.Text = "";
        txtName.Text = "";
        txtAddress.Text = "";
        txtK1From.Text = "";
        txtK1To.Text = "";
        txtElDiff.Text = "";
        txtMFactor.Text = "";
        txtUnitsEl.Text = "";
        txtUnitRate.Text = "";
        txtAdvance.Text = "";
        txtArrear.Text = "";
        txtInstalment.Text = "";
        txtFine.Text = "";
        txtDcCharges.Text = "";
        txtONMCharges.Text = "";
        txtBillCost.Text = "";
        txtBillAmt.Text = "";
        txtLatePayment.Text = "";
        txtAfterDue.Text = "";
    }

    protected void pcdFetchRecord(object sender, EventArgs e)
    {
        string btkNo = txtBTKNo.Text.Trim();

        pcdInit();

        if (string.IsNullOrEmpty(btkNo))
            return; // No BTK No entered

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;
        string sql = @"SELECT BARCODE, RESID, RESNAME, ADDRESS, READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, 
                          ADVPAYMNTNET, ARREARSNET, INSTAMTNET, FINECHRGSNET, DCCHRGSNET, ONMCHRDSNET, BILLCOSTNET, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT
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
                    txtBarcode.Text = dr["BARCODE"].ToString();
                    txtResID.Text = dr["RESID"].ToString();
                    txtName.Text = dr["RESNAME"].ToString();
                    txtAddress.Text = dr["ADDRESS"].ToString();
                    txtK1From.Text = dr["READPRV"].ToString();
                    txtK1To.Text = dr["READCURR"].ToString();
                    txtElDiff.Text = dr["READDIFF"].ToString();
                    txtMFactor.Text = dr["MFACTRATE"].ToString();
                    txtUnitsEl.Text = dr["UNITS"].ToString();
                    txtUnitRate.Text = dr["UNIT_RATE"].ToString();
                    txtAdvance.Text = dr["ADVPAYMNTNET"].ToString();
                    txtArrear.Text = dr["ARREARSNET"].ToString();
                    txtInstalment.Text = dr["INSTAMTNET"].ToString();
                    txtFine.Text = dr["FINECHRGSNET"].ToString();
                    txtDcCharges.Text = dr["DCCHRGSNET"].ToString();
                    txtONMCharges.Text = dr["ONMCHRDSNET"].ToString();
                    txtBillCost.Text = Convert.ToDecimal(dr["BILLCOSTNET"]).ToString("N0");
                    txtBillAmt.Text = Convert.ToDecimal(dr["BILAMNTBDDT"]).ToString("N0");
                    txtLatePayment.Text = Convert.ToDecimal(dr["BILAMNTLP"]).ToString("N0");
                    txtAfterDue.Text = Convert.ToDecimal(dr["BILAMNTADDT"]).ToString("N0");

                    txtK1From.Focus();
                }
            }
        }
    }

    protected void pcdReCalculateBill()
//    protected void pcdReCalculateBill(object sender, EventArgs e)
    {
        decimal mDiff = 0;
        decimal mUnits = 0;
        decimal BillCost = 0;
        decimal BillAmt = 0;
        decimal LatePayment = 0;
        decimal AfterDue = 0;

        txtBillCost.Text="";
        txtBillAmt.Text = "";
        txtLatePayment.Text = "";
        txtAfterDue.Text = "";

        decimal k1From = ToDecimal(txtK1From.Text);
        decimal k1To = ToDecimal(txtK1To.Text);
        decimal MFactor = ToDecimal(txtMFactor.Text);
        decimal UnitRate = ToDecimal(txtUnitRate.Text);

        decimal advance = ToDecimal(txtAdvance.Text);
        decimal arrear = ToDecimal(txtArrear.Text);
        decimal instalment = ToDecimal(txtInstalment.Text);
        decimal fine = ToDecimal(txtFine.Text);
        decimal dcCharges = ToDecimal(txtDcCharges.Text);
        decimal ONMCharges = ToDecimal(txtONMCharges.Text);

        mDiff = Math.Round(k1To - k1From, 0);
        mUnits = Math.Round(((k1To - k1From) * MFactor), 0);
        BillCost = Math.Round((mUnits * UnitRate), 0);

        txtElDiff.Text = (mDiff).ToString();
        txtUnitsEl.Text = (mUnits).ToString();
        txtBillCost.Text = BillCost.ToString("N0");

        if(instalment>0){
            BillAmt = Math.Round(((BillCost + ONMCharges + instalment + fine + dcCharges) - advance),0);
        }else{
            BillAmt = Math.Round(((BillCost + ONMCharges + arrear + fine + dcCharges) - advance), 0);
        }
        LatePayment = Math.Round((((BillCost + ONMCharges)/1001)*10),0);
        AfterDue = Math.Round((BillAmt + LatePayment),0);

        txtBillAmt.Text = BillAmt.ToString("N0");
        txtLatePayment.Text = LatePayment.ToString("N0");
        txtAfterDue.Text = AfterDue.ToString("N0");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pcdInit();
        txtBTKNo.Text = "";
        txtBTKNo.Focus();
    }

    protected void btnFixBill_Click(object sender, EventArgs e)
    {
        pcdReCalculateBill();
    }

    // 🔹 helper function (safe decimal conversion)
    private decimal ToDecimal(string val)
    {
        decimal d;
        decimal.TryParse(val, out d);
        return d;
    }









    protected void btnPost_Click(object sender, EventArgs e)
    {
        string btkNo = txtBTKNo.Text.Trim();
        if (string.IsNullOrEmpty(btkNo))
            return;

        int mTrancode = GetTranCode(btkNo);

        if (mTrancode <= 0)
            return;

        try
        {
            InsertIntoModified(mTrancode);
            UpdateModified(mTrancode);

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "msg",
                "alert('Record successfully updated');",
                true
            );
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "err",
                "alert('Error: " + ex.Message.Replace("'", "") + "');",
                true
            );
        }
    }



    private int GetTranCode(string btkNo)
    {
        int mTrancode = 0;
        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string sql = @"
        SELECT TRANCODE
        FROM BIL_ELEC
        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')
          AND REFCODE = :btkNo";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add("btkNo", OracleDbType.Varchar2).Value = btkNo;
            con.Open();

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
                mTrancode = Convert.ToInt32(result);
        }

        return mTrancode;
    }




    private void InsertIntoModified(int mTrancode)
    {
        int rows=0;

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string sql = @"
        INSERT INTO BIL_ELEC_MODIFYED (
            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT
        )
        SELECT
            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT
        FROM BIL_ELEC
        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')
          AND TRANCODE = :mTrancode";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;
            con.Open();
            cmd.ExecuteNonQuery();
            if (rows == 0)
            {
                // DEBUG ONLY
                rows = 0;
                //lblUser.Text = "No record inserted — SELECT returned 0 rows";
            }
        }
    }




    private void UpdateModified(int mTrancode)
    {
        string userId = "";
        string userName = "";
        string currentDate = "";
        string ipAddress = "";

        if (Session["User"] != null)
        {
            userId = Session["login_id"].ToString();
            userName = Session["login_name"].ToString();
            currentDate = DateTime.Now.ToString("dd-MMM-yy");
            ipAddress = Request.UserHostAddress;

            lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
        }

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string sql = @"
        UPDATE BIL_ELEC_MODIFYED SET
            READPRV_TOBE     = :K1From,
            READCURR_TOBE    = :K1To,
            READDIFF_TOBE    = :ElDiff,
            MFACTRATE_TOBE   = :MFactor,
            UNITS_TOBE       = :UnitsEl,
            UNIT_RATE_TOBE   = :UnitRate,
            ADVPAYMNT_TOBE   = :Advance,
            ARREARS_TOBE     = :Arrear,
            INSTAMT_TOBE     = :Instalment,
            FINECHRGS_TOBE   = :Fine,
            DCCHRGS_TOBE     = :DcCharges,
            ONMCHRDS_TOBE    = :ONMCharges,
            BILLCOST_TOBE    = :BillCost,
            BILAMNTBDDT_TOBE = :BillAmt,
            BILAMNTLP_TOBE   = :LatePayment,
            BILAMNTADDT_TOBE = :AfterDue,
            REMARKS=:Remarks
        WHERE TRANCODE = :mTrancode";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add("K1From", ToDecimal(txtK1From.Text));
            cmd.Parameters.Add("K1To", ToDecimal(txtK1To.Text));
            cmd.Parameters.Add("ElDiff", ToDecimal(txtElDiff.Text));
            cmd.Parameters.Add("MFactor", ToDecimal(txtMFactor.Text));
            cmd.Parameters.Add("UnitsEl", ToDecimal(txtUnitsEl.Text));
            cmd.Parameters.Add("UnitRate", ToDecimal(txtUnitRate.Text));
            cmd.Parameters.Add("Advance", ToDecimal(txtAdvance.Text));
            cmd.Parameters.Add("Arrear", ToDecimal(txtArrear.Text));
            cmd.Parameters.Add("Instalment", ToDecimal(txtInstalment.Text));
            cmd.Parameters.Add("Fine", ToDecimal(txtFine.Text));
            cmd.Parameters.Add("DcCharges", ToDecimal(txtDcCharges.Text));
            cmd.Parameters.Add("ONMCharges", ToDecimal(txtONMCharges.Text));
            cmd.Parameters.Add("BillCost", ToDecimal(txtBillCost.Text));
            cmd.Parameters.Add("BillAmt", ToDecimal(txtBillAmt.Text));
            cmd.Parameters.Add("LatePayment", ToDecimal(txtLatePayment.Text));
            cmd.Parameters.Add("AfterDue", ToDecimal(txtAfterDue.Text));
            cmd.Parameters.Add("Remarks", ToDecimal(txtRemarks.Text));
            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;

//            cmd.Parameters.Add("userId", OracleDbType.Varchar2).Value = userId;
//            cmd.Parameters.Add("currentDate", OracleDbType.Date).Value = DateTime.Now;
//            cmd.Parameters.Add("ipAddress", OracleDbType.Varchar2).Value = ipAddress;

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }


}
