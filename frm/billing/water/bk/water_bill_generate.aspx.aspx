<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="water_bill_generate.aspx.cs"
    Inherits="water_bill_generate" %>

<!DOCTYPE html>
<html lang="en">
    <head id="Head1" runat="server">
        <meta charset="UTF-8" />
        <title>Water Bill Generate</title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />

        <style>
            body {
                margin: 0;
                font-family: Arial, sans-serif;
                background: #f7f7f7;
                color: #333;
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

            .asp-input {
                width: 100%;
                height: 65px;
                padding: 10px;
                font-size: 26px;
                border: 1px solid #ccc;
                border-radius: 4px;
                box-sizing: border-box;
            }

            /* BUTTONS */
            .btn-row {
                margin-top: 30px;
                display: flex;
                align-items: center;
            }

            .btn-left {
                display: flex;
                gap: 15px;
            }

            .btn-right {
                margin-left: auto;
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

            #lblStatus {
                font-size: 22px;
                font-weight: bold;
                padding-top: 10px;
            }

            /* MESSAGE PANEL */
            .msg-panel {
                margin-top: 25px;
                padding: 20px;
                border: 1px solid #ccc;
                border-radius: 6px;
                background: #f9f9f9;
            }

            .msg-row {
                display: flex;
                margin-bottom: 12px;
            }

            .msg-col {
                flex: 1;
                font-size: 20px;
                font-weight: 600;
            }

            .msg-value {
                color: #0f7c57;
                font-weight: bold;
            }

            .readonly-box {
                border: none !important;
                background: transparent;
                box-shadow: none;
                pointer-events: none;   /* cursor & typing completely off */
            }

            .prv-bill {
                color: red;
            }

            .crnt-bill {
                color: blue;
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <div class="container">
                <!-- LEFT PANEL -->
                <div class="left-panel">
                    <h2>Bill Process</h2>
                    <ul>
                        <li>Process month</li>
                        <li>Generate bills</li>
                        <li>Water charges</li>
                    </ul>
                </div>

                <!-- FORM PANEL -->
                <div class="form-panel">
                    <h2>Water Bill Generate</h2>

                    <div class="row">
                        <div class="col">
                            <label>Process Month</label>
                            <asp:TextBox ID="txtProcessMonth"
                                         runat="server"
                                         CssClass="asp-input"
                                         placeholder="YYYYMM" />
                        </div>
                    </div>

                    <!-- BUTTON ROW -->
                    <div class="btn-row">
                        <div class="btn-left">
                            <asp:Button ID="btnProcess"
                                        runat="server"
                                        Text="Process"
                                        CssClass="asp-button"
                                        OnClick="btnProcess_Click" />

                            <asp:Button ID="btnCancel"
                                        runat="server"
                                        Text="Cancel"
                                        CssClass="asp-button"
                                        OnClick="btnCancel_Click" />
                        </div>

                        <div class="btn-right">
                            <asp:Button ID="btnExportExcel"
                                        runat="server"
                                        Text="Export To Excel"
                                        CssClass="asp-button"
                                        OnClick="btnExportExcel_Click" />
                        </div>
                    </div>

                    <!-- STATUS -->
                    <div class="row">
                        <div class="col">
                            <asp:Label ID="lblStatus" runat="server" />
                        </div>
                    </div>

                    <!-- MESSAGE SUMMARY -->
                    <asp:Panel ID="pnlMessages" runat="server"
                               CssClass="msg-panel"
                               Visible="false">
                    </asp:Panel>

                    <div class="row">
                        <div class="col">
                            <label>Previous No. of Billing</label>
                            <asp:TextBox ID="txtPrvBill"
                                         runat="server"
                                         CssClass="asp-input readonly-box prv-bill"
                                         ReadOnly="true" />
                        </div>

                        <div class="col">
                            <label>Current No. of Billing</label>
                            <asp:TextBox ID="txtCrntBill"
                                         runat="server"
                                         CssClass="asp-input readonly-box crnt-bill"
                                         ReadOnly="true" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col">
                            <label>Advances</label>
                            <asp:TextBox ID="txtAdvance"
                                         runat="server"
                                         CssClass="asp-input readonly-box crnt-bill"
                                         ReadOnly="true" />
                        </div>

                        <div class="col">
                            <label>Fines</label>
                            <asp:TextBox ID="txtFine"
                                         runat="server"
                                         CssClass="asp-input readonly-box crnt-bill"
                                         ReadOnly="true" />
                        </div>

                        <div class="col">
                            <label>Arrears</label>
                            <asp:TextBox ID="txtArrears"
                                         runat="server"
                                         CssClass="asp-input readonly-box crnt-bill"
                                         ReadOnly="true" />
                        </div>

                        <div class="col">
                            <label>Installments</label>
                            <asp:TextBox ID="txtInst"
                                         runat="server"
                                         CssClass="asp-input readonly-box crnt-bill"
                                         ReadOnly="true" />
                        </div>
                    </div>

                </div>
            </div>
        </form>
    </body>
</html>

