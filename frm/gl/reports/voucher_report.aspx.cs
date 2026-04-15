using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI;

public partial class voucher_report : System.Web.UI.Page
{
    private string connectionString = ConfigurationManager.ConnectionStrings["BackOfficeConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["VoucherKey"] != null)
            {
                string voucherKey = Request.QueryString["VoucherKey"];
                hfVoucherKey.Value = voucherKey;

                // Get voucher type - check both parameter names
                string voucherType = Request.QueryString["VoucherType"];
                if (string.IsNullOrEmpty(voucherType))
                {
                    voucherType = Request.QueryString["BookType"];
                }
                if (string.IsNullOrEmpty(voucherType))
                {
                    // Extract from voucher key (e.g., "1-CPV-100")
                    string[] parts = voucherKey.Split('-');
                    if (parts.Length >= 2)
                    {
                        voucherType = parts[1];
                    }
                    else
                    {
                        voucherType = "GJV";
                    }
                }

                // Set the voucher title based on type
                SetVoucherTitle(voucherType);

                // Load header data
                LoadVoucherHeader(voucherKey);

                // Load details data
                LoadVoucherDetails(voucherKey);

                lblPrintDate.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
            }
            else
            {
                Response.Write("<script>alert('No voucher selected');window.close();</script>");
            }
        }
    }

    private void SetVoucherTitle(string voucherType)
    {
        switch (voucherType)
        {
            case "CPV":
                lblVoucherTitle.Text = "CASH PAYMENT VOUCHER";
                break;
            case "CRV":
                lblVoucherTitle.Text = "CASH RECEIPT VOUCHER";
                break;
            case "GPV":
                lblVoucherTitle.Text = "GENERAL PAYMENT VOUCHER";
                break;
            case "GRV":
                lblVoucherTitle.Text = "GENERAL RECEIPT VOUCHER";
                break;
            case "GJV":
                lblVoucherTitle.Text = "JOURNAL VOUCHER";
                break;
            default:
                lblVoucherTitle.Text = voucherType + " VOUCHER";
                break;
        }
    }

    private void LoadVoucherHeader(string voucherKey)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
                SELECT 
                    VOUCHER_NUMBER,
                    VOUCHER_DATE,
                    POST,
                    VOUCHER_KEY
                FROM GL_FORMS
                WHERE VOUCHER_KEY = :voucherKey";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;

            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                lblVoucherNumber.Text = reader["VOUCHER_NUMBER"].ToString();
                lblVoucherKey.Text = reader["VOUCHER_KEY"].ToString();

                DateTime voucherDate = Convert.ToDateTime(reader["VOUCHER_DATE"]);
                lblVoucherDate.Text = voucherDate.ToString("dd/MM/yyyy");

                int post = Convert.ToInt32(reader["POST"]);
                if (post == 1)
                {
                    lblStatus.Text = "Posted";
                    lblStatus.CssClass = "status-posted";
                }
                else
                {
                    lblStatus.Text = "Unposted";
                    lblStatus.CssClass = "status-unposted";
                }
            }
            reader.Close();
        }
    }

    private void LoadVoucherDetails(string voucherKey)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
            SELECT 
                v.LINE_NUMBER,
                v.GL_CODE,
                CASE 
                    WHEN NVL(g.GL_DESCRP, '') != '' THEN g.GL_DESCRP
                    ELSE NVL(v.NARATION, '')
                END AS GL_DESCRIPTION,
                v.DR_CR,
                v.AMOUNT
            FROM GL_VOUCHERS v
            LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
            WHERE v.VOUCHER_KEY = :voucherKey
            ORDER BY v.LINE_NUMBER";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
        }

        // Process data to separate Debit and Credit
        DataTable processedDt = new DataTable();
        processedDt.Columns.Add("LINE_NUMBER", typeof(int));
        processedDt.Columns.Add("GL_CODE", typeof(string));
        processedDt.Columns.Add("GL_DESCRIPTION", typeof(string));
        processedDt.Columns.Add("DEBIT", typeof(decimal));
        processedDt.Columns.Add("CREDIT", typeof(decimal));

        decimal totalDebit = 0;
        decimal totalCredit = 0;

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = processedDt.NewRow();
            newRow["LINE_NUMBER"] = row["LINE_NUMBER"];
            newRow["GL_CODE"] = row["GL_CODE"];
            newRow["GL_DESCRIPTION"] = row["GL_DESCRIPTION"];

            string drcr = row["DR_CR"].ToString();
            decimal amount = Convert.ToDecimal(row["AMOUNT"]);

            if (drcr == "2" || drcr == "D")
            {
                newRow["DEBIT"] = amount;
                newRow["CREDIT"] = 0;
                totalDebit += amount;
            }
            else
            {
                newRow["DEBIT"] = 0;
                newRow["CREDIT"] = amount;
                totalCredit += amount;
            }

            processedDt.Rows.Add(newRow);
        }

        // Bind to GridView
        gvDetails.DataSource = processedDt;
        gvDetails.DataBind();

        // Set footer totals
        if (gvDetails.Rows.Count > 0 && gvDetails.FooterRow != null)
        {
            Label lblTotalDebit = (Label)gvDetails.FooterRow.FindControl("lblTotalDebit");
            Label lblTotalCredit = (Label)gvDetails.FooterRow.FindControl("lblTotalCredit");

            if (lblTotalDebit != null)
                lblTotalDebit.Text = totalDebit.ToString("N2");
            if (lblTotalCredit != null)
                lblTotalCredit.Text = totalCredit.ToString("N2");
        }

        // Set amount in words
        decimal totalAmount = totalDebit > totalCredit ? totalDebit : totalCredit;
        lblAmountInWords.Text = NumberToWords(totalAmount);
    }    
    private string NumberToWords(decimal number)
    {
        if (number == 0)
            return "ZERO ONLY";

        long rupees = (long)number;
        int paisa = (int)((number - rupees) * 100);

        string words = ConvertToWords(rupees) + " ONLY";

        if (paisa > 0)
        {
            words = ConvertToWords(rupees) + " AND " + ConvertToWords(paisa) + " PAISA ONLY";
        }

        return words.ToUpper();
    }

    private string ConvertToWords(long number)
    {
        if (number == 0)
            return "ZERO";

        string[] ones = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE",
                          "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN",
                          "SEVENTEEN", "EIGHTEEN", "NINETEEN" };

        string[] tens = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

        string[] thousands = { "", "THOUSAND", "MILLION", "BILLION" };

        string words = "";
        int i = 0;

        while (number > 0)
        {
            int chunk = (int)(number % 1000);
            if (chunk > 0)
            {
                string chunkWords = "";

                int hundreds = chunk / 100;
                int rest = chunk % 100;

                if (hundreds > 0)
                {
                    chunkWords += ones[hundreds] + " HUNDRED ";
                }

                if (rest >= 20)
                {
                    chunkWords += tens[rest / 10] + " " + ones[rest % 10] + " ";
                }
                else if (rest > 0)
                {
                    chunkWords += ones[rest] + " ";
                }

                words = chunkWords.Trim() + " " + thousands[i] + " " + words;
            }
            number /= 1000;
            i++;
        }

        return words.Trim();
    }

    protected void btnExportExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string voucherKey = hfVoucherKey.Value;

            // Get voucher header data
            DataTable headerData = GetVoucherHeaderData(voucherKey);

            // Get voucher details
            DataTable dt = GetVoucherDetailsData(voucherKey);

            // Process details to separate Debit and Credit
            DataTable processedDt = new DataTable();
            processedDt.Columns.Add("LINE_NUMBER", typeof(int));
            processedDt.Columns.Add("GL_CODE", typeof(string));
            processedDt.Columns.Add("GL_DESCRIPTION", typeof(string));
            processedDt.Columns.Add("DEBIT", typeof(decimal));
            processedDt.Columns.Add("CREDIT", typeof(decimal));

            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = processedDt.NewRow();
                newRow["LINE_NUMBER"] = row["LINE_NUMBER"];
                newRow["GL_CODE"] = row["GL_CODE"];
                newRow["GL_DESCRIPTION"] = row["GL_DESCRIPTION"];

                string drcr = row["DR_CR"].ToString();
                decimal amount = Convert.ToDecimal(row["AMOUNT"]);

                if (drcr == "2" || drcr == "D")
                {
                    newRow["DEBIT"] = amount;
                    newRow["CREDIT"] = 0;
                    totalDebit += amount;
                }
                else
                {
                    newRow["DEBIT"] = 0;
                    newRow["CREDIT"] = amount;
                    totalCredit += amount;
                }

                processedDt.Rows.Add(newRow);
            }

            // Get voucher header info
            string voucherNumber = "";
            string voucherDate = "";
            string voucherKeyValue = "";
            string status = "";

            if (headerData.Rows.Count > 0)
            {
                voucherNumber = headerData.Rows[0]["VOUCHER_NUMBER"].ToString();
                voucherDate = Convert.ToDateTime(headerData.Rows[0]["VOUCHER_DATE"]).ToString("dd/MM/yyyy");
                voucherKeyValue = headerData.Rows[0]["VOUCHER_KEY"].ToString();
                int post = Convert.ToInt32(headerData.Rows[0]["POST"]);
                status = post == 1 ? "Posted" : "Unposted";
            }

            // Get voucher title
            string voucherType = Request.QueryString["VoucherType"];
            if (string.IsNullOrEmpty(voucherType))
            {
                voucherType = Request.QueryString["BookType"];
            }
            if (string.IsNullOrEmpty(voucherType))
            {
                string[] parts = voucherKey.Split('-');
                if (parts.Length >= 2)
                    voucherType = parts[1];
                else
                    voucherType = "GJV";
            }

            string voucherTitle = GetVoucherTitleText(voucherType);

            // Get Pakistan time
            TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=Voucher_" + voucherNumber + "_" + pakistanTime.ToString("yyyyMMdd_HHmmss") + ".xls");
            Response.Charset = "";

            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Write HTML header
            hw.Write("<html><head><meta charset='UTF-8'><title>Voucher Report</title>");
            hw.Write("<style>");
            hw.Write("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; }");
            hw.Write(".company-header { text-align: center; border-bottom: 2px solid #0f7c57; padding-bottom: 15px; margin-bottom: 20px; }");
            hw.Write(".company-name { font-size: 18px; font-weight: bold; color: #0f7c57; }");
            hw.Write(".voucher-title { text-align: center; font-size: 16px; font-weight: bold; text-decoration: underline; margin: 15px 0; }");
            hw.Write(".voucher-info { width: 100%; margin: 15px 0; border-collapse: collapse; }");
            hw.Write(".voucher-info td { padding: 8px; border: 1px solid #ddd; }");
            hw.Write(".voucher-info td:first-child { width: 150px; font-weight: bold; background: #f5f5f5; }");
            hw.Write(".details-table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            hw.Write(".details-table th { background: #0f7c57; color: white; padding: 10px; border: 1px solid #0a5e40; }");
            hw.Write(".details-table td { padding: 8px; border: 1px solid #ddd; }");
            hw.Write(".amount-column { text-align: right; }");
            hw.Write(".total-row { background: #e6e6e6; font-weight: bold; }");
            hw.Write(".amount-words { margin: 20px 0; padding: 10px; background: #f5f5f5; border-left: 3px solid #0f7c57; }");
            hw.Write(".signature { margin-top: 30px; display: flex; justify-content: space-between; }");
            hw.Write(".signature-line { text-align: center; width: 200px; }");
            hw.Write(".signature-line hr { margin: 30px 0 5px; }");
            hw.Write(".footer { margin-top: 20px; text-align: center; font-size: 10px; color: #999; border-top: 1px solid #ddd; padding-top: 10px; }");
            hw.Write(".status-posted { color: green; font-weight: bold; }");
            hw.Write(".status-unposted { color: red; font-weight: bold; }");
            hw.Write("</style></head><body>");

            // Company Header
            hw.Write("<div class='company-header'>");
            hw.Write("<div class='company-name'>BAHRIA TOWN KARACHI</div>");
            hw.Write("</div>");

            // Voucher Title
            hw.Write("<div class='voucher-title'>" + voucherTitle + "</div>");

            // Voucher Info Table
            hw.Write("<table class='voucher-info'>");
            hw.Write("<tr><td>Voucher Number:</td><td><strong>" + voucherNumber + "</strong></td><td>Voucher Date:</td><td>" + voucherDate + "</td></tr>");
            hw.Write("<tr><td>Voucher Key:</td><td>" + voucherKeyValue + "</td><td>Status:</td><td class='" + (status == "Posted" ? "status-posted" : "status-unposted") + "'>" + status + "</td></tr>");
            hw.Write("</table>");

            // Details Table
            hw.Write("<table class='details-table'>");
            hw.Write("<thead><tr><th>S.No</th><th>GL Code</th><th>Account Description / Particulars</th><th>Debit</th><th>Credit</th></tr></thead>");
            hw.Write("<tbody>");

            int serialNo = 1;
            foreach (DataRow row in processedDt.Rows)
            {
                hw.Write("<tr>");
                hw.Write("<td>" + serialNo++ + "</td>");
                hw.Write("<td>" + row["GL_CODE"].ToString() + "</td>");
                hw.Write("<td>" + row["GL_DESCRIPTION"].ToString() + "</td>");
                hw.Write("<td class='amount-column'>" + (Convert.ToDecimal(row["DEBIT"]) == 0 ? "" : Convert.ToDecimal(row["DEBIT"]).ToString("N2")) + "</td>");
                hw.Write("<td class='amount-column'>" + (Convert.ToDecimal(row["CREDIT"]) == 0 ? "" : Convert.ToDecimal(row["CREDIT"]).ToString("N2")) + "</td>");
                hw.Write("</tr>");
            }

            // Total Row
            hw.Write("<tr class='total-row'>");
            hw.Write("<td colspan='3'><strong>TOTAL</strong></td>");
            hw.Write("<td class='amount-column'><strong>" + totalDebit.ToString("N2") + "</strong></td>");
            hw.Write("<td class='amount-column'><strong>" + totalCredit.ToString("N2") + "</strong></td>");
            hw.Write("</tr>");

            hw.Write("</tbody>");
            hw.Write("</table>");

            // Amount in Words
            decimal totalAmount = totalDebit > totalCredit ? totalDebit : totalCredit;
            hw.Write("<div class='amount-words'>");
            hw.Write("<strong>Amount in Words:</strong> " + NumberToWords(totalAmount));
            hw.Write("</div>");

            // Signature Section
            hw.Write("<div class='signature'>");
            hw.Write("<div class='signature-line'><hr /><span>Prepared By</span></div>");
            hw.Write("<div class='signature-line'><hr /><span>Checked By</span></div>");
            hw.Write("<div class='signature-line'><hr /><span>Authorized By</span></div>");
            hw.Write("</div>");

            // Footer
            hw.Write("<div class='footer'>");
            hw.Write("This is a computer generated document - No signature required<br />");
            hw.Write("Printed on: " + pakistanTime.ToString("dd/MM/yyyy hh:mm tt"));
            hw.Write("</div>");

            hw.Write("</body></html>");

            Response.Write(sw.ToString());
            Response.End();
        }
        catch (Exception ex)
        {
            Response.Write("<script>alert('Error exporting to Excel: " + ex.Message + "');</script>");
        }
    }

    private DataTable GetVoucherHeaderData(string voucherKey)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
            SELECT 
                VOUCHER_NUMBER,
                VOUCHER_DATE,
                POST,
                VOUCHER_KEY
            FROM GL_FORMS
            WHERE VOUCHER_KEY = :voucherKey";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
        }

        return dt;
    }

    private DataTable GetVoucherDetailsData(string voucherKey)
    {
        DataTable dt = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = @"
            SELECT 
                v.LINE_NUMBER,
                v.GL_CODE,
                CASE 
                    WHEN NVL(g.GL_DESCRP, '') != '' THEN g.GL_DESCRP
                    ELSE NVL(v.NARATION, '')
                END AS GL_DESCRIPTION,
                v.DR_CR,
                v.AMOUNT
            FROM GL_VOUCHERS v
            LEFT JOIN GL_GLMF g ON v.GL_CODE = g.GL_CODE
            WHERE v.VOUCHER_KEY = :voucherKey
            ORDER BY v.LINE_NUMBER";

            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add("voucherKey", OracleDbType.Varchar2).Value = voucherKey;

            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            da.Fill(dt);
        }

        return dt;
    }

    private string GetVoucherTitleText(string voucherType)
    {
        switch (voucherType)
        {
            case "CPV":
                return "CASH PAYMENT VOUCHER";
            case "CRV":
                return "CASH RECEIPT VOUCHER";
            case "GPV":
                return "GENERAL PAYMENT VOUCHER";
            case "GRV":
                return "GENERAL RECEIPT VOUCHER";
            case "GJV":
                return "JOURNAL VOUCHER";
            default:
                return voucherType + " VOUCHER";
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.close();</script>");
    }
}