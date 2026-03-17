using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

public partial class receivable_sl_type : System.Web.UI.Page
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
        Response.Redirect("~/main_menu/main_menu_gl.aspx");
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx");
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

                    InsertIntoSLType(conn);

                    transaction.Commit();

                    ShowMessage("Receivable SL Type saved successfully!");
                    ShowStatus("Record saved successfully!", "success");
                    hfCurrentMode.Value = "EDIT";

                    GenerateNewSLId();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowMessage("Database error: " + ex.Message);
                    ShowStatus("Error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error saving data: " + ex.Message);
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    private void InsertIntoSLType(OracleConnection conn)
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
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = GetCurrentLogId();

        cmd.ExecuteNonQuery();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrEmpty(txtGLCode.Text.Trim()))
        {
            ShowMessage("Please select a GL Code");
            txtGLCode.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
        {
            ShowMessage("Please enter GL SL Description");
            txtDescription.Focus();
            return false;
        }

        if (hfCurrentMode.Value == "ADD")
        {
            if (IsDuplicateDescription(txtGLCode.Text.Trim(), txtDescription.Text.Trim()))
            {
                ShowMessage("This description already exists for the selected GL Code");
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

    private int GetCurrentLogId()
    {
        return LogHelper.GetCurrentLogId(Session, Request);
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    #endregion
}