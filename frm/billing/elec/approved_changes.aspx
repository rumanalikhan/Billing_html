<%@ Page Language="C#" AutoEventWireup="true" CodeFile="approved_changes.aspx.cs" Inherits="approved_changes" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Approved Electricity Bill</title>

    <style>
        body {
            margin: 0;
            font-family: Arial;
            background: #f7f7f7;
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
            padding: 30px;
        }

        .left-panel {
            width: 260px;
            background: #0f7c57;
            color: white;
            padding: 20px;
            border-radius: 6px;
            margin-right: 20px;
        }

        .form-panel {
            flex: 1;
            background: white;
            padding: 25px;
            border-radius: 6px;
            box-shadow: 0 2px 6px rgba(0,0,0,.1);
        }

        .form-panel h2 {
            margin-bottom: 25px;
        }

        .row-wrapper {
            display: flex;
            gap: 40px;       /* space between left & right */
            margin-bottom: 18px;
        }

        .row_left,
        .row_right {
            display: flex;
            gap: 20px;
            margin-bottom: 18px;
            width: 700px;
        }

        /*.row_left {
            display: flex;
            gap: 20px;
            margin-bottom: 18px;
            width:700px;
        }

        .row_right {
            display: flex;
            gap: 20px;
            margin-bottom: 18px;
            width:700px;
            float:left;
        }*/

        .col_left {
            flex: 1;
        }

        label {
            font-weight: bold;
            display: block;
            margin-bottom: 6px;
        }

        .asp-input {
            width: 100%;
            height: 45px;
            padding: 6px 10px;
            font-size: 16px;
            border-radius: 4px;
            border: 1px solid #ccc;
        }

        .asp-button {
            background: #2d2a26;
            color: white;
            border: none;
            padding: 12px 24px;
            font-size: 16px;
            border-radius: 4px;
            cursor: pointer;
        }

        /* ===== VERTICAL K1 FORM ===== */
        .v-form {
            max-width: 520px;
            margin-bottom: 30px;
        }

        .v-row {
            display: flex;
            align-items: center;
            margin-bottom: 12px;
        }

        .k1-col {
            display: flex;
            gap: 20px;
            margin-bottom: 3px;
        }

        .v-label {
            width: 200px;
            font-weight: bold;
        }

        .v-input {
            flex: 1;
            height: 25px;
            padding: 6px 10px;
            font-size: 20px;
            border-radius: 4px;
        }

        .blue {
            border: 2px solid blue;
        }

        .black {
            border: 2px solid black;
            background: #f2f2f2;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

<header>Approved Electricity Bill</header>

<div style="text-align:center; font-weight:bold; margin:10px">
    <asp:Label ID="lblUser" runat="server" ForeColor="Blue"></asp:Label>
</div>

<div class="container">

    <!-- LEFT PANEL -->
    <div class="left-panel">
        <h3>Instructions</h3>
        <ul>
            <li>Select Maintenance Code from Drop Down</li>
            <li>Press Search button</li>
            <li>Check you bill Before and After</li>
            <li>Click Post to save</li>
        </ul>
    </div>

    <!-- FORM PANEL -->
    <div class="form-panel">

        <h2>Bill Details</h2>

        <!-- BTK -->
        <div class="row">
            <div class="col_left">
                <label>BTK Number</label>
                <asp:DropDownList ID="ddlBTKNo" runat="server"
                    CssClass="asp-input"
                    AutoPostBack="true">
                </asp:DropDownList>

                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="asp-button" OnClick="btnSearch_Click" />
                &nbsp;
                <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="asp-button" OnClick="btnExport_Click" /> 
        </div>

        <!-- STATUS -->
        <div class="row_left">
            <div class="col_left">
                <asp:Label ID="lblStatus" runat="server"
                    Style="font-size:18px;font-weight:bold;" />
            </div>
        </div>

        <!-- ===== K1 READINGS (VERTICAL) ===== -->
        <div class="row-wrapper">
            <!-- ROW 1 -->
            <div class="k1-row">
                <div class="k1-col">
                    <span class="v-label">Read K1 From</span>
                    <asp:TextBox ID="txtK1From_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K1 To</span>
                    <asp:TextBox ID="txtK1To_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">K1 Difference</span>
                    <asp:TextBox ID="txtElDiff_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Multiply Factor</span>
                    <asp:TextBox ID="txtMFactor_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Units</span>
                    <asp:TextBox ID="txtUnitsEl_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Rate</span>
                    <asp:TextBox ID="txtUnitRate_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K1)</span>
                    <asp:TextBox ID="txtBillCost_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K2 From</span>
                    <asp:TextBox ID="txtK2From_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K2 To</span>
                    <asp:TextBox ID="txtK2To_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Units</span>
                    <asp:TextBox ID="txtSlDiff_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Units Adjusted</span>
                    <asp:TextBox ID="txtUnitAdj_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Adj. Rates</span>
                    <asp:TextBox ID="txtAdjRate_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K2)</span>
                    <asp:TextBox ID="txtK2Amt_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">NEPRA RATE</span>
                    <asp:TextBox ID="txtFixedRate_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Adj</span>
                    <asp:TextBox ID="txtNetUnit_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Pruchase</span>
                    <asp:TextBox ID="txtUnitPurchase_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K3)</span>
                    <asp:TextBox ID="txtK3Amt_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Amount</span>
                    <asp:TextBox ID="txtTltAmt_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Previous Balance</span>
                    <asp:TextBox ID="txtPrevBal_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-1</span>
                    <asp:TextBox ID="txtQtr1_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-2</span>
                    <asp:TextBox ID="txtQtr2_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-3</span>
                    <asp:TextBox ID="txtQtr3_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-4</span>
                    <asp:TextBox ID="txtQtr4_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Amt</span>
                    <asp:TextBox ID="txtTotAmt_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">DC Charges</span>
                    <asp:TextBox ID="txtDcCharges_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Advance</span>
                    <asp:TextBox ID="txtAdvance_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Arrear</span>
                    <asp:TextBox ID="txtArrear_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Instalment</span>
                    <asp:TextBox ID="txtInstalment_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Fine</span>
                    <asp:TextBox ID="txtFine_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">O & M Charges</span>
                    <asp:TextBox ID="txtONMCharges_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Bill Amt</span>
                    <asp:TextBox ID="txtBillAmt_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Late Payment</span>
                    <asp:TextBox ID="txtLatePayment_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">After Due Dt</span>
                    <asp:TextBox ID="txtAfterDue_Left" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
            </div>
            <!-- ROW 2 -->
            <div class="k1-row">
                <div class="k1-col">
                    <span class="v-label">Read K1 From</span>
                    <asp:TextBox ID="txtK1From_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K1 To</span>
                    <asp:TextBox ID="txtK1To_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">K1 Difference</span>
                    <asp:TextBox ID="txtElDiff_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Multiply Factor</span>
                    <asp:TextBox ID="txtMFactor_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Units</span>
                    <asp:TextBox ID="txtUnitsEl_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Rate</span>
                    <asp:TextBox ID="txtUnitRate_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K1)</span>
                    <asp:TextBox ID="txtBillCost_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K2 From</span>
                    <asp:TextBox ID="txtK2From_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Read K2 To</span>
                    <asp:TextBox ID="txtK2To_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Units</span>
                    <asp:TextBox ID="txtSlDiff_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Units Adjusted</span>
                    <asp:TextBox ID="txtUnitAdj_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Adj. Rates</span>
                    <asp:TextBox ID="txtAdjRate_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K2)</span>
                    <asp:TextBox ID="txtK2Amt_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">NEPRA RATE</span>
                    <asp:TextBox ID="txtFixedRate_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Adj</span>
                    <asp:TextBox ID="txtNetUnit_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Unit Pruchase</span>
                    <asp:TextBox ID="txtUnitPurchase_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Amount (K3)</span>
                    <asp:TextBox ID="txtK3Amt_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Amount</span>
                    <asp:TextBox ID="txtTltAmt_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Previous Balance</span>
                    <asp:TextBox ID="txtPrevBal_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-1</span>
                    <asp:TextBox ID="txtQtr1_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-2</span>
                    <asp:TextBox ID="txtQtr2_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-3</span>
                    <asp:TextBox ID="txtQtr3_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Qtr-4</span>
                    <asp:TextBox ID="txtQtr4_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Total Amt</span>
                    <asp:TextBox ID="txtTotAmt_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">DC Charges</span>
                    <asp:TextBox ID="txtDcCharges_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Advance</span>
                    <asp:TextBox ID="txtAdvance_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Arrear</span>
                    <asp:TextBox ID="txtArrear_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Instalment</span>
                    <asp:TextBox ID="txtInstalment_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Fine</span>
                    <asp:TextBox ID="txtFine_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">O & M Charges</span>
                    <asp:TextBox ID="txtONMCharges_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Bill Amt</span>
                    <asp:TextBox ID="txtBillAmt_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">Late Payment</span>
                    <asp:TextBox ID="txtLatePayment_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
                <div class="k1-col">
                    <span class="v-label">After Due Dt</span>
                    <asp:TextBox ID="txtAfterDue_Right" runat="server" CssClass="v-input black" ReadOnly="true" />
                </div>
            </div>

        </div>
        <!-- BUTTONS -->
        <div style="margin-top:30px">
            <asp:Button ID="btnPost" runat="server" Text="Post" CssClass="asp-button" OnClick="btnPost_Click" /> 
            &nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" /> 
        </div>

    </div>
</div>

</form>
</body>
</html>
