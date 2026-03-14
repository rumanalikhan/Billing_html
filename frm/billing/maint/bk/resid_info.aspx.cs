using System;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client; // <-- Added namespace for Oracle

public partial class resid_info : System.Web.UI.Page
{
    string connStr = WebConfigurationManager
                        .ConnectionStrings["MyDbConnectionMNT"]
                        .ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) // sirf first load pe set karna
        {
            pcdInit();

            // Current system date
            DateTime dt = DateTime.Now;

            // Format as "dd-MMM-yy" (e.g., 19-Dec-25)
            txtConnectionDate.Text = dt.ToString("dd-MMM-yy");
        }
    }

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!IsPostBack) // sirf first load pe set karna
    //    {
    //        txtConnectionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
    //    }
    //}

    protected void pcdInit()
    {
        txtRegNo.Text = "";
        txtResId.Text = "";
        txtResIdE.Text = "";
        txtResName.Text = "";
        txtAddress.Text = "";
        txtCNIC.Text = "";
        txtContact.Text = "";
        txtCategory.Text = "";
        txtPrcnt.Text = "";
        txtBlock.Text = "";
        txtRegNo.Focus();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        pcdInit();
        txtRegNo.Focus();
    }

    protected void txtRegNo_TextChanged(object sender, EventArgs e)
    {
        string regNo = txtRegNo.Text.Trim();

        if (string.IsNullOrEmpty(regNo))
        {
            txtResName.Text = "";
            return;
        }

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
                        if (reader.Read()) // agar row exist karti hai
                        {
                            txtResName.Text = reader["OWNER_NAME"] != DBNull.Value
                                              ? reader["OWNER_NAME"].ToString()
                                              : "";

                            txtAddress.Text = reader["OWNER_ADDRESS"] != DBNull.Value
                                              ? reader["OWNER_ADDRESS"].ToString()
                                              : "";

                            txtCNIC.Text = reader["OWNER_CNIC"] != DBNull.Value
                                              ? reader["OWNER_CNIC"].ToString()
                                              : "";

                            txtContact.Text = reader["OWNER_MOBILE"] != DBNull.Value
                                              ? reader["OWNER_MOBILE"].ToString()
                                              : "";

                            txtCategory.Text = reader["CATEGORY_NAME"] != DBNull.Value
                                              ? reader["CATEGORY_NAME"].ToString()
                                              : "";

                            txtPrcnt.Text = reader["PRECINCT_NAME"] != DBNull.Value
                                              ? reader["PRECINCT_NAME"].ToString()
                                              : "";

                            txtBlock.Text = reader["BLOCK_NAME"] != DBNull.Value
                                              ? reader["BLOCK_NAME"].ToString()
                                              : "";

                            txtMaintCharges.Focus();
                        }
                        else
                        {
                            txtResName.Text = "";
                            txtAddress.Text = "";
                            txtCNIC.Text = "";
                            txtContact.Text = "";
                            txtCategory.Text = "";
                            txtPrcnt.Text = "";
                            txtBlock.Text = "";

                            txtResId.Focus();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Optional: show error or log
            txtResName.Text = "Error: " + ex.Message;
        }
    }
}
