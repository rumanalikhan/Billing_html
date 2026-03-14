using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

public partial class billing_summaries : System.Web.UI.Page
{
    /* CONNECTION STRING */
    private readonly string connStrMNT =
        WebConfigurationManager.ConnectionStrings["MyDbConnectionMNT"].ConnectionString;

    private readonly string connStrELEC =
        WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

    private readonly string connStrWATER =
        WebConfigurationManager.ConnectionStrings["MyDbConnectionWTR"].ConnectionString;

    //private readonly string connStrRENT =
    //    WebConfigurationManager.ConnectionStrings["MyDbConnectionRENT"].ConnectionString;

    //private readonly string connStrGAS =
    //    WebConfigurationManager.ConnectionStrings["MyDbConnectionGAS"].ConnectionString;

    //private readonly string connStrBNB =
    //    WebConfigurationManager.ConnectionStrings["MyDbConnectionBNB"].ConnectionString;


    /* RADIO BUTTON ENUM */
    public enum BillType
    {
        Maintenance = 1,
        Electric = 2,
        Water = 3,
        Gas = 4,
        Rent = 5,
        BNB = 6
    }

    /* CONFIG CLASS */
    private class BillExportConfig
    {
        public string ConnectionString { get; private set; }
        public string Sql { get; private set; }
        public string FileName { get; private set; }

        public BillExportConfig(string connectionString, string sql, string fileName)
        {
            ConnectionString = connectionString;
            Sql = sql;
            FileName = fileName;
        }
    }

    /* BILL CONFIGURATION */
    private Dictionary<BillType, BillExportConfig> BillConfig;

