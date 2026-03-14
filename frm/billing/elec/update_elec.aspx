<%@ Page Language="C#" AutoEventWireup="true" CodeFile="update_elec.aspx.cs" Inherits="update_elec" %>

<!DOCTYPE html>
<html>
    <head id="Head1" runat="server">
        <title>Update Electricity Bill</title>
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

            .blue-border {
                border: 2px solid blue;
                border-radius: 4px;
                padding: 4px;
            }
        </style>
    </head>

    <script type="text/javascript">
        function Common_KeyDown(e, ctrl) {
            e = e || window.event;

            if (e.key === "Enter" || e.keyCode === 13) {
                debugger;
                e.preventDefault();

                if (ctrl.id === '<%= txtBTKNo.ClientID %>') {
                    __doPostBack('<%= btnFetchRecord.UniqueID %>', '');
                }
                return false;
            }
            return true;
        }
    </script>

     <body>
        <form id="form1" runat="server">
            <header>
                Update Electricity Bill
            </header>
            <div id="header_user" style="width:56%;text-align:center; font-weight:bold;">
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue"></asp:Label>
            </div>


            <div class="container">
                <!-- LEFT PANEL -->
                <div class="left-panel" style="border:1px solid black;">
                    <h2>Instructions</h2>
                    <ul>
                        <li>Fill all fields carefully</li>
                        <li>Use correct BTK Maintenance Code</li>
                        <li>Click Post to save</li>
                        <li>Click Cancel to reset</li>
                        <li>Click Fix Bill for ReCalculation</li>
                    </ul>
                </div>

                <!-- FORM PANEL -->
                <div class="form-panel">
                    <h2>Bill Details</h2>

                    <!-- Row 1: BTK, Barcode, Res ID -->
                    <div class="row">
                        <div class="col">
                            <label>BTK Number</label>
                                <asp:TextBox ID="txtBTKNo" runat="server" CssClass="asp-input" onkeydown="return Common_KeyDown(event, this);" />
                                <asp:Button ID="btnFetchRecord" runat="server" Text="Fetch" OnClick="pcdFetchRecord" Style="display:none" />
                        </div>

                        <div class="col">
                            <label>Barcode</label>
                            <asp:TextBox ID="txtBarcode" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                        <div class="col">
                            <label>Res. ID</label>
                            <asp:TextBox ID="txtResID" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>

                        <!-- Buttons -->
<%--                        <div class="btn-row">
                            <asp:FileUpload ID="fuAttachment" runat="server" CssClass="asp-input" accept=".pdf" />
                            <asp:Button ID="btnAttachment" runat="server" Text="..." CssClass="asp-button" OnClick="pcdFileAttachment" />
                        </div>--%>
                    </div>

                    <!-- STATUS -->
                    <div class="row">
                        <div class="col">
                            <asp:Label ID="lblStatus" runat="server" style="font-size: 20px;font-weight: bold;" />
                        </div>
                    </div>

                    <!-- Name -->
                    <div class="row">
                        <div class="col">
                            <label>Name</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="asp-input"  ReadOnly="true"/>
                        </div>
                    </div>

                    <!-- Address -->
                    <div class="row">
                        <div class="col">
                            <label>Address</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="asp-input" TextMode="MultiLine"  ReadOnly="true"/>
                        </div>
                    </div>
                    <!-- Readings -->
                    <div class="row">
                        <div class="col">
                            <label>Read K1 From</label>
                            <asp:TextBox ID="txtK1From" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>Read K1 To</label>
                            <asp:TextBox ID="txtK1To" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>K1 To - K1 From</label>
                            <asp:TextBox ID="txtElDiff" runat="server" CssClass="asp-input" Style="border: 2px solid black; border-radius: 4px;" ReadOnly="true"/>
                        </div>
                        <div class="col">
                            <label>Multiply Factor</label>
                            <asp:TextBox ID="txtMFactor" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>Electric Units</label>
                            <asp:TextBox ID="txtUnitsEl" runat="server" CssClass="asp-input" Style="border: 2px solid black; border-radius: 4px;" ReadOnly="true"/>
                        </div>
                        <div class="col">
                            <label>Units Rate</label>
                            <asp:TextBox ID="txtUnitRate" runat="server" CssClass="asp-input" Style="border: 2px solid black; border-radius: 4px;" />
<%--                            <asp:TextBox ID="TextBox1" runat="server" CssClass="asp-input" Style="border: 2px solid black; border-radius: 4px;" ReadOnly="true"/>--%>
                        </div>
                    </div>

                    <!-- Financials -->
                    <div class="row">
                        <div class="col">
                            <label>Advance</label>
                            <asp:TextBox ID="txtAdvance" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>Arrear</label>
                            <asp:TextBox ID="txtArrear" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>Instalment</label>
                            <asp:TextBox ID="txtInstalment" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>Fine</label>
                            <asp:TextBox ID="txtFine" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                        <div class="col">
                            <label>DC Charges</label>
                            <asp:TextBox ID="txtDcCharges" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>O & M Charges</label>
                            <asp:TextBox ID="txtONMCharges" runat="server" CssClass="asp-input"  />
                        </div>
                        <div class="col">
                            <label>Bill Cost</label>
                            <asp:TextBox ID="txtBillCost" runat="server" CssClass="asp-input"  ReadOnly="true"/>
                        </div>
                        <div class="col">
                            <label>Bill Amt</label>
                            <asp:TextBox ID="txtBillAmt" runat="server" CssClass="asp-input"  ReadOnly="true"/>
                        </div>
                        <div class="col">
                            <label>Late Payment</label>
                            <asp:TextBox ID="txtLatePayment" runat="server" CssClass="asp-input"  ReadOnly="true"/>
                        </div>
                        <div class="col">
                            <label>After Due Dt</label>
                            <asp:TextBox ID="txtAfterDue" runat="server" CssClass="asp-input"  ReadOnly="true"/>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>Remarks</label>
                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="asp-input" Style="border: 2px solid blue; border-radius: 4px;" />
                        </div>
                    </div>

                    <!-- Buttons -->
                    <div class="btn-row">
                        <asp:Button ID="btnPost" runat="server" Text="Post" CssClass="asp-button" OnClick="btnPost_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnFixBill" runat="server" Text="Fix Bill" CssClass="asp-button" OnClick="btnFixBill_Click" />
                    </div>

                </div>
            </div>
        </form>
    </body>
</html>
