using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class water_bill_generate : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionWTR"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            txtProcessMonth.Focus();
        }
    }

    protected void btnProcess_Click(object sender, EventArgs e)
    {
        string processMonth = txtProcessMonth.Text.Trim();
        lblStatus.Text = "";
        decimal m_bil_trend = 0;

        decimal m_bil_cost = 0;
        decimal m_adv_cnt = 0;
        decimal m_adv_amt = 0;
        decimal m_arr_cnt = 0;
        decimal m_arr_amt = 0;
        decimal m_inst_cnt = 0;
        decimal m_inst_amt = 0;
        decimal m_inst_amt0 = 0;
        decimal m_inst_amt1 = 0;
        decimal m_fin_cnt = 0;
        decimal m_fin_amt = 0;
        decimal m_count_cmon = 0;

        decimal m_bil_cost_old = 0;
        decimal m_adv_cnt_old = 0;
        decimal m_adv_amt_old = 0;
        decimal m_arr_cnt_old = 0;
        decimal m_arr_amt_old = 0;
        decimal m_inst_cnt_old = 0;
        decimal m_inst_amt_old = 0;
        decimal m_inst_amt0_old = 0;
        decimal m_inst_amt1_old = 0;
        decimal m_fin_cnt_old = 0;
        decimal m_fin_amt_old = 0;
        decimal m_count_lmon = 0;

        // simple validation YYYYMM
        if (processMonth.Length != 6)
        {
            lblStatus.Text = "Please enter valid Process Month (YYYYMM).";
            lblStatus.ForeColor = System.Drawing.Color.Red;
            txtProcessMonth.Focus();
            return;
        }

        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                using (OracleCommand cmd = new OracleCommand("SP_GEN_BIL_WATER", con))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // IN parameter
//                    cmd.Parameters.Add("P_PROCESS_MONTH", OracleDbType.Varchar2).Value = processMonth;

                    // OUT parameter
                    cmd.Parameters.Add("m_return", OracleDbType.Int32).Direction =
                        System.Data.ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    lblStatus.Text = "Water Billing has been generated successfully...";
                    lblStatus.ForeColor = System.Drawing.Color.Green;

                    /*
                                        // Safe conversion without OracleDecimal
                                        object retObj = cmd.Parameters["m_return"].Value;
                                        int result = 0;

                                        if (retObj != null && retObj != DBNull.Value)
                                        {
                                            result = Convert.ToInt32(retObj);
                                        }
                                        else
                                        {
                                            result = -1; // default in case NULL
                                        }

                                        // Show message
                                        if (result == 0)
                                        {
                                            lblStatus.Text = "Maintenance Bills has been generated successfully...";
                                            lblStatus.ForeColor = System.Drawing.Color.Green;
                                        }
                                        else
                                        {
                                            lblStatus.Text = "Bill generation completed with value: " + result;
                                            lblStatus.ForeColor = System.Drawing.Color.Blue;
                                        }
                    */

                    // *** Old Month Data *** 
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT COUNT(*) FROM BILLS_WATER WHERE BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_count_lmon = Convert.ToDecimal(amtObj);
                        else
                            m_count_lmon = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(WATER_AMOUNT),0),0) FROM BILLS_WATER WHERE BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_bil_cost_old = Convert.ToDecimal(amtObj);
                        else
                            m_bil_cost_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER WHERE OPN_WALLET>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_adv_cnt_old = Convert.ToDecimal(amtObj);
                        else
                            m_adv_cnt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(OPN_WALLET),0),0) FROM BILLS_WATER WHERE OPN_WALLET>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_adv_amt_old = Convert.ToDecimal(amtObj);
                        else
                            m_adv_amt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(count(*),0) FROM BILLS_WATER WHERE OPN_ARREARS>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_arr_cnt_old = Convert.ToDecimal(amtObj);
                        else
                            m_arr_cnt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(OPN_ARREARS),0),0) FROM BILLS_WATER WHERE OPN_ARREARS>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_arr_amt_old = Convert.ToDecimal(amtObj);
                        else
                            m_arr_amt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER WHERE ARR_INSTALLMENT>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_cnt_old = Convert.ToDecimal(amtObj);
                        else
                            m_inst_cnt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(ARR_INSTALLMENT),0),0) FROM BILLS_WATER WHERE ARR_INSTALLMENT>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt_old = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*), 0) FROM BILLS_WATER A WHERE A.OPN_ARREARS > 0 AND BM_ID=(SELECT BM_ID FROM BILL_GENERATE WHERE IS_LOCKED='N') AND EXISTS (SELECT 1 FROM BILLS_WATER B WHERE B.RES_ID = A.RES_ID AND B.ARR_INSTALLMENT > 0 AND BM_ID=(SELECT BM_ID FROM BILL_GENERATE WHERE IS_LOCKED='N'))",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt0_old = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt0_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(A.ARR_INSTALLMENT), 0), 0) FROM BILLS_WATER A WHERE A.OPN_ARREARS > 0 AND BM_ID=(SELECT BM_ID FROM BILL_GENERATE WHERE IS_LOCKED='N') AND EXISTS (SELECT 1 FROM BILLS_WATER B WHERE B.RES_ID = A.RES_ID AND B.ARR_INSTALLMENT > 0 AND BM_ID=(SELECT BM_ID FROM BILL_GENERATE WHERE IS_LOCKED='N'))",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt1_old = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt1_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER WHERE FINE>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_fin_cnt_old = Convert.ToDecimal(amtObj);
                        else
                            m_fin_cnt_old = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(FINE),0),0) FROM BILLS_WATER WHERE FINE>0 AND BM_ID=(SELECT BM_ID FROM WATER_DATES WHERE ACTIVE=1)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_fin_amt_old = Convert.ToDecimal(amtObj);
                        else
                            m_fin_amt_old = 0;
                    }
                    // *** Old Month Data *** 

                    // *** New Month Data *** 
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT COUNT(*) FROM BILLS_WATER_TOBE",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_count_cmon = Convert.ToDecimal(amtObj);
                        else
                            m_count_cmon = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(WATER_AMOUNT),0),0) FROM BILLS_WATER_TOBE",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_bil_cost = Convert.ToDecimal(amtObj);
                        else
                            m_bil_cost = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER_TOBE WHERE OPN_WALLET>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_adv_cnt = Convert.ToDecimal(amtObj);
                        else
                            m_adv_cnt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(OPN_WALLET),0),0) FROM BILLS_WATER_TOBE WHERE OPN_WALLET>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_adv_amt = Convert.ToDecimal(amtObj);
                        else
                            m_adv_amt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(count(*),0) FROM BILLS_WATER_TOBE WHERE OPN_ARREARS>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_arr_cnt = Convert.ToDecimal(amtObj);
                        else
                            m_arr_cnt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(OPN_ARREARS),0),0) FROM BILLS_WATER_TOBE WHERE OPN_ARREARS>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_arr_amt = Convert.ToDecimal(amtObj);
                        else
                            m_arr_amt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER_TOBE WHERE ARR_INSTALLMENT>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_cnt = Convert.ToDecimal(amtObj);
                        else
                            m_inst_cnt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(ARR_INSTALLMENT),0),0) FROM BILLS_WATER_TOBE WHERE ARR_INSTALLMENT>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*), 0) FROM BILLS_WATER_TOBE A WHERE A.OPN_ARREARS > 0 AND EXISTS (SELECT 1 FROM BILLS_WATER_TOBE B WHERE B.RES_ID = A.RES_ID AND B.ARR_INSTALLMENT > 0)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt0 = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt0 = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(A.ARR_INSTALLMENT), 0), 0) FROM BILLS_WATER_TOBE A WHERE A.OPN_ARREARS > 0 AND EXISTS (SELECT 1 FROM BILLS_WATER_TOBE B WHERE B.RES_ID = A.RES_ID AND B.ARR_INSTALLMENT > 0)",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_inst_amt1 = Convert.ToDecimal(amtObj);
                        else
                            m_inst_amt1 = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(COUNT(*),0) FROM BILLS_WATER_TOBE WHERE FINE>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_fin_cnt = Convert.ToDecimal(amtObj);
                        else
                            m_fin_cnt = 0;
                    }

                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT NVL(ROUND(SUM(FINE),0),0) FROM BILLS_WATER_TOBE WHERE FINE>0",
                            con))
                    {
                        object amtObj = cmdCnt.ExecuteScalar();

                        if (amtObj != null && amtObj != DBNull.Value)
                            m_fin_amt = Convert.ToDecimal(amtObj);
                        else
                            m_fin_amt = 0;
                    }
                    // *** New Month Data *** 

                    m_bil_trend = (m_count_cmon - m_count_lmon);
                    if (m_bil_trend > 0)
                    {
                        lblTrend.Text = "▲ UPWARD " + m_bil_trend.ToString("N0") + " Bills";
                        lblTrend.ForeColor = System.Drawing.Color.Green;
                    }
                    else if (m_bil_trend < 0)
                    {
                        lblTrend.Text = "▼ DOWNWARD " + m_bil_trend.ToString("N0") + " Bills";
                        lblTrend.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        lblTrend.Text = "No Change";
                        lblTrend.ForeColor = System.Drawing.Color.Blue;
                    }

                    // *** Bill Calculation Old *** 
                    txtPrvBill.Text = "Bil.Cnt. " + m_count_lmon.ToString() + " Amt. " + (((m_bil_cost_old + m_arr_amt_old + m_inst_amt_old + m_fin_amt_old) - m_inst_amt1_old) - m_adv_amt_old).ToString("N0");
                    // *** Bill Calculation Old *** 

                    // *** Bill Calculation *** 
                    txtBillCost.Text = "Bil.Cnt. " + m_count_cmon.ToString() + " Amt. " + m_bil_cost.ToString("N0");
                    txtCrntBill.Text = (((m_bil_cost + m_arr_amt + m_inst_amt + m_fin_amt) - m_inst_amt1) - m_adv_amt).ToString("N0");
                    txtAdvance.Text = "Adv.Cnt. " + m_adv_cnt.ToString() + " Amt. " + m_adv_amt.ToString("N0");
                    txtFine.Text = "Fin.Cnt. " + m_fin_cnt.ToString() + " Amt. " + m_fin_amt.ToString("N0");
                    txtArrears.Text = "Arr.Cnt. " + (m_arr_cnt - m_inst_amt0).ToString() + " Amt. " + (m_arr_amt - m_inst_amt1).ToString("N0");
                    txtInst.Text = "Inst.Cnt. " + m_inst_cnt.ToString() + " Amt. " + m_inst_amt.ToString("N0");
                    // *** Bill Calculation *** 
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
        txtProcessMonth.Text = "";
        lblStatus.Text = "";
        txtPrvBill.Text = "";
        lblTrend.Text = "";
        txtBillCost.Text = "";
        txtCrntBill.Text = "";
        txtAdvance.Text = "";
        txtFine.Text = "";
        txtArrears.Text = "";
        txtInst.Text = "";

        txtProcessMonth.Focus();
    }

    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                string sql = "SELECT * FROM V_GEN_BIL_WATER";

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
                        "attachment; filename=V_GEN_BIL_WATER.csv"
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
}
