using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class main_menu_main_menu_gl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set user label text if user is logged in
            if (Session["Username"] != null)
            {
                lblUser.Text = "Welcome, " + Session["Username"].ToString();
            }
            else
            {
                lblUser.Text = "Welcome, Guest";
            }
        }
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        // Clear session and redirect to login page
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login.aspx");
    }

    protected void back_btn_Click(object sender, EventArgs e)
    {
        // Redirect to previous page or main menu
        if (Request.UrlReferrer != null)
        {
            Response.Redirect(Request.UrlReferrer.ToString());
        }
        else
        {
            Response.Redirect("~/main_menu.aspx");
        }
    }

    // Menu item click handlers for GL system
    protected void SetupMenuItem_Click(object sender, EventArgs e)
    {
        // You can add specific logic for each menu item here
        string menuItem = ((LinkButton)sender).ID;
        
        switch (menuItem)
        {
            case "btnChartOfAccounts":
                Response.Redirect("~/gl/chart_of_accounts.aspx");
                break;
            case "btnSubLedger":
                Response.Redirect("~/gl/chart_of_accounts_sub.aspx");
                break;
            case "btnBooksType":
                Response.Redirect("~/gl/books_type.aspx");
                break;
            case "btnCostCenter":
                Response.Redirect("~/gl/cost_center.aspx");
                break;
        }
    }

    protected void lnkCOA_Click(object sender, EventArgs e)
    {
        Response.Redirect("coa.aspx");
    }
    protected void TransactionMenuItem_Click(object sender, EventArgs e)
    {
        string menuItem = ((LinkButton)sender).ID;
        
        switch (menuItem)
        {
            case "btnBankReceiveables":
                Response.Redirect("~/gl/bank_receiveables.aspx");
                break;
            case "btnBankPayables":
                Response.Redirect("~/gl/bank_payables.aspx");
                break;
            case "btnCashReceiveables":
                Response.Redirect("~/gl/cash_receiveables.aspx");
                break;
            case "btnCashPayables":
                Response.Redirect("~/gl/cash_payables.aspx");
                break;
            case "btnJournalVoucher":
                Response.Redirect("~/gl/journal_voucher.aspx");
                break;
        }
    }

    protected void ReportingMenuItem_Click(object sender, EventArgs e)
    {
        string menuItem = ((LinkButton)sender).ID;
        
        switch (menuItem)
        {
            case "btnAuditTrial":
                Response.Redirect("~/gl/audit_trial.aspx");
                break;
            case "btnJournalGeneral":
                Response.Redirect("~/gl/journal_general.aspx");
                break;
            case "btnJournalGeneralSub":
                Response.Redirect("~/gl/journal_general_sub.aspx");
                break;
            case "btnTrialBalance":
                Response.Redirect("~/gl/trial_balance.aspx");
                break;
            case "btnTrialBalanceSub":
                Response.Redirect("~/gl/trial_balance_sub.aspx");
                break;
        }
    }
}
