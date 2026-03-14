using System;
using System.IO;
using System.Web.UI;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;

public partial class update_elec : System.Web.UI.Page
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

                HasFormAccess();
            }

            txtBTKNo.Text = "";
            txtBTKNo.Focus();
        }
    }

    private void HasFormAccess()
    {
        int rows = 0;
        int userId = 0;
        int mAllowed = 0;
        string formName = "update_elec";

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
                    txtBTKNo.Enabled = false;

                    lblStatus.Text = "You are not authorized to access this form. Please go back to the menu.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }

    protected void pcdInit()
    {
        lblStatus.Text = "";
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
        txtRemarks.Text = "";
    }

    //protected void pcdFileAttachment(object sender, EventArgs e)
    //{
    //    //if (!fuAttachment.HasFile)
    //    //{
    //    //    lblStatus.Text = "Please select a PDF file.";
    //    //    return;
    //    //}

    //    // extension check (server-side MUST)
    //    string extension = Path.GetExtension(fuAttachment.FileName).ToLower();

    //    if (extension != ".pdf")
    //    {
    //        lblStatus.Text = "Only PDF files are allowed.";
    //        return;
    //    }

    //    // save path
    //    string folderPath = Server.MapPath("~/Uploads/");
    //    if (!Directory.Exists(folderPath))
    //        Directory.CreateDirectory(folderPath);

    //    string fileName = Path.GetFileName(fuAttachment.FileName);
    //    string fullPath = Path.Combine(folderPath, fileName);

    //    fuAttachment.SaveAs(fullPath);

    //    // string variable with path
    //    savedFilePath = "";
    //    savedFilePath = fullPath;
    //}

    protected void pcdFetchRecord(object sender, EventArgs e)
    {
        string btkNo = txtBTKNo.Text.Trim();

        pcdInit();

        if (string.IsNullOrEmpty(btkNo))
            return;

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

        LatePayment = Math.Round(((BillCost + ONMCharges) * 0.1m), 0);

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

            lblStatus.Text = "Electric Bill has been updated successfully...";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Bill Updating has failed...";
            lblStatus.ForeColor = System.Drawing.Color.Red;
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
        int userId = 0;
        int rows = 0;
        string mDeptID = "";

        if (Session["User"] != null)
        {
            int.TryParse(Session["login_id"].ToString(), out userId);
        }

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string sql1 = @"SELECT DEPT_ID FROM LOGIN_INFO WHERE ID = :userId";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql1, con))
        {
            cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
            con.Open();

            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    mDeptID = dr["DEPT_ID"].ToString();
                }
            }
        }

        string sql = @"
        INSERT INTO BIL_ELEC_MODIFYED (
            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT, DEPT_ID
        )
        SELECT
            BIL_TYPE, TRANCODE, COMPID, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, RESCAT, BILCAT,
            READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, BILLCOST,
            READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, UNIT_AMOUNT_SLR, NET_UNITS_SLR_NET,
            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_AMT, Q1, Q2, Q3, Q4, TOT_PAYABLE_AMT_NET,
            ADVPAYMNT, ARREARS, INSTAMT, FINECHRGS, DCCHRGS, ONMCHRDS, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT, :mDeptID
        FROM BIL_ELEC
        WHERE BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')
          AND TRANCODE = :mTrancode";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add("mDeptID", OracleDbType.Varchar2).Value = mDeptID;
            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;
            con.Open();
            cmd.ExecuteNonQuery();
            if (rows == 0)
            {
                rows = 0;
            }
        }
    }

    private void UpdateModified(int mTrancode)
    {
        int userId = 0;
        string userName = "";
        string currentDate = "";
        string ipAddress = "";

        if (Session["User"] != null)
        {
            int.TryParse(Session["login_id"].ToString(), out userId);
            userName = Session["login_name"].ToString();
            currentDate = DateTime.Now.ToString("dd-MMM-yy");
            ipAddress = Request.UserHostAddress;

            lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
        }

        string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        string sql = @"
        UPDATE BIL_ELEC_MODIFYED O SET
            O.READPRV_TOBE     = :K1From,
            O.READCURR_TOBE    = :K1To,
            O.READDIFF_TOBE    = :ElDiff,
            O.MFACTRATE_TOBE   = :MFactor,
            O.UNITS_TOBE       = :UnitsEl,
            O.UNIT_RATE_TOBE   = :UnitRate,
            O.ADVPAYMNT_TOBE   = :Advance,
            O.ARREARS_TOBE     = :Arrear,
            O.INSTAMT_TOBE     = :Instalment,
            O.FINECHRGS_TOBE   = :Fine,
            O.DCCHRGS_TOBE     = :DcCharges,
            O.ONMCHRDS_TOBE    = :ONMCharges,
            O.BILLCOST_TOBE    = :BillCost,
            O.BILAMNTBDDT_TOBE = :BillAmt,
            O.BILAMNTLP_TOBE   = :LatePayment,
            O.BILAMNTADDT_TOBE = :AfterDue,
            O.REMARKS=:Remarks,
            O.MODIFYBY=:userId,
            O.MODIFYDT=:currentDate,
            O.MODIFYIP=:ipAddress
        WHERE O.TRANCODE = :mTrancode AND O.VERSION=(SELECT MAX(I.VERSION) FROM BIL_ELEC_MODIFYED I WHERE I.TRANCODE=O.TRANCODE)";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add("K1From", OracleDbType.Decimal).Value = ToDecimal(txtK1From.Text);
            cmd.Parameters.Add("K1To", OracleDbType.Decimal).Value = ToDecimal(txtK1To.Text);
            cmd.Parameters.Add("ElDiff", OracleDbType.Decimal).Value = ToDecimal(txtElDiff.Text);
            cmd.Parameters.Add("MFactor", OracleDbType.Decimal).Value = ToDecimal(txtMFactor.Text);
            cmd.Parameters.Add("UnitsEl", OracleDbType.Decimal).Value = ToDecimal(txtUnitsEl.Text);
            cmd.Parameters.Add("UnitRate", OracleDbType.Decimal).Value = ToDecimal(txtUnitRate.Text);
            cmd.Parameters.Add("Advance", OracleDbType.Decimal).Value = ToDecimal(txtAdvance.Text);
            cmd.Parameters.Add("Arrear", OracleDbType.Decimal).Value = ToDecimal(txtArrear.Text);
            cmd.Parameters.Add("Instalment", OracleDbType.Decimal).Value = ToDecimal(txtInstalment.Text);
            cmd.Parameters.Add("Fine", OracleDbType.Decimal).Value = ToDecimal(txtFine.Text);
            cmd.Parameters.Add("DcCharges", OracleDbType.Decimal).Value = ToDecimal(txtDcCharges.Text);
            cmd.Parameters.Add("ONMCharges", OracleDbType.Decimal).Value = ToDecimal(txtONMCharges.Text);
            cmd.Parameters.Add("BillCost", OracleDbType.Decimal).Value = ToDecimal(txtBillCost.Text);
            cmd.Parameters.Add("BillAmt", OracleDbType.Decimal).Value = ToDecimal(txtBillAmt.Text);
            cmd.Parameters.Add("LatePayment", OracleDbType.Decimal).Value = ToDecimal(txtLatePayment.Text);
            cmd.Parameters.Add("AfterDue", OracleDbType.Decimal).Value = ToDecimal(txtAfterDue.Text);

            cmd.Parameters.Add("Remarks", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();

            cmd.Parameters.Add("userId", OracleDbType.Decimal).Value = Convert.ToDecimal(userId);
            cmd.Parameters.Add("currentDate", OracleDbType.Date).Value = DateTime.Now;
            cmd.Parameters.Add("ipAddress", OracleDbType.Varchar2).Value = ipAddress;

            cmd.Parameters.Add("mTrancode", OracleDbType.Int32).Value = mTrancode;

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

}
