//using System;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.Configuration;
//using System.Collections.Generic;
//using Oracle.ManagedDataAccess.Client;

//public partial class chart_of_accounts : System.Web.UI.Page
//{
//    /* CONNECTION STRING */
//    private readonly string connStr =
//        WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;
    
//    //private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;
//    protected void Page_Load(object sender, EventArgs e)
//    {
//        if (!IsPostBack)
//        {
//            pcdInit();

//            // If first load, manually call the event
//            rbBillType_SelectedIndexChanged(null, null);

//            if (Session["User"] != null)
//            {
//                string userId = Session["login_id"].ToString();
//                string userName = Session["login_name"].ToString();
//                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
//                string ipAddress = Request.UserHostAddress;

//                lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
//            }
//        }
//    }

//    /* INITIALIZATION */
//    protected void pcdInit()
//    {
//        lblStatus.Text = "";

//        rbBillType.Items.Clear();

//        rbBillType.Items.Add(new ListItem("Assets", "A"));
//        rbBillType.Items.Add(new ListItem("Liability", "L"));
//        rbBillType.Items.Add(new ListItem("Capital", "C"));
//        rbBillType.Items.Add(new ListItem("Revenue", "R"));
//        rbBillType.Items.Add(new ListItem("Expenses", "E"));

//        rbBillType.SelectedValue = "A"; // default = Assets
//    }

//    /* CHANGE ON SELECTION */
//    protected void rbBillType_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        string family = rbBillType.SelectedValue;

//        LoadLevel2(family);

//        gvLevel3.DataSource = null;
//        gvLevel3.DataBind();

//        gvLevel4.DataSource = null;
//        gvLevel4.DataBind();

//        // 🔥 Auto-load top Level 2 record
//        LoadTopRecordOfLevel(2, null, family);
//    }

//    protected void gvLevel2_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        string parentCode = gvLevel2.SelectedDataKey.Value.ToString();

//        LoadLevel3(parentCode);

//        gvLevel4.DataSource = null;
//        gvLevel4.DataBind();

//        // 🔥 Auto-load top Level 3 record
//        LoadTopRecordOfLevel(3, parentCode, null);
//    }

//    protected void gvLevel3_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        string parentCode = gvLevel3.SelectedDataKey.Value.ToString();

//        LoadLevel4(parentCode);

//        // 🔥 Auto-load top Level 4 record
//        LoadTopRecordOfLevel(4, parentCode, null);
//    }

//    protected void gvLevel4_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        string code = gvLevel4.SelectedDataKey.Value.ToString();
//        LoadSingleAccount(code);
//    }

//    /* LOAD LEVELS */
//    private void LoadLevel2(string family)
//    {
//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();

//            string sql = @"SELECT GL_CODE AS Code,
//                              GL_DESCRP AS Description
//                       FROM GL_GLMF
//                       WHERE FAMILY = :family
//                       AND LEVELL = 2
//                       ORDER BY GL_CODE DESC";

//            OracleCommand cmd = new OracleCommand(sql, conn);
//            cmd.BindByName = true;
//            cmd.Parameters.Add("family", family);

//            gvLevel2.DataSource = cmd.ExecuteReader();
//            gvLevel2.DataBind();
//        }
//    }

//    private void LoadLevel3(string parentCode)
//    {
//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();

//            string sql = @"SELECT GL_CODE AS Code,
//                              GL_DESCRP AS Description
//                       FROM GL_GLMF
//                       WHERE PARENTT = :parent
//                       AND LEVELL = 3
//                       ORDER BY GL_CODE DESC";

//            OracleCommand cmd = new OracleCommand(sql, conn);
//            cmd.BindByName = true;
//            cmd.Parameters.Add("parent", parentCode);

//            gvLevel3.DataSource = cmd.ExecuteReader();
//            gvLevel3.DataBind();
//        }
//    }

//    private void LoadLevel4(string parentCode)
//    {
//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();

