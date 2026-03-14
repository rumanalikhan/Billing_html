using System;
using System.Web.UI;

public partial class menu_water : Page
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

    protected void btnGenerateBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/water_bill_generate.aspx");
    }

    protected void btnUoloadCSV_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/water_reading_upload.aspx");
    }

    protected void btnUpdateBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/water_reading_upload.aspx");
    }

    protected void btnArrears_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/arrears_upload.aspx");
    }

    protected void btnAdvances_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/advance_upload.aspx");
    }

    protected void btnPostingBills_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing/water/water_bill_posting.aspx");
    }
}
