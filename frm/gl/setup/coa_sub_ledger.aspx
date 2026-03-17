<%@ Page Language="C#" AutoEventWireup="true" CodeFile="coa_sub_ledger.aspx.cs" Inherits="coa_sub_legder" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Sub Ledger Entry</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />

    <style>
        body {
            margin: 0;
            font-family: Segoe UI, sans-serif;
            background-color: #f4f6f8;
        }

        .container {
            display: flex;
            height: 100vh;
        }

        /* SIDEBAR */
        .left-panel {
            background: #0f7c57;
            color: white;
            padding: 30px;
            border-radius: 8px;
            flex: 1;
            max-width: 300px;
            margin-right: 20px;
            height: auto;
        }

            .left-panel h3 {
                margin-bottom: 20px;
            }

            .left-panel ul {
                list-style: disc;
                padding-left: 20px;
            }

                .left-panel ul li {
                    margin-bottom: 10px;
                }

        .card {
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
            margin-bottom: 20px;
        }

        /* HEADER */
        header {
            position: sticky;
            top: 0;
            background: #fff;
            border-bottom: 1px solid #e5e7eb;
            padding: 12px 20px;
            display: flex;
            gap: 12px;
            align-items: center;
        }

        .header-btns {
            background-color: black;
            color: white !important;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.2s ease-in-out;
            text-decoration: none !important;
            display: inline-block;
        }

            .header-btns:hover {
                background-color: #333;
            }

        .header-border {
            border-top: 30px solid #000;
            width: 100%;
            margin-bottom: 0;
        }

        /* RIGHT CONTENT */
        .right-content {
            flex: 1;
            padding: 15px;
            background: #f4f4f4;
            overflow-y: auto;
        }

        /* DETAIL PANEL */
        .detail-panel {
            max-width: 1200px;
            margin: 20px auto;
            background: #ffffff;
            padding: 30px 40px;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
        }

        .panel-header {
            background: #dcdcdc;
            border: 1px solid #bdbdbd;
            padding: 4px 8px;
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            text-align: center;
        }

        /* SELECTION TABLE */
        .selection-table {
            width: 100%;
            border-collapse: collapse;
        }

            .selection-table td {
                padding: 4px 5px;
                vertical-align: middle;
            }

            .selection-table .label-cell {
                font-weight: 600;
                width: 120px;
                white-space: nowrap;
                font-size: 13px;
            }

        /* INPUTS */
        .asp-input {
            width: 100%;
            padding: 6px;
            font-size: 13px;
            border: 1px solid #bfbfbf;
            border-radius: 4px;
            box-sizing: border-box;
        }

            .asp-input:focus {
                border-color: #2e7d32;
                box-shadow: 0 0 10px rgba(46, 125, 50, 0.3);
                outline: none;
            }

        .readonly-field {
            background-color: #f5f5f5;
            font-weight: 500;
            border-color: #d9d9d9;
        }

        /* SEARCH FIELD */
        .search-field {
            background-image: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="%23999" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>');
            background-repeat: no-repeat;
            background-position: right 8px center;
            padding-right: 30px;
        }

        /* DETAIL GRID */
        .detail-grid {
            display: grid;
            grid-template-columns: 100px 1fr 100px 1fr 100px 1fr;
            gap: 10px 15px;
            align-items: center;
            margin-bottom: 15px;
        }

            .detail-grid label {
                font-weight: 600;
                font-size: 13px;
                white-space: nowrap;
            }

        .full-width-row {
            display: grid;
            grid-template-columns: 100px 1fr 100px 1fr;
            gap: 10px 15px;
            margin: 15px 0;
            align-items: center;
        }

        .last-row {
            display: grid;
            grid-template-columns: 100px 1fr 100px 1fr 100px 1fr;
            gap: 10px 15px;
            margin: 15px 0;
            align-items: center;
        }

            .full-width-row label, .last-row label {
                font-weight: 600;
                font-size: 13px;
                white-space: nowrap;
            }

        /* BUTTONS */
        .button-group {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 30px;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
        }

        .btn {
            padding: 8px 24px;
            border-radius: 6px;
            border: none;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
        }

        .btn-save {
            background: white;
            color: #1f6f4a;
            border: 1px solid #1f6f4a;
        }

        .btn-cancel {
            background: white;
            color: #ea4242;
            border: 1px solid #ea4242;
        }

        .btn-save:hover {
            background: #1f6f4a;
            color: white;
        }

        .btn-cancel:hover {
            background: #ce1f1f;
            color: white;
        }

        /* STATUS */
        .status-label {
            margin-top: 15px;
            padding: 8px;
            border-radius: 4px;
            text-align: center;
            font-weight: bold;
        }

        .status-success {
            background-color: #d4edda;
            color: #155724;
        }

        .status-error {
            background-color: #f8d7da;
            color: #721c24;
        }

        /* AUTOCOMPLETE */
        .ui-autocomplete {
            max-height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            z-index: 10000 !important;
            font-family: Segoe UI, sans-serif;
            font-size: 12px;
            border: 1px solid #999;
            background: white;
        }

            .ui-autocomplete .ui-menu-item {
                padding: 5px 10px;
                border-bottom: 1px solid #eee;
            }

                .ui-autocomplete .ui-menu-item:hover {
                    background-color: #4CAF50;
                    color: white;
                    cursor: pointer;
                }

        .ui-helper-hidden-accessible {
            display: none;
        }

        /* MODAL */
        .modal-background {
            background-color: rgba(0,0,0,0.5);
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 9999;
        }

        .modal-dialog {
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            padding: 20px;
            width: 350px;
            margin: 15% auto;
        }

        /* VALIDATOR */
        .validator-cell {
            color: red;
            font-size: 11px;
            margin-top: 5px;
        }

        /* WIDTH HELPERS */
        .w-80 {
            width: 80px;
        }

        .w-100 {
            width: 100px;
        }

        .w-150 {
            width: 150px;
        }

        .w-200 {
            width: 200px;
        }

        .w-250 {
            width: 250px;
        }

        .w-300 {
            width: 300px;
        }

        .w-full {
            width: 100%;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- HEADER -->
        <div id="border_header" class="header-border"></div>
        <header>
            <div style="font-weight: bold; font-size: 18px;">Sub Ledger</div>
            <div style="margin-left: auto; display: flex; gap: 10px;">
                <%--<asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>--%>
                <asp:LinkButton ID="btnGoBack" runat="server"
                    CssClass="header-btns"
                    OnClick="btnGoBack_Click"
                    CausesValidation="false">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server"
                    CssClass="header-btns"
                    OnClick="btnLogoff_Click"
                    CausesValidation="false">Log off</asp:LinkButton>
            </div>

        </header>

        <div class="container">
            <!-- LEFT PANEL -->
            <div class="left-panel">
                <h3>Sub Ledger Management</h3>
                <ul>
                    <li>Search and select GL SL Type</li>
                    <li>Enter SL Code and details</li>
                    <li>Use Save to commit changes</li>
                </ul>
            </div>

            <!-- RIGHT CONTENT -->
            <div class="right-content">
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfCurrentMode" runat="server" Value="ADD" />
                <asp:HiddenField ID="hfSubLedgerId" runat="server" Value="0" />
                <asp:HiddenField ID="hfSelectedGLCode" runat="server" Value="" />

                <!-- GL SL TYPE SELECTION CARD -->
                <div class="card">
                    <h3>GL SL Type Selection</h3>

                    <table class="selection-table">
                        <tr>
                            <td class="label-cell">Search GL SL Type:</td>
                            <td colspan="3">
                                <asp:TextBox ID="txtSearchGLSL" runat="server"
                                    CssClass="asp-input search-field w-900"
                                    placeholder="Type ID or Description to search..." />
                            </td>
                        </tr>
                        <tr>
                            <td class="label-cell">GL SL ID:</td>
                            <td>
                                <asp:TextBox ID="txtGLSLId" runat="server" ReadOnly="true" CssClass="asp-input w-300 readonly-field" /></td>
                            <td class="label-cell">GL SL Description:</td>
                            <td colspan="3">
                                <asp:TextBox ID="txtGLSLDesc" runat="server" ReadOnly="true" CssClass="asp-input w-300 readonly-field" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label-cell">Family:</td>
                            <td>
                                <asp:TextBox ID="txtFamily" runat="server" ReadOnly="true" CssClass="asp-input w-200 readonly-field" /></td>
                            <td class="label-cell">GL Code:</td>
                            <td>
                                <asp:TextBox ID="txtGLCode" runat="server" ReadOnly="true" CssClass="asp-input w-200 readonly-field" /></td>
                            <td class="label-cell">GL Description:</td>
                            <td>
                                <asp:TextBox ID="txtGLDesc" runat="server" ReadOnly="true" CssClass="asp-input w-200 readonly-field" /></td>
                        </tr>
                    </table>
                </div>

                <!-- SUB LEDGER DETAILS -->
                <div class="panel-header">Sub Ledger Details</div>

                <div class="detail-panel">
                    <!-- MAIN GRID -->
                    <div class="detail-grid">
                        <!-- Row 1 -->
                        <label>SL Code <span style="color: red;">*</span></label>
                        <asp:TextBox ID="txtSLCode" runat="server" CssClass="asp-input" MaxLength="15" />

                        <label>Description</label>
                        <asp:TextBox ID="txtDescrip" runat="server" CssClass="asp-input" MaxLength="100" />

                        <label>Contact Person</label>
                        <asp:TextBox ID="txtContactPerson" runat="server" CssClass="asp-input" MaxLength="30" />

                        <!-- Row 2 -->
                        <label>NTN</label>
                        <asp:TextBox ID="txtNTN" runat="server" CssClass="asp-input" MaxLength="20" />

                        <label>STN</label>
                        <asp:TextBox ID="txtSTN" runat="server" CssClass="asp-input" MaxLength="20" />

                        <label>Cell #1</label>
                        <asp:TextBox ID="txtCell1" runat="server" CssClass="asp-input" MaxLength="15" />

                        <!-- Row 3 -->
                        <label>Cell #2</label>
                        <asp:TextBox ID="txtCell2" runat="server" CssClass="asp-input" MaxLength="15" />

                        <label>Contact #1</label>
                        <asp:TextBox ID="txtContact1" runat="server" CssClass="asp-input" MaxLength="15" />

                        <label>Contact #2</label>
                        <asp:TextBox ID="txtContact2" runat="server" CssClass="asp-input" MaxLength="15" />

                        <!-- Row 4 -->
                        <label>Contact #3</label>
                        <asp:TextBox ID="txtContact3" runat="server" CssClass="asp-input" MaxLength="15" />

                        <label>Fax #1</label>
                        <asp:TextBox ID="txtFax1" runat="server" CssClass="asp-input" MaxLength="15" />

                        <label>Fax #2</label>
                        <asp:TextBox ID="txtFax2" runat="server" CssClass="asp-input" MaxLength="15" />

                        <!-- Row 5 -->
                        <label>Email 1</label>
                        <asp:TextBox ID="txtEmail1" runat="server" CssClass="asp-input" MaxLength="100" />

                        <label>Email 2</label>
                        <asp:TextBox ID="txtEmail2" runat="server" CssClass="asp-input" MaxLength="100" />

                        <label>URL</label>
                        <asp:TextBox ID="txtURL" runat="server" CssClass="asp-input" MaxLength="100" />
                    </div>

                    <!-- Full width row for Remarks and Address 1 -->
                    <div class="full-width-row">
                        <label>Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="asp-input" MaxLength="100" />

                        <label>Address 1</label>
                        <asp:TextBox ID="txtAdd1" runat="server" CssClass="asp-input" MaxLength="100" />
                    </div>

                    <!-- Last row for Address 2, City, Opening Balance -->
                    <div class="last-row">
                        <label>Address 2</label>
                        <asp:TextBox ID="txtAdd2" runat="server" CssClass="asp-input" MaxLength="100" />

                        <label>City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="asp-input" MaxLength="25" />

                        <label>Opening Bal</label>
                        <asp:TextBox ID="txtOpeningBalance" runat="server" Text="0.00" CssClass="asp-input" Style="text-align: right;" />
                        <ajaxToolkit:FilteredTextBoxExtender ID="ftbeOpeningBalance" runat="server"
                            TargetControlID="txtOpeningBalance" FilterType="Numbers, Custom" ValidChars="." />
                    </div>

                    <!-- Validator -->
                    <div class="validator-cell">
                        <asp:RequiredFieldValidator ID="rfvSLCode" runat="server"
                            ControlToValidate="txtSLCode" ErrorMessage="SL Code is required"
                            ForeColor="Red" Display="Dynamic" />
                    </div>

                    <!-- BUTTONS -->
                    <div class="button-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-cancel" OnClick="btnClear_Click" />
                    </div>

                    <!-- STATUS -->
                    <div id="statusContainer" runat="server" class="status-label" visible="false">
                        <asp:Label ID="lblStatus" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Message Popup -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="modal-dialog" Style="display: none;">
            <div style="text-align: center;">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Font-Size="14px" />
                <br />
                <br />
                <asp:Button ID="btnMessageOk" runat="server" Text="OK" CssClass="btn btn-save" OnClick="btnMessageOk_Click" />
            </div>
        </asp:Panel>

        <ajaxToolkit:ModalPopupExtender ID="mpeMessage" runat="server"
            TargetControlID="btnMessageOk"
            PopupControlID="pnlMessage"
            BackgroundCssClass="modal-background"
            CancelControlID="btnMessageOk" />

        <script type="text/javascript">
            function setupGLSLTypeAutoComplete() {
                $('#<%= txtSearchGLSL.ClientID %>').autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            type: "POST",
                            url: "coa_sub_ledger.aspx/SearchGLSLTypes",
                            data: JSON.stringify({ searchTerm: request.term }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        label: item.SUB_LEDGER_ID + " - " + item.DESCRIP + " (" + item.GL_CODE + ")",
                                        value: item.SUB_LEDGER_ID,
                                        desc: item.DESCRIP,
                                        glCode: item.GL_CODE,
                                        family: item.FAMILY
                                    };
                                }));
                            },
                            error: function (xhr, status, error) {
                                console.log("Search Error: " + error);
                            }
                        });
                    },
                    minLength: 1,
                    select: function (event, ui) {
                        $('#<%= txtSearchGLSL.ClientID %>').val(ui.item.label);
                        $('#<%= hfSubLedgerId.ClientID %>').val(ui.item.value);
                        __doPostBack('<%= txtSearchGLSL.UniqueID %>', '');
                        return false;
                    }
                });
            }

            function validateForm() {
                var slCode = $('#<%= txtSLCode.ClientID %>').val().trim();
                if (slCode === '') {
                    alert('SL Code is required');
                    $('#<%= txtSLCode.ClientID %>').focus();
                    return false;
                }

                var email1 = $('#<%= txtEmail1.ClientID %>').val().trim();
                if (email1 !== '') {
                    var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
                    if (!emailPattern.test(email1)) {
                        alert('Please enter a valid email address in Email 1');
                        $('#<%= txtEmail1.ClientID %>').focus();
                        return false;
                    }
                }

                var email2 = $('#<%= txtEmail2.ClientID %>').val().trim();
                if (email2 !== '') {
                    if (!emailPattern.test(email2)) {
                        alert('Please enter a valid email address in Email 2');
                        $('#<%= txtEmail2.ClientID %>').focus();
                        return false;
                    }
                }

                var openingBalance = $('#<%= txtOpeningBalance.ClientID %>').val().trim();
                if (openingBalance !== '' && isNaN(parseFloat(openingBalance))) {
                    alert('Please enter a valid number for Opening Balance');
                    $('#<%= txtOpeningBalance.ClientID %>').focus();
                    return false;
                }

                return true;
            }

            $(document).ready(function () {
                setupGLSLTypeAutoComplete();
                $('#<%= btnSave.ClientID %>').on('click', function (e) {
                    if (!validateForm()) {
                        e.preventDefault();
                        return false;
                    }
                });
            });

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (prm) {
                prm.add_endRequest(function () {
                    setupGLSLTypeAutoComplete();
                });
            }
        </script>
    </form>
</body>
</html>