//            string sql = @"SELECT g.GL_CODE AS Code,
//                               g.GL_DESCRP AS Description,
//                               NVL(ob.OPENING_BALANCE, 0) AS Opening_Balance
//                        FROM GL_GLMF g
//                        LEFT JOIN GL_GLMF_OPENING_BALANCE ob
//                               ON g.GL_CODE = ob.GL_CODE
//                               AND ob.COMP_ID = 1
//                        WHERE g.PARENTT = :parent
//                        AND g.LEVELL = 4
//                        ORDER BY g.GL_CODE DESC";

//            OracleCommand cmd = new OracleCommand(sql, conn);
//            cmd.BindByName = true;
//            cmd.Parameters.Add("parent", parentCode);

//            gvLevel4.DataSource = cmd.ExecuteReader();
//            gvLevel4.DataBind();
//        }
//    }

//    private void LoadSingleAccount(string code)
//    {
//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();

//            string sql = @"SELECT *
//                       FROM GL_GLMF
//                       WHERE GL_CODE = :code";

//            OracleCommand cmd = new OracleCommand(sql, conn);
//            cmd.BindByName = true;
//            cmd.Parameters.Add("code", code);

//            OracleDataReader dr = cmd.ExecuteReader();

//            if (dr.Read())
//            {
//                int gl_code = Convert.ToInt32(dr["GL_CODE"].ToString()); // convert varchar to int (for increment)
//                gl_code += 1; // increment by 1

//                txtParent.Text = dr["PARENTT"].ToString();
//                txtFamily.Text = dr["FAMILY"].ToString();
//                txtCode.Text = gl_code.ToString();
//                txtAccLevel.Text = dr["LEVELL"].ToString();
//                txtDesc.Text = dr["GL_DESCRP"].ToString();
//                txtGenDetail.Text = dr["GENERAL_DETAIL"].ToString();
//                txtAI.Text = dr["ACTIVE"].ToString();

//                int level = Convert.ToInt32(dr["LEVELL"]);

//                if (level == 4)
//                {
//                    LoadOpeningBalance(code, conn);
//                    txtOB.ReadOnly = false;
//                    txtOB.CssClass = "asp-input input-medium";
//                }
//                else
//                {
//                    txtOB.Text = "";
//                    txtOB.ReadOnly = true;
//                    txtOB.CssClass = "asp-input input-medium readonly-field";
//                }
//            }
//        }
//    }

//    private void LoadOpeningBalance(string code, OracleConnection conn)
//    {
//        string sql = @"SELECT OPENING_BALANCE
//                   FROM GL_GLMF_OPENING_BALANCE
//                   WHERE GL_CODE = :code";

//        OracleCommand cmd = new OracleCommand(sql, conn);
//        cmd.BindByName = true;
//        cmd.Parameters.Add("code", code);

//        object result = cmd.ExecuteScalar();

//        txtOB.Text = result == null ? "0" : result.ToString();
//    }

//    private void LoadTopRecordOfLevel(int lvl, string parentCode, string family)
//    {
//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();

//            string sql;

//            if (lvl == 2)
//            {
//                sql = @"SELECT GL_CODE
//                    FROM GL_GLMF
//                    WHERE FAMILY = :family
//                    AND LEVELL = 2
//                    ORDER BY GL_CODE DESC";
//            }
//            else
//            {
//                sql = @"SELECT GL_CODE
//                    FROM GL_GLMF
//                    WHERE PARENTT = :parent
//                    AND LEVELL = :lvl
//                    ORDER BY GL_CODE DESC";
//            }

//            OracleCommand cmd = new OracleCommand(sql, conn);
//            cmd.BindByName = true;

//            if (lvl == 2)
//                cmd.Parameters.Add("family", family);
//            else
//            {
//                cmd.Parameters.Add("parent", parentCode);
//                cmd.Parameters.Add("lvl", lvl);
//            }

//            object result = cmd.ExecuteScalar();

//            if (result != null)
//            {
//                LoadSingleAccount(result.ToString());
//            }
//            else
//            {
//                ClearForm();
//            }
//        }
//    }