    /* QUERY - CONFIG */
    protected void Page_Init(object sender, EventArgs e)
    {
        /* Queries for Summary */
        string queryMNT = @"SELECT 
                                TB.BG_ID, 
                                TB.BG_NAME, TB.DT_ISSUE, TB.DUE_DATE, TB.BARCODE, TB.RES_ID, 
                                TB.RES_NAME, TB.HOUSE_NO, 
                                NVL((SELECT MAX(R.CONTACT_NO) FROM Billing.RESIDENTIAL_INFO R WHERE R.RES_ID=TB.RES_ID AND ROWNUM=1), ' ') CONTACT_NO, 
                                TB.RCAT_ID, 
                                (SELECT C.RCAT_NM FROM Billing.RESIDENCE_CATEGORY C WHERE C.RCAT_ID=TB.RCAT_ID AND ROWNUM=1) Category, 
                                TB.PRECENT_ID, (SELECT MAX(P.PRECENT_NM) FROM Billing.PRECENT_MST P WHERE P.PRECENT_ID=TB.PRECENT_ID AND ROWNUM=1) Precnt_Nm, 
                                TB.BLOCK_ID, (SELECT MAX(B.BLOCK_NM) FROM Billing.BLOCK_MST B WHERE B.BLOCK_ID=TB.BLOCK_ID AND ROWNUM=1) Block_Nm, 
                                TB.LNK1, TB.KP, TB.SNBL, TB.UBL, 
                                TB.ADV, TB.BILL_COST, TB.WMC, TB.ARR, 
                                NVL((SELECT MAX(I.NOI) FROM BILLS.V_BIL_INST I WHERE I.BIL_TYPE='MAINT' AND I.BGID=TB.BG_ID AND I.RESID=TB.RES_ID),0) NOI, 
                                TB.INST, TB.FINE, TB.HC, 
                                CASE 
                                    WHEN TB.ADV=0 AND TB.ARR=0 THEN 
                                        ROUND((TB.BILL_COST+TB.FINE+TB.HC), 0) 
                                    WHEN TB.ADV>=TB.BILL_COST AND TB.ARR=0 THEN 
                                        ROUND(((TB.ADV-TB.BILL_COST)+TB.FINE+TB.HC), 0) 
                                    WHEN TB.ADV<TB.BILL_COST AND TB.ARR=0 THEN 
                                        ROUND(((TB.ADV-TB.BILL_COST)+TB.WMC+TB.ARR+TB.INST+TB.FINE+TB.HC), 0) 
                                    WHEN TB.ADV=0 AND TB.ARR>0 THEN 
                                        CASE 
                                            WHEN TB.ARR>0 AND INST=0 THEN 
                                                ROUND((TB.BILL_COST+TB.ARR+TB.FINE+TB.HC), 0) 
                                            WHEN TB.ARR>0 AND INST>0 THEN 
                                                ROUND((TB.BILL_COST+TB.INST+TB.FINE+TB.HC), 0) 
                                        END 
                                END NetAmtwDD, 
                                CASE 
                                      WHEN TB.ADV=ROUND((TB.BILL_COST+TB.WMC+TB.HC), 0) THEN 
                                        0 
                                      WHEN TB.ADV=0 THEN 
                                        ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0) 
                                      WHEN TB.ADV>(TB.BILL_COST+TB.WMC+TB.INST+TB.FINE+TB.HC) THEN 
                                        0 
                                      WHEN TB.ADV<(TB.BILL_COST+TB.WMC+TB.INST+TB.HC) THEN 
                                        ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0) 
                                END LP_Charges, 
                                CASE 
                                            WHEN TB.ADV=0 AND TB.ARR=0 THEN 
                                                (ROUND((TB.BILL_COST+TB.FINE+TB.HC), 0)+ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0)) 
                                            WHEN TB.ADV>=TB.BILL_COST AND TB.ARR=0 THEN 
                                                ROUND(((TB.ADV-TB.BILL_COST)+TB.FINE+TB.HC), 0) 
                                            WHEN TB.ADV<TB.BILL_COST AND TB.ARR=0 THEN 
                                                (ROUND(((TB.ADV-TB.BILL_COST)+TB.WMC+TB.ARR+TB.INST+TB.FINE+TB.HC), 0)+ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0)) 
                                            WHEN TB.ADV=0 AND TB.ARR>0 THEN 
                                                CASE 
                                                    WHEN TB.ARR>0 AND INST=0 THEN 
                                                        (ROUND((TB.BILL_COST+TB.ARR+TB.FINE+TB.HC), 0)+ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0)) 
                                                    WHEN TB.ARR>0 AND INST>0 THEN 
                                                        (ROUND((TB.BILL_COST+TB.INST+TB.FINE+TB.HC), 0)+ROUND(((TB.BILL_COST+TB.WMC+TB.HC)*0.1), 0)) 
                                                END 
                                END TotAmtADD, 
                                TB.AMNT_RECEIVE, TB.AMNT_REC_DATE, TB.AMNT_REMAINING, 
                                TB.PAY_IND, TB.PAYMENT_SOURCE
                        FROM ( 
                              SELECT 
                                      BG_ID, 
                                      (SELECT BG.BG_NAME FROM Billing.BILL_GENERATE BG WHERE BG.BG_ID=B.BG_ID AND BG.BTYPE_ID=3 AND ROWNUM=1) BG_NAME, 
                                      (SELECT R.BARCODE FROM Billing.RESIDENTIAL_BARCODE R WHERE R.RES_ID=B.RES_ID AND R.BTYPE_ID=3 AND ROWNUM=1) BARCODE, B.RES_ID, 
                                      (SELECT REGEXP_REPLACE(I.RES_NAME, '[^A-Za-z0-9 ]', '') FROM Billing.RESIDENTIAL_INFO I WHERE I.RES_ID=B.RES_ID AND ROWNUM=1) RES_NAME, 
                                      (SELECT REGEXP_REPLACE(I.HOUSE_NO, '[^A-Za-z0-9 ]', '') FROM Billing.RESIDENTIAL_INFO I WHERE I.RES_ID=B.RES_ID AND ROWNUM=1) HOUSE_NO, 
                                      (SELECT I.RCAT_ID FROM Billing.RESIDENTIAL_INFO I WHERE I.RES_ID=B.RES_ID AND ROWNUM=1) RCAT_ID, 
                                      (SELECT I.PRECENT_ID FROM Billing.RESIDENTIAL_INFO I WHERE I.RES_ID=B.RES_ID AND ROWNUM=1) PRECENT_ID, 
                                      (SELECT I.BLOCK_ID FROM Billing.RESIDENTIAL_INFO I WHERE I.RES_ID=B.RES_ID AND ROWNUM=1) BLOCK_ID, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1004 AND ROWNUM=1), 0) ADV, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1010 AND ROWNUM=1), 0) BILL_COST, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1036 AND ROWNUM=1), 0) WMC, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1002 AND ROWNUM=1), 0) ARR, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1014 AND ROWNUM=1), 0) INST, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1022 AND ROWNUM=1), 0) Fine, 
                                      NVL((SELECT D.BILL_AMOUNT FROM Billing.BILL_DETAIL D WHERE D.RES_ID=B.RES_ID AND D.BG_ID=B.BG_ID AND D.COLUMN_ID=1026 AND ROWNUM=1), 0) HC, 
                                      NVL(B.AMNT_RECEIVE, 0) AMNT_RECEIVE, 
                                      TO_CHAR(B.AMNT_REC_DATE, 'DD-MON-RR') AMNT_REC_DATE, 
                                      NVL(B.AMNT_REMAINING, 0) AMNT_REMAINING, 
                                      (SELECT TO_CHAR(BG.DT_ISSUE, 'DD-MON-RR') FROM Billing.BILL_GENERATE BG WHERE BG.BG_ID=B.BG_ID AND BG.BTYPE_ID=3 AND ROWNUM=1) DT_ISSUE, 
                                      (SELECT TO_CHAR(BG.DUE_DATE, 'DD-MON-RR') FROM Billing.BILL_GENERATE BG WHERE BG.BG_ID=B.BG_ID AND BG.BTYPE_ID=3 AND ROWNUM=1) DUE_DATE, 
                                      '10120004' || LPAD(B.RES_ID, 6, '0') LNK1, 
                                      '62030' || LPAD(B.RES_ID, 6, '0') KP, 
                                      '30' || LPAD(B.RES_ID, 6, '0') SNBL, 
                                      '30' || LPAD(B.RES_ID, 6, '0') UBL, 
                                      NVL(PAY_IND, 0) PAY_IND, 
                                      DECODE(PAY_IND, 0, 'OTHERS', 1, '1LINK', 2, 'KUICKPAY', 4, 'UBL', 10, 'CASH', 'UNKNOWN') AS PAYMENT_SOURCE 
                              FROM Billing.BILL_GENERATE_AMOUNT B WHERE BG_ID=(SELECT BG_ID FROM BILL_GENERATE WHERE BILL_MONTH=:BILLMONTH)) TB";

        string queryELEC = @"SELECT 
                            BGID,(SELECT G.BG_NAME FROM BILL_GENERATE G WHERE G.BILL_MONTH=:BILLMONTH) BG_NAME, 
                            TO_CHAR(ISSUEDT, 'DD-MON-RR') ISSUEDT,TO_CHAR(DUEDT, 'DD-MON-RR') DUEDT,BIL_TYPE,REFCODE,BARCODE,RESID, 
                            RESNAME,ADDRESS,CNIC_NO, 
                            (SELECT MAX(R.CONTACT_NO) FROM RESID_INFO R WHERE R.RES_ID=E.RESID) CONTACT_NO, 
                            RESCAT,BILCAT,PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
                            INTEGRATION1 LNK1,INTEGRATION2 KP,INTEGRATION3 SBNL, INTEGRATION3 UBL,
                            METERNO,READPRV,READCURR,READDIFF,MFACTRATE,UNITS,UNIT_RATE,
                            ADVPAYMNTNET,ARREARSNET,INSTAMTNET,FINECHRGSNET,PTVCHRGSNET,DCCHRGSNET,ONMCHRDSNET,
                            BILLCOSTNET,BILAMNTBDDT,BILAMNTLP,BILAMNTADDT,AMTRECEIVED,TO_CHAR(AMTRECDT, 'DD-MON-RR') AMTRECDT, AMTREMAINING,
                            NVL(BILPAYMOD, 0) BILPAYMOD, DECODE(BILPAYMOD, 0, 'OTHERS', 1, '1LINK', 2, 'KUICKPAY', 4, 'UBL', 10, 'CASH', 'UNKNOWN') AS PAYMENT_SOURCE,
                            READ_PRV_SLR,READ_CURR_SLR,READ_DIFF_SLR,UNITS_SLR,UNIT_RATE_SLR,UNIT_AMOUNT_SLR, 
                            NET_UNITS_SLR_NET,FIXED_RATES_NET,NET_AMNT_SLR_NET,ALL_K_TOT_NET,PRE_AMT, TOT_PAYABLE_AMT_NET, 
                            NVL(Q1, 0) Q1, NVL(Q2, 0) Q2, NVL(Q3, 0) Q3, NVL(Q4, 0) Q4, 
                            NVL(BIL_CHNG_RMRKS1, ' ') BIL_CHNG_RMRKS1, NVL(BIL_CHNG_RMRKS2, ' ') BIL_CHNG_RMRKS2, FAULTY_METER 
                        FROM BIL_ELEC E WHERE BIL_STOPED='N' AND BGID=(SELECT BG_ID FROM BILL_GENERATE WHERE BILL_MONTH=:BILLMONTH)";

        string queryWATER = @"SELECT 
                              (SELECT bm_id FROM BILLING_MONTH_WATER WHERE BILLMONTH=:BILLMONTH) bg_id,
                              (SELECT bm FROM BILLING_MONTH_WATER WHERE BILLMONTH=:BILLMONTH) BG_NAME,
                              TO_CHAR(W.ISSUE_DATE, 'DD-MON-RR') ISSUE_DATE, TO_CHAR(W.DUE_DATE, 'DD-MON-RR') DUE_DATE, 
                              W.BARCODE, W.REF_NO, W.RES_ID, W.RES_NAME, W.HOUSE_NO,w.CELLNO,w.CATE_ID,w.RCAT_NM,
                              w.PRECINCT_ID,W.PRECINCT_NM,W.BLOCK_ID, W.BLOCK_NM,
                              ONE_LINK, KUICKPAY, SNBL, SNBL ubl, 
                              W.METER_NO, W.WATER_METER_FROM, W.WATER_METER_TO, w.MFACTOR, w.WATER_UNITS, w.WATER_UNIT_RATE, 
                              w.OPN_WALLET,w.OPN_ARREARS, w.ARR_INSTALLMENT, w.INS,w.FINE,FIXED_CHARGES,w.SEVERAGE_COST, W.WATER_BOWSER_CHARGES,
                              w.AMOUNT_PAYBLE,w.NET_PAYBLE,w.LATE_CHARGES,w.NET_PAYBLE_AFTER_DUEDATE,
                              w.AMNT_RECEIVE, TO_CHAR(w.AMNT_REC_DATE, 'DD-MON-RR') AMNT_REC_DATE,w.PAY_IND,
                              DECODE(w.PAY_IND, 0, 'OTHERS', 1, '1LINK', 2, 'KUICKPAY', 4, 'UBL', 10, 'CASH', 'UNKNOWN') AS PAYMENT_SOURCE
                            FROM BILLS_WATER W WHERE W.BM_ID=(SELECT BM_ID FROM BILLING_MONTH_WATER WHERE BILLMONTH=:BILLMONTH)";


        string queryGAS = @"SELECT 
                            g.bm_id,g.BIL_MON_NM, 
                            TO_CHAR(G.ISSUE_DATE, 'DD-MON-RR') ISSUE_DATE, TO_CHAR(G.DUE_DATE, 'DD-MON-RR') DUE_DATE, 
                            g.REF_NO, g.barcode, G.RES_ID, G.RES_NAME, G.HOUSE_NO, g.CELLNO, g.CATE_ID, g.CATE_NM, 
                            G.PRECINCT_ID,g.PRECENT_NM, g.BLOCK_ID,g.BLOCK_NM,
                            g.LINK1, g.KP, g.SNBL_CD, g.UBL_ACCT,g.METER_NO,
                            G.GAS_METER_FROM, G.GAS_METER_TO, G.GAS_UNITS, G.MFACTOR, G.GAS_UNITS_MF, G.GAS_UNIT_RATE, G.GAS_AMOUNT, 
                            G.OPN_WALLET, G.OPN_ARREARS, G.ARR_INSTALLMENT, G.INS, G.T_CON_CHARGES, G. C_CON_CHARGES, 
                            G.FIXED_CHARGES, G.NET_PAYBLE, G.LATE_CHARGES, G.NET_PAYBLE_AFTER_DUEDATE, 
                            NVL(G.AMNT_RECEIVE,0) AMNT_RECEIVE, TO_CHAR(G.AMNT_REC_DATE, 'DD-MON-RR') AMNT_REC_DATE, g.PAY_IND, 
                            DECODE(g.PAY_IND, 0, 'OTHERS', 1, '1LINK', 2, 'KUICKPAY', 4, 'UBL', 10, 'CASH', 'UNKNOWN') AS PAYMENT_SOURCE 
                        FROM BILLS_GAS G WHERE G.BM_ID=(SELECT B.BG_ID FROM BILL_GENERATE B WHERE B.BILL_MONTH=:BILLMONTH)";

        string queryRENT = @"SELECT 
                              (SELECT I.BM_ID FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) BM,
                              (SELECT I.ISSUE_DATE FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) ISSUEDATE,
                              (SELECT I.DUE_DATE FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) DUEDATE,
                              (SELECT I.PERC FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) PERC,
                              (SELECT I.USER_ID FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) USER_ID,
                              (SELECT I.DATE_TIME FROM ISSUE_DUE_RENT I WHERE I.BM_ID=R.BG_ID) DATE_TIME,
                              R.RES_ID ,R.REF_NO ,R.CHL_NO ,R.ARREARS 
                              ,R.FIXED_CHARGES ,R.AMT_INDATE ,R.LC ,R.AMT_AFDATE ,R.NAME ,R.ADDRESS ,R.INS_AMT 
                              ,R.WAIVE_AMT ,R.FINE ,R.PAID 
                              ,R.WAIVE_AMT_BILL ,R.INST_AMT_ARR ,R.INST_AMT_BILL ,R.TOTAL_INST ,R.TOTAL_WAIVE ,R.WAIVE_AMT_ARR 
                              ,R.QUIK_PAY ,R.REF_RENT ,R.BILL_AMT_REC ,R.REC_DATE ,R.INFRA_STRUC_AMT ,R.BIL_STOPED ,
                              'RECEIVING' KE_R_ID 
                        FROM BILLS_RENT R WHERE R.BG_ID=(SELECT B.BG_ID FROM BILL_GENERATE B WHERE B.BILL_MONTH=:BILLMONTH)";

        string queryBNB = "SELECT * FROM BILL_BNB WHERE BG_ID=(SELECT B.BG_ID FROM BILL_GENERATE B WHERE B.BILL_MONTH=:BILLMONTH)";


        /* -- END QUERIES -- */

        BillConfig = new Dictionary<BillType, BillExportConfig>
        {
            {
                BillType.Maintenance,
                new BillExportConfig(
                    connStrMNT,
                    queryMNT,
                    "MAINT_SUMMARY.csv"
                )
            },
            {
                BillType.Electric,
                new BillExportConfig(
                    connStrELEC,
                    queryELEC,
                    "ELECTRIC_SUMMARY.csv"
                )
            },
            {
                BillType.Water,
                new BillExportConfig(
                    connStrWATER,
                    queryWATER,
                    "WATER_SUMMARY.csv"
                )
            },
            {
                BillType.Gas,
                new BillExportConfig(
                    connStrMNT,
                    queryGAS,
                    "GAS_SUMMARY.csv"
                )
            },
            {
                BillType.Rent,
                new BillExportConfig(
                    connStrMNT,
                    queryRENT,
                    "RENT_SUMMARY.csv"
                )
            },
            {
                BillType.BNB,
                new BillExportConfig(
                    connStrMNT,
                    queryBNB,
                    "BNB_SUMMARY.csv"
                )
            }
        };
    }

    /* PAGE LOAD */
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtProcessMonth.Focus();
            rbBillType.Items.Clear(); // make sure old items are gone

            foreach (BillType bt in Enum.GetValues(typeof(BillType)))
            {
                rbBillType.Items.Add(
                    new ListItem(bt.ToString(), ((int)bt).ToString())
                );
            }

            rbBillType.SelectedValue = ((int)BillType.Maintenance).ToString();
        }
    }

    /* LOGGER */
    private void Log(string msg)
    {
        lblStatus.Text += msg + "<br/>";
        lblStatus.ForeColor = System.Drawing.Color.Green;
    }

    /* CANCEL BUTTON */
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        txtProcessMonth.Text = "";
        lblStatus.Text = "";
        txtProcessMonth.Focus();
    }


    /* Generate Summary Button */
    /* MAIN LOGIC */
    protected void btnGenSummary_Click(object sender, EventArgs e)
    {
        try
        {
            // Parse selected BillType safely
            int selectedBillTypeInt;
            if (!int.TryParse(rbBillType.SelectedValue, out selectedBillTypeInt) ||
                !Enum.IsDefined(typeof(BillType), selectedBillTypeInt))
            {
                throw new InvalidOperationException("Selected bill type is not valid.");
            }

            BillType selectedBillType = (BillType)selectedBillTypeInt;

            if (!BillConfig.ContainsKey(selectedBillType))
                throw new InvalidOperationException("No configuration defined for this Bill Type.");

            var config = BillConfig[selectedBillType];

            // Bill Month Validation
            string billMonth = txtProcessMonth.Text.Trim();

            if (string.IsNullOrEmpty(billMonth) || billMonth.Length != 6 || !billMonth.All(char.IsDigit))
            {
                throw new InvalidOperationException("Please enter a valid Bill Month (YYYYMM).");
            }

            // Report
            Log("Starting actual export for " + selectedBillType);
            using (OracleConnection con = new OracleConnection(config.ConnectionString))
            {
                con.Open();
                ExportCsv(config.Sql, config.FileName, con, billMonth);
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Export Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    /* CSV EXPORT */
    private void ExportCsv(string sql, string fileName, OracleConnection con, string billMonth)
    {
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            // IMPORTANT: Oracle binds parameters by position unless specified
            cmd.BindByName = true;

            cmd.Parameters.Add(
                new OracleParameter("BILLMONTH", OracleDbType.Varchar2)
                {
                    Value = billMonth
                }
            );
            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // HEADER
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    sb.Append("\"" + dr.GetName(i) + "\"");
                    if (i < dr.FieldCount - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // DATA
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string value = dr[i] == DBNull.Value ? "" : dr[i].ToString();
                        value = value.Replace("\"", "\"\"");
                        sb.Append("\"" + value + "\"");

                        if (i < dr.FieldCount - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                // DOWNLOAD
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/csv";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AddHeader(
                    "Content-Disposition",
                    "attachment; filename = " + fileName
                );

                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();

                lblStatus.Text = "Report Generated Successfully!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
        }
    }
}
