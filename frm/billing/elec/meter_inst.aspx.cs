using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI.WebControls;
using System.Data;

public partial class meter_inst : System.Web.UI.Page
{
    string connMain = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    private bool msg = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ClearForm();
        }
    }

    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/main_menu/menu_elec.aspx");
    }

    protected void btnLogOff_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/login/Login.aspx");
    }

    protected void ClearForm()
    {
        txtSearchBarcode.Text = "";
        txtPrevOutstanding.Text = "0";
        txtDefaultCharges.Text = "0";
        txtInstRate.Text = "0";
        txtInstallments.Text = "0";
        txtMeterCost.Text = "62000";

        rb3Months.Checked = false;
        rb6Months.Checked = false;

        hdnExistingSrno.Value = "";
        hdnBarcode.Value = "";
        hdnResid.Value = "";
        hdnBillMonth.Value = "";

        lblConsumerName.Text = "-";
        lblResid.Text = "-";
        lblAddress.Text = "-";
        lblPrecinct.Text = "-";
        lblBlock.Text = "-";
        lblMeterNo.Text = "-";
        lblBillMonth.Text = "-";

        txtSearchBarcode.Focus();

        // statusMessage.Style["display"] = "none";
    }

    protected void CalculateFields()
    {
        decimal prevOutstanding = 0;
        decimal.TryParse(txtPrevOutstanding.Text, out prevOutstanding);

        decimal defaultCharges = prevOutstanding * 0.10m;
        txtDefaultCharges.Text = defaultCharges.ToString("0");

        int installments = 0;
        int instRate = 0;

        int.TryParse(txtInstallments.Text, out installments);
        int.TryParse(txtInstRate.Text, out instRate);

        if (installments == 0 || instRate == 0)
        {
            if (rb3Months.Checked)
            {
                txtInstRate.Text = "21000";
                txtInstallments.Text = "3";
            }
            else if (rb6Months.Checked)
            {
                txtInstRate.Text = "11000";
                txtInstallments.Text = "6";
            }
        }
    }
    
    protected void txtPrevOutstanding_TextChanged(object sender, EventArgs e)
    {
        CalculateFields();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        // Clear previous message
        statusMessage.Style["display"] = "none";
        statusMessage.InnerHtml = "";
        SearchConsumer();
    }

    protected void txtSearchBarcode_TextChanged(object sender, EventArgs e)
    {
        // Clear previous message
        statusMessage.Style["display"] = "none";
        statusMessage.InnerHtml = "";
        SearchConsumer();
    }