////    private string GetRootCode(string family)
////    {
////        using (OracleConnection conn = new OracleConnection(connStr))
////        {
////            conn.Open();

////            string sql = @"SELECT GL_CODE
////                       FROM GL_GLMF
////                       WHERE FAMILY = :family
////                       AND LEVELL = 1";

////            OracleCommand cmd = new OracleCommand(sql, conn);
////            cmd.BindByName = true;
////            cmd.Parameters.Add("family", family);

////            object result = cmd.ExecuteScalar();
////            return result == null ? "" : result.ToString();
////        }
////    }

//    /* LOGGER */
//    private void Log(string msg)
//    {
//        lblStatus.Text += msg + "<br/>";
//        lblStatus.ForeColor = System.Drawing.Color.Green;
//    }

//    /* BUTTONS */
//    protected void btnGoBack_Click(object sender, EventArgs e)
//    {
//        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
//    }

//    protected void btnLogoff_Click(object sender, EventArgs e)
//    {
//        Session.Clear();
//        Session.Abandon();
//        Response.Redirect("~/login/Login.aspx", false);
//    }

//    protected void btnSave_Click(object sender, EventArgs e)
//    {
//        // CREATE NEW LOG ENTRY FOR THIS TRANSACTION
//        int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

//        using (OracleConnection conn = new OracleConnection(connStr))
//        {
//            conn.Open();
//            OracleTransaction trans = conn.BeginTransaction();

//            try
//            {
//                string code = txtCode.Text.Trim();
//                string desc = txtDesc.Text.Trim();
//                string family = txtFamily.Text;
//                string parent = txtParent.Text;
//                int level = Convert.ToInt32(txtAccLevel.Text);
//                string genDetail = txtGenDetail.Text;
//                string active = txtAI.Text;

//                int compId = GetCurrentCompId();
//                int logId = transactionLogId;

//                decimal obValue = 0;
//                if (!string.IsNullOrWhiteSpace(txtOB.Text))
//                    obValue = Convert.ToDecimal(txtOB.Text);

//                // 🔎 CHECK IF EXISTS
//                string checkSql = "SELECT COUNT(*) FROM GL_GLMF WHERE GL_CODE = :code";

//                OracleCommand checkCmd = new OracleCommand(checkSql, conn);
//                checkCmd.Transaction = trans;
//                checkCmd.BindByName = true;
//                checkCmd.Parameters.Add("code", code);

//                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

//                bool isInsert = count == 0;

//                if (isInsert)
//                {
//                    // ================= INSERT GL_GLMF =================
//                    string insertDetails = @"
//            INSERT INTO GL_GLMF
//            (GL_CODE, GL_DESCRP, FAMILY, PARENTT, LEVELL,
//             GENERAL_DETAIL, ACTIVE, COMP_ID, LOG_ID, GL_CODE_KEY)
//            VALUES
//            (:code, :gl_desc, :family, :parent,
//             :lvl, :gen, :active, :comp, :log, :codeKey)";  // ← Changed :code to :codeKey for GL_CODE_KEY

//                    OracleCommand cmdInsert = new OracleCommand(insertDetails, conn);
//                    cmdInsert.Transaction = trans;
//                    cmdInsert.BindByName = true;

//                    // Add parameters with unique names
//                    cmdInsert.Parameters.Add("code", OracleDbType.Varchar2).Value = code;        // For GL_CODE
//                    cmdInsert.Parameters.Add("gl_desc", OracleDbType.Varchar2).Value = desc;
//                    cmdInsert.Parameters.Add("family", OracleDbType.Varchar2).Value = family;
//                    cmdInsert.Parameters.Add("parent", OracleDbType.Varchar2).Value = parent;
//                    cmdInsert.Parameters.Add("lvl", OracleDbType.Int32).Value = level;
//                    cmdInsert.Parameters.Add("gen", OracleDbType.Varchar2).Value = genDetail;
//                    cmdInsert.Parameters.Add("active", OracleDbType.Varchar2).Value = active;
//                    cmdInsert.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
//                    cmdInsert.Parameters.Add("log", OracleDbType.Int32).Value = logId;            // This is your LOG_ID from USER_OP_LOG
//                    cmdInsert.Parameters.Add("codeKey", OracleDbType.Varchar2).Value = code;      // For GL_CODE_KEY (separate parameter)

