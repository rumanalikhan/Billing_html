using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class coa : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadLevel2Data();
            LoadLevel3Data();
            LoadLevel4Data();
        }
    }

    private void LoadLevel2Data()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Code");
        dt.Columns.Add("ParentCode");
        dt.Columns.Add("Description");

        // Sample data from PDF
        dt.Rows.Add("2021", "3", "Current Assets");
        dt.Rows.Add("2022", "3", "Fixed Assets");

        gvLevel2.DataSource = dt;
        gvLevel2.DataBind();
    }

    private void LoadLevel3Data()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Code");
        dt.Columns.Add("ParentCode");
        dt.Columns.Add("Description");

        // Add sample Level-3 data
        dt.Rows.Add("301", "2021", "Cash and Bank");
        dt.Rows.Add("302", "2021", "Accounts Receivable");
        dt.Rows.Add("401", "2022", "Property");
        dt.Rows.Add("402", "2022", "Equipment");

        gvLevel3.DataSource = dt;
        gvLevel3.DataBind();
    }

    private void LoadLevel4Data()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Code");
        dt.Columns.Add("ParentCode");
        dt.Columns.Add("Description");

        // Add sample Level-4 data
        dt.Rows.Add("30101", "301", "Cash in Hand");
        dt.Rows.Add("30102", "301", "Bank Account");
        dt.Rows.Add("30201", "302", "Customer A");
        dt.Rows.Add("30202", "302", "Customer B");

        gvLevel4.DataSource = dt;
        gvLevel4.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            // Save logic here
            string accountCode = "£100:0001";
            string openingBalance = txtOpeningBalance.Text;

            // Get selected Level-1 item
            string selectedLevel1 = rblLevel1.SelectedValue;

            // Process selected accounts from all levels
            ProcessSelectedAccounts(gvLevel2, "Level-2");
            ProcessSelectedAccounts(gvLevel3, "Level-3");
            ProcessSelectedAccounts(gvLevel4, "Level-4");

            // Show success message
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Chart of Accounts saved successfully!');", true);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error saving data: {ex.Message}');", true);
        }
    }

    private void ProcessSelectedAccounts(GridView gridView, string level)
    {
        foreach (GridViewRow row in gridView.Rows)
        {
            CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
            if (chkSelect != null && chkSelect.Checked)
            {
                string code = row.Cells[1].Text; // Code column
                string parentCode = row.Cells[2].Text; // Parent Code column
                string description = row.Cells[3].Text; // Description column

                // Process the selected account here
                // You can save to database or perform other operations
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Clear all selections
        rblLevel1.ClearSelection();
        txtOpeningBalance.Text = "";

        // Reload data
        LoadLevel2Data();
        LoadLevel3Data();
        LoadLevel4Data();

        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Form has been reset.');", true);
    }
}