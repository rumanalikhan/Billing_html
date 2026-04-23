<%@ Page Language="C#" AutoEventWireup="true" CodeFile="meter_inst.aspx.cs" Inherits="meter_inst" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Meter Installment Plan</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f7f7f7;
        }

        /* Top Bar Styles */
        .top-bar {
            background: white;
            padding: 12px 30px;
            display: flex;
            justify-content: flex-end;
            gap: 15px;
            border-bottom: 1px solid #ddd;
        }

        .top-btn {
            background: black;
            color: white;
            border: none;
            padding: 8px 20px;
            font-size: 16px;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
        }

        .top-btn:hover {
            background: #333;
        }

        .container {
            display: flex;
            justify-content: center;
            padding: 40px;
        }

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
            font-size: 28px;
        }

        .left-panel ul {
            list-style: disc;
            padding-left: 20px;
        }

        .left-panel ul li {
            margin-bottom: 10px;
            font-size: 16px;
        }

        .form-panel {
            background: white;
            padding: 30px;
            border-radius: 8px;
            flex: 2;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .form-panel h2 {
            font-size: 40px;
            margin-bottom: 20px;
        }

        /* Status message below heading */
        .status-container {
            margin-bottom: 25px;
        }

        .status-message {
            padding: 12px 20px;
            border-radius: 4px;
            display: none;
            font-size: 16px;
        }

        .status-success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .status-error {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .status-info {
            background: #d1ecf1;
            color: #0c5460;
            border: 1px solid #bee5eb;
        }

        .row {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
        }

        .col {
            flex: 1;
        }

        label {
            font-size: 22px;
            font-weight: 600;
            display: block;
            margin-bottom: 5px;
        }

        .asp-input, select {
            width: 100%;
            height: 55px;
            font-size: 20px;
            padding: 8px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            margin-top: 5px;
            background: white;
        }

        .asp-input[readonly] {
            background: #f5f5f5;
            cursor: not-allowed;
        }

        .radio-group {
            display: flex;
            gap: 30px;
            align-items: center;
            height: 55px;
            margin-top: 5px;
        }

        .radio-group label {
            font-size: 18px;
            font-weight: normal;
            display: inline;
            margin-left: 8px;
        }

        .radio-group input {
            width: 20px;
            height: 20px;
        }

        .info-box {
            background: #e8f4f8;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 25px;
            border-left: 4px solid #0f7c57;
        }

        .info-box h4 {
            margin: 0 0 15px 0;
            color: #0f7c57;
            font-size: 20px;
        }

        .info-row {
            display: flex;
            gap: 20px;
            margin-bottom: 10px;
        }

        .info-item {
            flex: 1;
        }

        .info-item strong {
            font-size: 16px;
        }

        .info-item span {
            font-size: 16px;
        }

        .btn-row {
            margin-top: 25px;
            display: flex;
            gap: 15px;
        }

        .asp-button {
            background: #2d2a26;
            color: #fff;
            padding: 15px 30px;
            font-size: 22px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }

        .asp-button:hover {
            background: #444;
        }
    </style>

    <script type="text/javascript">
        function calculateTotals() {
            var prevOutstanding = parseFloat(document.getElementById('<%= txtPrevOutstanding.ClientID %>').value) || 0;
            var meterCost = parseFloat(document.getElementById('<%= txtMeterCost.ClientID %>').value) || 62000;
            var defaultCharges = prevOutstanding * 0.10;

            document.getElementById('<%= txtDefaultCharges.ClientID %>').value = defaultCharges.toFixed(0);

            var rb3Months = document.getElementById('<%= rb3Months.ClientID %>');
            var rb6Months = document.getElementById('<%= rb6Months.ClientID %>');
            var instRate = 0;
            var installments = 0;

            if (rb3Months && rb3Months.checked) {
                instRate = 21000;
                installments = 3;
            } else if (rb6Months && rb6Months.checked) {
                instRate = 11000;
                installments = 6;
            }

            document.getElementById('<%= txtInstRate.ClientID %>').value = instRate;
            document.getElementById('<%= txtInstallments.ClientID %>').value = installments;
        }

        function updatePlan() {
            calculateTotals();
        }
    </script>
</head>

<body>

<form id="form1" runat="server">
    <!-- Top Bar with Go Back and Log Off buttons -->
    <div class="top-bar">
        <asp:Button ID="btnGoBack" runat="server" Text="Go Back" CssClass="top-btn" OnClick="btnGoBack_Click" />
        <asp:Button ID="btnLogOff" runat="server" Text="Log Off" CssClass="top-btn" OnClick="btnLogOff_Click" />
    </div>

    <div class="container">

        <!-- LEFT PANEL -->
        <div class="left-panel">
            <h2>Meter Installment</h2>
            <ul>
                <li>Search consumer by barcode</li>
                <li>Select installment plan</li>
                <li>Previous outstanding auto-loaded</li>
                <li>Save to create plan</li>
            </ul>
        </div>

        <!-- FORM PANEL -->
        <div class="form-panel">
            <h2>Meter Installment Plan</h2>

            <!-- Status Message Below Heading -->
            <div class="status-container">
                <div id="statusMessage" runat="server" class="status-message"></div>
            </div>

            <!-- Search Row -->
            <div class="row">
                <div class="col">
                    <label>Search by Barcode / Reference Code</label>
                    <asp:TextBox ID="txtSearchBarcode" runat="server" CssClass="asp-input" AutoPostBack="true" OnTextChanged="txtSearchBarcode_TextChanged" />
                </div>
                <div class="col" style="max-width: 150px;">
                    <label>&nbsp;</label>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="asp-button" OnClick="btnSearch_Click" style="width: 100%;" />
                </div>
            </div>

            <!-- Consumer Information Box -->
            <div class="info-box">
                <h4>Consumer Information</h4>
                <div class="info-row">
                    <div class="info-item"><strong>Consumer Name:</strong> <asp:Label ID="lblConsumerName" runat="server" Text="-" /></div>
                    <div class="info-item"><strong>Resident ID:</strong> <asp:Label ID="lblResid" runat="server" Text="-" /></div>
                    <div class="info-item"><strong>Meter No:</strong> <asp:Label ID="lblMeterNo" runat="server" Text="-" /></div>
                </div>
                <div class="info-row">
                    <div class="info-item"><strong>Address:</strong> <asp:Label ID="lblAddress" runat="server" Text="-" /></div>
                </div>
                <div class="info-row">
                    <div class="info-item"><strong>Precinct:</strong> <asp:Label ID="lblPrecinct" runat="server" Text="-" /></div>
                    <div class="info-item"><strong>Block:</strong> <asp:Label ID="lblBlock" runat="server" Text="-" /></div>
                    <div class="info-item"><strong>Bill Month:</strong> <asp:Label ID="lblBillMonth" runat="server" Text="-" /></div>
                </div>
            </div>

            <!-- Hidden Fields -->
            <asp:HiddenField ID="hdnBarcode" runat="server" />
            <asp:HiddenField ID="hdnResid" runat="server" />
            <asp:HiddenField ID="hdnBillMonth" runat="server" />
            <asp:HiddenField ID="hdnExistingSrno" runat="server" />

            <!-- Installment Plan -->
            <div class="row">
                <div class="col">
                    <label>Select Installment Plan</label>
                    <div class="radio-group">
                        <asp:RadioButton ID="rb3Months" runat="server" GroupName="InstPlan" onclick="updatePlan()" />
                        <label>3 Months</label>
                        <asp:RadioButton ID="rb6Months" runat="server" GroupName="InstPlan" onclick="updatePlan()" />
                        <label>6 Months</label>
                    </div>
                </div>
            </div>

            <!-- Amount Details - READ ONLY -->
            <div class="row">
                <div class="col">
                    <label>Meter Cost</label>
                    <asp:TextBox ID="txtMeterCost" runat="server" CssClass="asp-input" Text="62000" ReadOnly="true" />
                </div>
                <div class="col">
                    <label>Previous Outstanding</label>
                    <asp:TextBox ID="txtPrevOutstanding" runat="server" CssClass="asp-input" ReadOnly="true" />
                </div>
                <div class="col">
                    <label>Default Charges (10%)</label>
                    <asp:TextBox ID="txtDefaultCharges" runat="server" CssClass="asp-input" ReadOnly="true" />
                </div>
            </div>

            <!-- Installment Details - EDITABLE -->
            <div class="row">
                <div class="col">
                    <label>Number of Installments</label>
                    <asp:TextBox ID="txtInstallments" runat="server" CssClass="asp-input" onchange="updatePlan()" />
                </div>
                <div class="col">
                    <label>Installment Amount (per month)</label>
                    <asp:TextBox ID="txtInstRate" runat="server" CssClass="asp-input" onchange="updatePlan()" />
                </div>
                <div class="col">
                    <label>&nbsp;</label>
                    <div style="height: 55px;"></div>
                </div>
            </div>

            <!-- Buttons -->
            <div class="btn-row">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="asp-button" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>
</form>

<script>
    // Initialize calculations on page load
    setTimeout(function () { calculateTotals(); }, 100);
</script>
</body>
</html>