//                    cmdInsert.ExecuteNonQuery();

//                    // ================= INSERT OB (ONLY LEVEL 4) =================
//                    if (level == 4)
//                    {
//                        string insertOB = @"
//                INSERT INTO GL_GLMF_OPENING_BALANCE
//                (GL_OB_KEY, GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
//                VALUES (:key, :code, :ob, :comp, :log)";

//                        OracleCommand cmdInsertOB = new OracleCommand(insertOB, conn);
//                        cmdInsertOB.Transaction = trans;
//                        cmdInsertOB.BindByName = true;

//                        cmdInsertOB.Parameters.Add("key", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
//                        cmdInsertOB.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
//                        cmdInsertOB.Parameters.Add("ob", OracleDbType.Decimal).Value = obValue;
//                        cmdInsertOB.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
//                        cmdInsertOB.Parameters.Add("log", OracleDbType.Int32).Value = logId;

//                        cmdInsertOB.ExecuteNonQuery();
//                    }
//                }
//                else
//                {
//                    // ================= UPDATE =================
//                    string updateSql = @"
//            UPDATE GL_GLMF
//            SET GL_DESCRP = :gl_desc
//            WHERE GL_CODE = :code";

//                    OracleCommand cmdUpdate = new OracleCommand(updateSql, conn);
//                    cmdUpdate.Transaction = trans;
//                    cmdUpdate.BindByName = true;

//                    cmdUpdate.Parameters.Add("gl_desc", OracleDbType.Varchar2).Value = desc;
//                    cmdUpdate.Parameters.Add("code", OracleDbType.Varchar2).Value = code;

//                    cmdUpdate.ExecuteNonQuery();

//                    if (level == 4)
//                    {
//                        string updateOB = @"
//                MERGE INTO GL_GLMF_OPENING_BALANCE ob
//                USING (SELECT :code AS GL_CODE FROM dual) src
//                ON (ob.GL_CODE = src.GL_CODE AND ob.COMP_ID = :comp)
//                WHEN MATCHED THEN
//                    UPDATE SET ob.OPENING_BALANCE = :ob
//                WHEN NOT MATCHED THEN
//                    INSERT (GL_OB_KEY, GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
//                    VALUES (:key, :code, :ob, :comp, :log)";

//                        OracleCommand cmdMerge = new OracleCommand(updateOB, conn);
//                        cmdMerge.Transaction = trans;
//                        cmdMerge.BindByName = true;

//                        cmdMerge.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
//                        cmdMerge.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
//                        cmdMerge.Parameters.Add("ob", OracleDbType.Decimal).Value = obValue;
//                        cmdMerge.Parameters.Add("key", OracleDbType.Varchar2).Value = Guid.NewGuid().ToString();
//                        cmdMerge.Parameters.Add("log", OracleDbType.Int32).Value = logId;

//                        cmdMerge.ExecuteNonQuery();
//                    }
//                }

//                trans.Commit();

//                // Update session with new log ID for next transaction
//                Session["CurrentLogId"] = transactionLogId;

//                // 🔄 Refresh UI
//                LoadSingleAccount(code);

//                if (level == 2) LoadLevel2(family);
//                else if (level == 3) LoadLevel3(parent);
//                else if (level == 4) LoadLevel4(parent);

//                lblStatus.ForeColor = System.Drawing.Color.Green;

//                if (isInsert)
//                    lblStatus.Text = "<b>New Record Added Successfully</b>";
//                else
//                    lblStatus.Text = "<b>Record Updated Successfully</b>";
//            }
//            catch (Exception ex)
//            {
//                trans.Rollback();
//                lblStatus.ForeColor = System.Drawing.Color.Red;
//                lblStatus.Text = "Error: " + ex.Message;
//            }
//        }
//    }

