using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using System.Web;

public partial class payable_sl_type : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set default session values
            if (Session["CurrentCompId"] == null)
                Session["CurrentCompId"] = 1;
            if (Session["CurrentLogId"] == null)
                Session["CurrentLogId"] = 0;
            if (Session["Username"] != null)
            {
                lblUser.Text = "Welcome, " + Session["Username"].ToString();
            }

            ClearForm();
            GenerateNewSLId();
            
            // Initialize bulk grid with 10 empty rows
            InitializeBulkGrid();
        }
        else
        {
            string eventTarget = Request.Params["__EVENTTARGET"];
            if (eventTarget == txtGLCode.UniqueID)
            {
                string glCode = hfSelectedGLCode.Value;
                if (!string.IsNullOrEmpty(glCode))
                {
                    LoadGLDetails(glCode);
                }
            }
        }
    }

    #region Header Methods

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

    #region Web Methods for Autocomplete

    [System.Web.Services.WebMethod]
    public static List<object> SearchGLCodes(string searchTerm)
    {
        List<object> results = new List<object>();
        string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT GL_CODE, GL_DESCRP, FAMILY 
                            FROM GL_GLMF 
                            WHERE (GL_CODE LIKE :searchTerm OR UPPER(GL_DESCRP) LIKE UPPER(:searchTerm))
                            AND ACTIVE = 1
                            AND ROWNUM <= 20
                            ORDER BY GL_CODE";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("searchTerm", OracleDbType.Varchar2).Value = "%" + searchTerm + "%";

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new
                {
                    GL_CODE = reader["GL_CODE"].ToString(),
                    GL_DESCRP = reader["GL_DESCRP"].ToString(),
                    FAMILY = reader["FAMILY"].ToString()
                });
            }
        }

        return results;
    }

    [System.Web.Services.WebMethod]
    public static int GetNextAvailableId()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT NVL(MAX(SUB_LEDGER_ID), 0) + 1 FROM GL_SL_TYPE";
            OracleCommand cmd = new OracleCommand(query, conn);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 1;
        }
    }

    #endregion

    #region Bulk Grid Methods

    private void InitializeBulkGrid()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("RowId", typeof(int));
        
        // Create 10 empty rows
        for (int i = 0; i < 8; i++)
        {
            DataRow row = dt.NewRow();
            row["RowId"] = i + 1;
            dt.Rows.Add(row);
        }
        
        gvBulkSL.DataSource = dt;
        gvBulkSL.DataBind();
    }

    protected void gvBulkSL_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblAutoId = (Label)e.Row.FindControl("lblAutoId");
            TextBox txtGLCode = (TextBox)e.Row.FindControl("txtGLCode");
            TextBox txtGLDesc = (TextBox)e.Row.FindControl("txtGLDesc");
            TextBox txtFamily = (TextBox)e.Row.FindControl("txtFamily");
            TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");
            HiddenField hfGLCode = (HiddenField)e.Row.FindControl("hfGLCode");
            HiddenField hfRowIndex = (HiddenField)e.Row.FindControl("hfRowIndex");
            
            // Set auto-generated ID
            int nextId = GetNextAvailableId();
            if (lblAutoId != null)
            {
                lblAutoId.Text = (nextId + e.Row.RowIndex).ToString();
            }
            
            // Set unique IDs for JavaScript
            if (txtGLCode != null)
            {
                txtGLCode.ID = "gridGLCode_" + e.Row.RowIndex;
                txtGLCode.CssClass = "glcode-input";
                txtGLCode.Attributes.Add("placeholder", "Search GL Code...");
                txtGLCode.Attributes.Add("autocomplete", "off");
            }
            
            if (txtGLDesc != null)
            {
                txtGLDesc.ID = "gridGLDesc_" + e.Row.RowIndex;
                txtGLDesc.CssClass = "readonly-field";
            }
            
            if (txtFamily != null)
            {
                txtFamily.ID = "gridFamily_" + e.Row.RowIndex;
                txtFamily.CssClass = "readonly-field";
            }
            
            if (txtDescription != null)
            {
                txtDescription.ID = "gridDesc_" + e.Row.RowIndex;
            }
            
            if (hfGLCode != null)
            {
                hfGLCode.ID = "hfGLCode_" + e.Row.RowIndex;
            }
            
            if (hfRowIndex != null)
            {
                hfRowIndex.Value = e.Row.RowIndex.ToString();
            }
        }
    }

    protected void btnSaveBulk_Click(object sender, EventArgs e)
    {
        try
        {
            List<BulkSLType> items = new List<BulkSLType>();
            
            foreach (GridViewRow row in gvBulkSL.Rows)
            {
                TextBox txtGLCode = (TextBox)row.FindControl("txtGLCode");
                TextBox txtDescription = (TextBox)row.FindControl("txtDescription");
                
                if (txtGLCode != null && txtDescription != null)
                {
                    string glCode = txtGLCode.Text.Trim();
                    string description = txtDescription.Text.Trim();
                    
                    if (!string.IsNullOrEmpty(glCode) && !string.IsNullOrEmpty(description))
                    {
                        items.Add(new BulkSLType { GL_CODE = glCode, DESCRIPTION = description });
                    }
                }
            }
            
            if (items.Count == 0)
            {
                ShowSnackbarMessage("No valid data to save", "error");
                return;
            }
            
            // Save bulk data
            int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);
            int compId = GetCurrentCompId();
            int savedCount = 0;
            int duplicateCount = 0;
            int invalidCount = 0;
            
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();
                
                try
                {
                    string getIdQuery = "SELECT NVL(MAX(SUB_LEDGER_ID), 0) FROM GL_SL_TYPE";
                    OracleCommand getIdCmd = new OracleCommand(getIdQuery, conn);
                    int nextId = Convert.ToInt32(getIdCmd.ExecuteScalar()) + 1;
                    
                    string insertQuery = @"INSERT INTO GL_SL_TYPE 
                        (SUB_LEDGER_ID, DESCRIP, COMP_ID, GL_CODE, FAMILY, LOG_ID)
                        VALUES 
                        (:subLedgerId, :descrip, :compId, :glCode, :family, :logId)";
                    
                    OracleCommand insertCmd = new OracleCommand(insertQuery, conn);
                    
                    foreach (var item in items)
                    {
                        // Validate GL Code exists
                        string validateQuery = "SELECT COUNT(*) FROM GL_GLMF WHERE GL_CODE = :glCode AND ACTIVE = 1";
                        OracleCommand validateCmd = new OracleCommand(validateQuery, conn);
                        validateCmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = item.GL_CODE;
                        int glExists = Convert.ToInt32(validateCmd.ExecuteScalar());
                        
                        if (glExists == 0)
                        {
                            invalidCount++;
                            continue;
                        }
                        
                        // Check duplicate
                        string checkQuery = "SELECT COUNT(*) FROM GL_SL_TYPE WHERE GL_CODE = :glCode AND UPPER(DESCRIP) = UPPER(:descrip)";
                        OracleCommand checkCmd = new OracleCommand(checkQuery, conn);
                        checkCmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = item.GL_CODE;
                        checkCmd.Parameters.Add("descrip", OracleDbType.Varchar2).Value = item.DESCRIPTION;
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (exists > 0)
                        {
                            duplicateCount++;
                            continue;
                        }
                        
                        // Get Family
                        string familyQuery = "SELECT FAMILY FROM GL_GLMF WHERE GL_CODE = :glCode";
                        OracleCommand familyCmd = new OracleCommand(familyQuery, conn);
                        familyCmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = item.GL_CODE;
                        object familyResult = familyCmd.ExecuteScalar();
                        string family = familyResult != null ? familyResult.ToString() : "";
                        
                        insertCmd.Parameters.Clear();
                        insertCmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = nextId++;
                        insertCmd.Parameters.Add("descrip", OracleDbType.Varchar2).Value = item.DESCRIPTION;
                        insertCmd.Parameters.Add("compId", OracleDbType.Int32).Value = compId;
                        insertCmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = item.GL_CODE;
                        insertCmd.Parameters.Add("family", OracleDbType.Varchar2).Value = family;
                        insertCmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;
                        
                        insertCmd.ExecuteNonQuery();
                        savedCount++;
                    }
                    
                    transaction.Commit();
                    
                    string message = savedCount+" record(s) saved successfully.";
                    if (duplicateCount > 0) message += duplicateCount+" duplicate(s) skipped.";
                    if (invalidCount > 0) message += invalidCount+" invalid GL Code(s) skipped.";
                    
                    ShowSnackbarMessage(message, "success");
                    ShowStatus(message, "success");
                    
                    // Refresh grid with empty rows
                    InitializeBulkGrid();
                    GenerateNewSLId();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowSnackbarMessage("Database error: " + ex.Message, "error");
                    ShowStatus("Error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage("Error saving data: " + ex.Message, "error");
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    #endregion

    #region Data Operations

    private void GenerateNewSLId()
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT NVL(MAX(SUB_LEDGER_ID), 0) + 1 FROM GL_SL_TYPE";
            OracleCommand cmd = new OracleCommand(query, conn);

            conn.Open();
            object result = cmd.ExecuteScalar();
            int newId = result != null ? Convert.ToInt32(result) : 1;

            txtGLSLId.Text = newId.ToString();
            hfSubLedgerId.Value = newId.ToString();
        }
    }

    private void LoadGLDetails(string glCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT GL_DESCRP, FAMILY FROM GL_GLMF WHERE GL_CODE = :glCode";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                txtGLDesc.Text = reader["GL_DESCRP"].ToString();
                txtFamily.Text = reader["FAMILY"].ToString();
            }
            reader.Close();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateForm())
                return;

            int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    if (hfCurrentMode.Value == "EDIT")
                    {
                        string deleteQuery = "DELETE FROM GL_SL_TYPE WHERE SUB_LEDGER_ID = :id";
                        OracleCommand deleteCmd = new OracleCommand(deleteQuery, conn);
                        deleteCmd.Parameters.Add("id", OracleDbType.Int32).Value = Convert.ToInt32(hfSubLedgerId.Value);
                        deleteCmd.ExecuteNonQuery();
                    }

                    InsertIntoSLType(conn, transactionLogId);

                    transaction.Commit();

                    Session["CurrentLogId"] = transactionLogId;

                    ShowSnackbarMessage("Payable SL Type saved successfully!", "success");
                    ShowStatus("Record saved successfully!", "success");
                    hfCurrentMode.Value = "EDIT";

                    GenerateNewSLId();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowSnackbarMessage("Database error: " + ex.Message, "error");
                    ShowStatus("Error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowSnackbarMessage("Error saving data: " + ex.Message, "error");
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    private void InsertIntoSLType(OracleConnection conn, int transactionLogId)
    {
        string query = @"INSERT INTO GL_SL_TYPE 
                    (SUB_LEDGER_ID, DESCRIP, COMP_ID, GL_CODE, FAMILY, LOG_ID)
                    VALUES 
                    (:subLedgerId, :descrip, :compId, :glCode, :family, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(txtGLSLId.Text);
        cmd.Parameters.Add("descrip", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim();
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = txtGLCode.Text.Trim();
        cmd.Parameters.Add("family", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtFamily.Text) ? (object)DBNull.Value : txtFamily.Text.Trim();
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;

        cmd.ExecuteNonQuery();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrEmpty(txtGLCode.Text.Trim()))
        {
            ShowSnackbarMessage("Please select a GL Code", "error");
            txtGLCode.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
        {
            ShowSnackbarMessage("Please enter GL SL Description", "error");
            txtDescription.Focus();
            return false;
        }

        if (hfCurrentMode.Value == "ADD")
        {
            if (IsDuplicateDescription(txtGLCode.Text.Trim(), txtDescription.Text.Trim()))
            {
                ShowSnackbarMessage("This description already exists for the selected GL Code", "error");
                txtDescription.Focus();
                return false;
            }
        }

        return true;
    }

    private bool IsDuplicateDescription(string glCode, string description)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM GL_SL_TYPE WHERE GL_CODE = :glCode AND UPPER(DESCRIP) = UPPER(:descrip)";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;
            cmd.Parameters.Add("descrip", OracleDbType.Varchar2).Value = description;

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    #endregion

    #region Utility Methods

    private void ClearForm()
    {
        hfCurrentMode.Value = "ADD";
        hfSelectedGLCode.Value = "";

        txtGLCode.Text = "";
        txtGLDesc.Text = "";
        txtFamily.Text = "";
        txtDescription.Text = "";

        statusContainer.Visible = false;
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearForm();
        GenerateNewSLId();
    }

    private void ShowSnackbarMessage(string message, string type)
    {
        string script = "showSnackbar('" + message.Replace("'", "\\'") + "', '" + type + "');";
        ScriptManager.RegisterStartupScript(this, GetType(), "SnackbarMessage", script, true);
    }

    private void ShowMessage(string message)
    {
        lblMessage.Text = message;
        mpeMessage.Show();
    }

    protected void btnMessageOk_Click(object sender, EventArgs e)
    {
        mpeMessage.Hide();
    }

    private void ShowStatus(string message, string type)
    {
        lblStatus.Text = message;
        statusContainer.Visible = true;
        statusContainer.Attributes["class"] = "status-label " + (type == "success" ? "status-success" : "status-error");
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    #endregion
}

// Bulk SL Type Class
public class BulkSLType
{
    public string GL_CODE { get; set; }
    public string DESCRIPTION { get; set; }
}