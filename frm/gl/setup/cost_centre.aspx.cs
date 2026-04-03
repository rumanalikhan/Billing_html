using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

public partial class cost_centre : System.Web.UI.Page
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

            ClearForm();
            //LoadCostCentresGrid();
            GenerateNextCostCentreCode();
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

    #region Data Operations

    private void GenerateNextCostCentreCode()
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT NVL(MAX(COST_CENTRE_CODE), 0) + 1 FROM GL_COST_CENTRE WHERE COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

            conn.Open();
            object result = cmd.ExecuteScalar();
            int newId = result != null ? Convert.ToInt32(result) : 1;

            txtCostCentreCode.Text = newId.ToString();
            hfCostCentreCode.Value = newId.ToString();
        }
    }

//    private void LoadCostCentresGrid()
//    {
//        try
//        {
//            using (OracleConnection conn = new OracleConnection(connectionString))
//            {
//                string query = @"SELECT COST_CENTRE_CODE, COST_CENTRE_DESCRP
//                                 FROM GL_COST_CENTRE 
//                                 WHERE COMP_ID = :compId
//                                 ORDER BY COST_CENTRE_CODE";

//                OracleCommand cmd = new OracleCommand(query, conn);
//                cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

//                OracleDataAdapter da = new OracleDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                da.Fill(dt);

//                gvCostCentres.DataSource = dt;
//                gvCostCentres.DataBind();
//            }
//        }
//        catch (Exception ex)
//        {
//            System.Diagnostics.Debug.WriteLine("Grid Load Error: " + ex.Message);
//        }
//    }

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
                        string updateQuery = @"UPDATE GL_COST_CENTRE 
                                               SET COST_CENTRE_DESCRP = :description, LOG_ID = :logId
                                               WHERE COST_CENTRE_CODE = :code AND COMP_ID = :compId";
                        OracleCommand updateCmd = new OracleCommand(updateQuery, conn);
                        updateCmd.Parameters.Add("description", OracleDbType.Varchar2).Value = txtDescription.Text.Trim();
                        updateCmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;
                        updateCmd.Parameters.Add("code", OracleDbType.Int32).Value = Convert.ToInt32(hfCostCentreCode.Value);
                        updateCmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        InsertIntoCostCentre(conn, transactionLogId);
                    }

                    transaction.Commit();

                    Session["CurrentLogId"] = transactionLogId;

                    //LoadCostCentresGrid();

                    // Clear form data
                    hfCurrentMode.Value = "ADD";
                    hfCostCentreCode.Value = "0";
                    txtDescription.Text = "";
                    GenerateNextCostCentreCode();

                    ShowStatus("Cost Center saved successfully!", "success");
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

    private void InsertIntoCostCentre(OracleConnection conn, int transactionLogId)
    {
        string query = @"INSERT INTO GL_COST_CENTRE 
                        (COST_CENTRE_CODE, COST_CENTRE_DESCRP, COMP_ID, LOG_ID)
                        VALUES 
                        (:code, :description, :compId, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);
        cmd.Parameters.Add("code", OracleDbType.Int32).Value = Convert.ToInt32(txtCostCentreCode.Text);
        cmd.Parameters.Add("description", OracleDbType.Varchar2).Value = txtDescription.Text.Trim();
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0) throw new Exception("No rows were inserted.");
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrEmpty(txtDescription.Text.Trim()))
        {
            ShowStatus("Please enter Cost Center Description", "error");
            txtDescription.Focus();
            return false;
        }

        if (IsDuplicateDescription(txtDescription.Text.Trim()))
        {
            ShowStatus("This Cost Center Description already exists!", "error");
            txtDescription.Focus();
            return false;
        }

        return true;
    }

    private bool IsDuplicateDescription(string description)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COUNT(*) FROM GL_COST_CENTRE 
                            WHERE UPPER(COST_CENTRE_DESCRP) = UPPER(:description) 
                            AND COMP_ID = :compId";

            if (hfCurrentMode.Value == "EDIT")
            {
                query += " AND COST_CENTRE_CODE != :code";
            }

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("description", OracleDbType.Varchar2).Value = description;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

            if (hfCurrentMode.Value == "EDIT")
            {
                cmd.Parameters.Add("code", OracleDbType.Int32).Value = Convert.ToInt32(hfCostCentreCode.Value);
            }

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    #endregion

    #region Grid Operations

    protected void gvCostCentres_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditRow")
        {
            int costCentreCode = Convert.ToInt32(e.CommandArgument);
            LoadCostCentreForEdit(costCentreCode);
        }
        else if (e.CommandName == "DeleteRow")
        {
            int costCentreCode = Convert.ToInt32(e.CommandArgument);
            DeleteCostCentre(costCentreCode);
        }
    }

    private void LoadCostCentreForEdit(int costCentreCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COST_CENTRE_CODE, COST_CENTRE_DESCRP 
                            FROM GL_COST_CENTRE 
                            WHERE COST_CENTRE_CODE = :code AND COMP_ID = :compId";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("code", OracleDbType.Int32).Value = costCentreCode;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                hfCurrentMode.Value = "EDIT";
                hfCostCentreCode.Value = costCentreCode.ToString();
                txtCostCentreCode.Text = reader["COST_CENTRE_CODE"].ToString();
                txtDescription.Text = reader["COST_CENTRE_DESCRP"].ToString();

                ShowStatus("Edit mode: You can modify this record.", "info");
            }
            reader.Close();
        }
    }

    private void DeleteCostCentre(int costCentreCode)
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
                    string deleteQuery = "DELETE FROM GL_COST_CENTRE WHERE COST_CENTRE_CODE = :code AND COMP_ID = :compId";
                    OracleCommand deleteCmd = new OracleCommand(deleteQuery, conn);
                    deleteCmd.Parameters.Add("code", OracleDbType.Int32).Value = costCentreCode;
                    deleteCmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
                    deleteCmd.ExecuteNonQuery();

                    transaction.Commit();
                    Session["CurrentLogId"] = transactionLogId;
                    ShowStatus("Cost Center deleted successfully!", "success");
                    //LoadCostCentresGrid();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ShowStatus("Error deleting: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    #endregion

    #region Utility Methods

    private void ClearForm()
    {
        hfCurrentMode.Value = "ADD";
        hfCostCentreCode.Value = "0";
        txtDescription.Text = "";
        GenerateNextCostCentreCode();

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