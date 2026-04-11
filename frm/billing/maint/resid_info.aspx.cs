using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Web.UI.WebControls;
using System.Data;

public partial class resid_info : System.Web.UI.Page
{
    int mEdit = 0;
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
                        .ConnectionString;
    string connMain = WebConfigurationManager
                        .ConnectionStrings["MyDbConnection"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            pcdCategory();
            pcdNewCategory();
            pcdPrcnt();
            pcdBlock();
            pcdInit();
        }

        if (ViewState["mEdit"] != null)
        {
            mEdit = (int)ViewState["mEdit"];
        }

        ShowModeIndicator();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ViewState["mEdit"] = mEdit;
    }

    protected void ShowModeIndicator()
    {
        if (mEdit == 1)
        {
            lblMode.Text = "EDIT MODE";
            lblMode.Attributes["class"] = "mode-indicator mode-edit";
        }
        else
        {
            lblMode.Text = "INSERT MODE";
            lblMode.Attributes["class"] = "mode-indicator mode-insert";
        }
    }

    protected void pcdInit()
    {
        statusMessage.Style["display"] = "none";
        statusMessage.InnerHtml = "";

        mEdit = 0;
        txtRegNo.Text = "";
        txtResId.Text = "";
        txtResIdE.Text = "";
        txtResName.Text = "";
        txtFatherName.Text = "";
        txtMaintCharges.Text = "";
        txtAddress.Text = "";
        txtCNIC.Text = "";
        txtContact.Text = "";
        txtStreet.Text = "";
        txtRemarks.Text = "";

        ddlCategory.ClearSelection();
        if (ddlCategory.Items.Count > 0)
        {
            ddlCategory.Items[0].Selected = true;
        }

        ddlPrcnt.ClearSelection();
        if (ddlPrcnt.Items.Count > 0)
        {
            ddlPrcnt.Items[0].Selected = true;
        }

        ddlBlock.ClearSelection();
        if (ddlBlock.Items.Count > 0)
        {
            ddlBlock.Items[0].Selected = true;
        }

        ddlNewCategory.ClearSelection();
        if (ddlNewCategory.Items.Count > 0)
        {
            ddlNewCategory.Items[0].Selected = true;
        }

        txtRegNo.Focus();
        txtRegNo.Enabled = true;
        txtResId.Enabled = true;
        txtResIdE.Enabled = true;
        txtResId.Text = "";
        txtResIdE.Text = "";

        txtResName.Enabled = true;
        txtFatherName.Enabled = true;
        txtAddress.Enabled = true;
        txtCNIC.Enabled = true;
        txtContact.Enabled = true;
        txtStreet.Enabled = true;
        txtMaintCharges.Enabled = true;
        txtRemarks.Enabled = true;
        ddlCategory.Enabled = true;
        ddlNewCategory.Enabled = true;
        ddlPrcnt.Enabled = true;
        ddlBlock.Enabled = true;

        ShowModeIndicator();
    }

    protected void pcdCategory()
    {
        string sql = "SELECT CATID, BILCAT FROM BIL_CAT ORDER BY BILCAT";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            ddlCategory.DataSource = dr;
            ddlCategory.DataTextField = "BILCAT";
            ddlCategory.DataValueField = "CATID";
            ddlCategory.DataBind();

            ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", "0"));
        }
    }

    protected void pcdNewCategory()
    {
        string sql = @"SELECT DISTINCT CAT_ID, CAT_NM 
                   FROM RES_CAT_MAINT_NEW 
                   WHERE CAT_ID IS NOT NULL
                   ORDER BY CAT_NM";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            ddlNewCategory.DataSource = dr;
            ddlNewCategory.DataTextField = "CAT_NM";
            ddlNewCategory.DataValueField = "CAT_ID";
            ddlNewCategory.DataBind();

            ddlNewCategory.Items.Insert(0, new ListItem("-- Select New Category --", "0"));
        }
    }

    protected void pcdPrcnt()
    {
        string sql = "SELECT PRECENT_ID, PRECENT_NM FROM PRECENT_MST WHERE IS_ACTIVE = 'Y' ORDER BY PRECENT_NM";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            ddlPrcnt.DataSource = dr;
            ddlPrcnt.DataTextField = "PRECENT_NM";
            ddlPrcnt.DataValueField = "PRECENT_ID";
            ddlPrcnt.DataBind();

            ddlPrcnt.Items.Insert(0, new ListItem("-- Select Precinct --", "0"));
        }
    }

    protected void pcdBlock()
    {
        string sql = "SELECT BLOCK_ID, BLOCK_NM FROM BLOCK_MST WHERE IS_ACTIVE = 'Y' ORDER BY BLOCK_NM";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            OracleDataReader dr = cmd.ExecuteReader();

            ddlBlock.DataSource = dr;
            ddlBlock.DataTextField = "BLOCK_NM";
            ddlBlock.DataValueField = "BLOCK_ID";
            ddlBlock.DataBind();

            ddlBlock.Items.Insert(0, new ListItem("-- Select Block --", "0"));
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        mEdit = 1;
        txtResId.Enabled = true;
        txtResIdE.Enabled = true;
        txtRegNo.Enabled = true;

        txtRegNo.Text = "";
        txtResId.Text = "";
        txtResIdE.Text = "";
        txtResName.Text = "";
        txtFatherName.Text = "";
        txtMaintCharges.Text = "";
        txtAddress.Text = "";
        txtCNIC.Text = "";
        txtContact.Text = "";
        txtStreet.Text = "";
        txtRemarks.Text = "";

        txtResId.Focus();
        ShowModeIndicator();
        ShowStatusMessage("Enter Residential ID and press Tab to load record", "info");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        statusMessage.Style["display"] = "none";
        statusMessage.InnerHtml = "";
        pcdInit();
        txtRegNo.Focus();
    }

    protected string GenerateNewElectricId(out bool isNew)
    {
        isNew = false;
        string newId = "";
        string sql = @"SELECT NVL(MAX(RES_ID), 0) + 1 FROM BILLS.RESID_INFO WHERE SUBSTR(TO_CHAR(RES_ID), 1, 2) >= '96'";

        using (OracleConnection con = new OracleConnection(connMain))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            newId = (result != null && result != DBNull.Value && Convert.ToInt32(result) > 0) ? result.ToString() : "96001";
            isNew = true; // This ID is newly generated
        }
        return newId;
    }

    // Overload for when you don't need the flag
    protected string GenerateNewElectricId()
    {
        bool dummy;
        return GenerateNewElectricId(out dummy);
    }

    protected string GenerateNewResidentialId()
    {
        string newId = "";
        string sql = @"SELECT NVL(MAX(RES_ID), 0) + 1 FROM RESIDENTIAL_INFO";

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(sql, con))
        {
            con.Open();
            object result = cmd.ExecuteScalar();
            newId = (result != null && result != DBNull.Value) ? result.ToString() : "1";
        }
        return newId;
    }

    protected void txtRegNo_TextChanged(object sender, EventArgs e)
    {
        string regNo = txtRegNo.Text.Trim();

        if (string.IsNullOrEmpty(regNo))
        {
            txtResName.Text = "";
            return;
        }

        // FIRST CHECK: Does this barcode already exist in our system?
        string checkBarcodeSql = @"SELECT COUNT(*) FROM RESIDENTIAL_BARCODE WHERE BTYPE_ID=3 AND BARCODE=:barcode";
        int barcodeExists = 0;

        using (OracleConnection con = new OracleConnection(connStr))
        using (OracleCommand cmd = new OracleCommand(checkBarcodeSql, con))
        {
            cmd.Parameters.Add(new OracleParameter("barcode", regNo));
            con.Open();
            barcodeExists = Convert.ToInt32(cmd.ExecuteScalar());
        }

        if (barcodeExists > 0 && mEdit == 0)
        {
            // Barcode exists in system - Just display the record, stay in INSERT mode
            LoadRecordByBarcode(regNo);
            ShowStatusMessage("Record already exists! You are viewing an existing record.", "info");
            return;
        }

        if (mEdit == 0)
        {
            // INSERT MODE - Check V_FILES for data
            try
            {
                using (OracleConnection con = new OracleConnection(connStr))
                {
                    con.Open();

                    string sql = @"SELECT 
                            OWNER_NAME, OWNER_ADDRESS, OWNER_CNIC, OWNER_MOBILE, CATEGORY_NAME, PRECINCT_NAME, BLOCK_NAME 
                        FROM prj_arch.V_FILES 
                        WHERE REG_NO = :p_regno";

                    using (OracleCommand cmd = new OracleCommand(sql, con))
                    {
                        cmd.Parameters.Add(new OracleParameter("p_regno", regNo));

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtResName.Text = reader["OWNER_NAME"] != DBNull.Value ? reader["OWNER_NAME"].ToString() : "";
                                txtAddress.Text = reader["OWNER_ADDRESS"] != DBNull.Value ? reader["OWNER_ADDRESS"].ToString() : "";
                                txtCNIC.Text = reader["OWNER_CNIC"] != DBNull.Value ? reader["OWNER_CNIC"].ToString() : "";
                                txtContact.Text = reader["OWNER_MOBILE"] != DBNull.Value ? reader["OWNER_MOBILE"].ToString() : "";

                                ddlCategory.ClearSelection();
                                ListItem item = ddlCategory.Items.FindByText(reader["CATEGORY_NAME"].ToString());
                                if (item != null) item.Selected = true;
                                else if (ddlCategory.Items.Count > 0) ddlCategory.Items[0].Selected = true;

                                ddlPrcnt.ClearSelection();
                                ListItem prcnt = ddlPrcnt.Items.FindByText(reader["PRECINCT_NAME"].ToString());
                                if (prcnt != null) prcnt.Selected = true;
                                else if (ddlPrcnt.Items.Count > 0) ddlPrcnt.Items[0].Selected = true;

                                ddlBlock.ClearSelection();
                                ListItem block = ddlBlock.Items.FindByText(reader["BLOCK_NAME"].ToString());
                                if (block != null) block.Selected = true;
                                else if (ddlBlock.Items.Count > 0) ddlBlock.Items[0].Selected = true;

                                // Generate new IDs using MAX+1 queries
                                txtResId.Text = GenerateNewResidentialId();
                                txtResIdE.Text = GenerateNewElectricId();
                                txtResId.Enabled = true;
                                txtResIdE.Enabled = true;
                                txtMaintCharges.Focus();
                            }
                            else
                            {
                                // Registration Number not found in V_FILES
                                txtResId.Text = GenerateNewResidentialId();
                                txtResIdE.Text = GenerateNewElectricId();
                                txtResId.Enabled = true;
                                txtResIdE.Enabled = true;
                                ShowStatusMessage("Registration Number not found in V_FILES. You can manually enter data.", "info");
                                txtResName.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage("Error: " + ex.Message, "error");
            }
        }
        else
        {
            // EDIT MODE - Get data from existing records using barcode
            LoadRecordByBarcode(regNo);
        }
    }

    //    protected void LoadRecordByBarcode(string barcode)
    //    {
    //        try
    //        {
    //            using (OracleConnection con = new OracleConnection(connStr))
    //            {
    //                con.Open();

    //                string sql = @"
    //                    SELECT 
    //                      B.RES_ID, 
    //                      R.RES_NAME, R.HOUSE_NO, R.CNIC_NO, R.CONTACT_NO, R.MAINT_CHARGES, R.STREET_ID, R.REMARKS, 
    //                      (SELECT BILCAT FROM BIL_CAT C WHERE C.CATID=R.RCAT_ID) BILCAT, 
    //                      (SELECT MAX(C.CAT_NM) FROM RES_CAT_MAINT_NEW C WHERE C.CAT_ID=R.NCAT_ID) CAT_NM,
    //                      (SELECT PRECENT_NM FROM PRECENT_MST P WHERE P.PRECENT_ID=R.PRECENT_ID) PRECENT_NM,
    //                      (SELECT BLOCK_NM FROM BLOCK_MST P WHERE P.BLOCK_ID=R.BLOCK_ID) BLOCK_NM
    //                    FROM RESIDENTIAL_BARCODE B, RESIDENTIAL_INFO R 
    //                    WHERE B.RES_ID = R.RES_ID AND BARCODE = :p_regno
    //                ";

    //                using (OracleCommand cmd = new OracleCommand(sql, con))
    //                {
    //                    cmd.Parameters.Add(new OracleParameter("p_regno", barcode));

    //                    using (OracleDataReader reader = cmd.ExecuteReader())
    //                    {
    //                        if (reader.Read())
    //                        {
    //                            txtResId.Text = reader["RES_ID"] != DBNull.Value ? reader["RES_ID"].ToString() : "";
    //                            txtResName.Text = reader["RES_NAME"] != DBNull.Value ? reader["RES_NAME"].ToString() : "";
    //                            txtAddress.Text = reader["HOUSE_NO"] != DBNull.Value ? reader["HOUSE_NO"].ToString() : "";
    //                            txtCNIC.Text = reader["CNIC_NO"] != DBNull.Value ? reader["CNIC_NO"].ToString() : "";
    //                            txtContact.Text = reader["CONTACT_NO"] != DBNull.Value ? reader["CONTACT_NO"].ToString() : "";
    //                            txtMaintCharges.Text = reader["MAINT_CHARGES"] != DBNull.Value ? reader["MAINT_CHARGES"].ToString() : "";
    //                            txtStreet.Text = reader["STREET_ID"] != DBNull.Value ? reader["STREET_ID"].ToString() : "";
    //                            txtRemarks.Text = reader["REMARKS"] != DBNull.Value ? reader["REMARKS"].ToString() : "";

    //                            ddlCategory.ClearSelection();
    //                            ListItem item = ddlCategory.Items.FindByText(reader["BILCAT"].ToString());
    //                            if (item != null) item.Selected = true;
    //                            else if (ddlCategory.Items.Count > 0) ddlCategory.Items[0].Selected = true;

    //                            ddlNewCategory.ClearSelection();
    //                            ListItem itemN = ddlNewCategory.Items.FindByText(reader["CAT_NM"].ToString());
    //                            if (itemN != null) itemN.Selected = true;
    //                            else if (ddlNewCategory.Items.Count > 0) ddlNewCategory.Items[0].Selected = true;

    //                            ddlPrcnt.ClearSelection();
    //                            ListItem prcnt = ddlPrcnt.Items.FindByText(reader["PRECENT_NM"].ToString());
    //                            if (prcnt != null) prcnt.Selected = true;
    //                            else if (ddlPrcnt.Items.Count > 0) ddlPrcnt.Items[0].Selected = true;

    //                            ddlBlock.ClearSelection();
    //                            ListItem block = ddlBlock.Items.FindByText(reader["BLOCK_NM"].ToString());
    //                            if (block != null) block.Selected = true;
    //                            else if (ddlBlock.Items.Count > 0) ddlBlock.Items[0].Selected = true;

    //                            LoadElectricId(txtResId.Text);
    //                            ShowStatusMessage("Record loaded successfully", "success");
    //                            txtMaintCharges.Focus();
    //                        }
    //                        else
    //                        {
    //                            ShowStatusMessage("Record not found with this Registration Number", "error");
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            ShowStatusMessage("Error: " + ex.Message, "error");
    //        }
    //    }

    protected void LoadRecordByBarcode(string barcode)
    {
        try
        {
            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                string sql = @"
                SELECT 
                  B.RES_ID, 
                  R.RES_NAME, R.HOUSE_NO, R.CNIC_NO, R.CONTACT_NO, R.MAINT_CHARGES, R.STREET_ID, R.REMARKS, 
                  (SELECT BILCAT FROM BIL_CAT C WHERE C.CATID=R.RCAT_ID) BILCAT, 
                  R.NCAT_ID,
                  (SELECT CAT_NM FROM RES_CAT_MAINT_NEW C WHERE C.CAT_ID = R.NCAT_ID AND ROWNUM = 1) CAT_NM,
                  (SELECT PRECENT_NM FROM PRECENT_MST P WHERE P.PRECENT_ID=R.PRECENT_ID) PRECENT_NM,
                  (SELECT BLOCK_NM FROM BLOCK_MST P WHERE P.BLOCK_ID=R.BLOCK_ID) BLOCK_NM
                FROM RESIDENTIAL_BARCODE B, RESIDENTIAL_INFO R 
                WHERE B.RES_ID = R.RES_ID AND BARCODE = :p_regno
            ";

                using (OracleCommand cmd = new OracleCommand(sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("p_regno", barcode));

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string resId = reader["RES_ID"] != DBNull.Value ? reader["RES_ID"].ToString() : "";
                            txtResId.Text = resId;
                            txtResName.Text = reader["RES_NAME"] != DBNull.Value ? reader["RES_NAME"].ToString() : "";
                            txtAddress.Text = reader["HOUSE_NO"] != DBNull.Value ? reader["HOUSE_NO"].ToString() : "";
                            txtCNIC.Text = reader["CNIC_NO"] != DBNull.Value ? reader["CNIC_NO"].ToString() : "";
                            txtContact.Text = reader["CONTACT_NO"] != DBNull.Value ? reader["CONTACT_NO"].ToString() : "";
                            txtMaintCharges.Text = reader["MAINT_CHARGES"] != DBNull.Value ? reader["MAINT_CHARGES"].ToString() : "";
                            txtStreet.Text = reader["STREET_ID"] != DBNull.Value ? reader["STREET_ID"].ToString() : "";
                            txtRemarks.Text = reader["REMARKS"] != DBNull.Value ? reader["REMARKS"].ToString() : "";

                            ddlCategory.ClearSelection();
                            ListItem item = ddlCategory.Items.FindByText(reader["BILCAT"].ToString());
                            if (item != null) item.Selected = true;
                            else if (ddlCategory.Items.Count > 0) ddlCategory.Items[0].Selected = true;

                            // Set New Category - First try by NCAT_ID
                            ddlNewCategory.ClearSelection();
                            bool categorySet = false;
                            int ncatId = reader["NCAT_ID"] != DBNull.Value ? Convert.ToInt32(reader["NCAT_ID"]) : 0;

                            if (ncatId > 0)
                            {
                                ListItem catItem = ddlNewCategory.Items.FindByValue(ncatId.ToString());
                                if (catItem != null)
                                {
                                    catItem.Selected = true;
                                    categorySet = true;
                                }
                            }

                            // If NCAT_ID is NULL or not found, try loading from RES_CAT_MAINT_NEW table using RES_ID
                            if (!categorySet && !string.IsNullOrEmpty(resId))
                            {
                                string sqlCatFromTable = @"SELECT CAT_ID, CAT_NM FROM RES_CAT_MAINT_NEW WHERE RES_ID = :RES_ID";
                                using (OracleCommand cmdCat = new OracleCommand(sqlCatFromTable, con))
                                {
                                    cmdCat.Parameters.Add(new OracleParameter("RES_ID", Convert.ToInt32(resId)));
                                    using (OracleDataReader drCat = cmdCat.ExecuteReader())
                                    {
                                        if (drCat.Read())
                                        {
                                            int catId = drCat["CAT_ID"] != DBNull.Value ? Convert.ToInt32(drCat["CAT_ID"]) : 0;
                                            if (catId > 0)
                                            {
                                                ListItem catItem = ddlNewCategory.Items.FindByValue(catId.ToString());
                                                if (catItem != null)
                                                {
                                                    catItem.Selected = true;
                                                    categorySet = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // If still not set, select first item
                            if (!categorySet && ddlNewCategory.Items.Count > 0)
                            {
                                ddlNewCategory.Items[0].Selected = true;
                                if (ncatId == 0)
                                {
                                    ShowStatusMessage("Info: No category found for this record. Please select one.", "info");
                                }
                            }

                            ddlPrcnt.ClearSelection();
                            ListItem prcnt = ddlPrcnt.Items.FindByText(reader["PRECENT_NM"].ToString());
                            if (prcnt != null) prcnt.Selected = true;
                            else if (ddlPrcnt.Items.Count > 0) ddlPrcnt.Items[0].Selected = true;

                            ddlBlock.ClearSelection();
                            ListItem block = ddlBlock.Items.FindByText(reader["BLOCK_NM"].ToString());
                            if (block != null) block.Selected = true;
                            else if (ddlBlock.Items.Count > 0) ddlBlock.Items[0].Selected = true;

                            LoadElectricId(txtResId.Text);
                            ShowStatusMessage("Record loaded successfully", "success");
                            txtMaintCharges.Focus();
                        }
                        else
                        {
                            ShowStatusMessage("Record not found with this Registration Number", "error");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowStatusMessage("Error: " + ex.Message, "error");
        }
    }
    protected void LoadElectricId(string resId)
    {
        try
        {
            string sql = @"SELECT RES_ID FROM BILLS.RESID_INFO WHERE RES_ID_BK = :RES_ID";

            using (OracleConnection con = new OracleConnection(connMain))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("RES_ID", Convert.ToInt32(resId)));
                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    txtResIdE.Text = result.ToString();
                    txtResIdE.Attributes.Remove("class");
                    txtResIdE.CssClass = "asp-input";

                    // Check if IDs are same
                    if (txtResId.Text.Trim() == txtResIdE.Text.Trim())
                    {
                        bool isNew;
                        string newId = GenerateNewElectricId(out isNew);
                        txtResIdE.Text = newId;
                        if (isNew)
                        {
                            txtResIdE.CssClass = "asp-input highlight-red";
                        }
                        ShowStatusMessage("WARNING: IDs were same! New Electric ID generated: " + newId, "error");
                        txtResIdE.Focus();
                    }
                }
                else
                {
                    bool isNew;
                    string newId = GenerateNewElectricId(out isNew);
                    txtResIdE.Text = newId;
                    if (isNew)
                    {
                        txtResIdE.CssClass = "asp-input highlight-red";
                    }
                    if (mEdit == 1)
                    {
                        ShowStatusMessage("New Electric ID generated: " + newId, "info");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            bool isNew;
            string newId = GenerateNewElectricId(out isNew);
            txtResIdE.Text = newId;
            if (isNew)
            {
                txtResIdE.CssClass = "asp-input highlight-red";
            }
            if (mEdit == 1)
            {
                ShowStatusMessage("New Electric ID generated: " + newId, "info");
            }
        }
    }
    protected void txtResId_TextChanged(object sender, EventArgs e)
    {
        if (mEdit == 1)
        {
            string resId = txtResId.Text.Trim();

            if (string.IsNullOrEmpty(resId))
            {
                ShowStatusMessage("Please enter a valid Residential ID", "error");
                return;
            }

            int parsedResId;
            if (!int.TryParse(resId, out parsedResId))
            {
                ShowStatusMessage("Residential ID must be a number", "error");
                txtResId.Text = "";
                return;
            }

            try
            {
                bool recordFound = LoadResidentialData(resId);

                if (!recordFound)
                {
                    ShowStatusMessage("Record not found with RES_ID: " + resId, "error");
                    txtResId.Text = "";
                    txtRegNo.Text = "";
                    txtResIdE.Text = "";
                    txtResName.Text = "";
                    txtFatherName.Text = "";
                    txtMaintCharges.Text = "";
                    txtAddress.Text = "";
                    txtCNIC.Text = "";
                    txtContact.Text = "";
                    txtStreet.Text = "";
                    txtRemarks.Text = "";
                    txtResId.Focus();
                }
                else
                {
                    txtRegNo.Enabled = true;
                    txtResId.Enabled = true;
                    txtResIdE.Enabled = true;
                    txtResName.Enabled = true;
                    txtFatherName.Enabled = true;
                    txtAddress.Enabled = true;
                    txtCNIC.Enabled = true;
                    txtContact.Enabled = true;
                    txtStreet.Enabled = true;
                    txtMaintCharges.Enabled = true;
                    txtRemarks.Enabled = true;
                    ddlCategory.Enabled = true;
                    ddlNewCategory.Enabled = true;
                    ddlPrcnt.Enabled = true;
                    ddlBlock.Enabled = true;

                    ShowStatusMessage("Record loaded successfully", "success");
                    txtResName.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage("Error loading data: " + ex.Message, "error");
            }
        }
    }

    protected bool LoadResidentialData(string resId)
    {
        try
        {
            string sql = @"SELECT * FROM RESIDENTIAL_INFO WHERE RES_ID = :RES_ID";
            bool recordFound = false;

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter("RES_ID", Convert.ToInt32(resId)));
                con.Open();

                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        recordFound = true;
                        txtResName.Text = dr["RES_NAME"] != DBNull.Value ? dr["RES_NAME"].ToString() : "";
                        txtFatherName.Text = dr["FATHERNAME"] != DBNull.Value ? dr["FATHERNAME"].ToString() : "";
                        txtAddress.Text = dr["HOUSE_NO"] != DBNull.Value ? dr["HOUSE_NO"].ToString() : "";
                        txtCNIC.Text = dr["CNIC_NO"] != DBNull.Value ? dr["CNIC_NO"].ToString() : "";
                        txtContact.Text = dr["CONTACT_NO"] != DBNull.Value ? dr["CONTACT_NO"].ToString() : "";
                        txtStreet.Text = dr["STREET_ID"] != DBNull.Value ? dr["STREET_ID"].ToString() : "";
                        txtMaintCharges.Text = dr["MAINT_CHARGES"] != DBNull.Value ? dr["MAINT_CHARGES"].ToString() : "0";
                        txtRemarks.Text = dr["REMARKS"] != DBNull.Value ? dr["REMARKS"].ToString() : "";

                        ddlCategory.ClearSelection();
                        if (dr["RCAT_ID"] != DBNull.Value && dr["RCAT_ID"].ToString() != "0")
                        {
                            ListItem item = ddlCategory.Items.FindByValue(dr["RCAT_ID"].ToString());
                            if (item != null) item.Selected = true;
                            else if (ddlCategory.Items.Count > 0) ddlCategory.Items[0].Selected = true;
                        }
                        else if (ddlCategory.Items.Count > 0)
                        {
                            ddlCategory.Items[0].Selected = true;
                        }

                        ddlPrcnt.ClearSelection();
                        if (dr["PRECENT_ID"] != DBNull.Value && dr["PRECENT_ID"].ToString() != "0")
                        {
                            ListItem prcnt = ddlPrcnt.Items.FindByValue(dr["PRECENT_ID"].ToString());
                            if (prcnt != null) prcnt.Selected = true;
                            else if (ddlPrcnt.Items.Count > 0) ddlPrcnt.Items[0].Selected = true;
                        }
                        else if (ddlPrcnt.Items.Count > 0)
                        {
                            ddlPrcnt.Items[0].Selected = true;
                        }

                        ddlBlock.ClearSelection();
                        if (dr["BLOCK_ID"] != DBNull.Value && dr["BLOCK_ID"].ToString() != "0")
                        {
                            ListItem block = ddlBlock.Items.FindByValue(dr["BLOCK_ID"].ToString());
                            if (block != null) block.Selected = true;
                            else if (ddlBlock.Items.Count > 0) ddlBlock.Items[0].Selected = true;
                        }
                        else if (ddlBlock.Items.Count > 0)
                        {
                            ddlBlock.Items[0].Selected = true;
                        }
                    }
                }
            }

            string sqlNewCat = @"SELECT CAT_ID, CAT_NM, CAT_COST FROM RES_CAT_MAINT_NEW WHERE RES_ID = :RES_ID";
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(sqlNewCat, con))
            {
                cmd.Parameters.Add(new OracleParameter("RES_ID", Convert.ToInt32(resId)));
                con.Open();

                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    ddlNewCategory.ClearSelection();
                    if (dr.Read() && dr["CAT_ID"] != DBNull.Value && dr["CAT_ID"].ToString() != "0")
                    {
                        ListItem newItem = ddlNewCategory.Items.FindByValue(dr["CAT_ID"].ToString());
                        if (newItem != null) newItem.Selected = true;
                        else if (ddlNewCategory.Items.Count > 0) ddlNewCategory.Items[0].Selected = true;
                    }
                    else if (ddlNewCategory.Items.Count > 0)
                    {
                        ddlNewCategory.Items[0].Selected = true;
                    }
                }
            }

            string sqlBarcode = @"SELECT BARCODE FROM RESIDENTIAL_BARCODE WHERE RES_ID = :RES_ID AND BTYPE_ID = 3";
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(sqlBarcode, con))
            {
                cmd.Parameters.Add(new OracleParameter("RES_ID", Convert.ToInt32(resId)));
                con.Open();

                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read() && dr["BARCODE"] != DBNull.Value)
                    {
                        txtRegNo.Text = dr["BARCODE"].ToString();
                    }
                }
            }

            LoadElectricId(resId);
            return recordFound;
        }
        catch (Exception ex)
        {
            ShowStatusMessage("Error: " + ex.Message, "error");
            return false;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (mEdit == 0)
        {
            // Check if record already exists before inserting
            string regNo = txtRegNo.Text.Trim();
            if (!string.IsNullOrEmpty(regNo))
            {
                string checkSql = @"SELECT COUNT(*) FROM RESIDENTIAL_BARCODE WHERE BTYPE_ID=3 AND BARCODE=:barcode";
                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(checkSql, con))
                {
                    cmd.Parameters.Add(new OracleParameter("barcode", regNo));
                    con.Open();
                    int exists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        ShowStatusMessage("Record already exists! Cannot save. Use Edit mode to modify.", "error");
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(txtResName.Text.Trim()))
            {
                ShowStatusMessage("Please enter Resident Name", "error");
                txtResName.Focus();
                return;
            }

            if (ddlCategory.SelectedValue == "0")
            {
                ShowStatusMessage("Please select Category", "error");
                ddlCategory.Focus();
                return;
            }

            if (ddlNewCategory.SelectedValue == "0")
            {
                ShowStatusMessage("Please select New Category", "error");
                ddlNewCategory.Focus();
                return;
            }

            InsertNewRecord();
        }
        else if (mEdit == 1)
        {
            if (string.IsNullOrEmpty(txtResId.Text.Trim()))
            {
                ShowStatusMessage("Please enter Residential ID", "error");
                txtResId.Focus();
                return;
            }

            // Just show warning but allow save - don't block
            if (txtResId.Text.Trim() == txtResIdE.Text.Trim())
            {
                string newId = GenerateNewElectricId();
                txtResIdE.Text = newId;
                ShowStatusMessage("Electric ID has been updated to: " + newId, "info");
            }

            if (string.IsNullOrEmpty(txtResName.Text.Trim()))
            {
                ShowStatusMessage("Please enter Resident Name", "error");
                txtResName.Focus();
                return;
            }

            UpdateExistingRecord();
        }
    }

//    protected void InsertNewRecord()
//    {
//        string userName = Session["User"] != null ? Session["login_name"].ToString() : "";
//        string ipAddress = Request.UserHostAddress;
//        int resID = Convert.ToInt32(txtResId.Text.Trim());
//        int electricId = Convert.ToInt32(txtResIdE.Text.Trim());

//        try
//        {
//            // INSERT RESIDENTIAL_INFO
//            using (OracleConnection con = new OracleConnection(connStr))
//            using (OracleCommand cmd = new OracleCommand(@"
//                INSERT INTO RESIDENTIAL_INFO (
//                    RES_ID, RES_CODE, RES_NAME, HOUSE_NO, CNIC_NO, 
//                    DT_INSERT, INSERT_BY, INSERT_IP, FATHERNAME, CONTACT_NO, RCAT_ID, M_FACT_RATE, 
//                    COMP_ID, PRECENT_ID, BLOCK_ID, MAINT_CHARGES, STREET_ID, REMARKS,
//                    BILL_WO, ELECT, WATER, GAS, BILL_STOPED, WATER_FIXED_BILLS
//                ) VALUES (
//                    :RES_ID,:RES_CODE,:RES_NAME,:HOUSE_NO,:CNIC_NO,
//                    :DT_INSERT,:INSERT_BY,:INSERT_IP,:FATHERNAME,:CONTACT_NO,:RCAT_ID,:M_FACT_RATE,
//                    :COMP_ID,:PRECENT_ID,:BLOCK_ID,:MAINT_CHARGES,:STREET_ID,:REMARKS,
//                    :BILL_WO,:ELECT,:WATER,:GAS,:BILL_STOPED,:WATER_FIXED_BILLS
//                )", con))
//            {
//                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
//                cmd.Parameters.Add("RES_CODE", OracleDbType.Varchar2).Value = "R-" + resID.ToString().PadLeft(6, '0');
//                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
//                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
//                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
//                cmd.Parameters.Add("DT_INSERT", OracleDbType.Date).Value = DateTime.Now;
//                cmd.Parameters.Add("INSERT_BY", OracleDbType.Varchar2).Value = userName;
//                cmd.Parameters.Add("INSERT_IP", OracleDbType.Varchar2).Value = ipAddress;
//                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
//                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
//                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
//                cmd.Parameters.Add("M_FACT_RATE", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("COMP_ID", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
//                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
//                cmd.Parameters.Add("MAINT_CHARGES", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
//                cmd.Parameters.Add("STREET_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtStreet.Text.Trim()) ? 0 : Convert.ToInt32(txtStreet.Text.Trim());
//                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();
//                cmd.Parameters.Add("BILL_WO", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("ELECT", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("WATER", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("GAS", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BILL_STOPED", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("WATER_FIXED_BILLS", OracleDbType.Int32).Value = 0;

//                con.Open();
//                cmd.ExecuteNonQuery();
//            }

//            // INSERT CATEGORY
//            using (OracleConnection con = new OracleConnection(connStr))
//            using (OracleCommand cmd = new OracleCommand(@"
//                INSERT INTO RES_CAT_MAINT_NEW (RES_ID, CAT_ID, CAT_NM, CAT_COST)
//                VALUES (:RES_ID, :CAT_ID, :CAT_NM, :CAT_COST)", con))
//            {
//                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
//                cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
//                cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
//                cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
//                con.Open();
//                cmd.ExecuteNonQuery();
//            }

//            // INSERT BILLS.RESID_INFO
//            using (OracleConnection con = new OracleConnection(connMain))
//            using (OracleCommand cmd = new OracleCommand(@"
//                  INSERT INTO BILLS.RESID_INFO (
//    RES_ID_BK, RES_ID, RES_CODE, RES_NAME, HOUSE_NO, CNIC_NO, 
//    FATHERNAME, CONTACT_NO, RCAT_ID, M_FACT_RATE, COMP_ID, 
//    PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
//    BIL_WO_MAINT, BIL_WO_ELEC, BIL_WO_WATER, BIL_WO_GAS, 
//    BIL_WO_SOLAR, BIL_WO_RENT, EMP_STATUS, EMP_ID, LOG_ID
//) VALUES (
//    :RES_ID_BK,:RES_ID,:RES_CODE,:RES_NAME,:HOUSE_NO,:CNIC_NO,
//    :FATHERNAME,:CONTACT_NO,:RCAT_ID,:M_FACT_RATE,:COMP_ID,
//    :PRECENT_ID,:PRECENT_NM,:BLOCK_ID,:BLOCK_NM,
//    :BIL_WO_MAINT,:BIL_WO_ELEC,:BIL_WO_WATER,:BIL_WO_GAS,
//    :BIL_WO_SOLAR,:BIL_WO_RENT,:EMP_STATUS,:EMP_ID,:LOG_ID
//)", con))
//            {
//                cmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;
//                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = electricId;
//                cmd.Parameters.Add("RES_CODE", OracleDbType.Varchar2).Value = "R-" + electricId.ToString().PadLeft(6, '0');
//                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
//                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
//                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
//                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
//                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
//                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
//                cmd.Parameters.Add("M_FACT_RATE", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("COMP_ID", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
//                cmd.Parameters.Add("PRECENT_NM", OracleDbType.Varchar2).Value = ddlPrcnt.SelectedItem.Text;
//                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
//                cmd.Parameters.Add("BLOCK_NM", OracleDbType.Varchar2).Value = ddlBlock.SelectedItem.Text;
//                cmd.Parameters.Add("BIL_WO_MAINT", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BIL_WO_ELEC", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BIL_WO_WATER", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BIL_WO_GAS", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BIL_WO_SOLAR", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("BIL_WO_RENT", OracleDbType.Int32).Value = 1;
//                cmd.Parameters.Add("EMP_STATUS", OracleDbType.Int32).Value = 0;
//                cmd.Parameters.Add("EMP_ID", OracleDbType.Int32).Value = 0;
//                cmd.Parameters.Add("LOG_ID", OracleDbType.Int32).Value = 0;

//                con.Open();
//                cmd.ExecuteNonQuery();
//            }

//            // INSERT RESIDENTIAL_BARCODE
//            using (OracleConnection con = new OracleConnection(connStr))
//            using (OracleCommand cmd = new OracleCommand(@"
//                INSERT INTO RESIDENTIAL_BARCODE (RES_ID, BARCODE, BTYPE_ID)
//                VALUES (:RES_ID, :BARCODE, :BTYPE_ID)", con))
//            {
//                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
//                cmd.Parameters.Add("BARCODE", OracleDbType.Varchar2).Value = txtRegNo.Text.Trim();
//                cmd.Parameters.Add("BTYPE_ID", OracleDbType.Int32).Value = 3;
//                con.Open();
//                cmd.ExecuteNonQuery();
//            }

//            ShowStatusMessage("Record saved successfully! Residential ID: " + resID + ", Electric ID: " + electricId, "success");
//            pcdInit();
//        }
//        catch (Exception ex)
//        {
//            ShowStatusMessage("Error: " + ex.Message, "error");
//        }
//    }

    //    protected void UpdateExistingRecord()
    //    {
    //        string userName = Session["User"] != null ? Session["login_name"].ToString() : "";
    //        string ipAddress = Request.UserHostAddress;
    //        int resID = Convert.ToInt32(txtResId.Text.Trim());
    //        int electricId = Convert.ToInt32(txtResIdE.Text.Trim());

    //        try
    //        {
    //            // UPDATE RESIDENTIAL_INFO
    //            using (OracleConnection con = new OracleConnection(connStr))
    //            using (OracleCommand cmd = new OracleCommand(@"
    //                UPDATE RESIDENTIAL_INFO SET
    //                    RES_NAME = :RES_NAME, HOUSE_NO = :HOUSE_NO, CNIC_NO = :CNIC_NO,
    //                    DT_UPDATE = :DT_UPDATE, UPDATE_BY = :UPDATE_BY, UPDATE_IP = :UPDATE_IP,
    //                    FATHERNAME = :FATHERNAME, CONTACT_NO = :CONTACT_NO, RCAT_ID = :RCAT_ID,
    //                    PRECENT_ID = :PRECENT_ID, BLOCK_ID = :BLOCK_ID,
    //                    MAINT_CHARGES = :MAINT_CHARGES, STREET_ID = :STREET_ID, REMARKS = :REMARKS
    //                WHERE RES_ID = :RES_ID", con))
    //            {
    //                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
    //                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
    //                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
    //                cmd.Parameters.Add("DT_UPDATE", OracleDbType.Date).Value = DateTime.Now;
    //                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Varchar2).Value = userName;
    //                cmd.Parameters.Add("UPDATE_IP", OracleDbType.Varchar2).Value = ipAddress;
    //                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
    //                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
    //                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
    //                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
    //                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
    //                cmd.Parameters.Add("MAINT_CHARGES", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
    //                cmd.Parameters.Add("STREET_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtStreet.Text.Trim()) ? 0 : Convert.ToInt32(txtStreet.Text.Trim());
    //                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();
    //                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;

    //                con.Open();
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    ShowStatusMessage("Record not found for update!", "error");
    //                    return;
    //                }
    //            }

    //            // UPDATE or INSERT RES_CAT_MAINT_NEW
    //            string checkSql = "SELECT COUNT(*) FROM RES_CAT_MAINT_NEW WHERE RES_ID = :RES_ID";
    //            int recordExists = 0;

    //            using (OracleConnection con = new OracleConnection(connStr))
    //            using (OracleCommand checkCmd = new OracleCommand(checkSql, con))
    //            {
    //                checkCmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
    //                con.Open();
    //                recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());
    //            }

    //            if (recordExists > 0)
    //            {
    //                using (OracleConnection con = new OracleConnection(connStr))
    //                using (OracleCommand cmd = new OracleCommand(@"
    //                    UPDATE RES_CAT_MAINT_NEW SET CAT_ID = :CAT_ID, CAT_NM = :CAT_NM, CAT_COST = :CAT_COST
    //                    WHERE RES_ID = :RES_ID", con))
    //                {
    //                    cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
    //                    cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
    //                    cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
    //                    cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
    //                    con.Open();
    //                    cmd.ExecuteNonQuery();
    //                }
    //            }
    //            else
    //            {
    //                using (OracleConnection con = new OracleConnection(connStr))
    //                using (OracleCommand cmd = new OracleCommand(@"
    //                    INSERT INTO RES_CAT_MAINT_NEW (RES_ID, CAT_ID, CAT_NM, CAT_COST)
    //                    VALUES (:RES_ID, :CAT_ID, :CAT_NM, :CAT_COST)", con))
    //                {
    //                    cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
    //                    cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
    //                    cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
    //                    cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
    //                    con.Open();
    //                    cmd.ExecuteNonQuery();
    //                }
    //            }
    //            // select count(*) from resid_info where res_id_bk = txtres_id.Text
    //            // UPDATE BILLS.RESID_INFO
    //            using (OracleConnection con = new OracleConnection(connMain))
    //            using (OracleCommand cmd = new OracleCommand(@"
    //                UPDATE BILLS.RESID_INFO SET
    //                    RES_NAME = :RES_NAME, HOUSE_NO = :HOUSE_NO, CNIC_NO = :CNIC_NO,
    //                    FATHERNAME = :FATHERNAME, CONTACT_NO = :CONTACT_NO, RCAT_ID = :RCAT_ID,
    //                    PRECENT_ID = :PRECENT_ID, PRECENT_NM = :PRECENT_NM,
    //                    BLOCK_ID = :BLOCK_ID, BLOCK_NM = :BLOCK_NM, 
    //                    RES_ID = :RES_ID_NEW, RES_CODE = :RES_CODE_NEW
    //                WHERE RES_ID_BK = :RES_ID_BK", con))
    //            {
    //                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
    //                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
    //                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
    //                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
    //                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
    //                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
    //                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
    //                cmd.Parameters.Add("PRECENT_NM", OracleDbType.Varchar2).Value = ddlPrcnt.SelectedItem.Text;
    //                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
    //                cmd.Parameters.Add("BLOCK_NM", OracleDbType.Varchar2).Value = ddlBlock.SelectedItem.Text;
    //                cmd.Parameters.Add("RES_ID_NEW", OracleDbType.Int32).Value = electricId;
    //                cmd.Parameters.Add("RES_CODE_NEW", OracleDbType.Varchar2).Value = "R-" + electricId.ToString().PadLeft(6, '0');
    //                cmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;

    //                con.Open();
    //                cmd.ExecuteNonQuery();
    //            }

    //            ShowStatusMessage("Record updated successfully! Electric ID: " + electricId, "success");
    //            pcdInit();
    //        }
    //        catch (Exception ex)
    //        {
    //            ShowStatusMessage("Error: " + ex.Message, "error");
    //        }
    //    }

    protected void InsertNewRecord()
    {
        string userName = Session["User"] != null ? Session["login_name"].ToString() : "";
        string ipAddress = Request.UserHostAddress;
        int resID = Convert.ToInt32(txtResId.Text.Trim());
        int electricId = Convert.ToInt32(txtResIdE.Text.Trim());

        try
        {
            // INSERT RESIDENTIAL_INFO
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(@"
            INSERT INTO RESIDENTIAL_INFO (
                RES_ID, RES_CODE, RES_NAME, HOUSE_NO, CNIC_NO, 
                DT_INSERT, INSERT_BY, INSERT_IP, FATHERNAME, CONTACT_NO, RCAT_ID, M_FACT_RATE, 
                COMP_ID, PRECENT_ID, BLOCK_ID, MAINT_CHARGES, STREET_ID, REMARKS,
                BILL_WO, ELECT, WATER, GAS, BILL_STOPED, WATER_FIXED_BILLS
            ) VALUES (
                :RES_ID,:RES_CODE,:RES_NAME,:HOUSE_NO,:CNIC_NO,
                :DT_INSERT,:INSERT_BY,:INSERT_IP,:FATHERNAME,:CONTACT_NO,:RCAT_ID,:M_FACT_RATE,
                :COMP_ID,:PRECENT_ID,:BLOCK_ID,:MAINT_CHARGES,:STREET_ID,:REMARKS,
                :BILL_WO,:ELECT,:WATER,:GAS,:BILL_STOPED,:WATER_FIXED_BILLS
            )", con))
            {
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                cmd.Parameters.Add("RES_CODE", OracleDbType.Varchar2).Value = "R-" + resID.ToString().PadLeft(6, '0');
                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
                cmd.Parameters.Add("DT_INSERT", OracleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("INSERT_BY", OracleDbType.Varchar2).Value = userName;
                cmd.Parameters.Add("INSERT_IP", OracleDbType.Varchar2).Value = ipAddress;
                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
                cmd.Parameters.Add("M_FACT_RATE", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("COMP_ID", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
                cmd.Parameters.Add("MAINT_CHARGES", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
                cmd.Parameters.Add("STREET_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtStreet.Text.Trim()) ? 0 : Convert.ToInt32(txtStreet.Text.Trim());
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();
                cmd.Parameters.Add("BILL_WO", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("ELECT", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("WATER", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("GAS", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BILL_STOPED", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("WATER_FIXED_BILLS", OracleDbType.Int32).Value = 0;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // INSERT CATEGORY
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(@"
            INSERT INTO RES_CAT_MAINT_NEW (RES_ID, CAT_ID, CAT_NM, CAT_COST)
            VALUES (:RES_ID, :CAT_ID, :CAT_NM, :CAT_COST)", con))
            {
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
                cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
                cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
                con.Open();
                cmd.ExecuteNonQuery();
            }

            // INSERT BILLS.RESID_INFO
            using (OracleConnection con = new OracleConnection(connMain))
            using (OracleCommand cmd = new OracleCommand(@"
            INSERT INTO BILLS.RESID_INFO (
                RES_ID_BK, RES_ID, RES_CODE, RES_NAME, HOUSE_NO, CNIC_NO, 
                FATHERNAME, CONTACT_NO, RCAT_ID, M_FACT_RATE, COMP_ID, 
                PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
                BIL_WO_MAINT, BIL_WO_ELEC, BIL_WO_WATER, BIL_WO_GAS, 
                BIL_WO_SOLAR, BIL_WO_RENT, EMP_STATUS, EMP_ID, LOG_ID
            ) VALUES (
                :RES_ID_BK,:RES_ID,:RES_CODE,:RES_NAME,:HOUSE_NO,:CNIC_NO,
                :FATHERNAME,:CONTACT_NO,:RCAT_ID,:M_FACT_RATE,:COMP_ID,
                :PRECENT_ID,:PRECENT_NM,:BLOCK_ID,:BLOCK_NM,
                :BIL_WO_MAINT,:BIL_WO_ELEC,:BIL_WO_WATER,:BIL_WO_GAS,
                :BIL_WO_SOLAR,:BIL_WO_RENT,:EMP_STATUS,:EMP_ID,:LOG_ID
            )", con))
            {
                cmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = electricId;
                cmd.Parameters.Add("RES_CODE", OracleDbType.Varchar2).Value = "R-" + electricId.ToString().PadLeft(6, '0');
                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
                cmd.Parameters.Add("M_FACT_RATE", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("COMP_ID", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
                cmd.Parameters.Add("PRECENT_NM", OracleDbType.Varchar2).Value = ddlPrcnt.SelectedItem.Text;
                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
                cmd.Parameters.Add("BLOCK_NM", OracleDbType.Varchar2).Value = ddlBlock.SelectedItem.Text;
                cmd.Parameters.Add("BIL_WO_MAINT", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BIL_WO_ELEC", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BIL_WO_WATER", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BIL_WO_GAS", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BIL_WO_SOLAR", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("BIL_WO_RENT", OracleDbType.Int32).Value = 1;
                cmd.Parameters.Add("EMP_STATUS", OracleDbType.Int32).Value = 0;
                cmd.Parameters.Add("EMP_ID", OracleDbType.Int32).Value = 0;
                cmd.Parameters.Add("LOG_ID", OracleDbType.Int32).Value = 0;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // INSERT RESIDENTIAL_BARCODE
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(@"
            INSERT INTO RESIDENTIAL_BARCODE (RES_ID, BARCODE, BTYPE_ID)
            VALUES (:RES_ID, :BARCODE, :BTYPE_ID)", con))
            {
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                cmd.Parameters.Add("BARCODE", OracleDbType.Varchar2).Value = txtRegNo.Text.Trim();
                cmd.Parameters.Add("BTYPE_ID", OracleDbType.Int32).Value = 3;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand("UPDATE RESIDENTIAL_INFO SET NCAT_ID = :NCAT_ID WHERE RES_ID = :RES_ID", con))
            {
                cmd.Parameters.Add("NCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            ShowStatusMessage("Record saved successfully! Residential ID: " + resID + ", Electric ID: " + electricId, "success");
            pcdInit();
        }
        catch (Exception ex)
        {
            ShowStatusMessage("Error: " + ex.Message, "error");
        }
    }

    protected void UpdateExistingRecord()
    {
        string userName = Session["User"] != null ? Session["login_name"].ToString() : "";
        string ipAddress = Request.UserHostAddress;
        int resID = Convert.ToInt32(txtResId.Text.Trim());
        int electricId = Convert.ToInt32(txtResIdE.Text.Trim());

        try
        {
            // UPDATE RESIDENTIAL_INFO (NO NCAT_ID HERE)
            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand(@"
            UPDATE RESIDENTIAL_INFO SET
                RES_NAME = :RES_NAME, HOUSE_NO = :HOUSE_NO, CNIC_NO = :CNIC_NO,
                DT_UPDATE = :DT_UPDATE, UPDATE_BY = :UPDATE_BY, UPDATE_IP = :UPDATE_IP,
                FATHERNAME = :FATHERNAME, CONTACT_NO = :CONTACT_NO, RCAT_ID = :RCAT_ID,
                PRECENT_ID = :PRECENT_ID, BLOCK_ID = :BLOCK_ID,
                MAINT_CHARGES = :MAINT_CHARGES, STREET_ID = :STREET_ID, REMARKS = :REMARKS
            WHERE RES_ID = :RES_ID", con))
            {
                cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
                cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
                cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
                cmd.Parameters.Add("DT_UPDATE", OracleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.Add("UPDATE_BY", OracleDbType.Varchar2).Value = userName;
                cmd.Parameters.Add("UPDATE_IP", OracleDbType.Varchar2).Value = ipAddress;
                cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
                cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
                cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlCategory.SelectedValue);
                cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
                cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
                cmd.Parameters.Add("MAINT_CHARGES", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
                cmd.Parameters.Add("STREET_ID", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtStreet.Text.Trim()) ? 0 : Convert.ToInt32(txtStreet.Text.Trim());
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = txtRemarks.Text.Trim();
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;

                con.Open();
                if (cmd.ExecuteNonQuery() == 0)
                {
                    ShowStatusMessage("Record not found for update!", "error");
                    return;
                }
            }

            // UPDATE or INSERT RES_CAT_MAINT_NEW
            string checkSql = "SELECT COUNT(*) FROM RES_CAT_MAINT_NEW WHERE RES_ID = :RES_ID";
            int recordExists = 0;

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand checkCmd = new OracleCommand(checkSql, con))
            {
                checkCmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                con.Open();
                recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());
            }

            if (recordExists > 0)
            {
                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(@"
                UPDATE RES_CAT_MAINT_NEW SET CAT_ID = :CAT_ID, CAT_NM = :CAT_NM, CAT_COST = :CAT_COST
                WHERE RES_ID = :RES_ID", con))
                {
                    cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
                    cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
                    cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
                    cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (OracleConnection con = new OracleConnection(connStr))
                using (OracleCommand cmd = new OracleCommand(@"
                INSERT INTO RES_CAT_MAINT_NEW (RES_ID, CAT_ID, CAT_NM, CAT_COST)
                VALUES (:RES_ID, :CAT_ID, :CAT_NM, :CAT_COST)", con))
                {
                    cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                    cmd.Parameters.Add("CAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
                    cmd.Parameters.Add("CAT_NM", OracleDbType.Varchar2).Value = ddlNewCategory.SelectedItem.Text;
                    cmd.Parameters.Add("CAT_COST", OracleDbType.Int32).Value = string.IsNullOrEmpty(txtMaintCharges.Text.Trim()) ? 0 : Convert.ToInt32(txtMaintCharges.Text.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Get RCAT_ID from RESIDENTIAL_INFO
            int rcatId = 0;
            string getRcatSql = "SELECT RCAT_ID FROM RESIDENTIAL_INFO WHERE RES_ID = :RES_ID";
            using (OracleConnection conTemp = new OracleConnection(connStr))
            using (OracleCommand getCmd = new OracleCommand(getRcatSql, conTemp))
            {
                getCmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                conTemp.Open();
                object result = getCmd.ExecuteScalar();
                rcatId = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }

            // CHECK IF RECORD EXISTS IN BILLS.RESID_INFO
            string checkBillsSql = "SELECT COUNT(*) FROM BILLS.RESID_INFO WHERE RES_ID_BK = :RES_ID_BK";
            int billsRecordExists = 0;

            using (OracleConnection con = new OracleConnection(connMain))
            using (OracleCommand checkCmd = new OracleCommand(checkBillsSql, con))
            {
                checkCmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;
                con.Open();
                billsRecordExists = Convert.ToInt32(checkCmd.ExecuteScalar());
            }

            if (billsRecordExists > 0)
            {
                using (OracleConnection con = new OracleConnection(connMain))
                using (OracleCommand cmd = new OracleCommand(@"
                UPDATE BILLS.RESID_INFO SET
                    RES_NAME = :RES_NAME, HOUSE_NO = :HOUSE_NO, CNIC_NO = :CNIC_NO,
                    FATHERNAME = :FATHERNAME, CONTACT_NO = :CONTACT_NO, 
                    RCAT_ID = :RCAT_ID_NEW,
                    PRECENT_ID = :PRECENT_ID, PRECENT_NM = :PRECENT_NM,
                    BLOCK_ID = :BLOCK_ID, BLOCK_NM = :BLOCK_NM, 
                    RES_ID = :RES_ID_NEW, RES_CODE = :RES_CODE_NEW
                WHERE RES_ID_BK = :RES_ID_BK", con))
                {
                    cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
                    cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
                    cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
                    cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
                    cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
                    cmd.Parameters.Add("RCAT_ID_NEW", OracleDbType.Int32).Value = rcatId > 0 ? rcatId : Convert.ToInt32(ddlCategory.SelectedValue);
                    cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
                    cmd.Parameters.Add("PRECENT_NM", OracleDbType.Varchar2).Value = ddlPrcnt.SelectedItem.Text;
                    cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
                    cmd.Parameters.Add("BLOCK_NM", OracleDbType.Varchar2).Value = ddlBlock.SelectedItem.Text;
                    cmd.Parameters.Add("RES_ID_NEW", OracleDbType.Int32).Value = electricId;
                    cmd.Parameters.Add("RES_CODE_NEW", OracleDbType.Varchar2).Value = "R-" + electricId.ToString().PadLeft(6, '0');
                    cmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (OracleConnection con = new OracleConnection(connMain))
                using (OracleCommand cmd = new OracleCommand(@"
                INSERT INTO BILLS.RESID_INFO (
                    RES_ID_BK, RES_ID, RES_CODE, RES_NAME, HOUSE_NO, CNIC_NO, 
                    FATHERNAME, CONTACT_NO, RCAT_ID, M_FACT_RATE, COMP_ID, 
                    PRECENT_ID, PRECENT_NM, BLOCK_ID, BLOCK_NM, 
                    BIL_WO_MAINT, BIL_WO_ELEC, BIL_WO_WATER, BIL_WO_GAS, 
                    BIL_WO_SOLAR, BIL_WO_RENT, EMP_STATUS, EMP_ID, LOG_ID
                ) VALUES (
                    :RES_ID_BK, :RES_ID, :RES_CODE, :RES_NAME, :HOUSE_NO, :CNIC_NO,
                    :FATHERNAME, :CONTACT_NO, :RCAT_ID, :M_FACT_RATE, :COMP_ID,
                    :PRECENT_ID, :PRECENT_NM, :BLOCK_ID, :BLOCK_NM,
                    :BIL_WO_MAINT, :BIL_WO_ELEC, :BIL_WO_WATER, :BIL_WO_GAS,
                    :BIL_WO_SOLAR, :BIL_WO_RENT, :EMP_STATUS, :EMP_ID, :LOG_ID
                )", con))
                {
                    cmd.Parameters.Add("RES_ID_BK", OracleDbType.Int32).Value = resID;
                    cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = electricId;
                    cmd.Parameters.Add("RES_CODE", OracleDbType.Varchar2).Value = "R-" + electricId.ToString().PadLeft(6, '0');
                    cmd.Parameters.Add("RES_NAME", OracleDbType.Varchar2).Value = txtResName.Text.Trim();
                    cmd.Parameters.Add("HOUSE_NO", OracleDbType.Varchar2).Value = txtAddress.Text.Trim();
                    cmd.Parameters.Add("CNIC_NO", OracleDbType.Varchar2).Value = txtCNIC.Text.Trim();
                    cmd.Parameters.Add("FATHERNAME", OracleDbType.Varchar2).Value = txtFatherName.Text.Trim();
                    cmd.Parameters.Add("CONTACT_NO", OracleDbType.Varchar2).Value = txtContact.Text.Trim();
                    cmd.Parameters.Add("RCAT_ID", OracleDbType.Int32).Value = rcatId > 0 ? rcatId : Convert.ToInt32(ddlCategory.SelectedValue);
                    cmd.Parameters.Add("M_FACT_RATE", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("COMP_ID", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("PRECENT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlPrcnt.SelectedValue);
                    cmd.Parameters.Add("PRECENT_NM", OracleDbType.Varchar2).Value = ddlPrcnt.SelectedItem.Text;
                    cmd.Parameters.Add("BLOCK_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlBlock.SelectedValue);
                    cmd.Parameters.Add("BLOCK_NM", OracleDbType.Varchar2).Value = ddlBlock.SelectedItem.Text;
                    cmd.Parameters.Add("BIL_WO_MAINT", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("BIL_WO_ELEC", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("BIL_WO_WATER", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("BIL_WO_GAS", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("BIL_WO_SOLAR", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("BIL_WO_RENT", OracleDbType.Int32).Value = 1;
                    cmd.Parameters.Add("EMP_STATUS", OracleDbType.Int32).Value = 0;
                    cmd.Parameters.Add("EMP_ID", OracleDbType.Int32).Value = 0;
                    cmd.Parameters.Add("LOG_ID", OracleDbType.Int32).Value = 0;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            using (OracleConnection con = new OracleConnection(connStr))
            using (OracleCommand cmd = new OracleCommand("UPDATE RESIDENTIAL_INFO SET NCAT_ID = :NCAT_ID WHERE RES_ID = :RES_ID", con))
            {
                cmd.Parameters.Add("NCAT_ID", OracleDbType.Int32).Value = Convert.ToInt32(ddlNewCategory.SelectedValue);
                cmd.Parameters.Add("RES_ID", OracleDbType.Int32).Value = resID;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            ShowStatusMessage("Record updated successfully! Electric ID: " + electricId, "success");
            pcdInit();
        }
        catch (Exception ex)
        {
            ShowStatusMessage("Error: " + ex.Message, "error");
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

        string script = "setTimeout(function() { var elem = document.getElementById('" + statusMessage.ClientID + "'); if(elem) elem.style.display = 'none'; }, 2000);";
        ClientScript.RegisterStartupScript(this.GetType(), "HideStatus_" + Guid.NewGuid().ToString(), script, true);
    }
}