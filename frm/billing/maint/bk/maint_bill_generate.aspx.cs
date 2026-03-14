using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class maint_bill_generate : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
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
        lblStatus.Text = ""; // reset

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

                using (OracleCommand cmd = new OracleCommand("SP_GEN_BIL_MAINT", con))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // IN parameter
                    cmd.Parameters.Add("P_PROCESS_MONTH", OracleDbType.Varchar2).Value = processMonth;

                    // OUT parameter
                    cmd.Parameters.Add("m_return", OracleDbType.Int32).Direction =
                        System.Data.ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    lblStatus.Text = "Maintenance Billing has been generated successfully...";
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

                    // ---- GET PREVIOUS BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Bill Count='|| TO_CHAR(COUNT(*), 'FM999G999G999G999')|| ' and Bill Cost='|| TO_CHAR(NVL(ROUND(SUM(AMNT_WTDATE), 0), 0),'FM999G999G999G999') FROM BILL_GENERATE_AMOUNT B WHERE B.BG_ID=(SELECT G.BG_ID FROM BILL_GENERATE G WHERE G.IS_LOCKED='N')",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtPrvBill.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }

                    // ---- GET CURRENT BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Bill Count='|| TO_CHAR(COUNT(*), 'FM999G999G999G999')|| ' and Bill Cost='|| TO_CHAR(NVL(ROUND(SUM(AMNT_WTDATE), 0), 0),'FM999G999G999G999') FROM BILL_GENERATE_AMOUNT_TOBEN",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtCrntBill.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }

                    // ---- GET ADVANCES BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Adv Cnt='||TO_CHAR(COUNT(*), 'FM999G999G999G999')||' and Cost='||TO_CHAR(NVL(ROUND(SUM(BILL_AMOUNT),0),0), 'FM999G999G999G999') FROM BILL_DETAIL_TOBEN WHERE COLUMN_ID=1004 AND BILL_AMOUNT>0",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtAdvance.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }

                    // ---- GET FINE BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Fine Cnt='||TO_CHAR(COUNT(*), 'FM999G999G999G999')||' and Cost='||TO_CHAR(NVL(ROUND(SUM(BILL_AMOUNT),0),0), 'FM999G999G999G999') FROM BILL_DETAIL_TOBEN WHERE COLUMN_ID=1022 AND BILL_AMOUNT>0",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtFine.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }

                    // ---- GET ARREARS BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Arer Cnt='||TO_CHAR(COUNT(*), 'FM999G999G999G999')||' and Cost='||TO_CHAR(NVL(ROUND(SUM(BILL_AMOUNT),0),0), 'FM999G999G999G999') FROM BILL_DETAIL_TOBEN WHERE COLUMN_ID=1002 AND BILL_AMOUNT>0",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtArrears.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }

                    // ---- GET INSTALLMENT BILL COUNT ----
                    using (OracleCommand cmdCnt =
                        new OracleCommand(
                            "SELECT 'Inst Cnt='||TO_CHAR(COUNT(*), 'FM999G999G999G999')||' and Bill Cost='||TO_CHAR(NVL(ROUND(SUM(BILL_AMOUNT),0),0), 'FM999G999G999G999') FROM BILL_DETAIL_TOBEN WHERE COLUMN_ID=1014 AND BILL_AMOUNT>0",
                            con))
                    {
                        object cnt = cmdCnt.ExecuteScalar();
                        txtInst.Text =
                            cnt == null ? "0" : cnt.ToString();
                    }
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

                string sql = "SELECT * FROM V_GEN_BIL_MAINT";

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
                        "attachment; filename=V_GEN_BIL_MAINT.csv"
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
