<%@ Page Language="C#" AutoEventWireup="true" CodeFile="resid_info.aspx.cs" Inherits="resid_info" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
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
            margin-bottom: 12px;
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
            height: 50px;
            padding: 10px;
            font-size: 16px;
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
            margin-right: 10px;
        }

        .asp-button:hover {
            background: #444;
        }
        
        .status-message {
            margin-top: 20px;
            padding: 10px;
            border-radius: 4px;
            display: none;
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            min-width: 300px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.2);
        }
        
        .status-success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .status-error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .status-info {
            background-color: #d1ecf1;
            color: #0c5460;
            border: 1px solid #bee5eb;
        }
        
        /* Loading indicator */
        .loading {
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 3px solid #f3f3f3;
            border-top: 3px solid #3498db;
            border-radius: 50%;
            animation: spin 1s linear infinite;
            margin-left: 10px;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>
    
    <script type="text/javascript">
        // JavaScript to manually trigger postback on Enter key
        function handleResIdEnter(event) {
            if (event.keyCode === 13) { // Enter key
                event.preventDefault();
                __doPostBack('<%= txtResId.UniqueID %>', '');
                return false;
            }
            return true;
        }

        // Show loading indicator
        function showLoading() {
            var btn = document.getElementById('<%= btnSave.ClientID %>');
            if (btn) {
                btn.disabled = true;
                btn.value = 'Loading...';
            }
        }
    </script>
</head>

<body>

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
                    <asp:TextBox ID="txtRegNo" runat="server" CssClass="asp-input" AutoPostBack="true" OnTextChanged="txtRegNo_TextChanged" />
                </div>
                <div class="col">
                    <label>Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="asp-input"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>New Category</label>
                    <asp:DropDownList ID="ddlNewCategory" runat="server" CssClass="asp-input"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>Maintenance Charges</label>
                    <asp:TextBox ID="txtMaintCharges" runat="server" CssClass="asp-input" />
                </div>
                <div class="col">
                    <label>Residential ID</label>
                    <asp:TextBox ID="txtResId" runat="server" CssClass="asp-input" Enabled="false" AutoPostBack="true" OnTextChanged="txtResId_TextChanged" onkeypress="return handleResIdEnter(event)" />
                </div>
                <div class="col">
                    <label>Residential ID (Electric)</label>
                    <asp:TextBox ID="txtResIdE" runat="server" CssClass="asp-input" Enabled="false" />
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
                    <asp:DropDownList ID="ddlPrcnt" runat="server" CssClass="asp-input"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>Block</label>
                    <asp:DropDownList ID="ddlBlock" runat="server" CssClass="asp-input"></asp:DropDownList>
                </div>
                <div class="col">
                    <label>Street</label>
                    <asp:TextBox ID="txtStreet" runat="server" CssClass="asp-input" />
                </div>
            </div>

            <div class="row">
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
            </div>
            <div class="btn-row">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="asp-button" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" />
                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="asp-button" OnClick="btnEdit_Click" />
            </div>
            
            <div id="statusMessage" runat="server" class="status-message"></div>
        </div>
    </div>
</form>
</body>
</html>