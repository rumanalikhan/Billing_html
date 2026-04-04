using System;
using System.Data;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

public partial class approved_final : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
                        .ConnectionString;

    /* PAGE LOAD */
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["User"] != null)
            {
                string userId = Session["login_id"].ToString();
                string userName = Session["login_name"].ToString();
                string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                string ipAddress = Request.UserHostAddress;

                lblUser.Text = "Current User id: " + userName + " | " + currentDate + "/" + ipAddress;               
            }

            LoadGrid();
            HasFormAccess();
        }
    }

    private void HasFormAccess()
    {
        int rows = 0;
        int userId = 0;
        int mAllowed = 0;
        string formName = "approved_final";

        if (Session["User"] != null)
        {
            int.TryParse(Session["login_id"].ToString(), out userId);
        }

        if (userId == 1 || userId == 23345)
        {
            rows = 1;
            mAllowed = 1;
        }
        else
        {
            string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

            string sql = @"SELECT ALLOWED FROM LOGIN_ACCESS WHERE LOGIN_ID = :userId AND FORM_NAME = :formName";

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add("formName", OracleDbType.Varchar2).Value = formName;

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    mAllowed = Convert.ToInt32(result);
                    rows = 1;
                }
                else
                {
                    rows = 0;
                    mAllowed = 0;
                }
            }
        }

        // 🔴 If user not allowed
        if (rows > 0 || mAllowed > 0)
        {
            foreach (GridViewRow row in gvResults.Rows)   // gvData = your GridView ID
            {
                LinkButton btnApprove = (LinkButton)row.FindControl("btnRowApprove");
                LinkButton btnReject = (LinkButton)row.FindControl("btnRowReject");

                btnApprove.Visible = true;
                btnReject.Visible = true;
            }
        }
        else
        {
            lblStatus.Text = "You are not authorized to access this form. Please go back to the menu.";
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    /* LOAD DATA IN GRIDS */
    private void LoadGrid()
    {
        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand(@"
                    SELECT REFCODE,
                           RESNAME,
                           BILCAT,
                           BILAMNTBDDT,
                           BILAMNTADDT,
                           BILAMNTBDDT_TOBE,
                           BILAMNTADDT_TOBE,
                           REMARKS_APPROVEDBY
                    FROM BIL_MAINT_MODIFYED
                    WHERE NVL(APPROVED_STATUS,0) = 1
                      AND NVL(FINAL_APP_STATUS,0) = 0
                      AND BGID=(SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')", con))
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvResults.DataSource = dt;
                        gvResults.DataBind();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error loading data: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void gvResults_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            GridView gv = (GridView)sender;

            // FIRST ROW (Modification)
            GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

            row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });
            //row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });
            row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });
            row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });

            TableHeaderCell mod = new TableHeaderCell();
            mod.Text = "MODIFICATION";
            mod.ColumnSpan = 4;
            mod.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(mod);

            row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });
            row1.Cells.Add(new TableHeaderCell() { Text = "", RowSpan = 2 });

            gv.Controls[0].Controls.AddAt(0, row1);

            // SECOND ROW (Before / After)
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);

            TableHeaderCell before = new TableHeaderCell();
            before.Text = "BEFORE";
            before.ColumnSpan = 2;
            before.HorizontalAlign = HorizontalAlign.Center;
            row2.Cells.Add(before);

            TableHeaderCell after = new TableHeaderCell();
            after.Text = "AFTER";
            after.ColumnSpan = 2;
            after.HorizontalAlign = HorizontalAlign.Center;
            row2.Cells.Add(after);

            gv.Controls[0].Controls.AddAt(1, row2);
        }
    }

    protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string before = DataBinder.Eval(e.Row.DataItem, "BILAMNTBDDT").ToString();
            string after = DataBinder.Eval(e.Row.DataItem, "BILAMNTADDT").ToString();
            string beforeTobe = DataBinder.Eval(e.Row.DataItem, "BILAMNTBDDT_TOBE").ToString();
            string afterTobe = DataBinder.Eval(e.Row.DataItem, "BILAMNTADDT_TOBE").ToString();

            e.Row.Cells[3].CssClass = "before-value";
            e.Row.Cells[4].CssClass = "before-value";
            e.Row.Cells[5].CssClass = "modified-value";
            e.Row.Cells[6].CssClass = "modified-value";
        }
    }
    
    /* ACTION - APPROVE or REJECT */
    protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "ApproveRow" && e.CommandName != "RejectRow")
            return;

        try
        {           
            // Ensure session exists
            if (Session["login_id"] == null)
            {
                lblStatus.Text = "Session expired. Please login again.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                return;
            }

            //string refCode = e.CommandArgument.ToString();
            string refCode = e.CommandArgument.ToString().Trim();
            //Console.WriteLine(refCode);

            /* REJECT BLOCK */
            if (e.CommandName == "RejectRow")
            {
                // **DO NOT update DB here!** Just show the panel
                lblStatus.Text = "";              // Clear previous status
                pnlRejectRemarks.Visible = true;  // Display remarks text box
                hfRejectRefCode.Value = refCode;  // store the row's refCode
                txtRejectRemarks.Text = "";       // clear previous remarks
            }

            /* APPROVE BLOCK */
            else if (e.CommandName == "ApproveRow")
            {
                pnlRejectRemarks.Visible = false;

                int mResID = 0;
                string sql = @"
                SELECT BA.RES_ID 
                FROM BILL_GENERATE_AMOUNT BA 
                WHERE 
                    BA.RES_ID = (
                        SELECT RB.RES_ID 
                        FROM RESIDENTIAL_BARCODE RB 
                        WHERE RB.BTYPE_ID = 3 
                          AND RB.RES_ID = BA.RES_ID 
                          AND RB.BARCODE = :refCode
                    ) 
                    AND BG_ID = (
                        SELECT BG_ID 
                        FROM BILL_GENERATE 
                        WHERE IS_LOCKED = 'N'
                )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    // ✅ colon removed
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();

                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["RES_ID"] != DBNull.Value)
                                mResID = Convert.ToInt32(dr["RES_ID"]);
                        }
                    }
                }

                string updateSql1 = @"
                    UPDATE BILL_DETAIL T
                    SET T.BILL_AMOUNT =
                    (
                        SELECT NVL(MAX(O.BILLCOST_TOBE),0)
                        FROM BIL_MAINT_MODIFYED O
                        WHERE O.REFCODE = :refCode
                          AND O.RESID = T.RES_ID
                          AND O.VERSION = (
                                SELECT MAX(I.VERSION)
                                FROM BIL_MAINT_MODIFYED I
                                WHERE I.REFCODE = O.REFCODE
                          ) 
                          AND O.BGID = (
                                SELECT A.BG_ID 
                                FROM BILL_GENERATE A 
                                WHERE A.IS_LOCKED='N'
                          )
                    )
                    WHERE 
                        T.RES_ID = :mResID 
                        AND T.COLUMN_ID = 1010 
                        AND T.BG_ID = (
                            SELECT B.BG_ID 
                            FROM BILL_GENERATE B 
                            WHERE B.IS_LOCKED='N'
                        )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.BindByName = true; // ✅ important

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                updateSql1 = @"
                    UPDATE BILL_DETAIL T
                    SET T.BILL_AMOUNT =
                    (
                        SELECT NVL(MAX(O.ADVPAYMNT_TOBE),0)
                        FROM BIL_MAINT_MODIFYED O
                        WHERE O.REFCODE = :refCode
                          AND O.RESID = T.RES_ID
                          AND O.VERSION = (
                                SELECT MAX(I.VERSION)
                                FROM BIL_MAINT_MODIFYED I
                                WHERE I.REFCODE = O.REFCODE
                          ) 
                          AND O.BGID = (
                                SELECT A.BG_ID 
                                FROM BILL_GENERATE A 
                                WHERE A.IS_LOCKED='N'
                          )
                    )
                    WHERE 
                        T.RES_ID = :mResID 
                        AND T.COLUMN_ID = 1004 
                        AND T.BG_ID = (
                            SELECT B.BG_ID 
                            FROM BILL_GENERATE B 
                            WHERE B.IS_LOCKED='N'
                        )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.BindByName = true; // ✅ important

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                updateSql1 = @"
                    UPDATE BILL_DETAIL T
                    SET T.BILL_AMOUNT =
                    (
                        SELECT NVL(MAX(O.ARREARS_TOBE),0)
                        FROM BIL_MAINT_MODIFYED O
                        WHERE O.REFCODE = :refCode
                          AND O.RESID = T.RES_ID
                          AND O.VERSION = (
                                SELECT MAX(I.VERSION)
                                FROM BIL_MAINT_MODIFYED I
                                WHERE I.REFCODE = O.REFCODE
                          ) 
                          AND O.BGID = (
                                SELECT A.BG_ID 
                                FROM BILL_GENERATE A 
                                WHERE A.IS_LOCKED='N'
                          )
                    )
                    WHERE 
                        T.RES_ID = :mResID 
                        AND T.COLUMN_ID = 1002 
                        AND T.BG_ID = (
                            SELECT B.BG_ID 
                            FROM BILL_GENERATE B 
                            WHERE B.IS_LOCKED='N'
                        )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.BindByName = true; // ✅ important

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                updateSql1 = @"
                    UPDATE BILL_DETAIL T
                    SET T.BILL_AMOUNT =
                    (
                        SELECT NVL(MAX(O.INSTAMT_TOBE),0)
                        FROM BIL_MAINT_MODIFYED O
                        WHERE O.REFCODE = :refCode
                          AND O.RESID = T.RES_ID
                          AND O.VERSION = (
                                SELECT MAX(I.VERSION)
                                FROM BIL_MAINT_MODIFYED I
                                WHERE I.REFCODE = O.REFCODE
                          ) 
                          AND O.BGID = (
                                SELECT A.BG_ID 
                                FROM BILL_GENERATE A 
                                WHERE A.IS_LOCKED='N'
                          )
                    )
                    WHERE 
                        T.RES_ID = :mResID 
                        AND T.COLUMN_ID = 1014 
                        AND T.BG_ID = (
                            SELECT B.BG_ID 
                            FROM BILL_GENERATE B 
                            WHERE B.IS_LOCKED='N'
                        )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.BindByName = true; // ✅ important

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                updateSql1 = @"
                    UPDATE BILL_DETAIL T
                    SET T.BILL_AMOUNT =
                    (
                        SELECT NVL(MAX(O.FINECHRGS_TOBE),0)
                        FROM BIL_MAINT_MODIFYED O
                        WHERE O.REFCODE = :refCode
                          AND O.RESID = T.RES_ID
                          AND O.VERSION = (
                                SELECT MAX(I.VERSION)
                                FROM BIL_MAINT_MODIFYED I
                                WHERE I.REFCODE = O.REFCODE
                          ) 
                          AND O.BGID = (
                                SELECT A.BG_ID 
                                FROM BILL_GENERATE A 
                                WHERE A.IS_LOCKED='N'
                          )
                    )
                    WHERE 
                        T.RES_ID = :mResID 
                        AND T.COLUMN_ID = 1022 
                        AND T.BG_ID = (
                            SELECT B.BG_ID 
                            FROM BILL_GENERATE B 
                            WHERE B.IS_LOCKED='N'
                        )
                ";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql1, con))
                {
                    cmd.BindByName = true; // ✅ important

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                //string connStr = WebConfigurationManager.ConnectionStrings["MyDbConnectionMNT"].ConnectionString;

                string updateSql = @"
                UPDATE BILL_GENERATE_AMOUNT T
                SET
                (
                    T.AMNT_WTDATE,
                    T.AMNT_AFDATE
                )
                =
                (
                    SELECT 
                        NVL(MAX(O.BILAMNTBDDT_TOBE),0),
                        NVL(MAX(O.BILAMNTADDT_TOBE),0)
                    FROM BIL_MAINT_MODIFYED O
                    WHERE O.REFCODE = :refCode
                      AND O.RESID = T.RES_ID
                      AND O.VERSION = (
                            SELECT MAX(I.VERSION)
                            FROM BIL_MAINT_MODIFYED I
                            WHERE I.REFCODE = O.REFCODE
                      ) 
                      AND O.BGID = (
                            SELECT A.BG_ID 
                            FROM BILL_GENERATE A 
                            WHERE A.IS_LOCKED='N'
                      )
                )
                WHERE 
                    T.RES_ID = :mResID
                    AND T.BG_ID = (
                        SELECT B.BG_ID 
                        FROM BILL_GENERATE B 
                        WHERE B.IS_LOCKED='N'
                    )";

                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(updateSql, con))
                {
                    cmd.BindByName = true; // ✅ important for named parameters

                    cmd.Parameters.Add("mResID", OracleDbType.Int32).Value = mResID;
                    cmd.Parameters.Add("refCode", OracleDbType.Varchar2).Value = refCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                int userId = 0;
                if (Session["User"] != null)
                {
                    string userName = Session["login_name"].ToString();
                    string currentDate = DateTime.Now.ToString("dd-MMM-yy");
                    string ipAddress = Request.UserHostAddress;
                    int.TryParse(Session["login_id"].ToString(), out userId);

                    updateSql = @"            
                        UPDATE BIL_MAINT_MODIFYED SET 
                            FINAL_APP_STATUS = 1,
                            REMARKS_FINAL_APP = 'APPROVED',
                            FINAL_APPROVALBY = :userId,
                            FINAL_APP_DT = :currentDate,
                            FINAL_APP_IP = :userIp
                        WHERE REFCODE = :refCode AND BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')";

                    using (OracleConnection con = new OracleConnection(connStr))
                    using (OracleCommand cmd = new OracleCommand(updateSql, con))
                    {
                        cmd.Parameters.Add("userId", OracleDbType.Int32).Value = userId;
                        cmd.Parameters.Add(":currentDate", OracleDbType.Varchar2).Value = currentDate;
                        cmd.Parameters.Add(":ipAddress", OracleDbType.Varchar2).Value = ipAddress;
                        cmd.Parameters.Add(":refCode", OracleDbType.Varchar2).Value = refCode;

                        con.Open();
                        int rows = cmd.ExecuteNonQuery();

                        lblStatus.Text = refCode + " - Approved Successfully ";
                        lblStatus.ForeColor = System.Drawing.Color.Green;

                        LoadGrid(); // refresh grid
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    /* BUTTONS */
    protected void btnSubmitReject_Click(object sender, EventArgs e)
    {
        string refCode = hfRejectRefCode.Value;
        string remarks = txtRejectRemarks.Text.Trim();

// *** will open Khalid sb approval *** 
        //if (string.IsNullOrEmpty(remarks))
        //{
        //    lblStatus.Text = "Please enter remarks before submitting rejection.";
        //    lblStatus.ForeColor = System.Drawing.Color.Red;
        //    return;
        //}

        try
        {
            int userId = Convert.ToInt32(Session["login_id"]);
            string userIp = Request.UserHostAddress;
            string currentDate = DateTime.Now.ToString("dd-MMM-yy");

            string updateSql = @"            
                UPDATE BIL_MAINT_MODIFYED SET 
                    FINAL_APP_STATUS = 2,
                    REMARKS_FINAL_APP = :remarks,
                    FINAL_APPROVALBY = :userId,
                    FINAL_APP_DT = :currentDate,
                    FINAL_APP_IP = :userIp
                WHERE REFCODE = :refCode AND BGID = (SELECT BG_ID FROM BILL_GENERATE WHERE IS_LOCKED='N')";

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(updateSql, con))
            {
                cmd.Parameters.Add(":remarks", OracleDbType.Varchar2).Value = remarks;
                cmd.Parameters.Add(":userId", OracleDbType.Int32).Value = userId;
                cmd.Parameters.Add(":currentDate", OracleDbType.Varchar2).Value = currentDate;
                cmd.Parameters.Add(":userIp", OracleDbType.Varchar2).Value = userIp;
                cmd.Parameters.Add(":refCode", OracleDbType.Varchar2).Value = refCode;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblStatus.Text = refCode + " - Rejected Successfully";
            lblStatus.ForeColor = System.Drawing.Color.Red;

            pnlRejectRemarks.Visible = false; // hide panel after submit
            LoadGrid(); // refresh grid
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Error: " + ex.Message;
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }

    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/menu_maintenance.aspx");
    }

    protected void btnLogoff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx");
    }


}