using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class main_menu : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null)
            {
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");

                // Client IP Address
                string ipAddress = Request.UserHostAddress;

                lblUser.Text = "Welcome " + userName + " | " + currentDate + "/" + ipAddress;
            }
            else
            {
                Response.Redirect("~/login/login.aspx");
            }
        }
    }

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!IsPostBack)
    //    {
    //        if (Session["User"] != null)
    //        {
    //            string currentDate = DateTime.Now.ToString("dd-MMM-yy");
    //            lblUser.Text = "Welcome: " + Session["User"].ToString() + " | " + currentDate; 
    //        }
    //        else
    //        {
    //            Response.Redirect("~/login/login.aspx");
    //        }
    //    }
    //}

    protected void btnUsers_Click(object sender, EventArgs e)
    {
    }

    protected void btnmain_menu_gl_Click(object sender, EventArgs e)
    {
        // Navigate to Meter Management page
        Response.Redirect("~/main_menu/main_menu_gl.aspx");
    }

    protected void btnmain_menu_bs_Click(object sender, EventArgs e)
    {
        // Navigate to Meter Management page
        Response.Redirect("~/main_menu/main_menu_bs.aspx");
    }

    //protected void btnMeterMgmt_Click(object sender, EventArgs e)
    //{
    //}

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx");
    }
    
    
}
