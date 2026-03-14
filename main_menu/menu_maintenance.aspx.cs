using System;
using System.Web.UI;

public partial class menu_maintenance : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // lblUser.Text = Session["USER_NAME"]?.ToString();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/main_menu_bs.aspx");
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login.aspx");
    }

    protected void btnResidentialInfo_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/resid_info.aspx");
    }

    protected void btnBarcodeMaintenance_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/maintenance/barcode_maintenance.aspx");
    }

    protected void btnBarcodeElectric_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/maintenance/barcode_electric.aspx");
    }

    protected void btnBarcodeWater_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/maintenance/barcode_water.aspx");
    }

    protected void btnBarcodeGas_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/maintenance/barcode_gas.aspx");
    }

    protected void btnGenerateBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/maint_bill_generate.aspx");
    }

    protected void btnUpdateBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/maintenance/update_bills.aspx");
    }

    protected void btnUploadArrears_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/arrears_upload.aspx");
    }

    protected void btnUploadAdvances_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/advance_upload.aspx");
    }

    protected void btnBillGenerateTobe_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/bill_generate_tobe.aspx");
    }

    protected void btnPostingBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/maint/maint_bill_posting.aspx");
    }
}
