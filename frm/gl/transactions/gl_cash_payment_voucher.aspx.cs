using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using System.Text;

public partial class GL_Cash_Payment_Voucher : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;
    private DataTable dtVoucherDetails;
    private const int DEFAULT_ROWS = 13;

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadBookTypes();
            LoadSearchBookTypes();
            SetDefaultDate();
            GenerateNewVoucherNumber();
            GenerateVoucherKey();
            InitializeGrid(DEFAULT_ROWS);
        }
        else
        {
            if (ViewState["VoucherDetails"] != null)
            {
                dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            }

            //string eventTarget = Request.Params["__EVENTTARGET"];
            //string eventArgument = Request.Params["__EVENTARGUMENT"];
            //if (eventTarget == "gvVoucherDetails" && eventArgument == "AddNew$Enter")
            //{
            //    AddNewRow();
            //}
        }
    }
    #endregion

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

    #region Grid Operations
    private void AddNewRow()
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
        DataRow dr = dtVoucherDetails.NewRow();
        dr["AMOUNT"] = 0;
        dr["DR_CR"] = "1";
        dtVoucherDetails.Rows.Add(dr);

        SaveViewStateAndBind();
        CalculateTotal();
    }

    private void InitializeGrid(int numberOfRows)
    {
        dtVoucherDetails = CreateDataTable();

        for (int i = 0; i < numberOfRows; i++)
        {
            DataRow dr = dtVoucherDetails.NewRow();
            dr["AMOUNT"] = 0;
            dr["DR_CR"] = "1";
            dtVoucherDetails.Rows.Add(dr);
        }

        SaveViewStateAndBind();
        CalculateTotal();
        hfCurrentMode.Value = "ADD";

        // Initially disable all SL Code fields (since no GL Code selected yet)
        foreach (GridViewRow row in gvVoucherDetails.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");
                if (txtSLCode != null)
                {
                    txtSLCode.Enabled = false;
                    txtSLCode.Style["background-color"] = "#f0f0f0";
                }
            }
        }
    }

    private DataTable CreateDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("GL_CODE", typeof(string));
        dt.Columns.Add("GL_BOOK_TYPE", typeof(string));
        dt.Columns.Add("SL_CODE", typeof(string));    
        dt.Columns.Add("SL_TYPE", typeof(string));     
        dt.Columns.Add("NARATION", typeof(string));
        dt.Columns.Add("BILL_NUMBER", typeof(string));
        dt.Columns.Add("CHEQUE_NUMBER", typeof(string));
        dt.Columns.Add("AMOUNT", typeof(decimal));
        dt.Columns.Add("COST_CENTRE_CODE", typeof(string));
        dt.Columns.Add("DR_CR", typeof(string));
        return dt;
    }

    private void SaveViewStateAndBind()
    {
        ViewState["VoucherDetails"] = dtVoucherDetails;
        gvVoucherDetails.DataSource = dtVoucherDetails;
        gvVoucherDetails.DataBind();
    }

    protected void gvVoucherDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteRow")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

            if (dtVoucherDetails.Rows.Count > 1)
            {
                dtVoucherDetails.Rows.RemoveAt(index);
            }
            else
            {
                ShowSnackbar("Cannot delete the last row");
                return;
            }

            SaveViewStateAndBind();
            CalculateTotal();
        }
    }

    protected void gvVoucherDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotal = (Label)e.Row.FindControl("lblTotalAmount");
            if (lblTotal != null)
            {
                lblTotal.Text = "Total: " + lblGrandTotal.Text;
            }
        }
    }

    protected void txtField_TextChanged(object sender, EventArgs e)
    {
        TextBox txtField = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtField.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

            if (dtVoucherDetails != null && dtVoucherDetails.Rows.Count > row.RowIndex)
            {
                string fieldValue = txtField.Text;
                string fieldId = txtField.ID;

                switch (fieldId)
                {
                    case "txtSLCode":
                        dtVoucherDetails.Rows[row.RowIndex]["SL_CODE"] = fieldValue;
                        break;
                    case "txtSLType":
                        dtVoucherDetails.Rows[row.RowIndex]["SL_TYPE"] = fieldValue;
                        break;
                    case "txtNarration":
                        dtVoucherDetails.Rows[row.RowIndex]["NARATION"] = fieldValue;
                        break;
                    case "txtBillNumber":
                        dtVoucherDetails.Rows[row.RowIndex]["BILL_NUMBER"] = fieldValue;
                        break;
                    case "txtChequeNumber":
                        dtVoucherDetails.Rows[row.RowIndex]["CHEQUE_NUMBER"] = fieldValue;
                        break;
                    case "txtCostCentreCode":
                        dtVoucherDetails.Rows[row.RowIndex]["COST_CENTRE_CODE"] = fieldValue;
                        break;
                }

                ViewState["VoucherDetails"] = dtVoucherDetails;
            }
        }
    }

    protected void txtGLCode_TextChanged(object sender, EventArgs e)
    {
        TextBox txtGLCode = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtGLCode.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            string glCode = txtGLCode.Text.Trim();
            if (string.IsNullOrEmpty(glCode)) return;

            string description = GetGLDescription(glCode);

            if (string.IsNullOrEmpty(description))
            {
                ShowSnackbar("Invalid GL Code or not a Detail account (GENERAL_DETAIL must be D)");
                txtGLCode.Text = "";

                TextBox txtGLType = (TextBox)row.FindControl("txtGLType");
                if (txtGLType != null) txtGLType.Text = "";

                TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");
                if (txtSLCode != null)
                {
                    txtSLCode.Enabled = false;
                    txtSLCode.Text = "";
                    txtSLCode.Style["background-color"] = "#f0f0f0";
                }
                return;
            }

            TextBox txtGLTypeCtrl = (TextBox)row.FindControl("txtGLType");
            if (txtGLTypeCtrl != null) txtGLTypeCtrl.Text = description;

            bool hasSubLedger = HasSubLedger(glCode);

            TextBox txtSLCodeCtrl = (TextBox)row.FindControl("txtSLCode");
            if (txtSLCodeCtrl != null)
            {
                txtSLCodeCtrl.Enabled = hasSubLedger;
                txtSLCodeCtrl.Style["background-color"] = hasSubLedger ? "#ffffff" : "#f0f0f0";
                if (!hasSubLedger) txtSLCodeCtrl.Text = "";
            }

            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            if (dtVoucherDetails != null && dtVoucherDetails.Rows.Count > row.RowIndex)
            {
                dtVoucherDetails.Rows[row.RowIndex]["GL_CODE"] = glCode;
                dtVoucherDetails.Rows[row.RowIndex]["GL_BOOK_TYPE"] = description;
                ViewState["VoucherDetails"] = dtVoucherDetails;
            }
        }
    }

    protected void txtSLCode_TextChanged(object sender, EventArgs e)
    {
        TextBox txtSLCode = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtSLCode.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            string slCode = txtSLCode.Text.Trim(); 
            if (string.IsNullOrEmpty(slCode)) return;

            int subLedgerId = GetSubLedgerIdFromSLCode(slCode);
            string description = GetSLDescriptionFromSLCode(slCode);

            if (subLedgerId == 0)
            {
                ShowSnackbar("Invalid SL Code", "warning");
                txtSLCode.Text = "";
                return;
            }

            TextBox txtSLType = (TextBox)row.FindControl("txtSLType");
            if (txtSLType != null)
            {
                txtSLType.Text = description;
            }

            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            if (dtVoucherDetails != null && dtVoucherDetails.Rows.Count > row.RowIndex)
            {
                dtVoucherDetails.Rows[row.RowIndex]["SL_TYPE"] = subLedgerId.ToString(); 
                dtVoucherDetails.Rows[row.RowIndex]["SL_CODE"] = slCode;                 
                ViewState["VoucherDetails"] = dtVoucherDetails;
            }
        }
    }

    private int GetSubLedgerIdFromSLCode(string slCode)
    {
        if (string.IsNullOrEmpty(slCode)) return 0;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT SUB_LEDGER_ID 
                        FROM GL_SL_GLMF 
                        WHERE SL_CODE = :slCode";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    private string GetSLDescriptionFromSLCode(string slCode)
    {
        if (string.IsNullOrEmpty(slCode)) return "";

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT DESCRIP 
                        FROM GL_SL_GLMF 
                        WHERE SL_CODE = :slCode";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    protected void txtAmount_TextChanged(object sender, EventArgs e)
    {
        TextBox txtAmount = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtAmount.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            decimal amount;

            if (decimal.TryParse(txtAmount.Text, out amount))
            {
                if (dtVoucherDetails.Rows.Count > row.RowIndex)
                {
                    dtVoucherDetails.Rows[row.RowIndex]["AMOUNT"] = amount;
                }
            }

            ViewState["VoucherDetails"] = dtVoucherDetails;
            CalculateTotal();
        }
    }

    protected void btnAddRows_Click(object sender, EventArgs e)
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        int currentRows = dtVoucherDetails.Rows.Count;

        // Add 10 new rows
        for (int i = 0; i < 10; i++)
        {
            DataRow dr = dtVoucherDetails.NewRow();
            dr["AMOUNT"] = 0;
            dr["DR_CR"] = "1";
            dtVoucherDetails.Rows.Add(dr);
        }

        SaveViewStateAndBind();
        CalculateTotal();

        foreach (GridViewRow row in gvVoucherDetails.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtGLCode = (TextBox)row.FindControl("txtGLCode");
                TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");

                if (txtGLCode != null && txtSLCode != null)
                {
                    string glCode = txtGLCode.Text.Trim();

                    if (!string.IsNullOrEmpty(glCode))
                    {
                        bool hasSubLedger = HasSubLedger(glCode);
                        txtSLCode.Enabled = hasSubLedger;
                        txtSLCode.Style["background-color"] = hasSubLedger ? "#ffffff" : "#f0f0f0";
                    }
                    else
                    {
                        txtSLCode.Enabled = false;
                        txtSLCode.Style["background-color"] = "#f0f0f0";
                    }
                }
            }
        }

        ShowSnackbar("Added 10 rows.", "success");
    }
    #endregion

    #region Data Operations
    private void UpdateDataTableFromGrid()
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        if (dtVoucherDetails == null)
        {
            dtVoucherDetails = CreateDataTable();
        }


        foreach (GridViewRow row in gvVoucherDetails.Rows)
        {
            if (row.RowType != DataControlRowType.DataRow) continue;

            TextBox txtGLCode = (TextBox)row.FindControl("txtGLCode");
            TextBox txtGLType = (TextBox)row.FindControl("txtGLType");
            TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");
            TextBox txtSLType = (TextBox)row.FindControl("txtSLType");
            TextBox txtNarration = (TextBox)row.FindControl("txtNarration");
            TextBox txtBillNumber = (TextBox)row.FindControl("txtBillNumber");
            TextBox txtChequeNumber = (TextBox)row.FindControl("txtChequeNumber");
            TextBox txtCostCentre = (TextBox)row.FindControl("txtCostCentreCode");
            TextBox txtAmount = (TextBox)row.FindControl("txtAmount");

            // If row index exists in DataTable, update it
            if (row.RowIndex < dtVoucherDetails.Rows.Count)
            {
                DataRow dr = dtVoucherDetails.Rows[row.RowIndex];
                dr["GL_CODE"] = txtGLCode != null ? txtGLCode.Text.Trim() : "";
                dr["GL_BOOK_TYPE"] = txtGLType != null ? txtGLType.Text.Trim() : "";
                dr["SL_CODE"] = txtSLCode != null ? txtSLCode.Text.Trim() : "";
                dr["SL_TYPE"] = txtSLType != null ? txtSLType.Text.Trim() : "";
                dr["NARATION"] = txtNarration != null ? txtNarration.Text.Trim() : "";
                dr["BILL_NUMBER"] = txtBillNumber != null ? txtBillNumber.Text.Trim() : "";
                dr["CHEQUE_NUMBER"] = txtChequeNumber != null ? txtChequeNumber.Text.Trim() : "";
                dr["COST_CENTRE_CODE"] = txtCostCentre != null ? txtCostCentre.Text.Trim() : "";

                decimal amount = 0;
                if (txtAmount != null && decimal.TryParse(txtAmount.Text, out amount))
                {
                    dr["AMOUNT"] = amount;
                }
                else
                {
                    dr["AMOUNT"] = 0;
                }
            }
            else
            {
                // Add new row if needed
                DataRow dr = dtVoucherDetails.NewRow();
                dr["GL_CODE"] = txtGLCode != null ? txtGLCode.Text.Trim() : "";
                dr["GL_BOOK_TYPE"] = txtGLType != null ? txtGLType.Text.Trim() : "";
                dr["SL_CODE"] = txtSLCode != null ? txtSLCode.Text.Trim() : "";
                dr["SL_TYPE"] = txtSLType != null ? txtSLType.Text.Trim() : "";
                dr["NARATION"] = txtNarration != null ? txtNarration.Text.Trim() : "";
                dr["BILL_NUMBER"] = txtBillNumber != null ? txtBillNumber.Text.Trim() : "";
                dr["CHEQUE_NUMBER"] = txtChequeNumber != null ? txtChequeNumber.Text.Trim() : "";
                dr["COST_CENTRE_CODE"] = txtCostCentre != null ? txtCostCentre.Text.Trim() : "";

                decimal amount = 0;
                if (txtAmount != null && decimal.TryParse(txtAmount.Text, out amount))
                {
                    dr["AMOUNT"] = amount;
                }
                else
                {
                    dr["AMOUNT"] = 0;
                }

                dr["DR_CR"] = "C";
                dtVoucherDetails.Rows.Add(dr);
            }
        }

        ViewState["VoucherDetails"] = dtVoucherDetails;
        System.Diagnostics.Debug.WriteLine("Updated DataTable with " + dtVoucherDetails.Rows.Count + " rows");
    }

    private void CalculateTotal()
    {
        decimal total = 0;
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        if (dtVoucherDetails != null)
        {
            foreach (DataRow row in dtVoucherDetails.Rows)
            {
                if (row["AMOUNT"] != DBNull.Value)
                {
                    total += Convert.ToDecimal(row["AMOUNT"]);
                }
            }
        }

        lblGrandTotal.Text = total.ToString("N2");
    }
    #endregion

    #region Database Operations
    private void LoadBookTypes()
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            // FOR CASH PAYMENT: Only load GL codes where BOOK_TYPE = 'CPV'
            string query = @"SELECT bt.GL_CODE 
                        FROM GL_BOOK_TYPE bt
                        WHERE bt.BOOK_TYPE = 'CPV' 
                        AND bt.GL_CODE IS NOT NULL 
                        ORDER BY bt.GL_CODE";

            OracleDataAdapter da = new OracleDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlBookType.DataSource = dt;
            ddlBookType.DataTextField = "GL_CODE";
            ddlBookType.DataValueField = "GL_CODE";
            ddlBookType.DataBind();
            ddlBookType.Items.Insert(0, new ListItem("-- Select Cash Type --", ""));
        }
    }

    private string GetGLDescription(string glCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT GL_DESCRP 
                            FROM GL_GLMF 
                            WHERE GL_CODE = :glCode 
                            AND ACTIVE = 1 
                            AND GENERAL_DETAIL = 'D'";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private string GetSLDescription(string subLedgerId)
    {
        if (string.IsNullOrEmpty(subLedgerId)) return "";

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT DESCRIP 
                        FROM GL_SL_GLMF 
                        WHERE SUB_LEDGER_ID = :subLedgerId";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(subLedgerId);

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private string GetSLCodeFromSubLedgerId(string subLedgerId)
    {
        if (string.IsNullOrEmpty(subLedgerId)) return "";

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT SL_CODE 
                        FROM GL_SL_GLMF 
                        WHERE SUB_LEDGER_ID = :subLedgerId";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("subLedgerId", OracleDbType.Int32).Value = Convert.ToInt32(subLedgerId);

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private bool IsDetailAccount(string glCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COUNT(*) 
                            FROM GL_GLMF 
                            WHERE GL_CODE = :glCode 
                            AND ACTIVE = 1
                            AND GENERAL_DETAIL = 'D'";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    private string GetBookTypeFromGLCode(string glCode)
    {
        if (string.IsNullOrEmpty(glCode)) return "CPV";

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT BOOK_TYPE FROM GL_BOOK_TYPE WHERE GL_CODE = :glCode AND BOOK_TYPE = 'CPV' AND ROWNUM = 1";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "CPV";
        }
    }

    private bool HasSubLedger(string glCode)
    {
        if (string.IsNullOrEmpty(glCode)) return false;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT COUNT(*) 
                        FROM GL_SL_TYPE 
                        WHERE GL_CODE = :glCode";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;

            conn.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
    #endregion

    #region Voucher Operations
    private void SaveVoucher()
    {
        try
        {
            // Force update from grid to DataTable
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

            if (dtVoucherDetails == null)
            {
                dtVoucherDetails = CreateDataTable();
            }

            // Clear and rebuild from grid
            dtVoucherDetails.Rows.Clear();

            foreach (GridViewRow row in gvVoucherDetails.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow) continue;

                TextBox txtGLCode = (TextBox)row.FindControl("txtGLCode");
                TextBox txtGLType = (TextBox)row.FindControl("txtGLType");
                TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");      
                TextBox txtSLType = (TextBox)row.FindControl("txtSLType");      
                TextBox txtNarration = (TextBox)row.FindControl("txtNarration");
                TextBox txtBillNumber = (TextBox)row.FindControl("txtBillNumber");
                TextBox txtChequeNumber = (TextBox)row.FindControl("txtChequeNumber");
                TextBox txtCostCentre = (TextBox)row.FindControl("txtCostCentreCode");
                TextBox txtAmount = (TextBox)row.FindControl("txtAmount");

                // Only add rows that have GL Code or Amount > 0
                string glCode = txtGLCode != null ? txtGLCode.Text.Trim() : "";
                decimal amount = 0;
                if (txtAmount != null && decimal.TryParse(txtAmount.Text, out amount)) { }

                if (!string.IsNullOrEmpty(glCode) || amount > 0)
                {
                    DataRow dr = dtVoucherDetails.NewRow();
                    dr["GL_CODE"] = glCode;
                    dr["GL_BOOK_TYPE"] = txtGLType != null ? txtGLType.Text.Trim() : "";
                    //dr["SL_TYPE"] = txtSLCode != null ? txtSLCode.Text.Trim() : "";    
                    //dr["SL_CODE"] = txtSLType != null ? txtSLType.Text.Trim() : ""; 
                    string slCode = txtSLCode != null ? txtSLCode.Text.Trim() : "";
                    int subLedgerId = 0;
                    if (!string.IsNullOrEmpty(slCode))
                    {
                        subLedgerId = GetSubLedgerIdFromSLCode(slCode);
                    }

                    dr["SL_TYPE"] = subLedgerId.ToString();  // Store SUB_LEDGER_ID
                    dr["SL_CODE"] = slCode;   
                    dr["NARATION"] = txtNarration != null ? txtNarration.Text.Trim() : "";
                    dr["BILL_NUMBER"] = txtBillNumber != null ? txtBillNumber.Text.Trim() : "";
                    dr["CHEQUE_NUMBER"] = txtChequeNumber != null ? txtChequeNumber.Text.Trim() : "";
                    dr["COST_CENTRE_CODE"] = txtCostCentre != null ? txtCostCentre.Text.Trim() : "";
                    dr["AMOUNT"] = amount;
                    dr["DR_CR"] = "C";

                    dtVoucherDetails.Rows.Add(dr);
                }
            }

            ViewState["VoucherDetails"] = dtVoucherDetails;

            if (!ValidateVoucher()) return;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                OracleTransaction transaction = conn.BeginTransaction();

                try
                {
                    if (hfCurrentMode.Value == "EDIT")
                    {
                        string deleteVoucherQuery = "DELETE FROM GL_VOUCHERS WHERE VOUCHER_KEY = :voucherKey";
                        OracleCommand deleteVoucherCmd = new OracleCommand(deleteVoucherQuery, conn);
                        deleteVoucherCmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblcpv.Text;
                        deleteVoucherCmd.ExecuteNonQuery();

                        string deleteFormQuery = "DELETE FROM GL_FORMS WHERE VOUCHER_KEY = :voucherKey";
                        OracleCommand deleteFormCmd = new OracleCommand(deleteFormQuery, conn);
                        deleteFormCmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblcpv.Text;
                        deleteFormCmd.ExecuteNonQuery();
                    }

                    int lineNumber = 1;
                    int savedRows = SaveVoucherEntries(conn, transaction, ref lineNumber);

                    InsertIntoGLForms(conn);

                    transaction.Commit();

                    ShowSnackbar("Voucher saved successfully! " + savedRows + " entries saved.", "success");
                    hfCurrentMode.Value = "EDIT";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogError("SaveVoucher-Transaction", ex);
                    ShowSnackbar("Error: " + ex.Message, "error");
                }
            }
        }
        catch (Exception ex)
        {
            LogError("SaveVoucher-Outer", ex);
            ShowSnackbar("Error saving voucher: " + ex.Message, "error");
        }
    }


    private int SaveVoucherEntries(OracleConnection conn, OracleTransaction transaction, ref int lineNumber)
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        if (dtVoucherDetails == null || dtVoucherDetails.Rows.Count == 0) return 0;

        int rowsSaved = 0;
        string debitGLCode = ddlBookType.SelectedValue;
        string bookType = GetBookTypeFromGLCode(debitGLCode);
        int transactionLogId = GetCurrentLogId();
        foreach (DataRow row in dtVoucherDetails.Rows)
        {
            string creditGLCode = row["GL_CODE"] != null ? row["GL_CODE"].ToString() : "";

            if (string.IsNullOrEmpty(creditGLCode)) continue;

            decimal amount = 0;
            if (row["AMOUNT"] != null && row["AMOUNT"] != DBNull.Value)
            {
                decimal.TryParse(row["AMOUNT"].ToString(), out amount);
            }

            if (amount == 0) continue;

            try
            {
                // SL_TYPE contains SUB_LEDGER_ID (numeric)
                int slTypeId = 0;
                if (row["SL_TYPE"] != null && !string.IsNullOrEmpty(row["SL_TYPE"].ToString()))
                {
                    int.TryParse(row["SL_TYPE"].ToString(), out slTypeId);
                }

                // SL_CODE contains the actual SL_CODE string (like 'AF0455') - max 15 chars
                string actualSLCode = row["SL_CODE"] != null ? row["SL_CODE"].ToString() : "";

                int costCentreCode = 0;
                if (row["COST_CENTRE_CODE"] != null && !string.IsNullOrEmpty(row["COST_CENTRE_CODE"].ToString()))
                    int.TryParse(row["COST_CENTRE_CODE"].ToString(), out costCentreCode);

                string billNumber = row["BILL_NUMBER"] != null ? row["BILL_NUMBER"].ToString() : "";
                string chequeNumber = row["CHEQUE_NUMBER"] != null ? row["CHEQUE_NUMBER"].ToString() : "";
                string narration = row["NARATION"] != null ? row["NARATION"].ToString() : "";

                // Insert Credit entry (DRCR_NUMBER = 1)
                InsertVoucherEntry(conn, transaction, ref lineNumber, bookType, creditGLCode,
                    slTypeId, actualSLCode, costCentreCode, billNumber, chequeNumber, "1", narration, amount, 1, transactionLogId);
                rowsSaved++;

                // Insert Debit entry (DRCR_NUMBER = 2)
                InsertVoucherEntry(conn, transaction, ref lineNumber, bookType, debitGLCode,
                    slTypeId, actualSLCode, costCentreCode, billNumber, chequeNumber, "2", narration, amount, 2, transactionLogId);
                rowsSaved++;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error inserting row: " + ex.Message);
                throw;
            }
        }

        return rowsSaved;
    }

    private void InsertVoucherEntry(OracleConnection conn, OracleTransaction transaction, ref int lineNumber,
    string bookType, string glCode, int slTypeId, string actualSLCode, int costCentreCode,
    string billNumber, string chequeNumber, string drCr, string narration, decimal amount, int drcrNumber, int logId)
    {
         System.Diagnostics.Debug.WriteLine("SL_CODE value being inserted: '{actualSLCode}', Length: {actualSLCode.Length}");
        string query = @"INSERT INTO GL_VOUCHERS 
                (VOUCHER_KEY, GL_BOOK_TYPE, VOUCHER_NUMBER, LINE_NUMBER, 
                 DRCR_NUMBER, GL_CODE, SL_TYPE, SL_CODE, COST_CENTRE_CODE,
                 BILL_NUMBER, CHEQUE_NUMBER, DR_CR, NARATION, AMOUNT, COMP_ID, LOG_ID)
                VALUES 
                (:voucherKey, :glBookType, :voucherNumber, :lineNumber,
                 :drcrNumber, :glCode, :slType, :slCode, :costCentreCode,
                 :billNumber, :chequeNumber, :drCr, :naration, :amount, :compId, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblcpv.Text;
        cmd.Parameters.Add("glBookType", OracleDbType.Varchar2).Value = bookType;
        cmd.Parameters.Add("voucherNumber", OracleDbType.Int32).Value = Convert.ToInt32(lblVoucherNumber.Text);
        cmd.Parameters.Add("lineNumber", OracleDbType.Int32).Value = lineNumber++;
        cmd.Parameters.Add("drcrNumber", OracleDbType.Int32).Value = drcrNumber;
        cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;
        cmd.Parameters.Add("slType", OracleDbType.Int32).Value = slTypeId == 0 ? (object)DBNull.Value : slTypeId;
        cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(actualSLCode) ? (object)DBNull.Value : actualSLCode;
        cmd.Parameters.Add("costCentreCode", OracleDbType.Int32).Value = costCentreCode == 0 ? (object)DBNull.Value : costCentreCode;
        cmd.Parameters.Add("billNumber", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(billNumber) ? (object)DBNull.Value : billNumber;
        cmd.Parameters.Add("chequeNumber", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(chequeNumber) ? (object)DBNull.Value : chequeNumber;
        cmd.Parameters.Add("drCr", OracleDbType.Varchar2).Value = drCr;
        cmd.Parameters.Add("naration", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(narration) ? (object)DBNull.Value : narration;
        cmd.Parameters.Add("amount", OracleDbType.Decimal).Value = amount;
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId(); // Use helper
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = GetCurrentLogId();
        cmd.ExecuteNonQuery();
    }

    private bool ValidateVoucher()
    {
        if (string.IsNullOrEmpty(ddlBookType.SelectedValue))
        {
            ShowSnackbar("Please select a GL Code for Debit entry", "warning");
            return false;
        }

        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
        decimal totalAmount = 0;
        bool hasValidLine = false;

        foreach (DataRow row in dtVoucherDetails.Rows)
        {
            string glCode = row["GL_CODE"] != null ? row["GL_CODE"].ToString() : "";

            if (!string.IsNullOrEmpty(glCode))
            {
                if (!IsDetailAccount(glCode))
                {
                    ShowSnackbar("GL Code " + glCode + " is not a Detail account. Only Detail accounts (GENERAL_DETAIL = D) are allowed.");
                    return false;
                }

                decimal amount = 0;
                if (row["AMOUNT"] != null && row["AMOUNT"] != DBNull.Value)
                {
                    amount = Convert.ToDecimal(row["AMOUNT"]);
                }

                if (amount > 0)
                {
                    hasValidLine = true;
                    totalAmount += amount;
                }
            }
        }

        if (!hasValidLine)
        {
            ShowSnackbar("Please enter at least one valid credit line with GL Code and amount greater than zero");
            return false;
        }

        if (totalAmount <= 0)
        {
            ShowSnackbar("Total amount must be greater than zero");
            return false;
        }

        return true;
    }

    private void LoadVoucher(string voucherKey)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string[] parts = voucherKey.Split('-');
            if (parts.Length >= 3)
            {
                lblVoucherNumber.Text = parts[2];
                lblcpv.Text = voucherKey;

                string bookType = parts[1];
                ListItem item = ddlBookType.Items.FindByValue(GetGLCodeFromBookType(bookType));
                if (item != null)
                {
                    ddlBookType.SelectedValue = item.Value;
                }
            }

            string query = @"SELECT GL_CODE, SL_CODE, SL_TYPE, COST_CENTRE_CODE,
                                BILL_NUMBER, CHEQUE_NUMBER, DR_CR, NARATION, AMOUNT,
                                LINE_NUMBER
                         FROM GL_VOUCHERS 
                         WHERE VOUCHER_KEY = :voucherKey
                         ORDER BY LINE_NUMBER";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            DataTable allLines = new DataTable();
            da.Fill(allLines);

            DataTable creditRows = CreateDataTable();

            foreach (DataRow row in allLines.Rows)
            {
                string drcr = row["DR_CR"].ToString();

                if (drcr == "D" || drcr == "2")
                {
                    string glCode = row["GL_CODE"].ToString();
                    ListItem item = ddlBookType.Items.FindByValue(glCode);
                    if (item != null) ddlBookType.SelectedValue = glCode;
                }
                else
                {
                    DataRow newRow = creditRows.NewRow();
                    newRow["GL_CODE"] = row["GL_CODE"];
                    newRow["GL_BOOK_TYPE"] = GetGLDescription(row["GL_CODE"].ToString());

                    string slTypeId = row["SL_TYPE"] != null ? row["SL_TYPE"].ToString() : "";    
                    string slCodeDesc = row["SL_CODE"] != null ? row["SL_CODE"].ToString() : "";   

                    newRow["SL_CODE"] = slCodeDesc;        
                    newRow["SL_TYPE"] = slTypeId;          

                    newRow["COST_CENTRE_CODE"] = row["COST_CENTRE_CODE"];
                    newRow["BILL_NUMBER"] = row["BILL_NUMBER"];
                    newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"];
                    newRow["NARATION"] = row["NARATION"];
                    newRow["AMOUNT"] = row["AMOUNT"];
                    newRow["DR_CR"] = "C";
                    creditRows.Rows.Add(newRow);
                }
            }

            dtVoucherDetails = creditRows;
            ViewState["VoucherDetails"] = dtVoucherDetails;
            gvVoucherDetails.DataSource = dtVoucherDetails;
            gvVoucherDetails.DataBind();

            // set the SL Code field states based on GL Code
            foreach (GridViewRow row in gvVoucherDetails.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    TextBox txtGLCode = (TextBox)row.FindControl("txtGLCode");
                    TextBox txtSLCode = (TextBox)row.FindControl("txtSLCode");
                    TextBox txtSLType = (TextBox)row.FindControl("txtSLType");

                    if (txtGLCode != null && txtSLCode != null)
                    {
                        string glCode = txtGLCode.Text.Trim();
                        bool hasSubLedger = HasSubLedger(glCode);
                        txtSLCode.Enabled = hasSubLedger;
                        txtSLCode.Style["background-color"] = hasSubLedger ? "#ffffff" : "#f0f0f0";
                    }
                }
            }

            CalculateTotal();
            hfCurrentMode.Value = "EDIT";
            lblStatus.Text = "UnPosted";
            lblStatus.CssClass = "status-unposted";
        }
    }
    // helper method to convert book type to GL code
    private string GetGLCodeFromBookType(string bookType)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT GL_CODE FROM GL_BOOK_TYPE WHERE BOOK_TYPE = :bookType AND ROWNUM = 1";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;

            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private void InsertIntoGLForms(OracleConnection conn)
    {
        // Parse date from voucher key or use current date
        DateTime voucherDate;
        string[] keyParts = lblcpv.Text.Split('-');

        if (!DateTime.TryParse(txtVoucherDate.Text, out voucherDate))
        {
            voucherDate = DateTime.Now; // Fallback to current date
        }

        string query = @"INSERT INTO GL_FORMS 
                    (VOUCHER_KEY, VOUCHER_DATE, VOUCHER_NUMBER, BOOK_TYPE, 
                     GL_FORM_NUMBER, COMP_ID, LOG_ID, POST)
                    VALUES 
                    (:voucherKey, :voucherDate, :voucherNumber, :bookType,
                     :glFormNumber, :compId, :logId, :post)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblcpv.Text;
        cmd.Parameters.Add("voucherDate", OracleDbType.Date).Value = voucherDate;

        // VOUCHER_NUMBER and GL_FORM_NUMBER are the same
        int voucherNum = Convert.ToInt32(lblVoucherNumber.Text);
        cmd.Parameters.Add("voucherNumber", OracleDbType.Int32).Value = voucherNum;

        string bookType = "CPV"; // Default
        if (!string.IsNullOrEmpty(lblcpv.Text))
        {
            string[] parts = lblcpv.Text.Split('-');
            if (parts.Length >= 2)
                bookType = parts[1];
        }
        cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;

        // GL_FORM_NUMBER is the same as VOUCHER_NUMBER
        cmd.Parameters.Add("glFormNumber", OracleDbType.Int32).Value = voucherNum;
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = Convert.ToInt32(hfCompId.Value);
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = GetCurrentLogId(); 
        cmd.Parameters.Add("post", OracleDbType.Int32).Value = (lblStatus.Text == "Posted") ? 1 : 0;
        cmd.ExecuteNonQuery();
    }
    #endregion

    #region Voucher Header
    private void SetDefaultDate()
    {
        txtVoucherDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
    }

    private void GenerateVoucherKey()
    {
        string bookType = GetBookTypeFromGLCode(ddlBookType.SelectedValue);
        if (string.IsNullOrEmpty(bookType)) bookType = "CPV";

        lblcpv.Text = "1-" + bookType + "-" + lblVoucherNumber.Text;
    }

    private void GenerateNewVoucherNumber()
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                // Get the next number from GL_FORMS table
                string query = @"SELECT NVL(MAX(GL_FORM_NUMBER), 0) + 1 
                            FROM GL_FORMS";

                OracleCommand cmd = new OracleCommand(query, conn);

                object result = cmd.ExecuteScalar();
                int newVoucherNo = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;

                lblVoucherNumber.Text = newVoucherNo.ToString();

                // Generate voucher key with book type
                string bookType = GetBookTypeFromGLCode(ddlBookType.SelectedValue);
                if (string.IsNullOrEmpty(bookType)) bookType = "CPV";

                lblcpv.Text = "1-" + bookType + "-" + newVoucherNo.ToString();
            }
        }
        catch (Exception ex)
        {
            LogError("GenerateNewVoucherNumber", ex);
            lblVoucherNumber.Text = "1";
            string bookType = string.IsNullOrEmpty(ddlBookType.SelectedValue) ? "CPV" : ddlBookType.SelectedValue;
            lblcpv.Text = "1-" + bookType + "-1";
        }
    }

    protected void ddlBookType_SelectedIndexChanged(object sender, EventArgs e)
    {
        GenerateVoucherKey();
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "Posted";
        lblStatus.CssClass = "status-posted";
        ShowSnackbar("Voucher marked as posted");
    }

    protected void btnUnposted_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "UnPosted";
        lblStatus.CssClass = "status-unposted";
        ShowSnackbar("Voucher marked as unposted");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveVoucher();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        txtVoucherDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        GenerateNewVoucherNumber(); 
        if (ddlBookType.Items.Count > 0) ddlBookType.SelectedIndex = 0;
        // GenerateVoucherKey()
        InitializeGrid(DEFAULT_ROWS);
        lblStatus.Text = "UnPosted";
        lblStatus.CssClass = "status-unposted";
        hfCurrentMode.Value = "ADD";
    }
    #endregion

    #region Navigation
    private List<string> GetVoucherKeys()
    {
        List<string> keys = new List<string>();
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT DISTINCT VOUCHER_KEY FROM GL_VOUCHERS ORDER BY VOUCHER_KEY";
            OracleCommand cmd = new OracleCommand(query, conn);
            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                keys.Add(reader["VOUCHER_KEY"].ToString());
            }
        }
        return keys;
    }

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        List<string> keys = GetVoucherKeys();
        int index = keys.IndexOf(lblcpv.Text);

        if (index > 0)
        {
            LoadVoucher(keys[index - 1]);
        }
        else
        {
            ShowSnackbar("This is the first voucher");
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        List<string> keys = GetVoucherKeys();
        int index = keys.IndexOf(lblcpv.Text);

        if (index < keys.Count - 1)
        {
            LoadVoucher(keys[index + 1]);
        }
        else
        {
            ShowSnackbar("This is the last voucher");
        }
    }

    protected void btnFirst_Click(object sender, EventArgs e)
    {
        List<string> keys = GetVoucherKeys();

        if (keys.Count > 0)
        {
            LoadVoucher(keys[0]);
        }
        else
        {
            ShowSnackbar("No vouchers found", "warning");
        }
    }

    protected void btnLast_Click(object sender, EventArgs e)
    {
        List<string> keys = GetVoucherKeys();

        if (keys.Count > 0)
        {
            LoadVoucher(keys[keys.Count - 1]);
        }
        else
        {
            ShowSnackbar("No vouchers found", "warning");
        }
    }
    #endregion

    #region Search Operations
    protected void btnCopyVoucher_Click(object sender, EventArgs e)
    {
        try
        {
            LoadSearchBookTypes();
            LoadSearchResults();
            mpeSearchVoucher.Show();
        }
        catch (Exception ex)
        {
            LogError("btnCopyVoucher_Click", ex);
            ShowSnackbar("Error opening search: " + ex.Message);
        }
    }

    private void LoadSearchBookTypes()
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT DISTINCT BOOK_TYPE 
                            FROM GL_BOOK_TYPE 
                            WHERE BOOK_TYPE IS NOT NULL
                            ORDER BY BOOK_TYPE";

            OracleDataAdapter da = new OracleDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlSearchBookType.DataSource = dt;
            ddlSearchBookType.DataTextField = "BOOK_TYPE";
            ddlSearchBookType.DataValueField = "BOOK_TYPE";
            ddlSearchBookType.DataBind();
            ddlSearchBookType.Items.Insert(0, new ListItem("-- All --", ""));
        }
    }

    private void LoadSearchResults()
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT VOUCHER_KEY, GL_BOOK_TYPE, VOUCHER_NUMBER, SUM(AMOUNT) AS TOTAL_AMOUNT ");
                query.Append("FROM GL_VOUCHERS WHERE 1=1 ");

                if (!string.IsNullOrEmpty(txtSearchVoucherKey.Text))
                {
                    query.Append(" AND VOUCHER_KEY LIKE :voucherKey");
                }

                if (!string.IsNullOrEmpty(ddlSearchBookType.SelectedValue))
                {
                    query.Append(" AND GL_BOOK_TYPE = :bookType");
                }

                query.Append(" GROUP BY VOUCHER_KEY, GL_BOOK_TYPE, VOUCHER_NUMBER ORDER BY VOUCHER_KEY DESC");

                OracleCommand cmd = new OracleCommand(query.ToString(), conn);

                if (!string.IsNullOrEmpty(txtSearchVoucherKey.Text))
                {
                    cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = "%" + txtSearchVoucherKey.Text + "%";
                }

                if (!string.IsNullOrEmpty(ddlSearchBookType.SelectedValue))
                {
                    cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = ddlSearchBookType.SelectedValue;
                }

                conn.Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    dt = new DataTable();
                    dt.Columns.Add("VOUCHER_KEY", typeof(string));
                    dt.Columns.Add("GL_BOOK_TYPE", typeof(string));
                    dt.Columns.Add("VOUCHER_NUMBER", typeof(string));
                    dt.Columns.Add("TOTAL_AMOUNT", typeof(decimal));

                    DataRow dr = dt.NewRow();
                    dr["VOUCHER_KEY"] = "No records found";
                    dr["GL_BOOK_TYPE"] = "";
                    dr["VOUCHER_NUMBER"] = "";
                    dr["TOTAL_AMOUNT"] = 0;
                    dt.Rows.Add(dr);
                }

                gvSearchResults.DataSource = dt;
                gvSearchResults.DataBind();
            }
            catch (Exception ex)
            {
                LogError("LoadSearchResults", ex);
                throw;
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadSearchResults();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtSearchVoucherKey.Text = "";
        if (ddlSearchBookType.Items.Count > 0) ddlSearchBookType.SelectedIndex = 0;
        LoadSearchResults();
    }

    protected void btnCloseSearch_Click(object sender, EventArgs e)
    {
        mpeSearchVoucher.Hide();
    }

    protected void gvSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SelectVoucher")
        {
            string voucherKey = e.CommandArgument.ToString();
            LoadVoucher(voucherKey);
            mpeSearchVoucher.Hide();
        }
    }

    protected void gvSearchResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onclick"] = "javascript:__doPostBack('" + gvSearchResults.ID + "', 'Select$" + e.Row.RowIndex + "')";
            e.Row.Style["cursor"] = "pointer";
        }
    }
    #endregion

    #region Web Methods
    [System.Web.Services.WebMethod]
    public static List<object> SearchGLCodes(string searchTerm)
    {
        List<object> results = new List<object>();
        string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"SELECT GL_CODE, GL_DESCRP 
                            FROM GL_GLMF 
                            WHERE (UPPER(GENERAL_DETAIL) IN ('D', '1'))
                            AND (UPPER(ACTIVE) IN ('Y', '1'))
                            AND (GL_CODE LIKE :searchTerm OR GL_DESCRP LIKE :searchTerm)
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
                    GL_DESCRP = reader["GL_DESCRP"].ToString()
                });
            }
        }

        return results;
    }

    [System.Web.Services.WebMethod]
    public static List<object> SearchSLCodes(string searchTerm, string glCode)
    {
        List<object> results = new List<object>();
        string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
            SELECT s.SUB_LEDGER_ID, s.SL_CODE, s.DESCRIP 
            FROM GL_SL_GLMF s
            INNER JOIN GL_SL_TYPE t ON s.SUB_LEDGER_ID = t.SUB_LEDGER_ID
            WHERE t.GL_CODE = :glCode 
            AND (TO_CHAR(s.SUB_LEDGER_ID) LIKE :searchTerm 
                 OR UPPER(s.SL_CODE) LIKE UPPER(:searchTerm) 
                 OR UPPER(s.DESCRIP) LIKE UPPER(:searchTerm))
            AND ROWNUM <= 20 
            ORDER BY s.SL_CODE";

            OracleCommand cmd = new OracleCommand(query, conn);

            cmd.Parameters.Add("glCode", OracleDbType.Varchar2).Value = glCode;
            cmd.Parameters.Add("searchTerm", OracleDbType.Varchar2).Value = "%" + searchTerm + "%";

            conn.Open();

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(new
                    {
                        SUB_LEDGER_ID = reader["SUB_LEDGER_ID"].ToString(),
                        SL_CODE = reader["SL_CODE"].ToString(),
                        DESCRIP = reader["DESCRIP"].ToString()
                    });
                }
            }
        }

        return results;
    }
    #endregion

    #region Utility Methods
    private void ShowAlert(string message)
    {
        string script = "<script>alert('" + message.Replace("'", "\\'") + "');</script>";
        Response.Write(script);
    }

    private void LogError(string methodName, Exception ex)
    {
        string errorMessage = "Error in " + methodName + ": " + ex.Message + "\n" + ex.StackTrace;
        if (ex.InnerException != null)
        {
            errorMessage += "\nInner Exception: " + ex.InnerException.Message;
        }

        System.Diagnostics.Debug.WriteLine(errorMessage);

    }
    #endregion

    #region Logging Helpers

    private int GetCurrentLogId()
    {
        return LogHelper.GetCurrentLogId(Session, Request);
    }

    private int GetCurrentCompId()
    {
        return Session["CurrentCompId"] != null ? Convert.ToInt32(Session["CurrentCompId"]) : 1;
    }

    #endregion

    public string script { get; set; }

    private void ShowSnackbar(string message, string type = "info")
    {
        string escapedMessage = message.Replace("'", "\\'");

        string script = @"<script type='text/javascript'>
        (function() {
            if (typeof showSnackbar === 'function') {
                showSnackbar('" + escapedMessage + @"', '" + type + @"');
            } else {
                setTimeout(function() {
                    showSnackbar('" + escapedMessage + @"', '" + type + @"');
                }, 100);
            }
        })();
    </script>";

        if (ScriptManager.GetCurrent(this) != null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSnackbar_" + Guid.NewGuid().ToString().Replace("-", ""), script, false);
        }
        else
        {
            Response.Write(script);
        }
    }

}