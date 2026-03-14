using System;
using System.Web.UI;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;

public partial class bill_generate_tobe : Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadNextBGID();

            //txtCompID.Text = "1";
            //txtIsLocked.Text = "N";
            txtGenerateDate.Text = DateTime.Now.ToString("MM-dd-yyyy");


            lblStatus.Text = ""; 
            if (Session["User"] != null) 
            { 
                string currentDate = DateTime.Now.ToString("dd-MMM-yy"); 
                lblUser.Text = "Welcome: " + Session["User"].ToString() + " | " + currentDate; 
            } 
            else 
            { 
                Response.Redirect("~/login/login.aspx"); 
            }
        }

        txtGenerateDate.Focus();
        //pcdInit();
    }

    protected void btnBack_Click(object sender, EventArgs e) 
    { 
        Response.Redirect("~/main_menu/menu_elec.aspx"); 
    } 
    
    protected void btnLogoff_Click(object sender, EventArgs e) 
    { 
        Session.Clear(); 
        Session.Abandon(); 
        Response.Redirect("~/login.aspx"); 
    }

    private void LoadNextBGID()
    {
        string sql = @"SELECT (BG_ID + 1) FROM BILL_GENERATE WHERE BTYPE_ID = 1 AND IS_LOCKED = 'N'";
        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            if (result != null)
                txtBGID.Text = result.ToString();
        }
    }
    
    protected void btnPost_Click(object sender, EventArgs e)
    {
        try
        {
            // BG_ID Validation
            if (string.IsNullOrWhiteSpace(txtBGID.Text))
            {
                lblStatus.Text = "BG ID is missing. Please click [Fetch ID] and try [Post] again.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // 1️. Parse dates safely
            // Dates Validation
            DateTime issueDate, dueDate;

            if (!DateTime.TryParseExact(txtIssueDate.Text, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out issueDate)
             ||
                !DateTime.TryParseExact(txtDueDate.Text, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dueDate))
            {
                lblStatus.Text = "Invalid Issue or Due Date.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // 2️. Validation
            if (dueDate <= issueDate)
            {
                lblStatus.Text = "Due Date must be greater than Issue Date.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }
            
            // ----------------

            // 3️. Set Date Reading (UI)
            // DT_READING = ISSUE_DATE (display format)
            //DateTime dateReading = issueDate;

            // Valid Date = last day of Issue month
            DateTime validDate = new DateTime(issueDate.Year, issueDate.Month,
                                              DateTime.DaysInMonth(issueDate.Year, issueDate.Month));

            // UI display (MM-dd-yyyy)
            //txtDateReading.Text = dateReading.ToString("MM-dd-yyyy");
            //txtDateReading.Text = issueDate;
            txtValidDate.Text = validDate.ToString("MM-dd-yyyy");

            // Server-side BILL_MONTH calculation (failsafe)
            var prevMonth = issueDate.AddMonths(-1);
            txtBillMonth.Text = prevMonth.ToString("yyyyMM");

            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                new OracleCommand("TRUNCATE TABLE BILL_GENERATE_TOBE", con).ExecuteNonQuery();
                // GENERATE_BY, GENERATE_IP, GENERATE_PC
                string sql = @"
                    INSERT INTO BILL_GENERATE_TOBE
                    (
                        BG_ID, BTYPE_ID, BG_NAME, DT_GENERATE,
                        GENERATE_BY, GENERATE_IP, GENERATE_PC,
                        DT_ISSUE, DUE_DATE, DT_READING,
                        BILL_MONTH, COMP_ID, IS_LOCKED,
                        VALID_DATE, ISSUE_DATE
                    )
                    VALUES
                    (
                        :BG_ID, 1, :BG_NAME, SYSDATE,
                        :GENERATE_BY, :GENERATE_IP, :GENERATE_PC,
                        :DT_ISSUE, :DUE_DATE, :DT_READING,
                        :BILL_MONTH, 1, 'N',
                        :VALID_DATE, :ISSUE_DATE
                    )";


                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add("BG_ID", txtBGID.Text);
                    cmd.Parameters.Add("BG_NAME", txtBGName.Text);

                    cmd.Parameters.Add("GENERATE_BY", Session["User"].ToString());
                    cmd.Parameters.Add("GENERATE_IP", Request.UserHostAddress);
                    cmd.Parameters.Add("GENERATE_PC", Environment.MachineName);

                    cmd.Parameters.Add("DT_ISSUE", OracleDbType.Date).Value = issueDate;
                    cmd.Parameters.Add("DUE_DATE", OracleDbType.Date).Value = dueDate;

                    cmd.Parameters.Add("DT_READING", OracleDbType.Date).Value = issueDate;

                    cmd.Parameters.Add("BILL_MONTH", txtBillMonth.Text);

                    cmd.Parameters.Add("VALID_DATE", OracleDbType.Date).Value = validDate;
                    cmd.Parameters.Add("ISSUE_DATE", OracleDbType.Date).Value = issueDate;
                    cmd.ExecuteNonQuery();
                }
            }

            lblStatus.Text = "Bill Generate To-Be created successfully";
            lblStatus.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lblStatus.Text = ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pcdInit();
        txtGenerateDate.Focus();

        lblStatus.Text = "Data Cleared successfully";
        lblStatus.ForeColor = System.Drawing.Color.Orange;

        // Reload BG_ID and DT_GENERATE
        LoadNextBGID();
        txtGenerateDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
    }

    private void pcdInit()
    {
        txtBGID.Text = "";
        txtBGName.Text="";
        txtBillMonth.Text = "";
        txtIssueDate.Text = "";
        txtDueDate.Text = "";
        txtValidDate.Text = "";
        txtGenerateDate.Text = "";
    }

   
}