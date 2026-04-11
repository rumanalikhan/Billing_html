using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

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

    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.close();</script>");
    }
}