using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;
using System.Text;
using System.Drawing;

public partial class GL_Journal_Voucher : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;
    private DataTable dtVoucherDetails;
    private const int DEFAULT_ROWS = 13;

    #region Page Events
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Hide Book Type dropdown - it's not needed for Journal Voucher
            //ddlBookType.Visible = false;

            // Set fixed Journal Type label
            // You might want to add a label to show "Journal Voucher" instead

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
    private void InitializeGrid(int numberOfRows)
    {
        dtVoucherDetails = CreateDataTable();

        for (int i = 0; i < numberOfRows; i++)
        {
            DataRow dr = dtVoucherDetails.NewRow();
            dr["DEBIT"] = 0;
            dr["CREDIT"] = 0;
            dr["DR_CR"] = "";
            dtVoucherDetails.Rows.Add(dr);
        }

        SaveViewStateAndBind();
        CalculateTotals();
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
        dt.Columns.Add("DEBIT", typeof(decimal));
        dt.Columns.Add("CREDIT", typeof(decimal));
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
                ShowSnackbar("Cannot delete the last row", "warning");
                return;
            }

            SaveViewStateAndBind();
            CalculateTotals();
        }
    }

    protected void gvVoucherDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            // Calculate totals for footer
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            if (ViewState["TotalDebit"] != null)
                totalDebit = Convert.ToDecimal(ViewState["TotalDebit"]);
            if (ViewState["TotalCredit"] != null)
                totalCredit = Convert.ToDecimal(ViewState["TotalCredit"]);

            Label lblTotalDebit = (Label)e.Row.FindControl("lblTotalDebit");
            Label lblTotalCredit = (Label)e.Row.FindControl("lblTotalCredit");

            if (lblTotalDebit != null)
                lblTotalDebit.Text = totalDebit.ToString("N2");
            if (lblTotalCredit != null)
                lblTotalCredit.Text = totalCredit.ToString("N2");
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Set SL Code field state based on GL Code
            TextBox txtGLCode = (TextBox)e.Row.FindControl("txtGLCode");
            TextBox txtSLCode = (TextBox)e.Row.FindControl("txtSLCode");

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
                ShowSnackbar("Invalid GL Code or not a Detail account (GENERAL_DETAIL must be D)", "error");
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

    protected void btnAddRows_Click(object sender, EventArgs e)
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        // Add 10 new rows
        for (int i = 0; i < 10; i++)
        {
            DataRow dr = dtVoucherDetails.NewRow();
            dr["DEBIT"] = 0;
            dr["CREDIT"] = 0;
            dr["DR_CR"] = "";
            dtVoucherDetails.Rows.Add(dr);
        }

        SaveViewStateAndBind();
        CalculateTotals();

        // Update SL Code field states
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

        // Clear existing rows but keep structure
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
            TextBox txtDebit = (TextBox)row.FindControl("txtDebit");
            TextBox txtCredit = (TextBox)row.FindControl("txtCredit");

            string glCode = txtGLCode != null ? txtGLCode.Text.Trim() : "";

            decimal debit = 0;
            decimal credit = 0;

            if (txtDebit != null) decimal.TryParse(txtDebit.Text, out debit);
            if (txtCredit != null) decimal.TryParse(txtCredit.Text, out credit);

            // Only add rows that have GL Code or amount
            if (!string.IsNullOrEmpty(glCode) || debit > 0 || credit > 0)
            {
                DataRow dr = dtVoucherDetails.NewRow();
                dr["GL_CODE"] = glCode;
                dr["GL_BOOK_TYPE"] = txtGLType != null ? txtGLType.Text.Trim() : "";

                string slCode = txtSLCode != null ? txtSLCode.Text.Trim() : "";
                int subLedgerId = 0;
                if (!string.IsNullOrEmpty(slCode))
                {
                    subLedgerId = GetSubLedgerIdFromSLCode(slCode);
                }

                dr["SL_TYPE"] = subLedgerId.ToString();
                dr["SL_CODE"] = slCode;
                dr["NARATION"] = txtNarration != null ? txtNarration.Text.Trim() : "";
                dr["BILL_NUMBER"] = txtBillNumber != null ? txtBillNumber.Text.Trim() : "";
                dr["CHEQUE_NUMBER"] = txtChequeNumber != null ? txtChequeNumber.Text.Trim() : "";
                dr["COST_CENTRE_CODE"] = txtCostCentre != null ? txtCostCentre.Text.Trim() : "";
                dr["DEBIT"] = debit;
                dr["CREDIT"] = credit;

                // Set DR_CR based on which amount is entered
                if (debit > 0)
                    dr["DR_CR"] = "2"; // Debit (2 as per your requirement)
                else if (credit > 0)
                    dr["DR_CR"] = "1"; // Credit (1 as per your requirement)
                else
                    dr["DR_CR"] = "";

                dtVoucherDetails.Rows.Add(dr);
            }
        }

        ViewState["VoucherDetails"] = dtVoucherDetails;
        System.Diagnostics.Debug.WriteLine("Updated DataTable with " + dtVoucherDetails.Rows.Count + " rows");

        CalculateTotals();
    }
    #endregion

    #region Database Operations
    private void LoadBookTypes()
    {
        // This method is kept but not used - Book Type dropdown is hidden
        // You can remove this method if not needed elsewhere
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
        // For Journal Voucher, always return "GJV" or a fixed value
        return "GJV";
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
            UpdateDataTableFromGrid();

            if (!ValidateDebitCreditBalance())
            {
                return;
            }

            if (!ValidateVoucherDetails())
            {
                return;
            }

            // CREATE NEW LOG ENTRY FOR THIS TRANSACTION
            int transactionLogId = LogHelper.CreateTransactionLog(Session, Request);

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
                        deleteVoucherCmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblGJV.Text;
                        deleteVoucherCmd.ExecuteNonQuery();

                        string deleteFormQuery = "DELETE FROM GL_FORMS WHERE VOUCHER_KEY = :voucherKey";
                        OracleCommand deleteFormCmd = new OracleCommand(deleteFormQuery, conn);
                        deleteFormCmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblGJV.Text;
                        deleteFormCmd.ExecuteNonQuery();
                    }

                    int lineNumber = 1;
                    int savedRows = SaveVoucherEntries(conn, transaction, ref lineNumber, transactionLogId);
                    InsertIntoGLForms(conn, transactionLogId);

                    transaction.Commit();
                    Session["CurrentLogId"] = transactionLogId;

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

    private int SaveVoucherEntries(OracleConnection conn, OracleTransaction transaction, ref int lineNumber, int transactionLogId)
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        if (dtVoucherDetails == null || dtVoucherDetails.Rows.Count == 0) return 0;

        int rowsSaved = 0;
        string bookType = "GJV"; // Fixed book type for Journal Voucher

        foreach (DataRow row in dtVoucherDetails.Rows)
        {
            string glCode = row["GL_CODE"] != null ? row["GL_CODE"].ToString() : "";
            decimal debit = row["DEBIT"] != DBNull.Value ? Convert.ToDecimal(row["DEBIT"]) : 0;
            decimal credit = row["CREDIT"] != DBNull.Value ? Convert.ToDecimal(row["CREDIT"]) : 0;

            if (string.IsNullOrEmpty(glCode) || (debit == 0 && credit == 0)) continue;

            try
            {
                int slTypeId = 0;
                if (row["SL_TYPE"] != null && !string.IsNullOrEmpty(row["SL_TYPE"].ToString()))
                {
                    int.TryParse(row["SL_TYPE"].ToString(), out slTypeId);
                }

                string actualSLCode = row["SL_CODE"] != null ? row["SL_CODE"].ToString() : "";

                int costCentreCode = 0;
                if (row["COST_CENTRE_CODE"] != null && !string.IsNullOrEmpty(row["COST_CENTRE_CODE"].ToString()))
                    int.TryParse(row["COST_CENTRE_CODE"].ToString(), out costCentreCode);

                string billNumber = row["BILL_NUMBER"] != null ? row["BILL_NUMBER"].ToString() : "";
                string chequeNumber = row["CHEQUE_NUMBER"] != null ? row["CHEQUE_NUMBER"].ToString() : "";
                string narration = row["NARATION"] != null ? row["NARATION"].ToString() : "";

                // For Credit entries
                if (credit > 0)
                {
                    InsertVoucherEntry(conn, transaction, ref lineNumber, bookType, glCode,
                        slTypeId, actualSLCode, costCentreCode, billNumber, chequeNumber, "1", narration, credit, 1, transactionLogId);
                    rowsSaved++;
                }

                // For Debit entries
                if (debit > 0)
                {
                    InsertVoucherEntry(conn, transaction, ref lineNumber, bookType, glCode,
                        slTypeId, actualSLCode, costCentreCode, billNumber, chequeNumber, "2", narration, debit, 2, transactionLogId);
                    rowsSaved++;
                }
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
        string query = @"INSERT INTO GL_VOUCHERS 
            (VOUCHER_KEY, GL_BOOK_TYPE, VOUCHER_NUMBER, LINE_NUMBER, 
             DRCR_NUMBER, GL_CODE, SL_TYPE, SL_CODE, COST_CENTRE_CODE,
             BILL_NUMBER, CHEQUE_NUMBER, DR_CR, NARATION, AMOUNT, COMP_ID, LOG_ID)
            VALUES 
            (:voucherKey, :glBookType, :voucherNumber, :lineNumber,
             :drcrNumber, :glCode, :slType, :slCode, :costCentreCode,
             :billNumber, :chequeNumber, :drCr, :naration, :amount, :compId, :logId)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblGJV.Text;
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
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = GetCurrentCompId();
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = logId;

        cmd.ExecuteNonQuery();
    }

    private bool ValidateVoucherDetails()
    {
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        foreach (DataRow row in dtVoucherDetails.Rows)
        {
            string glCode = row["GL_CODE"] != null ? row["GL_CODE"].ToString() : "";

            if (!string.IsNullOrEmpty(glCode))
            {
                if (!IsDetailAccount(glCode))
                {
                    ShowSnackbar("GL Code " + glCode + " is not a Detail account. Only Detail accounts (GENERAL_DETAIL = D) are allowed.", "error");
                    return false;
                }
            }
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
                lblGJV.Text = voucherKey;
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

            DataTable voucherRows = CreateDataTable();

            foreach (DataRow row in allLines.Rows)
            {
                string drcr = row["DR_CR"].ToString();

                DataRow newRow = voucherRows.NewRow();
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

                // Set Debit or Credit based on DR_CR
                decimal amount = row["AMOUNT"] != DBNull.Value ? Convert.ToDecimal(row["AMOUNT"]) : 0;
                if (drcr == "D" || drcr == "2")
                {
                    newRow["DEBIT"] = amount;
                    newRow["CREDIT"] = 0;
                }
                else
                {
                    newRow["DEBIT"] = 0;
                    newRow["CREDIT"] = amount;
                }

                newRow["DR_CR"] = drcr;
                voucherRows.Rows.Add(newRow);
            }

            dtVoucherDetails = voucherRows;
            ViewState["VoucherDetails"] = dtVoucherDetails;
            gvVoucherDetails.DataSource = dtVoucherDetails;
            gvVoucherDetails.DataBind();

            // Set the SL Code field states based on GL Code
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

            CalculateTotals();
            hfCurrentMode.Value = "EDIT";
            lblStatus.Text = "UnPosted";
            lblStatus.CssClass = "status-unposted";
        }
    }

    private void InsertIntoGLForms(OracleConnection conn, int transactionLogId)
    {
        DateTime voucherDate;
        if (!DateTime.TryParse(txtVoucherDate.Text, out voucherDate))
        {
            voucherDate = DateTime.Now;
        }

        string query = @"INSERT INTO GL_FORMS 
                (VOUCHER_KEY, VOUCHER_DATE, VOUCHER_NUMBER, BOOK_TYPE, 
                 GL_FORM_NUMBER, COMP_ID, LOG_ID, POST)
                VALUES 
                (:voucherKey, :voucherDate, :voucherNumber, :bookType,
                 :glFormNumber, :compId, :logId, :post)";

        OracleCommand cmd = new OracleCommand(query, conn);

        cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = lblGJV.Text;
        cmd.Parameters.Add("voucherDate", OracleDbType.Date).Value = voucherDate;

        int voucherNum = Convert.ToInt32(lblVoucherNumber.Text);
        cmd.Parameters.Add("voucherNumber", OracleDbType.Int32).Value = voucherNum;

        string bookType = "GJV";
        cmd.Parameters.Add("bookType", OracleDbType.Varchar2).Value = bookType;

        cmd.Parameters.Add("glFormNumber", OracleDbType.Int32).Value = voucherNum;
        cmd.Parameters.Add("compId", OracleDbType.Int32).Value = Convert.ToInt32(hfCompId.Value);
        cmd.Parameters.Add("logId", OracleDbType.Int32).Value = transactionLogId;
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
        // Fixed book type "GJV" for Journal Voucher
        string bookType = "GJV";
        lblGJV.Text = "1-" + bookType + "-" + lblVoucherNumber.Text;
    }

    private void GenerateNewVoucherNumber()
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT NVL(MAX(GL_FORM_NUMBER), 0) + 1 
                            FROM GL_FORMS";

                OracleCommand cmd = new OracleCommand(query, conn);

                object result = cmd.ExecuteScalar();
                int newVoucherNo = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;

                lblVoucherNumber.Text = newVoucherNo.ToString();

                // Generate voucher key with fixed book type
                string bookType = "GJV";
                lblGJV.Text = "1-" + bookType + "-" + newVoucherNo.ToString();
            }
        }
        catch (Exception ex)
        {
            LogError("GenerateNewVoucherNumber", ex);
            lblVoucherNumber.Text = "1";
            lblGJV.Text = "1-GJV-1";
        }
    }

    protected void ddlBookType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Not used - dropdown is hidden
    }

    protected void btnPost_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "Posted";
        lblStatus.CssClass = "status-posted";
        ShowSnackbar("Voucher marked as posted", "success");
    }

    protected void btnUnposted_Click(object sender, EventArgs e)
    {
        lblStatus.Text = "UnPosted";
        lblStatus.CssClass = "status-unposted";
        ShowSnackbar("Voucher marked as unposted", "info");
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
        int index = keys.IndexOf(lblGJV.Text);

        if (index > 0)
        {
            LoadVoucher(keys[index - 1]);
        }
        else
        {
            ShowSnackbar("This is the first voucher", "info");
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        List<string> keys = GetVoucherKeys();
        int index = keys.IndexOf(lblGJV.Text);

        if (index < keys.Count - 1)
        {
            LoadVoucher(keys[index + 1]);
        }
        else
        {
            ShowSnackbar("This is the last voucher", "info");
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
            ShowSnackbar("Error opening search: " + ex.Message, "error");
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

    #region Debit/Credit Balance Logic

    protected void txtDebit_TextChanged(object sender, EventArgs e)
    {
        TextBox txtDebit = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtDebit.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            decimal debit;

            if (decimal.TryParse(txtDebit.Text, out debit))
            {
                if (dtVoucherDetails.Rows.Count > row.RowIndex)
                {
                    dtVoucherDetails.Rows[row.RowIndex]["DEBIT"] = debit;
                    // Clear credit if debit is entered
                    dtVoucherDetails.Rows[row.RowIndex]["CREDIT"] = 0;

                    // Update the credit textbox in UI
                    TextBox txtCredit = (TextBox)row.FindControl("txtCredit");
                    if (txtCredit != null)
                    {
                        txtCredit.Text = "0.00";
                    }
                }
            }

            ViewState["VoucherDetails"] = dtVoucherDetails;
            CalculateTotals();
        }
    }

    protected void txtCredit_TextChanged(object sender, EventArgs e)
    {
        TextBox txtCredit = (TextBox)sender;
        GridViewRow row = (GridViewRow)txtCredit.NamingContainer;

        if (row.RowType == DataControlRowType.DataRow)
        {
            dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];
            decimal credit;

            if (decimal.TryParse(txtCredit.Text, out credit))
            {
                if (dtVoucherDetails.Rows.Count > row.RowIndex)
                {
                    dtVoucherDetails.Rows[row.RowIndex]["CREDIT"] = credit;
                    // Clear debit if credit is entered
                    dtVoucherDetails.Rows[row.RowIndex]["DEBIT"] = 0;

                    // Update the debit textbox in UI
                    TextBox txtDebit = (TextBox)row.FindControl("txtDebit");
                    if (txtDebit != null)
                    {
                        txtDebit.Text = "0.00";
                    }
                }
            }

            ViewState["VoucherDetails"] = dtVoucherDetails;
            CalculateTotals();
        }
    }

    private void CalculateTotals()
    {
        decimal totalDebit = 0;
        decimal totalCredit = 0;
        dtVoucherDetails = (DataTable)ViewState["VoucherDetails"];

        if (dtVoucherDetails != null)
        {
            foreach (DataRow row in dtVoucherDetails.Rows)
            {
                if (row["DEBIT"] != DBNull.Value)
                {
                    totalDebit += Convert.ToDecimal(row["DEBIT"]);
                }
                if (row["CREDIT"] != DBNull.Value)
                {
                    totalCredit += Convert.ToDecimal(row["CREDIT"]);
                }
            }
        }

        // Store totals in ViewState for validation
        ViewState["TotalDebit"] = totalDebit;
        ViewState["TotalCredit"] = totalCredit;

        // Display both totals
        lblGrandTotal.Text = string.Format("Debit: {0:N2} | Credit: {1:N2} | Difference: {2:N2}",
            totalDebit, totalCredit, totalDebit - totalCredit);

        // Change color based on balance
        if (totalDebit != totalCredit)
        {
            lblGrandTotal.ForeColor = Color.Red;
        }
        else
        {
            lblGrandTotal.ForeColor = Color.Black;
        }

        // Update footer totals if grid is bound
        if (gvVoucherDetails.Rows.Count > 0 && gvVoucherDetails.Rows[gvVoucherDetails.Rows.Count - 1].RowType == DataControlRowType.Footer)
        {
            GridViewRow footer = gvVoucherDetails.Rows[gvVoucherDetails.Rows.Count - 1];
            Label lblTotalDebit = (Label)footer.FindControl("lblTotalDebit");
            Label lblTotalCredit = (Label)footer.FindControl("lblTotalCredit");

            if (lblTotalDebit != null) lblTotalDebit.Text = totalDebit.ToString("N2");
            if (lblTotalCredit != null) lblTotalCredit.Text = totalCredit.ToString("N2");
        }
    }

    private bool ValidateDebitCreditBalance()
    {
        decimal totalDebit = ViewState["TotalDebit"] != null ? Convert.ToDecimal(ViewState["TotalDebit"]) : 0;
        decimal totalCredit = ViewState["TotalCredit"] != null ? Convert.ToDecimal(ViewState["TotalCredit"]) : 0;

        if (totalDebit == 0 && totalCredit == 0)
        {
            ShowSnackbar("Please enter at least one Debit or Credit amount", "warning");
            return false;
        }

        if (totalDebit != totalCredit)
        {
            ShowSnackbar(string.Format("Journal entry is not balanced! Debit: {0:N2}, Credit: {1:N2}, Difference: {2:N2}",
                totalDebit, totalCredit, totalDebit - totalCredit), "error");
            return false;
        }

        return true;
    }

    #endregion

    #region Utility Methods
    private void LogError(string methodName, Exception ex)
    {
        string errorMessage = "Error in " + methodName + ": " + ex.Message + "\n" + ex.StackTrace;
        if (ex.InnerException != null)
        {
            errorMessage += "\nInner Exception: " + ex.InnerException.Message;
        }

        System.Diagnostics.Debug.WriteLine(errorMessage);
    }

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
}