protected void SearchConsumer()
{
    string barcode = txtSearchBarcode.Text.Trim();

    if (string.IsNullOrEmpty(barcode))
    {
        if (!msg)
            ShowStatusMessage("Please enter Barcode/Reference Code", "error");
        return;
    }

    try
    {
        string billMonth = "";
        string getBillMonthSql = "SELECT BILL_MONTH FROM BILL_GENERATE WHERE IS_LOCKED = 'N' AND ROWNUM = 1";
        using (OracleConnection con = new OracleConnection(connMain))
        using (OracleCommand cmd = new OracleCommand(getBillMonthSql, con))
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                billMonth = result.ToString();
                lblBillMonth.Text = billMonth;
                hdnBillMonth.Value = billMonth;
            }
            else
            {
                if (!msg)
                    ShowStatusMessage("No active bill generation period found.", "error");
                return;
            }
        }

        string sql = @"
            SELECT REFCODE, RESID, RESNAME, ADDRESS, PRECENT_NM, BLOCK_NM, METERNO
            FROM BILLS.BIL_ELEC 
            WHERE REFCODE = :barcode AND ROWNUM = 1";

        using (OracleConnection con = new OracleConnection(connMain))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            cmd.Parameters.Add(new OracleParameter("barcode", barcode));
            con.Open();

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    hdnBarcode.Value = reader["REFCODE"].ToString();
                    hdnResid.Value = reader["RESID"].ToString();

                    lblConsumerName.Text = reader["RESNAME"] != DBNull.Value ? reader["RESNAME"].ToString() : "-";
                    lblResid.Text = reader["RESID"].ToString();
                    lblAddress.Text = reader["ADDRESS"] != DBNull.Value ? reader["ADDRESS"].ToString() : "-";
                    lblPrecinct.Text = reader["PRECENT_NM"] != DBNull.Value ? reader["PRECENT_NM"].ToString() : "-";
                    lblBlock.Text = reader["BLOCK_NM"] != DBNull.Value ? reader["BLOCK_NM"].ToString() : "-";
                    lblMeterNo.Text = reader["METERNO"] != DBNull.Value ? reader["METERNO"].ToString() : "-";

                    LoadExistingPlan(hdnBarcode.Value);
                    CalculateFields();
                    
                    // Only show success message if not suppressed
                    if (!msg)
                        ShowStatusMessage("Consumer found successfully!", "success");
                }
                else
                {
                    if (!msg)
                        ShowStatusMessage("Consumer not found with this Barcode/Reference Code", "error");
                    ClearForm();
                }
            }
        }
    }
    catch (Exception ex)
    {
        if (!msg)
            ShowStatusMessage("Error: " + ex.Message, "error");
    }
}
    protected void LoadExistingPlan(string barcode)
    {
        try
        {
            string sql = @"
            SELECT SRNO, TOTOUTSTANDING, INSTPLAN
            FROM METER_INST_MST 
            WHERE BARCODE = :barcode AND STATUS = 'A'
            ORDER BY SRNO DESC";

            using (OracleConnection con = new OracleConnection(connMain))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("barcode", barcode));
                con.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int existingSrno = Convert.ToInt32(reader["SRNO"]);
                        decimal prevOutstanding = reader["TOTOUTSTANDING"] != DBNull.Value ? Convert.ToDecimal(reader["TOTOUTSTANDING"]) : 0;
                        int existingInstPlan = reader["INSTPLAN"] != DBNull.Value ? Convert.ToInt32(reader["INSTPLAN"]) : 0;

                        hdnExistingSrno.Value = existingSrno.ToString();
                        txtPrevOutstanding.Text = prevOutstanding.ToString("0");

                        if (existingInstPlan == 3)
                        {
                            rb3Months.Checked = true;
                        }
                        else if (existingInstPlan == 6)
                        {
                            rb6Months.Checked = true;
                        }

                        // Only show message if not suppressed
                        if (!msg)
                            ShowStatusMessage("Existing active plan found. Previous outstanding: " + prevOutstanding, "info");
                    }
                    else
                    {
                        hdnExistingSrno.Value = "";
                        txtPrevOutstanding.Text = "0";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            hdnExistingSrno.Value = "";
            txtPrevOutstanding.Text = "0";
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(hdnBarcode.Value))
        {
            ShowStatusMessage("Please search and select a consumer first.", "error");
            return;
        }

        if (!rb3Months.Checked && !rb6Months.Checked)
        {
            ShowStatusMessage("Please select an installment plan (3 months or 6 months).", "error");
            return;
        }

        try
        {
            int instPlan = rb3Months.Checked ? 3 : 6;

            decimal instRate = 0;
            decimal.TryParse(txtInstRate.Text, out instRate);

            int numberOfInstallments = 0;
            int.TryParse(txtInstallments.Text, out numberOfInstallments);

            decimal meterCost = 62000;
            decimal prevPlan = 0;
            decimal defaultCharges = 0;

            decimal.TryParse(txtPrevOutstanding.Text, out prevPlan);
            decimal.TryParse(txtDefaultCharges.Text, out defaultCharges);

            // Calculate total outstanding = meterCost + prevPlan + defaultCharges
            decimal totalOutstanding = meterCost + prevPlan + defaultCharges;

            int currentSrno = 0;
            string billMonth = hdnBillMonth.Value;
            string savedBarcode = hdnBarcode.Value;  // Save the barcode
            string savedResid = hdnResid.Value;

            using (OracleConnection con = new OracleConnection(connMain))
            {
                con.Open();

                // Check for duplicate active plan
                string checkDuplicateSql = @"
                SELECT COUNT(*) FROM METER_INST_MST 
                WHERE BARCODE = :barcode 
                AND INSTPLAN = :instplan 
                AND INSTRATE = :instrate
                AND STATUS = 'A'";

                using (OracleCommand checkCmd = new OracleCommand(checkDuplicateSql, con))
                {
                    checkCmd.Parameters.Add("barcode", OracleDbType.Varchar2).Value = hdnBarcode.Value;
                    checkCmd.Parameters.Add("instplan", OracleDbType.Int32).Value = instPlan;
                    checkCmd.Parameters.Add("instrate", OracleDbType.Int32).Value = instRate;
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        ShowStatusMessage("An active plan with same details already exists for this consumer!", "error");
                        return;
                    }
                }

                // Suspend existing active plan
                if (!string.IsNullOrEmpty(hdnExistingSrno.Value))
                {
                    string updatePrevSql = "UPDATE METER_INST_MST SET STATUS = 'S' WHERE SRNO = :srno";
                    using (OracleCommand updateCmd = new OracleCommand(updatePrevSql, con))
                    {
                        updateCmd.Parameters.Add(new OracleParameter("srno", Convert.ToInt32(hdnExistingSrno.Value)));
                        updateCmd.ExecuteNonQuery();
                    }
                }

                // Get next SRNO
                string getSrnoSql = "SELECT NVL(MAX(SRNO), 0) + 1 FROM METER_INST_MST";
                using (OracleCommand srnoCmd = new OracleCommand(getSrnoSql, con))
                {
                    currentSrno = Convert.ToInt32(srnoCmd.ExecuteScalar());
                }

                // Insert into METER_INST_MST
                string insertSql = @"
                INSERT INTO METER_INST_MST (
                    SRNO, BARCODE, RESID, INSTMONTH, PREVPLAN, DEFAULTCHARGES, 
                    METERCOST, INSTPLAN, INSTRATE, STATUS, TOTALPAID, TOTOUTSTANDING
                ) VALUES (
                    :srno, :barcode, :resid, :instmonth, :prevplan, :defaultcharges,
                    :metercost, :instplan, :instrate, 'A', 0, :totoutstanding
                )";

                using (OracleCommand insertCmd = new OracleCommand(insertSql, con))
                {
                    insertCmd.Parameters.Add("srno", OracleDbType.Int32).Value = currentSrno;
                    insertCmd.Parameters.Add("barcode", OracleDbType.Varchar2).Value = hdnBarcode.Value;
                    insertCmd.Parameters.Add("resid", OracleDbType.Int32).Value = Convert.ToInt32(hdnResid.Value);
                    insertCmd.Parameters.Add("instmonth", OracleDbType.Int32).Value = Convert.ToInt32(billMonth);
                    insertCmd.Parameters.Add("prevplan", OracleDbType.Int32).Value = prevPlan;
                    insertCmd.Parameters.Add("defaultcharges", OracleDbType.Int32).Value = defaultCharges;
                    insertCmd.Parameters.Add("metercost", OracleDbType.Int32).Value = meterCost;
                    insertCmd.Parameters.Add("instplan", OracleDbType.Int32).Value = instPlan;
                    insertCmd.Parameters.Add("instrate", OracleDbType.Int32).Value = instRate;
                    insertCmd.Parameters.Add("totoutstanding", OracleDbType.Int32).Value = totalOutstanding;
                    insertCmd.ExecuteNonQuery();
                }

                // Generate installment details
                GenerateInstallmentDetails(con, currentSrno, hdnBarcode.Value, Convert.ToInt32(hdnResid.Value),
                                           billMonth, numberOfInstallments, instRate, totalOutstanding);
            }

            // Show success message
            string successMsg = "Installment plan saved successfully!";
            ShowStatusMessage(successMsg, "success");

            // DON'T call ClearForm() - just reload the data
            txtSearchBarcode.Text = savedBarcode;
            hdnBarcode.Value = savedBarcode;
            hdnResid.Value = savedResid;

            // Reload the data without showing any messages
            msg = true;
            ReloadConsumerData(savedBarcode);
            msg = false;

            CalculateFields();
        }
        catch (Exception ex)
        {
            msg = false;
            ShowStatusMessage("Error: " + ex.Message, "error");
        }
    }
    
    protected void GenerateInstallmentDetails(OracleConnection con, int srno, string barcode, int resid,
                                               string startMonth, int numberOfInstallments, decimal instRate, decimal totalOutstanding)
    {
        try
        {
            // Delete existing details for this SRNO
            string deleteSql = "DELETE FROM METER_INST_DTL WHERE SRNO = :srno";
            using (OracleCommand deleteCmd = new OracleCommand(deleteSql, con))
            {
                deleteCmd.Parameters.Add("srno", OracleDbType.Int32).Value = srno;
                deleteCmd.ExecuteNonQuery();
            }

            int currentMonth = Convert.ToInt32(startMonth);
            decimal remainingAmount = totalOutstanding;
            int installmentNo = 1;

            while (remainingAmount > 0 && installmentNo <= numberOfInstallments)
            {
                decimal installmentAmount = instRate;
                if (installmentNo == numberOfInstallments)
                {
                    installmentAmount = remainingAmount;
                }

                string insertSql = @"
                    INSERT INTO METER_INST_DTL (SRNO, BARCODE, RESID, INSTMONTH, INSTRATE, INSTRECEIVED)
                    VALUES (:srno, :barcode, :resid, :instmonth, :instrate, 0)";

                using (OracleCommand insertCmd = new OracleCommand(insertSql, con))
                {
                    insertCmd.Parameters.Add("srno", OracleDbType.Int32).Value = srno;
                    insertCmd.Parameters.Add("barcode", OracleDbType.Varchar2).Value = barcode;
                    insertCmd.Parameters.Add("resid", OracleDbType.Int32).Value = resid;
                    insertCmd.Parameters.Add("instmonth", OracleDbType.Int32).Value = currentMonth;
                    insertCmd.Parameters.Add("instrate", OracleDbType.Int32).Value = installmentAmount;
                    insertCmd.ExecuteNonQuery();
                }

                remainingAmount -= installmentAmount;
                installmentNo++;

                // Next month
                int year = currentMonth / 100;
                int month = currentMonth % 100;
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
                currentMonth = year * 100 + month;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error generating installments: " + ex.Message);
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Hide the status message first
        statusMessage.Style["display"] = "none";
        statusMessage.InnerHtml = "";
        ClearForm();
    }

    protected void ReloadConsumerData(string barcode)
    {
        try
        {
            string sql = @"
            SELECT REFCODE, RESID, RESNAME, ADDRESS, PRECENT_NM, BLOCK_NM, METERNO
            FROM BILLS.BIL_ELEC 
            WHERE REFCODE = :barcode AND ROWNUM = 1";

            using (OracleConnection con = new OracleConnection(connMain))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("barcode", barcode));
                con.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hdnBarcode.Value = reader["REFCODE"].ToString();
                        hdnResid.Value = reader["RESID"].ToString();

                        lblConsumerName.Text = reader["RESNAME"] != DBNull.Value ? reader["RESNAME"].ToString() : "-";
                        lblResid.Text = reader["RESID"].ToString();
                        lblAddress.Text = reader["ADDRESS"] != DBNull.Value ? reader["ADDRESS"].ToString() : "-";
                        lblPrecinct.Text = reader["PRECENT_NM"] != DBNull.Value ? reader["PRECENT_NM"].ToString() : "-";
                        lblBlock.Text = reader["BLOCK_NM"] != DBNull.Value ? reader["BLOCK_NM"].ToString() : "-";
                        lblMeterNo.Text = reader["METERNO"] != DBNull.Value ? reader["METERNO"].ToString() : "-";

                        // Load the plan without showing messages
                        LoadExistingPlan(hdnBarcode.Value);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Silent fail - don't show error message during reload
        }
    }

    protected void ShowStatusMessage(string message, string type)
    {
        statusMessage.InnerHtml = message;
        statusMessage.Style["display"] = "block";

        if (type == "success")
        {
            statusMessage.Attributes["class"] = "status-message status-success";
        }
        else if (type == "error")
        {
            statusMessage.Attributes["class"] = "status-message status-error";
        }
        else
        {
            statusMessage.Attributes["class"] = "status-message status-info";
        }

        // Auto hide after 5 seconds
        string script = "setTimeout(function() { var elem = document.getElementById('" + statusMessage.ClientID + "'); if(elem) elem.style.display = 'none'; }, 2000);";
        ClientScript.RegisterStartupScript(this.GetType(), "HideStatus_" + Guid.NewGuid().ToString(), script, true);
    }
}