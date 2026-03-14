using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class VoucherLine
{
    public int RowID { get; set; }
    public string GLCode { get; set; }
    public string AccountName { get; set; }
    public string Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}
public partial class gl_vouch : System.Web.UI.Page
{
    private int editingRowIndex = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Initialize the page
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtVoucherNo.Text = GenerateVoucherNumber();
            
            // Initialize voucher entries list in session
            if (Session["VoucherEntries"] == null)
            {
                Session["VoucherEntries"] = new List<VoucherLine>();
            }
            
            // Bind the grid
            BindVoucherGrid();
        }
    }

    private string GenerateVoucherNumber()
    {
        // Simple voucher number generation
        // In real app, this would come from database
        return "VCH-" + DateTime.Now.ToString("yyyyMMdd") + "-001";
    }

    private void BindVoucherGrid()
    {
        List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];
        
        // Add row numbers if needed
        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].RowID = i + 1;
        }
        
        gvVoucherEntries.DataSource = entries;
        gvVoucherEntries.DataBind();
        
        // Update totals
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];
        
        decimal totalDebit = 0;
        decimal totalCredit = 0;
        
        foreach (var line in entries)
        {
            totalDebit += line.Debit;
            totalCredit += line.Credit;
        }
        
        lblTotalDebit.Text = totalDebit.ToString("N2");
        lblTotalCredit.Text = totalCredit.ToString("N2");
        
        decimal difference = totalDebit - totalCredit;
        lblDifference.Text = difference.ToString("N2");
        
        // Color code the difference
        if (difference == 0)
        {
            lblDifference.ForeColor = System.Drawing.Color.Green;
        }
        else
        {
            lblDifference.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void gvVoucherEntries_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // Set the row being edited
        gvVoucherEntries.EditIndex = e.NewEditIndex;
        editingRowIndex = e.NewEditIndex;

        // Rebind the grid to show edit mode
        BindVoucherGrid();
    }

    protected void gvVoucherEntries_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        // Get the row being updated
        GridViewRow row = gvVoucherEntries.Rows[e.RowIndex];

        // Get the values from edit controls
        string glCode = ((DropDownList)row.FindControl("ddlEditAccount")).SelectedValue;
        string accountName = ((DropDownList)row.FindControl("ddlEditAccount")).SelectedItem.Text;
        string description = ((TextBox)row.FindControl("txtEditDescription")).Text;
        string debitText = ((TextBox)row.FindControl("txtEditDebit")).Text;
        string creditText = ((TextBox)row.FindControl("txtEditCredit")).Text;

        // Parse amounts
        decimal debit = 0;
        decimal credit = 0;
        decimal.TryParse(debitText, out debit);
        decimal.TryParse(creditText, out credit);

        // Validation
        if (debit > 0 && credit > 0)
        {
            ShowMessage("Cannot have both Debit and Credit in same line", "error");
            return;
        }

        // Update the entry in session
        List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];
        int rowIndex = e.RowIndex;

        if (rowIndex >= 0 && rowIndex < entries.Count)
        {
            entries[rowIndex].GLCode = glCode;
            entries[rowIndex].AccountName = accountName;
            entries[rowIndex].Description = description;
            entries[rowIndex].Debit = debit;
            entries[rowIndex].Credit = credit;
        }

        Session["VoucherEntries"] = entries;

        // Exit edit mode
        gvVoucherEntries.EditIndex = -1;
        editingRowIndex = -1;

        // Rebind the grid
        BindVoucherGrid();

        ShowMessage("Line updated", "success");
    }

    protected void gvVoucherEntries_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // Exit edit mode without saving
        gvVoucherEntries.EditIndex = -1;
        editingRowIndex = -1;

        // Rebind the grid
        BindVoucherGrid();

        ShowMessage("Edit cancelled", "info");
    }

    protected void gvVoucherEntries_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        // Get the row index to delete
        int rowIndex = e.RowIndex;

        List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];

        if (rowIndex >= 0 && rowIndex < entries.Count)
        {
            entries.RemoveAt(rowIndex);
            Session["VoucherEntries"] = entries;

            // If we're deleting the row that was being edited, exit edit mode
            if (rowIndex == editingRowIndex)
            {
                gvVoucherEntries.EditIndex = -1;
                editingRowIndex = -1;
            }
            // If we're deleting a row above the edit row, adjust edit index
            else if (rowIndex < editingRowIndex)
            {
                editingRowIndex--;
                gvVoucherEntries.EditIndex = editingRowIndex;
            }

            BindVoucherGrid();
            ShowMessage("Line deleted", "info");
        }
    }

    protected void gvVoucherEntries_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "AddNew")
        {
            // Get the footer row
            GridViewRow footerRow = gvVoucherEntries.FooterRow;

            // Get values from footer controls
            string glCode = ((TextBox)footerRow.FindControl("txtNewGLCode")).Text;
            DropDownList ddlAccount = (DropDownList)footerRow.FindControl("ddlNewAccount");
            string accountName = ddlAccount.SelectedItem.Text;
            string accountValue = ddlAccount.SelectedValue;
            string description = ((TextBox)footerRow.FindControl("txtNewDescription")).Text;
            string debitText = ((TextBox)footerRow.FindControl("txtNewDebit")).Text;
            string creditText = ((TextBox)footerRow.FindControl("txtNewCredit")).Text;

            // Use dropdown if GL code not entered manually
            if (string.IsNullOrEmpty(glCode) && !string.IsNullOrEmpty(accountValue))
            {
                glCode = accountValue;
            }

            // Parse amounts
            decimal debit = 0;
            decimal credit = 0;

            decimal.TryParse(debitText, out debit);
            decimal.TryParse(creditText, out credit);

            // Validation
            if (string.IsNullOrEmpty(glCode))
            {
                ShowMessage("Please select or enter a GL Code", "error");
                return;
            }

            if (debit == 0 && credit == 0)
            {
                ShowMessage("Please enter either Debit or Credit amount", "error");
                return;
            }

            if (debit > 0 && credit > 0)
            {
                ShowMessage("Cannot have both Debit and Credit in same line", "error");
                return;
            }

            // Create new line
            VoucherLine newLine = new VoucherLine
            {
                GLCode = glCode,
                AccountName = accountName,
                Description = description,
                Debit = debit,
                Credit = credit
            };

            // Add to session list
            List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];
            entries.Add(newLine);
            Session["VoucherEntries"] = entries;

            // Rebind grid
            BindVoucherGrid();

            ShowMessage("Line added", "success");
        }
    }

    protected void btnAddSample_Click(object sender, EventArgs e)
    {
        // Add sample voucher lines for testing
        List<VoucherLine> entries = new List<VoucherLine>
        {
            new VoucherLine { GLCode = "1010", AccountName = "Cash Account", Description = "Office supplies purchase", Debit = 0, Credit = 500 },
            new VoucherLine { GLCode = "5020", AccountName = "Rent Expense", Description = "Office supplies purchase", Debit = 500, Credit = 0 }
        };
        
        Session["VoucherEntries"] = entries;
        BindVoucherGrid();
        
        ShowMessage("Sample data loaded. Note: Debits = Credits (500)", "success");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            List<VoucherLine> entries = (List<VoucherLine>)Session["VoucherEntries"];
            
            // Validate
            if (entries.Count == 0)
            {
                ShowMessage("Cannot save empty voucher", "error");
                return;
            }
            
            // Check if debits equal credits
            decimal totalDebit = 0;
            decimal totalCredit = 0;
            
            foreach (var line in entries)
            {
                totalDebit += line.Debit;
                totalCredit += line.Credit;
            }
            
            if (totalDebit != totalCredit)
            {
                ShowMessage("Cannot save: Debits ({totalDebit:N2}) do not equal Credits ({totalCredit:N2})", "error");
                return;
            }
            
            // Here you would save to database
            // For demo, we'll just show what would be saved
            
            string message = "Voucher saved successfully!<br/><br/>";
            message += "Voucher No: {txtVoucherNo.Text}<br/>";
            message += "Date: {txtDate.Text}<br/>";
            message += "Type: {ddlVoucherType.SelectedItem.Text}<br/>";
            message += "Narration: {txtNarration.Text}<br/>";
            message += "Total Debits: {totalDebit:N2}<br/>";
            message += "Total Credits: {totalCredit:N2}<br/>";
            message += "Number of lines: {entries.Count}";
            
            ShowMessage(message, "success");
            
            // Optional: Clear after save
            // btnClear_Click(sender, e);
        }
        catch (Exception ex)
        {
            ShowMessage("Error: " + ex.Message, "error");
        }
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        // Clear all entries
        Session["VoucherEntries"] = new List<VoucherLine>();
        BindVoucherGrid();
        
        // Clear header fields (optional)
        txtNarration.Text = "";
        txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        txtVoucherNo.Text = GenerateVoucherNumber();
        
        ShowMessage("Form cleared", "info");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Clear and redirect or just clear
        Session["VoucherEntries"] = null;
        Response.Redirect("main_menu/main_menu_gl.aspx");
    }

    private void ShowMessage(string message, string type)
    {
        lblMessage.Text = message;
        lblMessage.CssClass = "message-box " + type;
        lblMessage.Visible = true;
    }
}
