using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class getstart : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        // Redirect to Login page
        Response.Redirect("~/login/Login.aspx");
    }

    protected void btnComplains_Click(object sender, EventArgs e)
    {
        // Redirect to Complains page
        Response.Redirect("~/complains/Complains.aspx");
    }

}