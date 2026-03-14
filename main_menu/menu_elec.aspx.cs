using System;
using System.Web.UI;

public partial class menu_elec : Page
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

    //  *** Column-1 *** 
    protected void btnBillGenerateTobe_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/bill_generate_tobe.aspx");
    }

    protected void btnUoloadCSVElec_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/elec_reading_upload.aspx");
    }

    protected void btnUoloadCSVSolr_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/solr_reading_upload.aspx");
    }

    protected void btnArrearsUpdate_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/arrears_upload.aspx");
    }

    protected void btnAdvancesBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/advance_upload.aspx");
    }

    protected void btnDcCharges_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/dc_charges.aspx");
    }

    // *** Column-2 *** 
    protected void btnGenerateBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/elec_bill_generate.aspx");
    }

    protected void btnUpdateBillsElec_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/update_elec.aspx");
    }

    protected void btnUpdateBillsSolr_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/update_solr.aspx");
    }

    //  *** Column-3 *** 
    protected void btnApprovedChanges_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/elec/approved_changes.aspx");
    }

    protected void btnPostingBills_Click(object sender, EventArgs e)
    {
       Response.Redirect("~/frm/billing/elec/elec_bill_posting.aspx");
    }
}
