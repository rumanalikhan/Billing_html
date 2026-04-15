using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;

public partial class sub_ledger_report : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["CurrentCompId"] == null)
                Session["CurrentCompId"] = 1;
            if (Session["CurrentLogId"] == null)
                Session["CurrentLogId"] = 0;

            // Set Report Date/Time
            lblReportDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string slCode = txtSLCode.Text.Trim();

        if (string.IsNullOrEmpty(slCode))
        {
            ShowStatus("Please enter Sub Ledger Code", "error");
            return;
        }

        LoadReport(slCode);
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtSLCode.Text = "";
        slInfo.Visible = false;
        gvReport.DataSource = null;
        gvReport.DataBind();
        ShowStatus("Form cleared", "info");
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/main_menu_gl.aspx", false);
    }

    private void LoadReport(string slCode)
    {
        try
        {
            string slName = GetSubLedgerName(slCode);

            if (string.IsNullOrEmpty(slName))
            {
                ShowStatus("Sub Ledger Code not found: " + slCode, "error");
                return;
            }

            lblSLCode.Text = slCode;
            lblSLName.Text = slName;
            lblPrintDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            slInfo.Visible = true;

            DataTable dt = GetSubLedgerData(slCode);

            if (dt.Rows.Count == 0)
            {
                ShowStatus("No transactions found for Sub Ledger: " + slCode, "info");
                gvReport.DataSource = null;
                gvReport.DataBind();
                return;
            }

            //DataTable processedDt = ProcessData(dt);
            DataTable processedDt = ProcessData(dt, slCode);

            gvReport.DataSource = processedDt;
            gvReport.DataBind();

            ShowStatus("Report loaded successfully. Total transactions: " + (processedDt.Rows.Count - 1), "success");
        }
        catch (Exception ex)
        {
            ShowStatus("Error: " + ex.Message, "error");
        }
    }

    private string GetSubLedgerName(string slCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT DESCRIP FROM GL_SL_GLMF WHERE SL_CODE = :slCode";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : "";
        }
    }

    private DataTable GetSubLedgerData(string slCode)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
            SELECT 
                v.SL_CODE,
                s.DESCRIP AS SL_NAME,
                f.VOUCHER_DATE,
                f.VOUCHER_KEY,
                f.VOUCHER_NUMBER,
                v.GL_CODE,
                NVL(g.GL_DESCRP, 'A/R Maintenance – Residential') AS GL_DESCRIPTION,
                NVL(v.NARATION, '') AS PARTICULARS,
                NVL(v.BILL_NUMBER, '') AS BILL_NUMBER,
                NVL(v.CHEQUE_NUMBER, '') AS CHEQUE_NUMBER,
                v.AMOUNT,
                v.DR_CR
            FROM GL_VOUCHERS v
            INNER JOIN GL_FORMS f ON v.VOUCHER_KEY = f.VOUCHER_KEY
            INNER JOIN GL_SL_GLMF s ON v.SL_CODE = s.SL_CODE
            LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
            WHERE v.SL_CODE = :slCode
            ORDER BY f.VOUCHER_DATE, f.VOUCHER_NUMBER";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
        }

        return dt;
    }

    //private DataTable ProcessData(DataTable dt)
    //{
    //    DataTable result = new DataTable();
    //    result.Columns.Add("TRANS_DATE", typeof(string));
    //    result.Columns.Add("VOUCHER_KEY", typeof(string));
    //    result.Columns.Add("VOUCHER_NUMBER", typeof(int));
    //    result.Columns.Add("GL_CODE", typeof(string));
    //    result.Columns.Add("GL_DESCRIPTION", typeof(string));
    //    result.Columns.Add("PARTICULARS", typeof(string));
    //    result.Columns.Add("BILL_NUMBER", typeof(string));
    //    result.Columns.Add("CHEQUE_NUMBER", typeof(string));
    //    result.Columns.Add("DEBIT", typeof(decimal));
    //    result.Columns.Add("CREDIT", typeof(decimal));
    //    result.Columns.Add("RUNNING_BALANCE", typeof(decimal));

    //    decimal runningBalance = 0;
    //    decimal totalDebit = 0;
    //    decimal totalCredit = 0;

    //    foreach (DataRow row in dt.Rows)
    //    {
    //        DataRow newRow = result.NewRow();

    //        newRow["TRANS_DATE"] = Convert.ToDateTime(row["VOUCHER_DATE"]).ToString("dd-MM-yyyy");
    //        newRow["VOUCHER_KEY"] = row["VOUCHER_KEY"];
    //        newRow["VOUCHER_NUMBER"] = row["VOUCHER_NUMBER"];
    //        newRow["GL_CODE"] = row["GL_CODE"];
    //        newRow["GL_DESCRIPTION"] = row["GL_DESCRIPTION"];
    //        newRow["PARTICULARS"] = row["PARTICULARS"];
    //        newRow["BILL_NUMBER"] = row["BILL_NUMBER"];
    //        newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"];

    //        string drcr = row["DR_CR"].ToString().ToUpper();
    //        decimal amount = Convert.ToDecimal(row["AMOUNT"]);

    //        if (drcr == "2" || drcr == "D")
    //        {
    //            newRow["DEBIT"] = amount;
    //            newRow["CREDIT"] = 0;
    //            totalDebit += amount;
    //            runningBalance += amount;
    //        }
    //        else
    //        {
    //            newRow["DEBIT"] = 0;
    //            newRow["CREDIT"] = amount;
    //            totalCredit += amount;
    //            runningBalance -= amount;
    //        }

    //        newRow["RUNNING_BALANCE"] = runningBalance;

    //        result.Rows.Add(newRow);
    //    }

    //    // Add Total Row
    //    DataRow totalRow = result.NewRow();
    //    totalRow["TRANS_DATE"] = "";
    //    totalRow["VOUCHER_KEY"] = "";
    //    totalRow["VOUCHER_NUMBER"] = 0;
    //    totalRow["GL_CODE"] = "";
    //    totalRow["GL_DESCRIPTION"] = "";
    //    totalRow["PARTICULARS"] = "TOTAL";
    //    totalRow["BILL_NUMBER"] = "";
    //    totalRow["CHEQUE_NUMBER"] = "";
    //    totalRow["DEBIT"] = totalDebit;
    //    totalRow["CREDIT"] = totalCredit;
    //    totalRow["RUNNING_BALANCE"] = runningBalance;
    //    result.Rows.Add(totalRow);

    //    return result;
    //}

    private DataTable ProcessData(DataTable dt, string slCode)
    {
        DataTable result = new DataTable();
        result.Columns.Add("TRANS_DATE", typeof(string));
        result.Columns.Add("VOUCHER_KEY", typeof(string));
        result.Columns.Add("VOUCHER_NUMBER", typeof(int));
        result.Columns.Add("GL_CODE", typeof(string));
        result.Columns.Add("GL_DESCRIPTION", typeof(string));
        result.Columns.Add("PARTICULARS", typeof(string));
        result.Columns.Add("BILL_NUMBER", typeof(string));
        result.Columns.Add("CHEQUE_NUMBER", typeof(string));
        result.Columns.Add("DEBIT", typeof(decimal));
        result.Columns.Add("CREDIT", typeof(decimal));
        result.Columns.Add("RUNNING_BALANCE", typeof(decimal));

        // Get Opening Balance
        decimal openingBalance = GetOpeningBalance(slCode);

        // Add Opening Balance as first row
        DataRow openingRow = result.NewRow();
        openingRow["TRANS_DATE"] = "";
        openingRow["VOUCHER_KEY"] = "";
        openingRow["VOUCHER_NUMBER"] = 0;
        openingRow["GL_CODE"] = "";
        openingRow["GL_DESCRIPTION"] = "OPENING BALANCE";
        openingRow["PARTICULARS"] = "";
        openingRow["BILL_NUMBER"] = "";
        openingRow["CHEQUE_NUMBER"] = "";
        openingRow["DEBIT"] = 0;
        openingRow["CREDIT"] = 0;
        openingRow["RUNNING_BALANCE"] = Math.Abs(openingBalance);
        result.Rows.Add(openingRow);

        decimal runningBalance = openingBalance;
        decimal totalDebit = openingBalance >= 0 ? openingBalance : 0;
        decimal totalCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0;

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = result.NewRow();

            newRow["TRANS_DATE"] = Convert.ToDateTime(row["VOUCHER_DATE"]).ToString("dd-MM-yyyy");
            newRow["VOUCHER_KEY"] = row["VOUCHER_KEY"];
            newRow["VOUCHER_NUMBER"] = row["VOUCHER_NUMBER"];
            newRow["GL_CODE"] = row["GL_CODE"];
            newRow["GL_DESCRIPTION"] = row["GL_DESCRIPTION"];
            newRow["PARTICULARS"] = row["PARTICULARS"];
            newRow["BILL_NUMBER"] = row["BILL_NUMBER"];
            newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"];

            string drcr = row["DR_CR"].ToString().ToUpper();
            decimal amount = Convert.ToDecimal(row["AMOUNT"]);

            if (drcr == "2" || drcr == "D")
            {
                newRow["DEBIT"] = amount;
                newRow["CREDIT"] = 0;
                totalDebit += amount;
                runningBalance += amount;
            }
            else
            {
                newRow["DEBIT"] = 0;
                newRow["CREDIT"] = amount;
                totalCredit += amount;
                runningBalance -= amount;
            }

            newRow["RUNNING_BALANCE"] = runningBalance;

            result.Rows.Add(newRow);
        }

        // Add Total Row
        DataRow totalRow = result.NewRow();
        totalRow["TRANS_DATE"] = "";
        totalRow["VOUCHER_KEY"] = "";
        totalRow["VOUCHER_NUMBER"] = 0;
        totalRow["GL_CODE"] = "";
        totalRow["GL_DESCRIPTION"] = "";
        totalRow["PARTICULARS"] = "TOTAL";
        totalRow["BILL_NUMBER"] = "";
        totalRow["CHEQUE_NUMBER"] = "";
        totalRow["DEBIT"] = totalDebit;
        totalRow["CREDIT"] = totalCredit;
        totalRow["RUNNING_BALANCE"] = runningBalance;
        result.Rows.Add(totalRow);

        return result;
    }

    private decimal GetOpeningBalance(string slCode)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT NVL(OPENING_BALANCE, 0) FROM GL_SL_OPENING_BALANCE WHERE SL_CODE = :slCode AND COMP_ID = :compId";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("slCode", OracleDbType.Varchar2).Value = slCode;
            cmd.Parameters.Add("compId", OracleDbType.Int32).Value = 1;
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }

    //protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {
    //        DataRowView row = (DataRowView)e.Row.DataItem;

    //        string particulars = row["PARTICULARS"].ToString();
    //        bool isTotalRow = (particulars == "TOTAL");

    //        if (isTotalRow)
    //        {
    //            e.Row.CssClass = "total-row";

    //            for (int i = 0; i < e.Row.Cells.Count; i++)
    //            {
    //                if (i != 5)
    //                    e.Row.Cells[i].Text = "";
    //            }
    //            e.Row.Cells[5].Text = "TOTAL";
    //            e.Row.Cells[5].Font.Bold = true;

    //            e.Row.Cells[8].Text = Convert.ToDecimal(row["DEBIT"]).ToString("N2");
    //            e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;
    //            e.Row.Cells[9].Text = Convert.ToDecimal(row["CREDIT"]).ToString("N2");
    //            e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;

    //            decimal closingBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
    //            string closingDrCr = closingBalance >= 0 ? "DR" : "CR";
    //            e.Row.Cells[10].Text = Math.Abs(closingBalance).ToString("N2") + " " + closingDrCr;
    //            e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;

    //            if (closingDrCr == "CR")
    //            {
    //                e.Row.Cells[10].ForeColor = Color.Red;
    //            }
    //            return;
    //        }

    //        Label lblBalance = (Label)e.Row.FindControl("lblBalance");
    //        if (lblBalance != null)
    //        {
    //            decimal balance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
    //            string drCr = balance >= 0 ? "DR" : "CR";
    //            lblBalance.Text = Math.Abs(balance).ToString("N2") + " " + drCr;

    //            if (drCr == "CR")
    //            {
    //                lblBalance.ForeColor = Color.Red;
    //                lblBalance.Font.Bold = true;
    //            }
    //        }
    //    }
    //}

    protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRowView row = (DataRowView)e.Row.DataItem;

            string glDescription = row["GL_DESCRIPTION"].ToString();
            bool isOpeningRow = (glDescription == "OPENING BALANCE");
            bool isTotalRow = (row["PARTICULARS"].ToString() == "TOTAL");

            if (isOpeningRow)
            {
                e.Row.CssClass = "total-row";
                // Clear Debit and Credit cells
                e.Row.Cells[8].Text = "";
                e.Row.Cells[9].Text = "";
                decimal balance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                e.Row.Cells[10].Text = balance.ToString("N2");
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;
                return;
            }

            if (isTotalRow)
            {
                e.Row.CssClass = "total-row";
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i != 5)
                        e.Row.Cells[i].Text = "";
                }
                e.Row.Cells[5].Text = "TOTAL";
                e.Row.Cells[5].Font.Bold = true;
                e.Row.Cells[8].Text = Convert.ToDecimal(row["DEBIT"]).ToString("N2");
                e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[9].Text = Convert.ToDecimal(row["CREDIT"]).ToString("N2");
                e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[10].Text = Convert.ToDecimal(row["RUNNING_BALANCE"]).ToString("N2");
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Right;
                return;
            }

            // Format Running Balance for regular rows
            Label lblBalance = (Label)e.Row.FindControl("lblBalance");
            if (lblBalance != null)
            {
                decimal balance = Convert.ToDecimal(row["RUNNING_BALANCE"]);
                lblBalance.Text = balance.ToString("N2");
            }
        }
    }

    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string slCode = txtSLCode.Text.Trim();

            if (string.IsNullOrEmpty(slCode))
            {
                ShowStatus("Please enter Sub Ledger Code", "error");
                return;
            }

            string slName = GetSubLedgerName(slCode);

            if (string.IsNullOrEmpty(slName))
            {
                ShowStatus("Sub Ledger Code not found: " + slCode, "error");
                return;
            }

            DataTable dt = GetSubLedgerData(slCode);

            if (dt.Rows.Count == 0)
            {
                ShowStatus("No transactions found for Sub Ledger: " + slCode, "info");
                return;
            }

            DataTable processedDt = ProcessDataForExcel(dt, slCode, slName);

            // Get Pakistan time
            TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=SubLedger_" + slCode + "_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
            Response.Charset = "";

            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Write HTML header
            hw.Write("<html><head><meta charset='UTF-8'><title>Sub Ledger Report</title>");
            hw.Write("<style>");
            hw.Write("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
            hw.Write(".company-header { text-align: center; margin-bottom: 20px; }");
            hw.Write(".company-name { font-size: 20px; font-weight: bold; color: #0f7c57; }");
            hw.Write(".company-sub { font-size: 12px; color: #555; }");
            hw.Write(".report-title { text-align: center; font-size: 16px; font-weight: bold; text-decoration: underline; margin: 15px 0; }");
            hw.Write(".sl-info { background: #e8f0fe; padding: 10px; margin-bottom: 15px; border-left: 3px solid #0f7c57; font-size: 12px; }");
            hw.Write(".report-table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            hw.Write(".report-table th { background: #0f7c57; color: white; padding: 10px; border: 1px solid #0a5e42; text-align: left; }");
            hw.Write(".report-table td { padding: 8px; border: 1px solid #ddd; }");
            hw.Write(".amount-column { text-align: right; }");
            hw.Write(".total-row { background: #dcdcdc; font-weight: bold; }");
            hw.Write(".footer { margin-top: 20px; text-align: center; font-size: 10px; color: #999; border-top: 1px solid #ddd; padding-top: 10px; }");
            hw.Write("</style></head><body>");

            // Company Header
            hw.Write("<div class='company-header'>");
            hw.Write("<div class='company-name'>BAHRIA TOWN KARACHI</div>");
            hw.Write("<div class='company-sub'>GL ACCOUNTING SYSTEM</div>");
            hw.Write("</div>");

            // Report Title
            hw.Write("<div class='report-title'>SUB LEDGER REPORT</div>");

            // Sub Ledger Info
            hw.Write("<div class='sl-info'>");
            hw.Write("<strong>Sub Ledger:</strong> " + slCode + " - " + slName + "<br />");
            hw.Write("<strong>Printed On:</strong> " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt"));
            hw.Write("</div>");

            // Build table
            hw.Write("<table class='report-table'>");
            hw.Write("<thead>");
            hw.Write("<tr>");
            hw.Write("<th>DATE</th>");
            hw.Write("<th>VOUCHER KEY</th>");
            hw.Write("<th>VOUCHER NO</th>");
            hw.Write("<th>GL CODE</th>");
            hw.Write("<th>GL DESCRIPTION</th>");
            hw.Write("<th>PARTICULARS</th>");
            hw.Write("<th>BILL NO</th>");
            hw.Write("<th>CHEQUE NO</th>");
            hw.Write("<th>DEBIT</th>");
            hw.Write("<th>CREDIT</th>");
            hw.Write("<th>RUNNING BALANCE</th>");
            hw.Write("</tr>");
            hw.Write("</thead>");
            hw.Write("<tbody>");

            foreach (DataRow row in processedDt.Rows)
            {
                string transDate = row["TRANS_DATE"].ToString();
                string voucherKey = row["VOUCHER_KEY"].ToString();
                string voucherNumber = row["VOUCHER_NUMBER"].ToString();
                string glCode = row["GL_CODE"].ToString();
                string glDescription = row["GL_DESCRIPTION"].ToString();
                string particulars = row["PARTICULARS"].ToString();
                string billNumber = row["BILL_NUMBER"].ToString();
                string chequeNumber = row["CHEQUE_NUMBER"].ToString();
                decimal debit = Convert.ToDecimal(row["DEBIT"]);
                decimal credit = Convert.ToDecimal(row["CREDIT"]);
                decimal runningBalance = Convert.ToDecimal(row["RUNNING_BALANCE"]);

                string runningBalanceText = "";
                if (glDescription == "OPENING BALANCE")
                {
                    runningBalanceText = Math.Abs(runningBalance).ToString("N2");
                }
                else if (particulars == "TOTAL")
                {
                    runningBalanceText = Math.Abs(runningBalance).ToString("N2");
                }
                else
                {
                    string drCr = runningBalance >= 0 ? "DR" : "CR";
                    runningBalanceText = Math.Abs(runningBalance).ToString("N2") + " " + drCr;
                }

                hw.Write("<tr>");
                hw.Write("<td>" + transDate + "</td>");
                hw.Write("<td>" + voucherKey + "</td>");
                hw.Write("<td>" + voucherNumber + "</td>");
                hw.Write("<td>" + glCode + "</td>");
                hw.Write("<td>" + glDescription + "</td>");
                hw.Write("<td>" + particulars + "</td>");
                hw.Write("<td>" + billNumber + "</td>");
                hw.Write("<td>" + chequeNumber + "</td>");
                hw.Write("<td class='amount-column'>" + (debit == 0 ? "" : debit.ToString("N2")) + "</td>");
                hw.Write("<td class='amount-column'>" + (credit == 0 ? "" : credit.ToString("N2")) + "</td>");

                if (glDescription == "OPENING BALANCE" || particulars == "TOTAL")
                {
                    hw.Write("<td class='amount-column'>" + runningBalanceText + "</td>");
                }
                else if (runningBalance < 0)
                {
                    hw.Write("<td class='amount-column' style='color:red;font-weight:bold;'>" + runningBalanceText + "</td>");
                }
                else
                {
                    hw.Write("<td class='amount-column'>" + runningBalanceText + "</td>");
                }
                hw.Write("</tr>");
            }

            hw.Write("</tbody>");
            hw.Write("</table>");

            // Footer
            hw.Write("<div class='footer'>");
            hw.Write("This is a computer generated document - No signature required");
            hw.Write("</div>");

            hw.Write("</body></html>");

            Response.Write(sw.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            ShowStatus("Error exporting to Excel: " + ex.Message, "error");
        }
    }

    private DataTable ProcessDataForExcel(DataTable dt, string slCode, string slName)
    {
        DataTable result = new DataTable();
        result.Columns.Add("TRANS_DATE", typeof(string));
        result.Columns.Add("VOUCHER_KEY", typeof(string));
        result.Columns.Add("VOUCHER_NUMBER", typeof(string));
        result.Columns.Add("GL_CODE", typeof(string));
        result.Columns.Add("GL_DESCRIPTION", typeof(string));
        result.Columns.Add("PARTICULARS", typeof(string));
        result.Columns.Add("BILL_NUMBER", typeof(string));
        result.Columns.Add("CHEQUE_NUMBER", typeof(string));
        result.Columns.Add("DEBIT", typeof(decimal));
        result.Columns.Add("CREDIT", typeof(decimal));
        result.Columns.Add("RUNNING_BALANCE", typeof(decimal));

        // Get Opening Balance
        decimal openingBalance = GetOpeningBalance(slCode);

        // Add Opening Balance as first row
        DataRow openingRow = result.NewRow();
        openingRow["TRANS_DATE"] = "";
        openingRow["VOUCHER_KEY"] = "";
        openingRow["VOUCHER_NUMBER"] = "";
        openingRow["GL_CODE"] = "";
        openingRow["GL_DESCRIPTION"] = "OPENING BALANCE";
        openingRow["PARTICULARS"] = "";
        openingRow["BILL_NUMBER"] = "";
        openingRow["CHEQUE_NUMBER"] = "";
        openingRow["DEBIT"] = 0;
        openingRow["CREDIT"] = 0;
        openingRow["RUNNING_BALANCE"] = openingBalance;
        result.Rows.Add(openingRow);

        decimal runningBalance = openingBalance;
        decimal totalDebit = openingBalance >= 0 ? openingBalance : 0;
        decimal totalCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0;

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = result.NewRow();

            newRow["TRANS_DATE"] = Convert.ToDateTime(row["VOUCHER_DATE"]).ToString("dd-MM-yyyy");
            newRow["VOUCHER_KEY"] = row["VOUCHER_KEY"];
            newRow["VOUCHER_NUMBER"] = row["VOUCHER_NUMBER"].ToString();
            newRow["GL_CODE"] = row["GL_CODE"];
            newRow["GL_DESCRIPTION"] = row["GL_DESCRIPTION"];
            newRow["PARTICULARS"] = row["PARTICULARS"];
            newRow["BILL_NUMBER"] = row["BILL_NUMBER"];
            newRow["CHEQUE_NUMBER"] = row["CHEQUE_NUMBER"];

            string drcr = row["DR_CR"].ToString().ToUpper();
            decimal amount = Convert.ToDecimal(row["AMOUNT"]);

            if (drcr == "2" || drcr == "D")
            {
                newRow["DEBIT"] = amount;
                newRow["CREDIT"] = 0;
                totalDebit += amount;
                runningBalance += amount;
            }
            else
            {
                newRow["DEBIT"] = 0;
                newRow["CREDIT"] = amount;
                totalCredit += amount;
                runningBalance -= amount;
            }

            newRow["RUNNING_BALANCE"] = runningBalance;
            result.Rows.Add(newRow);
        }

        // Add Total Row
        DataRow totalRow = result.NewRow();
        totalRow["TRANS_DATE"] = "";
        totalRow["VOUCHER_KEY"] = "";
        totalRow["VOUCHER_NUMBER"] = "";
        totalRow["GL_CODE"] = "";
        totalRow["GL_DESCRIPTION"] = "";
        totalRow["PARTICULARS"] = "TOTAL";
        totalRow["BILL_NUMBER"] = "";
        totalRow["CHEQUE_NUMBER"] = "";
        totalRow["DEBIT"] = totalDebit;
        totalRow["CREDIT"] = totalCredit;
        totalRow["RUNNING_BALANCE"] = runningBalance;
        result.Rows.Add(totalRow);

        return result;
    }

    private void ShowStatus(string message, string type)
    {
        lblStatus.Text = message;
        if (type == "success")
            lblStatus.ForeColor = Color.Green;
        else if (type == "error")
            lblStatus.ForeColor = Color.Red;
        else
            lblStatus.ForeColor = Color.Blue;
    }
}