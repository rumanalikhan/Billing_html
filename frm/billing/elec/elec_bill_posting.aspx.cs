using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class elec_bill_posting : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
        .ConnectionStrings["MyDbConnection"].ConnectionString;

    // Logger
    void Log(string msg)
    {
        lblStatus.Text += msg + "<br/>";
    }

    // --- Temporary FLow (DRY RUN)
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        if (txtPin.Text.Trim() != "3534")
        {
            lblStatus.Text = "Invalid password";
            lblStatus.ForeColor = System.Drawing.Color.Red;
            return;
        }

        using (OracleConnection con = new OracleConnection(connStr))
        {
            con.Open();
            OracleTransaction tran = con.BeginTransaction();

            try
            {
                int bgId;
                int bgIdOLD;
                int bgCount;

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N'", con))
                {
                    cmd.Transaction = tran;
                    bgIdOLD = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT BG_ID FROM BILL_GENERATE_TOBE", con))
                {
                    cmd.Transaction = tran;
                    bgId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                
                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM BIL_ELEC WHERE BGID = :BG_ID", con))
                {
                    cmd.Transaction = tran;
                    cmd.Parameters.Add(":BG_ID", bgId);
                    bgCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (bgCount > 0)
                {
                    lblStatus.Text = "Billing already posted " + bgCount;
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return; // 🚀 Process yahin stop ho jayega
                }

                using (OracleCommand cmd = new OracleCommand(@"UPDATE BILL_GENERATE SET IS_LOCKED='Y' WHERE BG_ID=:bgIdOLD", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    @"INSERT INTO BILL_GENERATE (
                        BTYPE_ID, BG_NAME, BG_DETAILS, DT_GENERATE, GENERATE_BY, GENERATE_IP, GENERATE_PC, DT_ISSUE, DUE_DATE, TOTAL_AMOUNT, DT_READING, 
                        BILL_MONTH, COMP_ID, IS_LOCKED, LOCKED_BY, LOCKED_IP, LOCKED_PC, LOCKED_DT, VALID_DATE, ISSUE_DATE, LOG_ID)
                        SELECT 
                            BTYPE_ID, BG_NAME, BG_DETAILS, DT_GENERATE, GENERATE_BY, GENERATE_IP, GENERATE_PC, DT_ISSUE, DUE_DATE, TOTAL_AMOUNT, DT_READING, 
                            BILL_MONTH, COMP_ID, IS_LOCKED, LOCKED_BY, LOCKED_IP, LOCKED_PC, LOCKED_DT, VALID_DATE, ISSUE_DATE, LOG_ID FROM BILL_GENERATE_TOBE", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    @"INSERT INTO BIL_ELEC (
                        COMPID, BILLMODE, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, ADDRESS, PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
                        CONSUMERCODE1, CONSUMERCODE2, CONSUMERCODE3, CONSUMERCODE4, CONSUMERCODE5, INTEGRATION1, INTEGRATION2, INTEGRATION3, INTEGRATION4, INTEGRATION5, 
                        BANKNAME1, BANKCODE1, BANKNAME2, BANKCODE2, BANKNAME3, BANKCODE3, BANKNAME4, BANKCODE4, BANKNAME5, BANKCODE5, 
                        RESCAT, BILCAT, METERNO, READINGDT, READOB, READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, LPCHRGS, PTVCHRGS, FXDCHRGS, ONMCHRDS, HORTICHRGS, MISCCHRGS, DCCHRGS, 
                        ADVPAYMNT, ARREARS, FINECHRGS, PAIDBYBAHRIA, TAXAMT, INSTAMT, ADDCOLADD1, ADDCOLADD2, ADDCOLADD3, ADDCOLADD4, ADDCOLDED1, ADDCOLDED2, ADDCOLDED3, ADDCOLDED4, BILLCOST, 
                        PTVCHRGSADJ, FXDCHRGSADJ, ONMCHRDSADJ, HORTICHRGSADJ, MISCCHRGSADJ, DCCHRGSADJ, ADVPAYMNTADJ, ARREARSADJ, FINECHRGSADJ, PAIDBYBAHRIAADJ, TAXAMTADJ, INSTAMTADJ, ADDCOLADD1ADJ, 
                        ADDCOLADD2ADJ, ADDCOLADD3ADJ, ADDCOLADD4ADJ, ADDCOLDED1ADJ, ADDCOLDED2ADJ, ADDCOLDED3ADJ, ADDCOLDED4ADJ, BILLCOSTADJ, PTVCHRGSNET, FXDCHRGSNET, ONMCHRDSNET, HORTICHRGSNET, 
                        MISCCHRGSNET, DCCHRGSNET, ADVPAYMNTNET, ARREARSNET, FINECHRGSNET, PAIDBYBAHRIANET, TAXAMTNET, INSTAMTNET, ADDCOLADD1NET, ADDCOLADD2NET, ADDCOLADD3NET, ADDCOLADD4NET, ADDCOLDED1NET, 
                        ADDCOLDED2NET, ADDCOLDED3NET, ADDCOLDED4NET, BILLCOSTNET, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT, AMTRECDT, AMTRECEIVED, AMTREMAINING, BILPAYMOD, BILCREATEBY, BILCREATEDT, BILCREATEIP, 
                        BILRECEBY, BILRECDT, BILRECIP, POSTDT, POSTBY, POSTIP, BILLINGMONTH, ISSUEDT, DUEDT, EXPIRYDT, LOGID, BILPOST, RESERVED, BANK_MNEMONIC, READPRV_SOLR, READCURR_SOLR, READDIFF_SOLR, 
                        UNITS_SOLR, UNIT_RATE_SOLR, UNITS_NET_SOLR, BIL_TYPE, NET_UNITS_SLR, FIXED_RATES, NET_AMNT_SLR, ALL_K_TOT, PRE_UNTS, PRE_AMT, TOT_PAYABLE_AMT, NET_UNITS_SLR_NET, 
                        FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_UNTS_NET, PRE_AMT_NET, TOT_PAYABLE_AMT_NET, READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, 
                        UNIT_AMOUNT_SLR, MR_DT, BIL_STOPED, SM_MSNO, LPCHRGSS, GROSAMT, NETAMT, CMTCODE, CNIC_NO, INST_HEAD, ENERGY_TAX, EXTRA_TAX, FURTHER_TAX, SALES_TAX, INCOME_TAX, TOTAL_TAX, 
                        BIL_COST, RATE1, RATE2, UNIT_RATE_ADJ, BILL_AMOUNT, Q1, Q2, Q3, Q4, BIL_CHNG_RMRKS1, BIL_CHNG_RMRKS2, NOS, NOS_RATE, NOAPT, NOAPT_RATE, NOS_AMT, NOAPT_AMT, NOS_REMARKS, 
                        NOAPR_REMARKS, FAULTY_METER, FAULTY_METER_AB, FM_HEAD, CELLNO) 
                        SELECT 
                            COMPID, BILLMODE, BILLMONTH, BGID, REFCODE, BARCODE, RESID, RESNAME, ADDRESS, PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
                            CONSUMERCODE1, CONSUMERCODE2, CONSUMERCODE3, CONSUMERCODE4, CONSUMERCODE5, INTEGRATION1, INTEGRATION2, INTEGRATION3, INTEGRATION4, INTEGRATION5, 
                            BANKNAME1, BANKCODE1, BANKNAME2, BANKCODE2, BANKNAME3, BANKCODE3, BANKNAME4, BANKCODE4, BANKNAME5, BANKCODE5, 
                            RESCAT, BILCAT, METERNO, READINGDT, READOB, READPRV, READCURR, READDIFF, MFACTRATE, UNITS, UNIT_RATE, LPCHRGS, PTVCHRGS, FXDCHRGS, ONMCHRDS, HORTICHRGS, MISCCHRGS, DCCHRGS, 
                            ADVPAYMNT, ARREARS, FINECHRGS, PAIDBYBAHRIA, TAXAMT, INSTAMT, ADDCOLADD1, ADDCOLADD2, ADDCOLADD3, ADDCOLADD4, ADDCOLDED1, ADDCOLDED2, ADDCOLDED3, ADDCOLDED4, BILLCOST, 
                            PTVCHRGSADJ, FXDCHRGSADJ, ONMCHRDSADJ, HORTICHRGSADJ, MISCCHRGSADJ, DCCHRGSADJ, ADVPAYMNTADJ, ARREARSADJ, FINECHRGSADJ, PAIDBYBAHRIAADJ, TAXAMTADJ, INSTAMTADJ, ADDCOLADD1ADJ, 
                            ADDCOLADD2ADJ, ADDCOLADD3ADJ, ADDCOLADD4ADJ, ADDCOLDED1ADJ, ADDCOLDED2ADJ, ADDCOLDED3ADJ, ADDCOLDED4ADJ, BILLCOSTADJ, PTVCHRGSNET, FXDCHRGSNET, ONMCHRDSNET, HORTICHRGSNET, 
                            MISCCHRGSNET, DCCHRGSNET, ADVPAYMNTNET, ARREARSNET, FINECHRGSNET, PAIDBYBAHRIANET, TAXAMTNET, INSTAMTNET, ADDCOLADD1NET, ADDCOLADD2NET, ADDCOLADD3NET, ADDCOLADD4NET, ADDCOLDED1NET, 
                            ADDCOLDED2NET, ADDCOLDED3NET, ADDCOLDED4NET, BILLCOSTNET, BILAMNTBDDT, BILAMNTLP, BILAMNTADDT, AMTRECDT, AMTRECEIVED, AMTREMAINING, BILPAYMOD, BILCREATEBY, BILCREATEDT, BILCREATEIP, 
                            BILRECEBY, BILRECDT, BILRECIP, POSTDT, POSTBY, POSTIP, BILLINGMONTH, ISSUEDT, DUEDT, EXPIRYDT, LOGID, BILPOST, RESERVED, BANK_MNEMONIC, READPRV_SOLR, READCURR_SOLR, READDIFF_SOLR, 
                            UNITS_SOLR, UNIT_RATE_SOLR, UNITS_NET_SOLR, BIL_TYPE, NET_UNITS_SLR, FIXED_RATES, NET_AMNT_SLR, ALL_K_TOT, PRE_UNTS, PRE_AMT, TOT_PAYABLE_AMT, NET_UNITS_SLR_NET, 
                            FIXED_RATES_NET, NET_AMNT_SLR_NET, ALL_K_TOT_NET, PRE_UNTS_NET, PRE_AMT_NET, TOT_PAYABLE_AMT_NET, READ_PRV_SLR, READ_CURR_SLR, READ_DIFF_SLR, UNITS_SLR, UNIT_RATE_SLR, 
                            UNIT_AMOUNT_SLR, MR_DT, BIL_STOPED, SM_MSNO, LPCHRGSS, GROSAMT, NETAMT, CMTCODE, CNIC_NO, INST_HEAD, ENERGY_TAX, EXTRA_TAX, FURTHER_TAX, SALES_TAX, INCOME_TAX, TOTAL_TAX, 
                            BIL_COST, RATE1, RATE2, UNIT_RATE_ADJ, BILL_AMOUNT, Q1, Q2, Q3, Q4, BIL_CHNG_RMRKS1, BIL_CHNG_RMRKS2, NOS, NOS_RATE, NOAPT, NOAPT_RATE, NOS_AMT, NOAPT_AMT, NOS_REMARKS, 
                            NOAPR_REMARKS, FAULTY_METER, FAULTY_METER_AB, FM_HEAD, CELLNO 
                        FROM BIL_ELEC_TOBE", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM BIL_ELEC WHERE BGID = :BG_ID", con))
                {
                    cmd.Transaction = tran;
                    cmd.Parameters.Add(":BG_ID", bgId);
                    bgCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                lblStatus.Text = "Billing posted... " + bgCount;
                lblStatus.ForeColor = System.Drawing.Color.Green;
                return; // 🚀 Process yahin stop ho jayega
            }
            catch (Exception ex)
            {
                tran.Rollback();
                lblStatus.Text = "Unexpected error occurred. Contact admin.";
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "";
    }
}
