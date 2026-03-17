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

    // Setup Menu Item Click Handler
    protected void SetupMenuItem_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string buttonId = btn.ID;

        switch (buttonId)
        {
            case "lnkCOA":
                Response.Redirect("~/frm/gl/setup/chart_of_accounts.aspx");
                break;
            case "btnSubLedger":
                Response.Redirect("~/frm/gl/setup/coa_sub_ledger.aspx");
                break;
            case "btnBooksType":  
                Response.Redirect("~/frm/books_type.aspx");
                break;
            case "btnCostCenter": 
                Response.Redirect("~/frm/cost_center.aspx");
                break;
            default:
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Menu item not configured: " + buttonId + "');", true);
                break;
        }
    }

    // Transaction Menu Item Click Handler
    protected void TransactionMenuItem_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string buttonId = btn.ID;

        switch (buttonId)
        {
            case "btnBankReceiveables":
                Response.Redirect("~/frm/gl/transactions/gl_receipt_voucher.aspx");
                break;
            case "btnBankPayables":
                Response.Redirect("~/frm/gl/transactions/gl_payment_voucher.aspx");
                break;
            case "btnCashReceiveables":
                Response.Redirect("~/frm/gl/transactions/gl_cash_receipt_voucher.aspx");
                break;
            case "btnCashPayables":
                Response.Redirect("~/frm/gl/transactions/gl_cash_payment_voucher.aspx");
                break;
            case "btnJournalVoucher":
                Response.Redirect("~/frm/gl/transactions/gl_journal_voucher.aspx");
                break;
            default:
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Transaction menu item not configured: " + buttonId + "');", true);
                break;
        }
    }

    // Reporting Menu Item Click Handler
    protected void ReportingMenuItem_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string buttonId = btn.ID;

        switch (buttonId)
        {
            case "btnAuditTrial":
                Response.Redirect("~/frm/gl/audit_trial.aspx");
                break;
            case "btnJournalGeneral":
                Response.Redirect("~/frm/gl/journal_general.aspx");
                break;
            case "btnJournalGeneralSub":
                Response.Redirect("~/frm/gl/journal_general_sub.aspx");
                break;
            case "btnTrialBalance":
                Response.Redirect("~/frm/gl/trial_balance.aspx");
                break;
            case "btnTrialBalanceSub":
                Response.Redirect("~/frm/gl/trial_balance_sub.aspx");
                break;
            default:
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    "alert('Reporting menu item not configured: " + buttonId + "');", true);
                break;
        }
    }
}