using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;

public partial class chart_of_accounts : System.Web.UI.Page
{
    private readonly string connStr = WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["CurrentCompId"] == null)
                Session["CurrentCompId"] = 1;
            if (Session["CurrentLogId"] == null)
                Session["CurrentLogId"] = 0;
            if (Session["Username"] != null)
            {
                lblUser.Text = "Welcome, " + Session["Username"].ToString();
            }

            rbBillType.SelectedValue = "A";
            LoadLevel2("A");
            ClearForm();
        }
    }

    #region Load Levels

    private void LoadLevel2(string family)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT GL_CODE AS Code, GL_DESCRP AS Description
                           FROM GL_GLMF
                           WHERE FAMILY = :family AND LEVELL = 2 AND COMP_ID = :compId
                           ORDER BY GL_CODE";
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("family", family);
            cmd.Parameters.Add("compId", GetCurrentCompId());
            gvLevel2.DataSource = cmd.ExecuteReader();
            gvLevel2.DataBind();
        }
    }

    private void LoadLevel3(string parentCode)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT GL_CODE AS Code, GL_DESCRP AS Description
                           FROM GL_GLMF
                           WHERE PARENTT = :parent AND LEVELL = 3 AND COMP_ID = :compId
                           ORDER BY GL_CODE";
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("parent", parentCode);
            cmd.Parameters.Add("compId", GetCurrentCompId());
            gvLevel3.DataSource = cmd.ExecuteReader();
            gvLevel3.DataBind();
        }
    }

    private void LoadLevel4(string parentCode)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT GL_CODE AS Code, GL_DESCRP AS Description
                           FROM GL_GLMF
                           WHERE PARENTT = :parent AND LEVELL = 4 AND COMP_ID = :compId
                           ORDER BY GL_CODE";
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("parent", parentCode);
            cmd.Parameters.Add("compId", GetCurrentCompId());
            gvLevel4.DataSource = cmd.ExecuteReader();
            gvLevel4.DataBind();
        }
    }

    private void LoadSingleAccount(string code)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT * FROM GL_GLMF WHERE GL_CODE = :code AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add("code", code);
            cmd.Parameters.Add("compId", GetCurrentCompId());
            OracleDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                long nextCode = Convert.ToInt64(dr["GL_CODE"].ToString()) + 1;

                txtParent.Text = dr["PARENTT"].ToString();
                txtFamily.Text = dr["FAMILY"] != DBNull.Value ? dr["FAMILY"].ToString() : "";
                txtCode.Text = nextCode.ToString();
                txtAccLevel.Text = dr["LEVELL"].ToString();
                txtDesc.Text = dr["GL_DESCRP"].ToString();
                txtGenDetail.Text = dr["GENERAL_DETAIL"].ToString();
                txtAI.Text = dr["ACTIVE"].ToString();

                int level = Convert.ToInt32(dr["LEVELL"]);

                if (level == 4)
                {
                    LoadOpeningBalance(code, conn);
                    txtOB.ReadOnly = false;
                    txtOB.CssClass = "asp-input input-medium";
                }
                else
                {
                    txtOB.Text = "";
                    txtOB.ReadOnly = true;
                    txtOB.CssClass = "asp-input input-medium readonly-field";
                }
            }
            dr.Close();
        }
    }

    private void LoadOpeningBalance(string code, OracleConnection conn)
    {
        string sql = @"SELECT OPENING_BALANCE FROM GL_GLMF_OPENING_BALANCE WHERE GL_CODE = :code AND COMP_ID = :compId";
        OracleCommand cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add("code", code);
        cmd.Parameters.Add("compId", GetCurrentCompId());
        object result = cmd.ExecuteScalar();
        txtOB.Text = result == null ? "0" : result.ToString();
    }

    #endregion

    #region Grid Events

    protected void rbBillType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string family = rbBillType.SelectedValue;
        LoadLevel2(family);
        gvLevel3.DataSource = null;
        gvLevel3.DataBind();
        gvLevel4.DataSource = null;
        gvLevel4.DataBind();
        ClearForm();
    }

    protected void gvLevel2_SelectedIndexChanged(object sender, EventArgs e)
    {
        string code = gvLevel2.SelectedDataKey.Value.ToString();
        LoadLevel3(code);
        LoadSingleAccount(code);
        gvLevel4.DataSource = null;
        gvLevel4.DataBind();
    }

    protected void gvLevel3_SelectedIndexChanged(object sender, EventArgs e)
    {
        string code = gvLevel3.SelectedDataKey.Value.ToString();
        LoadLevel4(code);
        LoadSingleAccount(code);
    }

    protected void gvLevel4_SelectedIndexChanged(object sender, EventArgs e)
    {
        string code = gvLevel4.SelectedDataKey.Value.ToString();
        LoadSingleAccount(code);
    }

    #endregion

    #region Button Events

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                OracleTransaction trans = conn.BeginTransaction();

                string code = txtCode.Text.Trim();
                string desc = txtDesc.Text.Trim();
                string family = txtFamily.Text;
                string parent = txtParent.Text;
                int level = Convert.ToInt32(txtAccLevel.Text);
                string genDetail = txtGenDetail.Text;
                string active = txtAI.Text;
                decimal obValue = string.IsNullOrWhiteSpace(txtOB.Text) ? 0 : Convert.ToDecimal(txtOB.Text);

                // Check if exists - FIXED: removed duplicate parameter
                string checkSql = "SELECT COUNT(*) FROM GL_GLMF WHERE GL_CODE = :pCode AND COMP_ID = :pCompId";
                OracleCommand checkCmd = new OracleCommand(checkSql, conn);
                checkCmd.Transaction = trans;
                checkCmd.Parameters.Add("pCode", code);
                checkCmd.Parameters.Add("pCompId", GetCurrentCompId());
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists == 0)
                {
                    // INSERT - FIXED: unique parameter names
                    string insertSql = @"INSERT INTO GL_GLMF 
                    (GL_CODE, GL_DESCRP, FAMILY, PARENTT, LEVELL, GENERAL_DETAIL, ACTIVE, COMP_ID, LOG_ID, GL_CODE_KEY)
                    VALUES (:pCode, :pDesc, :pFamily, :pParent, :pLevel, :pGenDetail, :pActive, :pCompId, :pLogId, :pCodeKey)";
                    OracleCommand insertCmd = new OracleCommand(insertSql, conn);
                    insertCmd.Transaction = trans;
                    insertCmd.Parameters.Add("pCode", code);
                    insertCmd.Parameters.Add("pDesc", desc);
                    insertCmd.Parameters.Add("pFamily", family);
                    insertCmd.Parameters.Add("pParent", parent);
                    insertCmd.Parameters.Add("pLevel", level);
                    insertCmd.Parameters.Add("pGenDetail", genDetail);
                    insertCmd.Parameters.Add("pActive", active);
                    insertCmd.Parameters.Add("pCompId", GetCurrentCompId());
                    insertCmd.Parameters.Add("pLogId", transactionLogId);
                    insertCmd.Parameters.Add("pCodeKey", code);
                    insertCmd.ExecuteNonQuery();

                    // Insert opening balance ONLY for Level 4
                    if (level == 4 && obValue != 0)
                    {
                        string insertObSql = @"INSERT INTO GL_GLMF_OPENING_BALANCE 
                        (GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
                        VALUES (:pCode, :pOb, :pCompId, :pLogId)";
                        OracleCommand insertObCmd = new OracleCommand(insertObSql, conn);
                        insertObCmd.Transaction = trans;
                        insertObCmd.Parameters.Add("pCode", code);
                        insertObCmd.Parameters.Add("pOb", obValue);
                        insertObCmd.Parameters.Add("pCompId", GetCurrentCompId());
                        insertObCmd.Parameters.Add("pLogId", transactionLogId);
                        insertObCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // UPDATE - FIXED: unique parameter names
                    string updateSql = @"UPDATE GL_GLMF SET GL_DESCRP = :pDesc WHERE GL_CODE = :pCode AND COMP_ID = :pCompId";
                    OracleCommand updateCmd = new OracleCommand(updateSql, conn);
                    updateCmd.Transaction = trans;
                    updateCmd.Parameters.Add("pDesc", desc);
                    updateCmd.Parameters.Add("pCode", code);
                    updateCmd.Parameters.Add("pCompId", GetCurrentCompId());
                    updateCmd.ExecuteNonQuery();

                    // Update opening balance ONLY for Level 4
                    if (level == 4)
                    {
                        string checkObSql = "SELECT COUNT(*) FROM GL_GLMF_OPENING_BALANCE WHERE GL_CODE = :pCode AND COMP_ID = :pCompId";
                        OracleCommand checkObCmd = new OracleCommand(checkObSql, conn);
                        checkObCmd.Transaction = trans;
                        checkObCmd.Parameters.Add("pCode", code);
                        checkObCmd.Parameters.Add("pCompId", GetCurrentCompId());
                        int obExists = Convert.ToInt32(checkObCmd.ExecuteScalar());

                        if (obExists > 0)
                        {
                            string updateObSql = @"UPDATE GL_GLMF_OPENING_BALANCE 
                            SET OPENING_BALANCE = :pOb, LOG_ID = :pLogId 
                            WHERE GL_CODE = :pCode AND COMP_ID = :pCompId";
                            OracleCommand updateObCmd = new OracleCommand(updateObSql, conn);
                            updateObCmd.Transaction = trans;
                            updateObCmd.Parameters.Add("pOb", obValue);
                            updateObCmd.Parameters.Add("pLogId", transactionLogId);
                            updateObCmd.Parameters.Add("pCode", code);
                            updateObCmd.Parameters.Add("pCompId", GetCurrentCompId());
                            updateObCmd.ExecuteNonQuery();
                        }
                        else if (obValue != 0)
                        {
                            string insertObSql = @"INSERT INTO GL_GLMF_OPENING_BALANCE 
                            (GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
                            VALUES (:pCode, :pOb, :pCompId, :pLogId)";
                            OracleCommand insertObCmd = new OracleCommand(insertObSql, conn);
                            insertObCmd.Transaction = trans;
                            insertObCmd.Parameters.Add("pCode", code);
                            insertObCmd.Parameters.Add("pOb", obValue);
                            insertObCmd.Parameters.Add("pCompId", GetCurrentCompId());
                            insertObCmd.Parameters.Add("pLogId", transactionLogId);
                            insertObCmd.ExecuteNonQuery();
                        }
                    }
                }

                trans.Commit();
                Session["CurrentLogId"] = transactionLogId;

                // Refresh UI
                RefreshUI(level, parent, family);

                ShowStatus("Record saved successfully!", "success");
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }
    private void RefreshUI(int level, string parent, string family)
    {
        LoadLevel2(family);

        if (level == 2)
        {
            // Find parent Level 2's parent (Level 1 family)
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT PARENTT FROM GL_GLMF WHERE GL_CODE = :code AND COMP_ID = :compId";
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("code", parent);
                cmd.Parameters.Add("compId", GetCurrentCompId());
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    LoadLevel3(result.ToString());
                }
            }
        }
        else if (level == 3)
        {
            LoadLevel3(parent);
        }
        else if (level == 4)
        {
            LoadLevel4(parent);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        ShowStatus("Form cleared", "info");
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/gl/reports/chart_of_accounts_report.aspx", false);
    }

    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx", false);
    }

    #endregion

    #region Utility Methods

    private void ClearForm()
    {
        txtParent.Text = "";
        txtFamily.Text = "";
        txtCode.Text = "";
        txtAccLevel.Text = "";
        txtDesc.Text = "";
        txtGenDetail.Text = "";
        txtAI.Text = "";
        txtOB.Text = "";
        txtOB.ReadOnly = true;
        txtOB.CssClass = "asp-input input-medium readonly-field";
        statusContainer.Visible = false;
    }

    private void ShowStatus(string message, string type)
    {
        lblStatus.Text = message;
        statusContainer.Visible = true;

        if (type == "success")
            statusContainer.Attributes["class"] = "status-msg status-success";
        else if (type == "error")
            statusContainer.Attributes["class"] = "status-msg status-error";
        else
            statusContainer.Attributes["class"] = "status-msg";
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    #endregion
}