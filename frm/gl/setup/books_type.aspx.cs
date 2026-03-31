using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

public partial class books_type : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

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

            LoadBookTypeMaster();
            ClearForm();
            LoadBookTypesGrid();
            gridContainer.Visible = false;
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

    #region Load Master Data

    private void LoadBookTypeMaster()
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = @"SELECT BT_ID, BT_DESCR FROM GL_BOOK_TYPE_MST ORDER BY BT_ID";
                OracleCommand cmd = new OracleCommand(query, conn);
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();

                ddlBookType.Items.Clear();
                ddlBookType.Items.Add(new ListItem("-- Select Book Type --", ""));

                while (reader.Read())
                {
                    string btId = reader["BT_ID"].ToString();
                    string btDescr = reader["BT_DESCR"].ToString();
                    ddlBookType.Items.Add(new ListItem(btId + " - " + btDescr, btId));
                }
                reader.Close();
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error loading book types: " + ex.Message, "error");
        }
    }

    #endregion

    #region Data Operations

    private void GenerateNextBookTypeId()
    {
        string selectedBookType = ddlBookType.SelectedValue;

        if (string.IsNullOrEmpty(selectedBookType))
        {
            txtBookTypeId.Text = "";
            hfBookTypeId.Value = "";
            hfCurrentBookType.Value = "";
            return;
        }

        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = @"SELECT NVL(MAX(BOOK_TYPE_ID), 0) + 1 
                                FROM GL_BOOK_TYPE 
                                WHERE BOOK_TYPE = :bookType AND COMP_ID = :compId";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = selectedBookType;
                cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

                conn.Open();
                object result = cmd.ExecuteScalar();
                int newId = result != null ? Convert.ToInt32(result) : 1;

                txtBookTypeId.Text = newId.ToString();
                hfBookTypeId.Value = newId.ToString();
                hfCurrentBookType.Value = selectedBookType;
            }
        }
        catch (Exception ex)
        {
            txtBookTypeId.Text = "1";
            hfBookTypeId.Value = "1";
            hfCurrentBookType.Value = selectedBookType;
        }
    }

    protected void ddlBookType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Clear any existing status message first
        statusContainer.Visible = false;
        statusContainer.Style["display"] = "none";
        lblStatus.Text = "";

        if (hfCurrentMode.Value == "ADD")
        {
            GenerateNextBookTypeId();
        }
        else
        {
            if (hfCurrentBookType.Value != ddlBookType.SelectedValue && !string.IsNullOrEmpty(hfCurrentBookType.Value))
            {
                ShowStatus("Changing book type will reset the ID. Save will create a new record.", "info");
                GenerateNextBookTypeId();
            }
        }
    }

    private void LoadBookTypesGrid()
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = @"SELECT BOOK_TYPE_ID, GL_CODE, BOOK_TYPE
                                 FROM GL_BOOK_TYPE 
                                 WHERE COMP_ID = :compId
                                 ORDER BY BOOK_TYPE, BOOK_TYPE_ID";

                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBookTypes.DataSource = dt;
                gvBookTypes.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Grid Load Error: " + ex.Message);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateForm())
                return;

            int transactionLogId = CreateTransactionLog();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    if (hfCurrentMode.Value == "EDIT")
                    {
                        string deleteQuery = "DELETE FROM GL_BOOK_TYPE WHERE BOOK_TYPE_ID = :id AND BOOK_TYPE = :bookType AND COMP_ID = :compId";
                        OracleCommand deleteCmd = new OracleCommand(deleteQuery, conn);
                        deleteCmd.Parameters.Add("id", OracleDbType.Int32).Value = Convert.ToInt32(hfBookTypeId.Value);
                        deleteCmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = hfCurrentBookType.Value;
                        deleteCmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
                        deleteCmd.ExecuteNonQuery();
                    }

                    InsertIntoBookType(conn, transactionLogId);
                    transaction.Commit();

                    Session["CurrentLogId"] = transactionLogId;

                    LoadBookTypesGrid();

                    // Clear form data
                    hfCurrentMode.Value = "ADD";
                    hfBookTypeId.Value = "0";
                    hfCurrentBookType.Value = "";
                    if (ddlBookType.Items.Count > 0)
                        ddlBookType.SelectedIndex = 0;
                    txtGLCode.Text = "";
                    GenerateNextBookTypeId();

                    ShowStatus("Book Type saved successfully!", "success");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowStatus("Database error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    private int CreateTransactionLog()
    {
        try
        {
            return LogHelper.CreateTransactionLog(Session, Request);
        }
        catch
        {
            return 0;
        }
    }

    private void InsertIntoBookType(OracleConnection conn, int transactionLogId)
    {
        string query = @"INSERT INTO GL_BOOK_TYPE 
                        (BOOK_TYPE_ID, GL_CODE, BOOK_TYPE, COMP_ID, LOG_ID)
                        VALUES 
                        (:bookTypeId, :glCode, :bookType, :compId, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);
        cmd.Parameters.Add("bookTypeId", OracleDbType.Int32).Value = Convert.ToInt32(txtBookTypeId.Text);
        cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = txtGLCode.Text.Trim();
        cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = ddlBookType.SelectedValue;
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0) throw new Exception("No rows were inserted.");
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrEmpty(ddlBookType.SelectedValue))
        {
            ShowStatus("Please select a Book Type", "error");
            ddlBookType.Focus();
            return false;
        }

        if (string.IsNullOrEmpty(txtGLCode.Text.Trim()))
        {
            ShowStatus("Please enter a GL Code", "error");
            txtGLCode.Focus();
            return false;
        }

        if (IsDuplicateMapping(txtGLCode.Text.Trim(), ddlBookType.SelectedValue))
        {
            ShowStatus("This GL Code and Book Type combination already exists!", "error");
            return false;
        }

        if (IsBookTypeIdExists(Convert.ToInt32(txtBookTypeId.Text), ddlBookType.SelectedValue))
        {
            ShowStatus("This Book Type ID already exists. Please try again.", "error");
            GenerateNextBookTypeId();
            return false;
        }

        return true;
    }

    private bool IsDuplicateMapping(string glCode, string bookType)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COUNT(*) FROM GL_BOOK_TYPE 
                            WHERE GL_CODE = :glCode AND BOOK_TYPE = :bookType AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;
            cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    private bool IsBookTypeIdExists(int bookTypeId, string bookType)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COUNT(*) FROM GL_BOOK_TYPE 
                            WHERE BOOK_TYPE_ID = :bookTypeId AND BOOK_TYPE = :bookType AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("bookTypeId", OracleDbType.Int32).Value = bookTypeId;
            cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    #endregion

    #region Grid Operations

    protected void gvBookTypes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditRow")
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            int bookTypeId = Convert.ToInt32(args[0]);
            string bookType = args[1];
            LoadBookTypeForEdit(bookTypeId, bookType);
        }
        else if (e.CommandName == "DeleteRow")
        {
            int bookTypeId = Convert.ToInt32(e.CommandArgument);
            DeleteBookType(bookTypeId);
        }
    }

    private void LoadBookTypeForEdit(int bookTypeId, string bookType)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT BOOK_TYPE_ID, GL_CODE, BOOK_TYPE 
                            FROM GL_BOOK_TYPE 
                            WHERE BOOK_TYPE_ID = :id AND BOOK_TYPE = :bookType AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("id", OracleDbType.Int32).Value = bookTypeId;
            cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                hfCurrentMode.Value = "EDIT";
                hfBookTypeId.Value = bookTypeId.ToString();
                txtBookTypeId.Text = reader["BOOK_TYPE_ID"].ToString();
                txtGLCode.Text = reader["GL_CODE"].ToString();

                string selectedBookType = reader["BOOK_TYPE"].ToString();
                foreach (ListItem item in ddlBookType.Items)
                {
                    if (item.Value == selectedBookType)
                    {
                        item.Selected = true;
                        break;
                    }
                }
                hfCurrentBookType.Value = selectedBookType;
                ShowStatus("Edit mode: You can modify this record.", "info");
            }
            reader.Close();
        }
    }

    private void DeleteBookType(int bookTypeId)
    {
        try
        {
            int transactionLogId = CreateTransactionLog();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    string deleteQuery = "DELETE FROM GL_BOOK_TYPE WHERE BOOK_TYPE_ID = :id AND COMP_ID = :compId";
                    OracleCommand deleteCmd = new OracleCommand(deleteQuery, conn);
                    deleteCmd.Parameters.Add("id", OracleDbType.Int32).Value = bookTypeId;
                    deleteCmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
                    deleteCmd.ExecuteNonQuery();

                    transaction.Commit();
                    Session["CurrentLogId"] = transactionLogId;
                    ShowStatus("✓ Record deleted successfully!", "success");
                    LoadBookTypesGrid();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowStatus("✗ Error deleting: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowStatus("✗ Error: " + ex.Message, "error");
        }
    }

    #endregion

    #region Utility Methods

    private void ClearForm()
    {
        hfCurrentMode.Value = "ADD";
        hfBookTypeId.Value = "0";
        hfCurrentBookType.Value = "";

        if (ddlBookType.Items.Count > 0)
            ddlBookType.SelectedIndex = 0;

        txtGLCode.Text = "";
        GenerateNextBookTypeId();

        // Clear and hide status
        lblStatus.Text = "";
        statusContainer.Visible = false;
        statusContainer.Style["display"] = "none";
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearForm();
        ShowStatus("Form cleared", "info");
    }

    private void ShowStatus(string message, string type)
    {
        // Force hide any existing status
        statusContainer.Visible = false;
        statusContainer.Style["display"] = "none";
        lblStatus.Text = "";

        // Set new message
        lblStatus.Text = message;
        statusContainer.Visible = true;
        statusContainer.Style["display"] = "block";
        statusContainer.Attributes["class"] = "status-label";

        if (type == "success")
        {
            statusContainer.Attributes["class"] += " status-success";
            string script = "setTimeout(function() { var elem = document.getElementById('" + statusContainer.ClientID + "'); if(elem) { elem.style.display = 'none'; } }, 3000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideStatus_" + Guid.NewGuid().ToString(), script, true);
        }
        else if (type == "error")
        {
            statusContainer.Attributes["class"] += " status-error";
        }
        else if (type == "info")
        {
            statusContainer.Attributes["class"] += " status-info";
            string script = "setTimeout(function() { var elem = document.getElementById('" + statusContainer.ClientID + "'); if(elem) { elem.style.display = 'none'; } }, 3000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideStatus_" + Guid.NewGuid().ToString(), script, true);
        }
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

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    #endregion
}