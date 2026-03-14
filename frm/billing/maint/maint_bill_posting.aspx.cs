using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class maint_bill_posting : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
        .ConnectionStrings["MyDbConnectionMNT"].ConnectionString;

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
                    "SELECT COUNT(*) FROM BILL_GENERATE_AMOUNT WHERE BG_ID = :BG_ID", con))
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
                    @"INSERT INTO BILL_DETAIL (
                        RES_ID, BCD_ID, COLUMN_ID, BTYPE_ID, COLUMN_TYPE, C_OPERATOR, COMP_ID, BG_ID, PRECENT_ID, BLOCK_ID, LOCATION_ID, BILL_AMOUNT, 
                        REMARKS, DT_UPDATE, UPDATE_BY, UPDATE_IP, UPDATE_PC, DT_CREATE, CREATE_BY, CREATE_IP, GENERATE_PC, MRP_RATE, RCAT_ID)
                        SELECT 
                            RES_ID, BCD_ID, COLUMN_ID, BTYPE_ID, COLUMN_TYPE, C_OPERATOR, COMP_ID, BG_ID, PRECENT_ID, BLOCK_ID, LOCATION_ID, BILL_AMOUNT, 
                            REMARKS, DT_UPDATE, UPDATE_BY, UPDATE_IP, UPDATE_PC, DT_CREATE, CREATE_BY, CREATE_IP, GENERATE_PC, MRP_RATE, RCAT_ID
                        FROM BILL_DETAIL_TOBEN", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    @"INSERT INTO BILL_GENERATE_AMOUNT (
                        RES_ID, AMNT_WALLET, AMNT_WTDATE, AMNT_AFDATE, AMNT_RECEIVE, AMNT_REMAINING, AMNT_REC_DATE, PAY_METH_ID, IS_HALF_PAY, IS_PAID, BILL_MONTH, IS_DELETE, COMP_ID, BG_ID, CHEQUE_NO, 
                        PV_NO, IS_CANCEL, DT_CANCEL, CANCEL_BY, CANCEL_IP, M_FACTOR, DT_METER_READING, DT_ISSUE, DT_DUE, PAY_REC_BY, PAY_UPLOAD_DT, PAY_UPLOAD_IP, PAY_IND, BTYPE_ID1, MRP_RATE, 
                        INST_HEAD, CHALLAN_NO, CHALLAN_NO1, CHALLAN_NO2, CHALLAN_NO3, BIL_STOPED, SUM_BIL_AMT, CHALLAN_NO4, CHALLAN_NO5, CHALLAN_NO6, CHALLAN_NO7, CHALLAN_NO8, CHALLAN_NO9, 
                        CHALLAN_NO10, CHALLAN_STATUS, CHALLAN_TYPE, CHALLAN_AMOUNT, CELLNO, CHALLAN_AMOUNT1, CHALLAN_AMOUNT2, CHALLAN_AMOUNT3, CHALLAN_AMOUNT4, CHALLAN_AMOUNT5, CHALLAN_AMOUNT6, 
                        CHALLAN_AMOUNT7, CHALLAN_AMOUNT8, CHALLAN_AMOUNT9, CHALLAN_AMOUNT10, RES_ID_BK)
                        SELECT 
                            RES_ID, AMNT_WALLET, AMNT_WTDATE, AMNT_AFDATE, AMNT_RECEIVE, AMNT_REMAINING, AMNT_REC_DATE, PAY_METH_ID, IS_HALF_PAY, IS_PAID, BILL_MONTH, IS_DELETE, COMP_ID, BG_ID, CHEQUE_NO, 
                            PV_NO, IS_CANCEL, DT_CANCEL, CANCEL_BY, CANCEL_IP, M_FACTOR, DT_METER_READING, DT_ISSUE, DT_DUE, PAY_REC_BY, PAY_UPLOAD_DT, PAY_UPLOAD_IP, PAY_IND, BTYPE_ID1, MRP_RATE, 
                            INST_HEAD, CHALLAN_NO, CHALLAN_NO1, CHALLAN_NO2, CHALLAN_NO3, BIL_STOPED, SUM_BIL_AMT, CHALLAN_NO4, CHALLAN_NO5, CHALLAN_NO6, CHALLAN_NO7, CHALLAN_NO8, CHALLAN_NO9, 
                            CHALLAN_NO10, CHALLAN_STATUS, CHALLAN_TYPE, CHALLAN_AMOUNT, CELLNO, CHALLAN_AMOUNT1, CHALLAN_AMOUNT2, CHALLAN_AMOUNT3, CHALLAN_AMOUNT4, CHALLAN_AMOUNT5, CHALLAN_AMOUNT6, 
                            CHALLAN_AMOUNT7, CHALLAN_AMOUNT8, CHALLAN_AMOUNT9, CHALLAN_AMOUNT10, RES_ID_BK
                        FROM BILL_GENERATE_AMOUNT_TOBEN", con))
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }

                using (OracleCommand cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM BILL_GENERATE_AMOUNT WHERE BG_ID = :BG_ID", con))
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
