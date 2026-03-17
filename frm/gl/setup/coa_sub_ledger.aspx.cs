using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

public partial class coa_sub_legder : System.Web.UI.Page
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

            ClearForm();

            if (Session["Username"] != null)
            {
                lblUser.Text = "Welcome, " + Session["Username"].ToString();
            }
        }
        else
        {
            string eventTarget = Request.Params["__EVENTTARGET"];
            if (eventTarget == txtSearchGLSL.UniqueID)
            {
                string subLedgerId = hfSubLedgerId.Value;
                if (!string.IsNullOrEmpty(subLedgerId) && subLedgerId != "0")
                {
                    LoadGLSLDetails(subLedgerId);
                }
            }
        }
    }

    //#region Header Methods

    ////protected void btnGoBack_Click(object sender, EventArgs e)
    ////{
    ////    Response.Redirect("~/main_menu/main_menu_bs.aspx");
    ////}

    ////protected void btnLogoff_Click(object sender, EventArgs e)
    ////{
    ////    Session.Clear();
    ////    Session.Abandon();
    ////    Response.Redirect("~/login/Login.aspx");
    ////}


    //#endregion

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
    public static List<object> SearchGLSLTypes(string searchTerm)
    {
        List<object> results = new List<object>();
        string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT SUB_LEDGER_ID, DESCRIP, GL_CODE, FAMILY 
                            FROM GL_SL_TYPE 
                            WHERE TO_CHAR(SUB_LEDGER_ID) LIKE :searchTerm 
                               OR UPPER(DESCRIP) LIKE UPPER(:searchTerm)
                               OR UPPER(GL_CODE) LIKE UPPER(:searchTerm)
                            ORDER BY SUB_LEDGER_ID";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("searchTerm", OracleDbType.Varchar2).Value = "%" + searchTerm + "%";

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new
                {
                    SUB_LEDGER_ID = reader["SUB_LEDGER_ID"].ToString(),
                    DESCRIP = reader["DESCRIP"].ToString(),
                    GL_CODE = reader["GL_CODE"].ToString(),
                    FAMILY = reader["FAMILY"].ToString()
                });
            }
        }

        return results;
    }

    #endregion

    #region GL SL Type Selection

    private void LoadGLSLDetails(string subLedgerId)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT SUB_LEDGER_ID, DESCRIP, GL_CODE, FAMILY 
                            FROM GL_SL_TYPE 
                            WHERE SUB_LEDGER_ID = :subLedgerId";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(subLedgerId);

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                hfSubLedgerId.Value = reader["SUB_LEDGER_ID"].ToString();
                hfSelectedGLCode.Value = reader["GL_CODE"].ToString();
                txtGLSLId.Text = reader["SUB_LEDGER_ID"].ToString();
                txtGLSLDesc.Text = reader["DESCRIP"].ToString();
                txtGLCode.Text = reader["GL_CODE"].ToString();
                txtFamily.Text = reader["FAMILY"].ToString();

                GetGLDescription(reader["GL_CODE"].ToString());
            }
            reader.Close();
        }
    }

    private void GetGLDescription(string glCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT GL_DESCRP FROM GL_GLMF WHERE GL_CODE = :glCode";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            object result = cmd.ExecuteScalar();
            txtGLDesc.Text = result != null ? result.ToString() : "";
        }
    }

    #endregion

    #region Save Operation

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
                    int subLedgerId = Convert.ToInt32(hfSubLedgerId.Value);
                    string slCode = txtSLCode.Text.Trim();

                    if (hfCurrentMode.Value == "EDIT")
                    {
                        DeleteExistingRecords(conn, slCode);
                    }

                    InsertIntoGLSLGLMF(conn);
                    InsertIntoOpeningBalance(conn);

                    transaction.Commit();

                    ShowMessage("Sub Ledger saved successfully!");
                    ShowStatus("Record saved successfully!", "success");
                    hfCurrentMode.Value = "EDIT";
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

    private void DeleteExistingRecords(OracleConnection conn, string slCode)
    {
        string deleteGLMFQuery = "DELETE FROM GL_SL_GLMF WHERE SL_CODE = :slCode";
        OracleCommand deleteGLMFCmd = new OracleCommand(deleteGLMFQuery, conn);
        deleteGLMFCmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
        deleteGLMFCmd.ExecuteNonQuery();

        string deleteOBQuery = "DELETE FROM GL_SL_OPENING_BALANCE WHERE SL_CODE = :slCode";
        OracleCommand deleteOBCmd = new OracleCommand(deleteOBQuery, conn);
        deleteOBCmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
        deleteOBCmd.ExecuteNonQuery();
    }

    private void InsertIntoGLSLGLMF(OracleConnection conn)
    {
        string query = @"INSERT INTO GL_SL_GLMF 
                        (SL_CODE, DESCRIP, CONTACT_PERSON, NTN, STN, 
                         ADD1, ADD2, CITY, CONTACT1, CONTACT2, CONTACT3,
                         CELL1, CELL2, FAX1, FAX2, EMAIL1, EMAIL2, URLL,
                         REMARKS, COMP_ID, SUB_LEDGER_ID, LOG_ID)
                        VALUES 
                        (:slCode, :descrip, :contactPerson, :ntn, :stn,
                         :add1, :add2, :city, :contact1, :contact2, :contact3,
                         :cell1, :cell2, :fax1, :fax2, :email1, :email2, :url,
                         :remarks, :compId, :subLedgerId, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtSLCode.Text) ? (object)DBNull.Value : txtSLCode.Text.Trim();
        cmd.Parameters.Add("descrip", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtDescrip.Text) ? (object)DBNull.Value : txtDescrip.Text.Trim();
        cmd.Parameters.Add("contactPerson", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtContactPerson.Text) ? (object)DBNull.Value : txtContactPerson.Text.Trim();
        cmd.Parameters.Add("ntn", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtNTN.Text) ? (object)DBNull.Value : txtNTN.Text.Trim();
        cmd.Parameters.Add("stn", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtSTN.Text) ? (object)DBNull.Value : txtSTN.Text.Trim();
        cmd.Parameters.Add("add1", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtAdd1.Text) ? (object)DBNull.Value : txtAdd1.Text.Trim();
        cmd.Parameters.Add("add2", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtAdd2.Text) ? (object)DBNull.Value : txtAdd2.Text.Trim();
        cmd.Parameters.Add("city", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtCity.Text) ? (object)DBNull.Value : txtCity.Text.Trim();
        cmd.Parameters.Add("contact1", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtContact1.Text) ? (object)DBNull.Value : txtContact1.Text.Trim();
        cmd.Parameters.Add("contact2", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtContact2.Text) ? (object)DBNull.Value : txtContact2.Text.Trim();
        cmd.Parameters.Add("contact3", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtContact3.Text) ? (object)DBNull.Value : txtContact3.Text.Trim();
        cmd.Parameters.Add("cell1", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtCell1.Text) ? (object)DBNull.Value : txtCell1.Text.Trim();
        cmd.Parameters.Add("cell2", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtCell2.Text) ? (object)DBNull.Value : txtCell2.Text.Trim();
        cmd.Parameters.Add("fax1", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtFax1.Text) ? (object)DBNull.Value : txtFax1.Text.Trim();
        cmd.Parameters.Add("fax2", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtFax2.Text) ? (object)DBNull.Value : txtFax2.Text.Trim();
        cmd.Parameters.Add("email1", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtEmail1.Text) ? (object)DBNull.Value : txtEmail1.Text.Trim();
        cmd.Parameters.Add("email2", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtEmail2.Text) ? (object)DBNull.Value : txtEmail2.Text.Trim();
        cmd.Parameters.Add("url", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtURL.Text) ? (object)DBNull.Value : txtURL.Text.Trim();
        cmd.Parameters.Add("remarks", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(txtRemarks.Text) ? (object)DBNull.Value : txtRemarks.Text.Trim();
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(hfSubLedgerId.Value);
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = GetCurrentLogId();

        cmd.ExecuteNonQuery();
    }

    private void InsertIntoOpeningBalance(OracleConnection conn)
    {
        decimal openingBalance = 0;
        decimal.TryParse(txtOpeningBalance.Text, out openingBalance);

        string query = @"INSERT INTO GL_SL_OPENING_BALANCE 
                        (OPENING_BALANCE, COMP_ID, SUB_LEDGER_ID, SL_CODE, LOG_ID)
                        VALUES 
                        (:openingBalance, :compId, :subLedgerId, :slCode, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("openingBalance", OracleDbType.Decimal).Value = openingBalance;
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(hfSubLedgerId.Value);
        cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = txtSLCode.Text.Trim();
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = GetCurrentLogId();

        cmd.ExecuteNonQuery();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrEmpty(hfSubLedgerId.Value) || hfSubLedgerId.Value == "0")
        {
            ShowMessage("Please select a GL SL Type first");
            return false;
        }

        if (string.IsNullOrEmpty(txtSLCode.Text.Trim()))
        {
            ShowMessage("SL Code is required");
            txtSLCode.Focus();
            return false;
        }

        if (hfCurrentMode.Value == "ADD")
        {
            if (IsDuplicateSLCode(txtSLCode.Text.Trim()))
            {
                ShowMessage("SL Code already exists. Please use a different code.");
                txtSLCode.Focus();
                return false;
            }
        }

        if (!string.IsNullOrEmpty(txtEmail1.Text.Trim()) && !IsValidEmail(txtEmail1.Text.Trim()))
        {
            ShowMessage("Please enter a valid email address in Email 1");
            txtEmail1.Focus();
            return false;
        }

        if (!string.IsNullOrEmpty(txtEmail2.Text.Trim()) && !IsValidEmail(txtEmail2.Text.Trim()))
        {
            ShowMessage("Please enter a valid email address in Email 2");
            txtEmail2.Focus();
            return false;
        }

        decimal openingBalance;
        if (!decimal.TryParse(txtOpeningBalance.Text, out openingBalance))
        {
            ShowMessage("Please enter a valid number for Opening Balance");
            txtOpeningBalance.Focus();
            return false;
        }

        return true;
    }

    private bool IsDuplicateSLCode(string slCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT COUNT(*) FROM GL_SL_GLMF WHERE SL_CODE = :slCode";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Utility Methods

    private void ClearForm()
    {
        hfSubLedgerId.Value = "0";
        hfSelectedGLCode.Value = "";
        hfCurrentMode.Value = "ADD";

        txtSearchGLSL.Text = "";
        txtGLSLId.Text = "";
        txtGLSLDesc.Text = "";
        txtGLCode.Text = "";
        txtFamily.Text = "";
        txtGLDesc.Text = "";

        txtSLCode.Text = "";
        txtDescrip.Text = "";
        txtContactPerson.Text = "";
        txtNTN.Text = "";
        txtSTN.Text = "";
        txtCell1.Text = "";
        txtCell2.Text = "";
        txtContact1.Text = "";
        txtContact2.Text = "";
        txtContact3.Text = "";
        txtFax1.Text = "";
        txtFax2.Text = "";
        txtEmail1.Text = "";
        txtEmail2.Text = "";
        txtURL.Text = "";
        txtRemarks.Text = "";
        txtAdd1.Text = "";
        txtAdd2.Text = "";
        txtCity.Text = "";
        txtOpeningBalance.Text = "0.00";

        statusContainer.Visible = false;
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearForm();
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