using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class water_bill_posting : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
        .ConnectionStrings["MyDbConnectionWTR"].ConnectionString;

    void Log(string msg)
    {
        lblStatus.Text += msg + "<br/>";
    }

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
                    "SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1", con))
                {
                    cmd.Transaction = tran;
                    bgIdOLD = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT BM_ID FROM WATER_DATES_TOBE", con))
                {
                    cmd.Transaction = tran;
                    bgId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                
                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM BILLS_WATER WHERE BM_ID = :BG_ID", con))
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

                using (OracleCommand cmd = new OracleCommand(
                    @"INSERT INTO BILLS_WATER (
                            BILL_ID, RES_ID, REF_NO, RES_NAME, HOUSE_NO, CATE_ID, PRECINCT_ID, WATER_UNITS, WATER_METER_FROM, WATER_METER_TO, WATER_UNIT_RATE, WATER_AMOUNT, OPN_ARREARS, 
                            OPN_WALLET, LATE_CHARGES, ARREARS, WALLET, NET_PAYBLE, BM_ID, NET_PAYBLE_AFTER_DUEDATE, ISSUE_DATE, DUE_DATE, AMOUNT_PAYBLE, PAID, CREATED_BY, DT_CREATED, MODIFIED_BY, 
                            MODIFIED_DT, PAIDBYBAHRIA, FIXED_CHARGES, LATE_CHAR_PERC, BANK_VALIDITY, ARR_INSTALLMENT, INS, CHARGES, AMNT_RECEIVE, AMNT_REC_DATE, PAY_IND, UBL_ACCT, BIL_STOPED, SEVERAGE_COST, 
                            FIXED_CHARGES_NOA, FIXED_CHARGES_NOS, GALLONS, BLANK_COLUMN, SNBL, ONE_LINK, KUICKPAY, BILLING_MONTH, READING_DATE, RCAT_NM, SNBL_AC, METER_NO, PR_GALON, MFACTOR, METER_TYPE, 
                            BIL_TYPE, ADJUSTMENT, FINE, SEVERAGE_RATE, PRECINCT_NM, INST_NM, BARCODE, WATER_BOWSER_NO, WATER_BOWSER_CHARGES, INST_HEAD, NOS_REMARKS, NOAPR_REMARKS, NOS, NOS_RATE, NOAPT, 
                            NOAPT_RATE, NOS_AMT, NOAPT_AMT, BLOCK_ID, BLOCK_NM, CELLNO) 
                        SELECT 
                            BILL_ID, RES_ID, REF_NO, RES_NAME, HOUSE_NO, CATE_ID, PRECINCT_ID, WATER_UNITS, WATER_METER_FROM, WATER_METER_TO, WATER_UNIT_RATE, WATER_AMOUNT, OPN_ARREARS, 
                            OPN_WALLET, LATE_CHARGES, ARREARS, WALLET, NET_PAYBLE, BM_ID, NET_PAYBLE_AFTER_DUEDATE, ISSUE_DATE, DUE_DATE, AMOUNT_PAYBLE, PAID, CREATED_BY, DT_CREATED, MODIFIED_BY, 
                            MODIFIED_DT, PAIDBYBAHRIA, FIXED_CHARGES, LATE_CHAR_PERC, BANK_VALIDITY, ARR_INSTALLMENT, INS, CHARGES, AMNT_RECEIVE, AMNT_REC_DATE, PAY_IND, UBL_ACCT, BIL_STOPED, SEVERAGE_COST, 
                            FIXED_CHARGES_NOA, FIXED_CHARGES_NOS, GALLONS, BLANK_COLUMN, SNBL, ONE_LINK, KUICKPAY, BILLING_MONTH, READING_DATE, RCAT_NM, SNBL_AC, METER_NO, PR_GALON, MFACTOR, METER_TYPE, 
                            BIL_TYPE, ADJUSTMENT, FINE, SEVERAGE_RATE, PRECINCT_NM, INST_NM, BARCODE, WATER_BOWSER_NO, WATER_BOWSER_CHARGES, INST_HEAD, NOS_REMARKS, NOAPR_REMARKS, NOS, NOS_RATE, NOAPT, 
                            NOAPT_RATE, NOS_AMT, NOAPT_AMT, BLOCK_ID, BLOCK_NM, CELLNO 
                        FROM BILLS_WATER_TOBE", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM BILLS_WATER WHERE BM_ID = :BG_ID", con))
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