//    private int GetCurrentLogId()
//    {
//        return LogHelper.GetCurrentLogId(Session, Request);
//    }

//    private int GetCurrentCompId()
//    {
//        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
//    }

//    protected void btnCancel_Click(object sender, EventArgs e)
//    {
//        // Clear Form
//        ClearForm();

//        // Reset Radio to Assets
//        rbBillType.SelectedValue = "A";

//        // Reload Level 2
//        LoadLevel2("A");        

//        // Clear Lower Levels
//        gvLevel3.DataSource = null;
//        gvLevel3.DataBind();

//        gvLevel4.DataSource = null;
//        gvLevel4.DataBind();

//        // Load Top Record of Level 2
//        LoadTopRecordOfLevel(2, null, "A");        
//    }

//    protected void btnPrint_Click(object sender, EventArgs e)
//    {

//    }

//    private void ClearForm()
//    {
//        lblStatus.Text = "";

//        txtParent.Text = "";
//        txtAI.Text = "";
//        txtFamily.Text = "";
//        txtCode.Text = "";
//        txtAccLevel.Text = "";
//        txtDesc.Text = "";
//        txtGenDetail.Text = "";
//        txtOB.Text = ""; 
//    }

//}

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

public partial class chart_of_accounts : System.Web.UI.Page
{
    /* CONNECTION STRING */
    private readonly string connStr =
        WebConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pcdInit();

            // Load all levels on initial load
            LoadAllLevels();

