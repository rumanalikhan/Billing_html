<%@ Page Language="C#" AutoEventWireup="true" CodeFile="resid_info.aspx.cs" Inherits="resid_info" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Residential Information</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f7f7f7;
            color: #333;
        }

        header {
            background: #2d2a26;
            color: white;
            padding: 15px 40px;
            font-size: 20px;
            font-weight: bold;
        }

        .container {
            display: flex;
            justify-content: center;
            padding: 40px;
        }

        /* LEFT PANEL */
        .left-panel {
            background: #0f7c57;
            color: white;
            padding: 30px;
            border-radius: 8px;
            flex: 1;
            max-width: 300px;
            margin-right: 20px;
        }

        .left-panel h2 {
            margin-bottom: 20px;
        }

        .left-panel ul {
            list-style: disc;
            padding-left: 20px;
        }

        .left-panel ul li {
            margin-bottom: 10px;
        }

        /* FORM PANEL */
        .form-panel {
            background: white;
            padding: 30px;
            border-radius: 8px;
            flex: 2;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .form-panel h2 {
            font-size: 45px;
            font-weight: 600;
            margin-bottom: 30px;
        }

        .row {
            display: flex;
            gap: 20px;
            margin-bottom: 20px;
        }

        .col {
            flex: 1;
        }

        label {
            display: block;
            padding-left: 10px;
            padding-bottom: 5px;
            font-size: 22px;
            font-weight: 600;
        }

        .asp-input, textarea, select {
            width: 100%;
            height: 65px;
            padding: 10px;
            font-size: 26px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
        }

        textarea {
            height: 100px;
            resize: none;
        }

        .btn-row {
            margin-top: 30px;
        }

        .asp-button {
            background: #2d2a26;
            color: white;
            border: none;
            padding: 15px 30px;
            font-size: 26px;
            border-radius: 4px;
            cursor: pointer;
        }

        .asp-button:hover {
            background: #444;
        }
    </style>
</head>

<body>
<%--<header>
    Residential Information
</header>--%>

<form id="form1" runat="server">
    <div class="container">

        <!-- LEFT INFO PANEL -->
        <div class="left-panel">
            <h2>Resident Details</h2>
            <ul>
                <li>Basic information</li>
                <li>Identification details</li>
                <li>Contact information</li>
                <li>Client category</li>
            </ul>
        </div>

        <!-- FORM PANEL -->
        <div class="form-panel">
            <h2>Residential Info</h2>

            <div class="row">
                <div class="col">
                    <label>Registration Number</label>
                    <asp:TextBox ID="txtRegNo" runat="server" CssClass="asp-input" AutoPostBack="true" OnTextChanged="txtRegNo_TextChanged" onkeydown="return txtRegNo_KeyDown(event);" />
                </div>
                <div class="col">
                    <label>Residential ID</label>
                    <asp:TextBox ID="txtResId" runat="server" CssClass="asp-input" />
                </div>
                <div class="col">
                    <label>Residential ID (Electric)</label>
                    <asp:TextBox ID="txtResIdE" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Resident Name</label>
                    <asp:TextBox ID="txtResName" runat="server" CssClass="asp-input" />
                </div>
                <div class="col">
                    <label>Father Name</label>
                    <asp:TextBox ID="txtFatherName" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Address</label>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Precinct</label>
                    <asp:TextBox ID="txtPrcnt" runat="server" CssClass="asp-input" />
<%--                    <asp:DropDownList ID="ddPrcnt" runat="server" CssClass="asp-input">
                        <asp:ListItem Text="-- Select --" />
                        <asp:ListItem Text="Residential" />
                        <asp:ListItem Text="Commercial" />
                    </asp:DropDownList>--%>
                </div>
                <div class="col">
                    <label>Block</label>
                    <asp:TextBox ID="txtBlock" runat="server" CssClass="asp-input" />
<%--                    <asp:DropDownList ID="ddBlock" runat="server" CssClass="asp-input">
                        <asp:ListItem Text="-- Select --" />
                        <asp:ListItem Text="Residential" />
                        <asp:ListItem Text="Commercial" />
                    </asp:DropDownList>--%>
                </div>
                <div class="col">
                    <label>Connection Date</label>
                        <asp:TextBox 
                            ID="txtConnectionDate" 
                            runat="server" 
                            CssClass="asp-input" />
<%--                    <asp:TextBox 
                        ID="txtConnectionDate" 
                        runat="server" 
                        CssClass="asp-input" 
                        TextMode="Date" />--%>
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>NTN No</label>
                    <asp:TextBox ID="txtNTN" runat="server" CssClass="asp-input" />
                </div>
                <div class="col">
                    <label>CNIC Number</label>
                    <asp:TextBox ID="txtCNIC" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Contact No</label>
                    <asp:TextBox ID="txtContact" runat="server" CssClass="asp-input" />
                </div>
                <div class="col">
                    <label>Category</label>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="asp-input" />
<%--                    <asp:DropDownList ID="ddlClientCategory" runat="server" CssClass="asp-input">
                        <asp:ListItem Text="-- Select --" />
                        <asp:ListItem Text="Residential" />
                        <asp:ListItem Text="Commercial" />
                    </asp:DropDownList>--%>
                </div>
                <div class="col">
                    <label>Maintenance Charges</label>
                    <asp:TextBox ID="txtMaintCharges" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Comments</label>
                    <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" />
                </div>
            </div>

            <div class="btn-row">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="asp-button" />
<%--                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" />--%>
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" />
            </div>

        </div>
    </div>
</form>
</body>
</html>

<script type="text/javascript">
    function txtRegNo_KeyDown(e) {
        if (e.key === "Enter") {
            e.preventDefault();
            __doPostBack('<%= txtRegNo.UniqueID %>', '');
            return false;
        }
        return true;
    }
</script>
