using System;
using System.Web.UI;

public partial class main_menu_bs : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Optional: user label
            // lblUser.Text = Session["USER_NAME"]?.ToString();
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        // Go back to main menu
        Response.Redirect("~/main_menu/main_menu.aspx");
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login.aspx");
    }

    protected void btnMaintenance_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/menu_maintenance.aspx");
    }

    protected void btnElectric_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/menu_elec.aspx");
    }

    protected void btnWater_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/menu_water.aspx");
    }

    protected void btnGas_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/gas/default.aspx");
    }

    protected void btnRental_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/rental/default.aspx");
    }

    protected void btnBNB_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/billing_html/bnb/default.aspx");
    }

    protected void btnGenSummary_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/billing_summaries.aspx");
    }

    protected void btnCollectionUpdate_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frm/collection_update.aspx");
    }
}