            if (Session["User"] != null)
            {
                string userId = Session["login_id"].ToString();
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                string ipAddress = Request.UserHostAddress;

                lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;
            }
        }
    }

    /* INITIALIZATION */
    protected void pcdInit()
    {
        lblStatus.Text = "";
        ClearForm();
    }

    /* LOAD ALL LEVELS */
    private void LoadAllLevels()
    {
        LoadLevel1();

        // Auto-load first Level 1 record and its children
        LoadTopRecordOfLevel(1, null, null);
    }

    /* LOAD LEVEL 1 - ALL FAMILIES */
    private void LoadLevel1()
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql = @"SELECT GL_CODE AS Code,
                              GL_DESCRP AS Description
                       FROM GL_GLMF
                       WHERE LEVELL = 1
                       ORDER BY GL_CODE";

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;

            gvLevel1.DataSource = cmd.ExecuteReader();
            gvLevel1.DataBind();
        }
    }

    /* LOAD LEVEL 2 BASED ON SELECTED LEVEL 1 */
    private void LoadLevel2(string parentCode)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql = @"SELECT GL_CODE AS Code,
                              GL_DESCRP AS Description
                       FROM GL_GLMF
                       WHERE PARENTT = :parent
                       AND LEVELL = 2
                       ORDER BY GL_CODE";

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("parent", parentCode);

            gvLevel2.DataSource = cmd.ExecuteReader();
            gvLevel2.DataBind();
        }
    }

    /* LOAD LEVEL 3 BASED ON SELECTED LEVEL 2 */
    private void LoadLevel3(string parentCode)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql = @"SELECT g.GL_CODE AS Code,
                               g.GL_DESCRP AS Description,
                               NVL(ob.OPENING_BALANCE, 0) AS Opening_Balance
                        FROM GL_GLMF g
                        LEFT JOIN GL_GLMF_OPENING_BALANCE ob
                               ON g.GL_CODE = ob.GL_CODE
                               AND ob.COMP_ID = 1
                        WHERE g.PARENTT = :parent
                        AND g.LEVELL = 3
                        ORDER BY g.GL_CODE";

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("parent", parentCode);

            gvLevel3.DataSource = cmd.ExecuteReader();
            gvLevel3.DataBind();
        }
    }

    /* LEVEL 1 SELECTION CHANGED */
    protected void gvLevel1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string parentCode = gvLevel1.SelectedDataKey.Value.ToString();

        // Load Level 2 for this Level 1
        LoadLevel2(parentCode);

        // Clear Level 3
        gvLevel3.DataSource = null;
        gvLevel3.DataBind();

        // Auto-load top Level 2 record
        LoadTopRecordOfLevel(2, parentCode, null);
    }

    /* LEVEL 2 SELECTION CHANGED */
    protected void gvLevel2_SelectedIndexChanged(object sender, EventArgs e)
    {
        string parentCode = gvLevel2.SelectedDataKey.Value.ToString();

        // Load Level 3 for this Level 2
        LoadLevel3(parentCode);

        // Auto-load top Level 3 record
        LoadTopRecordOfLevel(3, parentCode, null);
    }

    /* LEVEL 3 SELECTION CHANGED */
    protected void gvLevel3_SelectedIndexChanged(object sender, EventArgs e)
    {
        string code = gvLevel3.SelectedDataKey.Value.ToString();
        LoadSingleAccount(code);
    }

    /* LOAD SINGLE ACCOUNT DETAILS */
    private void LoadSingleAccount(string code)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql = @"SELECT *
                       FROM GL_GLMF
                       WHERE GL_CODE = :code";

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("code", code);

            OracleDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                int gl_code = Convert.ToInt32(dr["GL_CODE"].ToString());
                gl_code += 1; // increment by 1 for new code suggestion

                txtParent.Text = dr["PARENTT"].ToString();
                txtFamily.Text = dr["FAMILY"] != DBNull.Value ? dr["FAMILY"].ToString() : "";
                txtCode.Text = gl_code.ToString();
                txtAccLevel.Text = dr["LEVELL"].ToString();
                txtDesc.Text = dr["GL_DESCRP"].ToString();
                txtGenDetail.Text = dr["GENERAL_DETAIL"].ToString();
                txtAI.Text = dr["ACTIVE"].ToString();

                int level = Convert.ToInt32(dr["LEVELL"]);

                // Only Level 3 can have opening balance
                if (level == 3)
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
        }
    }

    /* LOAD OPENING BALANCE */
    private void LoadOpeningBalance(string code, OracleConnection conn)
    {
        string sql = @"SELECT OPENING_BALANCE
                   FROM GL_GLMF_OPENING_BALANCE
                   WHERE GL_CODE = :code";

        OracleCommand cmd = new OracleCommand(sql, conn);
        cmd.BindByName = true;
        cmd.Parameters.Add("code", code);

        object result = cmd.ExecuteScalar();

        txtOB.Text = result == null ? "0" : result.ToString();
    }

    /* AUTO-LOAD TOP RECORD OF SELECTED LEVEL */
    private void LoadTopRecordOfLevel(int lvl, string parentCode, string family)
    {
        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();

            string sql;

            if (lvl == 1)
            {
                sql = @"SELECT GL_CODE
                    FROM GL_GLMF
                    WHERE LEVELL = 1
                    ORDER BY GL_CODE";
            }
            else
            {
                sql = @"SELECT GL_CODE
                    FROM GL_GLMF
                    WHERE PARENTT = :parent
                    AND LEVELL = :lvl
                    ORDER BY GL_CODE";
            }

            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;

            if (lvl == 1)
            {
                // No parameters needed
            }
            else
            {
                cmd.Parameters.Add("parent", parentCode);
                cmd.Parameters.Add("lvl", lvl);
            }

            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                LoadSingleAccount(result.ToString());
            }
            else
            {
                ClearForm();
            }
        }
    }

    /* BUTTON HANDLERS */
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

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

        using (OracleConnection conn = new OracleConnection(connStr))
        {
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();

            try
            {
                string code = txtCode.Text.Trim();
                string desc = txtDesc.Text.Trim();
                string family = txtFamily.Text;
                string parent = txtParent.Text;
                int level = Convert.ToInt32(txtAccLevel.Text);
                string genDetail = txtGenDetail.Text;
                string active = txtAI.Text;

                int compId = GetCurrentCompId();
                int logId = transactionLogId;

                decimal obValue = 0;
                if (!string.IsNullOrWhiteSpace(txtOB.Text))
                    obValue = Convert.ToDecimal(txtOB.Text);

                // Check if record exists
                string checkSql = "SELECT COUNT(*) FROM GL_GLMF WHERE GL_CODE = :code";
                OracleCommand checkCmd = new OracleCommand(checkSql, conn);
                checkCmd.Transaction = trans;
                checkCmd.BindByName = true;
                checkCmd.Parameters.Add("code", code);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                bool isInsert = count == 0;

                if (isInsert)
                {
                    // INSERT INTO GL_GLMF
                    string insertDetails = @"
                    INSERT INTO GL_GLMF
                    (GL_CODE, GL_DESCRP, FAMILY, PARENTT, LEVELL,
                     GENERAL_DETAIL, ACTIVE, COMP_ID, LOG_ID, GL_CODE_KEY)
                    VALUES
                    (:code, :gl_desc, :family, :parent,
                     :lvl, :gen, :active, :comp, :log, :codeKey)";

                    OracleCommand cmdInsert = new OracleCommand(insertDetails, conn);
                    cmdInsert.Transaction = trans;
                    cmdInsert.BindByName = true;

                    cmdInsert.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
                    cmdInsert.Parameters.Add("gl_desc", OracleDbType.Varchar2).Value = desc;
                    cmdInsert.Parameters.Add("family", OracleDbType.Varchar2).Value = family;
                    cmdInsert.Parameters.Add("parent", OracleDbType.Varchar2).Value = parent;
                    cmdInsert.Parameters.Add("lvl", OracleDbType.Int32).Value = level;
                    cmdInsert.Parameters.Add("gen", OracleDbType.Varchar2).Value = genDetail;
                    cmdInsert.Parameters.Add("active", OracleDbType.Varchar2).Value = active;
                    cmdInsert.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
                    cmdInsert.Parameters.Add("log", OracleDbType.Int32).Value = logId;
                    cmdInsert.Parameters.Add("codeKey", OracleDbType.Varchar2).Value = code;

                    cmdInsert.ExecuteNonQuery();

                    // INSERT OPENING BALANCE (ONLY FOR LEVEL 3)
                    if (level == 3)
                    {
                        string insertOB = @"
                        INSERT INTO GL_GLMF_OPENING_BALANCE
                        (GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
                        VALUES (:code, :ob, :comp, :log)";

                        OracleCommand cmdInsertOB = new OracleCommand(insertOB, conn);
                        cmdInsertOB.Transaction = trans;
                        cmdInsertOB.BindByName = true;

                        cmdInsertOB.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
                        cmdInsertOB.Parameters.Add("ob", OracleDbType.Decimal).Value = obValue;
                        cmdInsertOB.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
                        cmdInsertOB.Parameters.Add("log", OracleDbType.Int32).Value = logId;

                        cmdInsertOB.ExecuteNonQuery();
                    }
                }
                else
                {
                    // UPDATE GL_GLMF
                    string updateSql = @"
                    UPDATE GL_GLMF
                    SET GL_DESCRP = :gl_desc
                    WHERE GL_CODE = :code";

                    OracleCommand cmdUpdate = new OracleCommand(updateSql, conn);
                    cmdUpdate.Transaction = trans;
                    cmdUpdate.BindByName = true;

                    cmdUpdate.Parameters.Add("gl_desc", OracleDbType.Varchar2).Value = desc;
                    cmdUpdate.Parameters.Add("code", OracleDbType.Varchar2).Value = code;

                    cmdUpdate.ExecuteNonQuery();

                    // UPDATE OPENING BALANCE (ONLY FOR LEVEL 3)
                    if (level == 3)
                    {
                        // First check if record exists
                        string checkOB = "SELECT COUNT(*) FROM GL_GLMF_OPENING_BALANCE WHERE GL_CODE = :code AND COMP_ID = :comp";
                        OracleCommand checkOBCmd = new OracleCommand(checkOB, conn);
                        checkOBCmd.Transaction = trans;
                        checkOBCmd.BindByName = true;
                        checkOBCmd.Parameters.Add("code", code);
                        checkOBCmd.Parameters.Add("comp", compId);

                        int obCount = Convert.ToInt32(checkOBCmd.ExecuteScalar());

                        if (obCount > 0)
                        {
                            // Update existing record
                            string updateOB = @"
                            UPDATE GL_GLMF_OPENING_BALANCE
                            SET OPENING_BALANCE = :ob, LOG_ID = :log
                            WHERE GL_CODE = :code AND COMP_ID = :comp";

                            OracleCommand cmdUpdateOB = new OracleCommand(updateOB, conn);
                            cmdUpdateOB.Transaction = trans;
                            cmdUpdateOB.BindByName = true;

                            cmdUpdateOB.Parameters.Add("ob", OracleDbType.Decimal).Value = obValue;
                            cmdUpdateOB.Parameters.Add("log", OracleDbType.Int32).Value = logId;
                            cmdUpdateOB.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
                            cmdUpdateOB.Parameters.Add("comp", OracleDbType.Int32).Value = compId;

                            cmdUpdateOB.ExecuteNonQuery();
                        }
                        else
                        {
                            // Insert new record
                            string insertOB = @"
                            INSERT INTO GL_GLMF_OPENING_BALANCE
                            (GL_CODE, OPENING_BALANCE, COMP_ID, LOG_ID)
                            VALUES (:code, :ob, :comp, :log)";

                            OracleCommand cmdInsertOB = new OracleCommand(insertOB, conn);
                            cmdInsertOB.Transaction = trans;
                            cmdInsertOB.BindByName = true;

                            cmdInsertOB.Parameters.Add("code", OracleDbType.Varchar2).Value = code;
                            cmdInsertOB.Parameters.Add("ob", OracleDbType.Decimal).Value = obValue;
                            cmdInsertOB.Parameters.Add("comp", OracleDbType.Int32).Value = compId;
                            cmdInsertOB.Parameters.Add("log", OracleDbType.Int32).Value = logId;

                            cmdInsertOB.ExecuteNonQuery();
                        }
                    }
                }

                trans.Commit();
                Session["CurrentLogId"] = transactionLogId;

                // Refresh the UI based on the level
                RefreshUIAfterSave(level, code, parent);

                lblStatus.ForeColor = System.Drawing.Color.Green;
                lblStatus.Text = isInsert ? "<b>New Record Added Successfully</b>" : "<b>Record Updated Successfully</b>";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                lblStatus.ForeColor = System.Drawing.Color.Red;
                lblStatus.Text = "Error: " + ex.Message;
            }
        }
    }
    private void RefreshUIAfterSave(int level, string code, string parent)
    {
        // Reload all levels to refresh data
        LoadLevel1();

        if (level == 1)
        {
            LoadTopRecordOfLevel(1, null, null);
        }
        else if (level == 2)
        {
            // Find parent Level 1 for this Level 2
            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT PARENTT FROM GL_GLMF WHERE GL_CODE = :code";
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add("code", code);
                object parentLevel1 = cmd.ExecuteScalar();

                if (parentLevel1 != null)
                {
                    LoadLevel2(parentLevel1.ToString());
                    LoadSingleAccount(code);
                }
            }
        }
        else if (level == 3)
        {
            LoadLevel3(parent);
            LoadSingleAccount(code);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ClearForm();
        LoadAllLevels();
        lblStatus.Text = "";
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        // You can integrate your report here
        // For now, we'll redirect to the report
        Response.Redirect("~/frm/gl/reports/chart_of_accounts_report.aspx", false);
    }
    private void ClearForm()
    {
        txtParent.Text = "";
        txtAI.Text = "";
        txtFamily.Text = "";
        txtCode.Text = "";
        txtAccLevel.Text = "";
        txtDesc.Text = "";
        txtGenDetail.Text = "";
        txtOB.Text = "";
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }
}
