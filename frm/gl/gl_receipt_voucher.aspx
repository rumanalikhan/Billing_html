<%@ Page Language="C#" AutoEventWireup="true" CodeFile="gl_receipt_voucher.aspx.cs" Inherits="GL_Receipt_Voucher" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Bank Receipt Voucher</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
    <style type="text/css">
        /* ===== RESET & GLOBAL STYLES ===== */
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 0;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f0f0f0;
            height: 100vh;
            overflow: hidden;
        }

        /* ===== MAIN CONTAINER ===== */
        .container {
            width: 100%;
            height: 100vh;
            margin: 0;
            background-color: white;
            padding: 10px 15px;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        /* ===== BUTTON PANEL ===== */
        .button-panel {
            margin: 5px 0 10px;
            padding: 8px 10px;
            background-color: #e6e6e6;
            border: 1px solid #999;
            border-radius: 3px;
            display: flex;
            flex-wrap: wrap;
            gap: 5px;
            align-items: center;
            flex-shrink: 0;
        }

        .status-container {
            margin-left: auto;
            display: flex;
            align-items: center;
        }

        /* ===== ICON BUTTONS ===== */
        .icon-btn {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            padding: 4px 6px;
            border-radius: 4px;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            min-width: 40px;
        }

            .icon-btn:hover {
                transform: scale(1.1);
                background-color: rgba(0,0,0,0.02);
            }

        .copy-btn {
            color: #2196F3;
        }

        .post-btn {
            color: #4f9c52;
        }

        .unpost-btn {
            color: #d44339;
        }

        .save-btn {
            color: #4f9c52;
        }

        .clear-btn {
            color: #d78204;
        }

        .nav-btn {
            color: #2196F3;
        }

        .add-btn {
            color: #4CAF50;
            font-size: 16px;
            padding: 8px 16px;
        }

            .add-btn i {
                margin-right: 5px;
            }

        /* Hover effects */
        .copy-btn:hover {
            color: #1976D2;
        }

        .post-btn:hover {
            color: #388E3C;
        }

        .unpost-btn:hover {
            color: #d32f2f;
        }

        .save-btn:hover {
            color: #388E3C;
        }

        .clear-btn:hover {
            color: #f57c00;
        }

        .nav-btn:hover {
            color: #3187dc;
        }

        .add-btn:hover {
            color: #2d6a2d;
        }

        /* ===== VOUCHER HEADER ===== */
        .voucher-header {
            background-color: #e6e6e6;
            padding: 8px 10px;
            margin-bottom: 10px;
            border: 1px solid #999;
            border-radius: 3px;
            flex-shrink: 0;
        }

            .voucher-header table {
                width: 100%;
                border-collapse: collapse;
            }

            .voucher-header td {
                padding: 5px;
            }

        .header-label {
            font-weight: bold;
            background-color: #d4d4d4;
            padding: 5px 8px;
            width: 100px;
            white-space: nowrap;
        }

        .header-value {
            background-color: white;
            padding: 5px 8px;
            width: 180px;
        }

        .voucher-key {
            font-family: monospace;
            font-size: 11px;
            color: #666;
        }

        /* ===== GRID STYLES ===== */
        .grid-container {
            margin-top: 5px;
            overflow-x: auto;
            width: 100%;
            border: 1px solid #ddd;
            border-radius: 3px;
            flex-grow: 1;
            overflow-y: auto;
            position: relative;
        }

        .gridview-style {
            width: 100%;
            border-collapse: collapse;
            font-size: 12px;
            table-layout: fixed;
        }

            .gridview-style th {
                background-color: #4CAF50;
                color: white;
                padding: 10px 5px;
                text-align: left;
                white-space: nowrap;
                font-weight: bold;
                border: 1px solid #45a049;
            }

            .gridview-style td {
                padding: 5px;
                border: 1px solid #ddd;
                background-color: white;
            }

            .gridview-style tr:nth-child(even) td {
                background-color: #f9f9f9;
            }

            .gridview-style tr:hover td {
                background-color: #f5f5f5;
            }

            .gridview-style input[type="text"] {
                width: 100%;
                box-sizing: border-box;
                padding: 6px 4px;
                border: 1px solid #ccc;
                border-radius: 3px;
                font-size: 12px;
            }

                .gridview-style input[type="text"]:focus {
                    border-color: #4CAF50;
                    outline: none;
                    box-shadow: 0 0 3px #4CAF50;
                }

            .gridview-style tfoot tr {
                position: sticky;
                bottom: 0;
                background-color: #e6e6e6;
                z-index: 10;
            }

            .gridview-style tfoot td {
                background-color: #e6e6e6;
                font-weight: bold;
                border-top: 2px solid #999;
            }

        /* Column widths */
        .col-gl-code {
            width: 80px;
        }

        .col-gl-type {
            width: 90px;
        }

        .col-sl-code {
            width: 80px;
        }

        .col-sl-type {
            width: 60px;
        }

        .col-narration {
            width: 120px;
        }

        .col-bill {
            width: 100px;
        }

        .col-cheque {
            width: 100px;
        }

        .col-amount {
            width: 100px;
        }

        .col-cost {
            width: 80px;
        }

        .col-action {
            width: 20px;
        }

        /* Grid action icons */
        .grid-icon {
            background: none;
            border: none;
            font-size: 16px;
            cursor: pointer;
            padding: 5px 8px;
            border-radius: 3px;
            transition: all 0.2s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }

        .delete-icon {
            color: #f44336;
        }

            .delete-icon:hover {
                color: #d32f2f;
                transform: scale(1.2);
            }

        /* ===== STATUS LABELS ===== */
        .status-posted {
            background-color: #d4edda;
            color: #155724;
            padding: 6px 15px;
            border-radius: 3px;
            font-weight: bold;
            display: inline-block;
        }

        .status-unposted {
            background-color: #f8d7da;
            color: #721c24;
            padding: 6px 15px;
            border-radius: 3px;
            font-weight: bold;
            display: inline-block;
        }

        /* ===== TOTAL ROW ===== */
        .total-row {
            margin-top: 10px;
            padding: 10px 15px;
            background-color: #e6e6e6;
            font-weight: bold;
            font-size: 16px;
            text-align: right;
            border: 1px solid #999;
            border-radius: 3px;
            flex-shrink: 0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .total-amount {
            margin-left: auto;
            font-weight: bold;
            color: #333;
        }

        /* ===== AUTOCOMPLETE ===== */
        .ui-autocomplete {
            max-height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            z-index: 1000 !important;
            font-family: Arial, Helvetica, sans-serif;
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

        /* GL Description field read-only */
        .gl-type-input[readonly] {
            background-color: #f0f0f0;
            cursor: default;
        }

        /* ===== MODAL STYLES ===== */
        .modal-background {
            background-color: rgba(0,0,0,0.5);
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 1000;
        }

        .modal-dialog {
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: white;
            border-radius: 5px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            z-index: 1001;
            max-width: 90%;
            max-height: 90%;
            overflow-y: auto;
        }

        .modal-header {
            padding: 15px;
            background-color: #4CAF50;
            color: white;
            border-radius: 5px 5px 0 0;
        }

        .modal-body {
            padding: 15px;
        }

        .modal-footer {
            padding: 15px;
            background-color: #f5f5f5;
            border-radius: 0 0 5px 5px;
            text-align: right;
        }

        /* ===== HEADINGS ===== */
        h2 {
            margin: 0 0 15px;
            color: #333;
        }

        h4 {
            margin: 10px 0 5px;
            color: #555;
        }

        /* ===== RESPONSIVE ===== */
        @media screen and (max-width: 1400px) {
            .gridview-style {
                font-size: 11px;
            }

                .gridview-style input[type="text"] {
                    padding: 4px;
                    font-size: 11px;
                }
        }

        /* Inline snackbar styles - appears next to heading */
        .snackbar-inline {
            visibility: hidden;
            min-width: 250px;
            background-color: #333;
            color: #fff;
            text-align: left;
            border-radius: 4px;
            padding: 8px 16px;
            box-shadow: 0 3px 6px rgba(0,0,0,0.16), 0 3px 6px rgba(0,0,0,0.23);
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 13px;
            margin-left: 15px;
            flex-shrink: 0; /* Prevents shrinking */
            max-width: 400px;
        }

            .snackbar-inline.show {
                visibility: visible;
                animation: fadein 0.3s;
            }

            .snackbar-inline.success {
                background-color: #4CAF50;
            }

            .snackbar-inline.error {
                background-color: #f44336;
            }

            .snackbar-inline.warning {
                background-color: #ff9800;
            }

            .snackbar-inline.info {
                background-color: #2196F3;
            }

            .snackbar-inline .snackbar-close {
                cursor: pointer;
                font-weight: bold;
                margin-left: 15px;
                padding: 0 5px;
                opacity: 0.8;
            }

                .snackbar-inline .snackbar-close:hover {
                    opacity: 1;
                }

        @keyframes fadein {
            from {
                opacity: 0;
                transform: translateX(20px);
            }

            to {
                opacity: 1;
                transform: translateX(0);
            }
        }
    </style>
    <script type="text/javascript">
        // AutoComplete setup for GL Code
        function setupGLAutoComplete() {
            $('.gl-code-input').autocomplete({
                source: function (request, response) {
                    $.ajax({
                        type: "POST",
                        url: "GL_Receipt_Voucher.aspx/SearchGLCodes",
                        data: JSON.stringify({ searchTerm: request.term }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.GL_CODE + " - " + item.GL_DESCRP,
                                    value: item.GL_CODE,
                                    desc: item.GL_DESCRP
                                };
                            }));
                        },
                        error: function (xhr, status, error) {
                            console.log("GL AutoComplete Error: " + error);
                        }
                    });
                },
                minLength: 2,
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    var row = $(this).closest('tr');
                    row.find('.gl-type-input').val(ui.item.desc);
                    __doPostBack($(this).attr('name'), '');
                    return false;
                }
            });
        }

        // SL Code AutoComplete setup - SINGLE VERSION
        function setupSLAutoComplete() {

            $('.sl-code-input').autocomplete({
                source: function (request, response) {

                    var row = $(this.element).closest('tr');
                    var glCodeInput = row.find('.gl-code-input');
                    var glCode = glCodeInput ? glCodeInput.val() : '';

                    console.log("GL Code from row: '" + glCode + "'");

                    if (!glCode) {
                        response([]);
                        return;
                    }


                    $.ajax({
                        type: "POST",
                        url: "GL_Receipt_Voucher.aspx/SearchSLCodes",
                        data: JSON.stringify({ searchTerm: request.term, glCode: glCode }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data) {

                            if (!data.d || data.d.length === 0) {
                                response([]);
                                return;
                            }

                            var mapped = $.map(data.d, function (item) {
                                return {
                                    label: item.SL_CODE + " - " + item.DESCRIP,
                                    value: item.SL_CODE,
                                    subLedgerId: item.SUB_LEDGER_ID,
                                    desc: item.DESCRIP
                                };
                            });

                            response(mapped);
                        },
                        error: function (xhr, status, error) {
                            console.log("❌ AJAX Error: " + error);
                            console.log("Response:", xhr.responseText);
                            response([]);
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    var row = $(this).closest('tr');
                    row.find('.sl-type-input').val(ui.item.desc);
                    __doPostBack($(this).attr('name'), '');
                    return false;
                },
                open: function () {
                },
                close: function () {
                    console.log("Autocomplete dropdown closed");
                }
            });
        }

        // Snackbar functions
        function showSnackbar(message, type, duration) {
            var snackbar = document.getElementById('snackbar');
            var messageSpan = document.getElementById('snackbarMessage');

            if (!snackbar || !messageSpan) return;

            if (snackbar.timeout) {
                clearTimeout(snackbar.timeout);
            }

            messageSpan.textContent = message;
            snackbar.className = 'snackbar-inline show';
            snackbar.classList.add(type);

            snackbar.timeout = setTimeout(function () {
                closeSnackbar();
            }, duration || 3000);
        }

        function closeSnackbar() {
            var snackbar = document.getElementById('snackbar');
            if (snackbar) {
                snackbar.className = 'snackbar-inline';
                if (snackbar.timeout) {
                    clearTimeout(snackbar.timeout);
                }
            }
        }

        // Initialize on document ready
        $(document).ready(function () {
            setupGLAutoComplete();
            setupSLAutoComplete();
        });

        // Re-attach after postback
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm) {
            prm.add_endRequest(function () {
                setupGLAutoComplete();
                setupSLAutoComplete();
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container">
            <h2>Bank Receipt Voucher</h2>

            <!-- Button Panel -->
            <div class="button-panel">
                <asp:LinkButton ID="btnCopyVoucher" runat="server" CssClass="icon-btn copy-btn"
                    OnClick="btnCopyVoucher_Click" ToolTip="Copy Voucher" Visible="false">
    <i class="fas fa-copy"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnPost" runat="server" CssClass="icon-btn post-btn"
                    OnClick="btnPost_Click" ToolTip="Post"><i class="fas fa-check-circle"></i></asp:LinkButton>
                <asp:LinkButton ID="btnUnposted" runat="server" CssClass="icon-btn unpost-btn"
                    OnClick="btnUnposted_Click" ToolTip="Unpost"><i class="fas fa-times-circle"></i></asp:LinkButton>
                <asp:LinkButton ID="btnSave" runat="server" CssClass="icon-btn save-btn"
                    OnClick="btnSave_Click" ToolTip="Save Voucher"><i class="fas fa-save"></i></asp:LinkButton>
                <asp:LinkButton ID="btnClear" runat="server" CssClass="icon-btn clear-btn"
                    OnClick="btnClear_Click" ToolTip="Clear"><i class="fas fa-eraser"></i></asp:LinkButton>

                <asp:LinkButton ID="btnFirst" runat="server" CssClass="icon-btn nav-btn"
                    OnClick="btnFirst_Click" ToolTip="First Voucher">
    <i class="fas fa-chevron-left"></i><i class="fas fa-chevron-left"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="btnPrevious" runat="server" CssClass="icon-btn nav-btn"
                    OnClick="btnPrevious_Click" ToolTip="Previous Voucher">
    <i class="fas fa-chevron-left"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="btnNext" runat="server" CssClass="icon-btn nav-btn"
                    OnClick="btnNext_Click" ToolTip="Next Voucher">
    <i class="fas fa-chevron-right"></i>
                </asp:LinkButton>

                <asp:LinkButton ID="btnLast" runat="server" CssClass="icon-btn nav-btn"
                    OnClick="btnLast_Click" ToolTip="Last Voucher">
    <i class="fas fa-chevron-right"></i><i class="fas fa-chevron-right"></i>
                </asp:LinkButton>

                <div class="status-container">
                    <asp:Label ID="lblStatus" runat="server" CssClass="status-unposted" Text="UnPosted" />
                </div>
            </div>

            <!-- Voucher Header -->
            <div class="voucher-header">
                <table>
                    <tr>
                        <td class="header-label">Voucher Date:</td>
                        <td class="header-value">
                            <asp:TextBox ID="txtVoucherDate" runat="server" Width="130px" ReadOnly="true"
                                Style="background-color: #f0f0f0;" />
                        </td>
                        <td class="header-label">Voucher Number:</td>
                        <td class="header-value">
                            <asp:Label ID="lblVoucherNumber" runat="server" Font-Bold="true" Text="407" />
                        </td>
                        <td class="header-label">Book Type:</td>
                        <td class="header-value">
                            <asp:DropDownList ID="ddlBookType" runat="server" Width="220px"
                                DataTextField="BOOK_TYPE" DataValueField="GL_CODE"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlBookType_SelectedIndexChanged" />
                        </td>
                        <td class="header-label">GRV:</td>
                        <td class="header-value">
                            <asp:Label ID="lblgrv" runat="server" Text="Auto-generated" CssClass="voucher-key" />
                        </td>
                    </tr>
                </table>
            </div>

            <!-- Heading Row with Snackbar -->
            <div style="display: flex; justify-content: space-between; align-items: center; margin: 10px 0 5px 0;">
                <h4 style="margin: 0;">Voucher Details</h4>

                <!-- Snackbar Notification - In same row as heading -->
                <div id="snackbar" class="snackbar-inline">
                    <span id="snackbarMessage"></span>
                    <span class="snackbar-close" onclick="closeSnackbar()">✕</span>
                </div>
            </div>
            <!-- Voucher Details Grid -->
            <div class="grid-container">
                <asp:GridView ID="gvVoucherDetails" runat="server"
                    CssClass="gridview-style"
                    AutoGenerateColumns="False"
                    ShowFooter="True"
                    GridLines="Both"
                    OnRowCommand="gvVoucherDetails_RowCommand"
                    OnRowDataBound="gvVoucherDetails_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="GL Code" HeaderStyle-CssClass="col-gl-code">
                            <ItemTemplate>
                                <asp:TextBox ID="txtGLCode" runat="server" CssClass="gl-code-input"
                                    Text='<%# Bind("GL_CODE") %>' AutoPostBack="true"
                                    OnTextChanged="txtGLCode_TextChanged" placeholder="GL Code" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Description" HeaderStyle-CssClass="col-gl-type">
                            <ItemTemplate>
                                <asp:TextBox ID="txtGLType" runat="server" CssClass="gl-type-input"
                                    Text='<%# Bind("GL_BOOK_TYPE") %>' placeholder="Description"
                                    ReadOnly="true" Style="background-color: #f0f0f0;" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="SL Code" HeaderStyle-CssClass="col-sl-code">
                            <ItemTemplate>
                                <asp:TextBox ID="txtSLCode" runat="server"
                                    CssClass="sl-code-input"
                                    Text='<%# Bind("SL_CODE") %>'
                                    AutoPostBack="true"
                                    OnTextChanged="txtSLCode_TextChanged"
                                    placeholder="SL Code" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="SL Description" HeaderStyle-CssClass="col-sl-type">
                            <ItemTemplate>
                                <asp:TextBox ID="txtSLType" runat="server"
                                    CssClass="sl-type-input"
                                    Text='<%# Bind("SL_TYPE") %>'
                                    placeholder="SL Description"
                                    ReadOnly="true"
                                    Style="background-color: #f0f0f0;" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Narration" HeaderStyle-CssClass="col-narration">
                            <ItemTemplate>
                                <asp:TextBox ID="txtNarration" runat="server" Text='<%# Bind("NARATION") %>'
                                    AutoPostBack="true" OnTextChanged="txtField_TextChanged" placeholder="Enter narration" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Bill No." HeaderStyle-CssClass="col-bill">
                            <ItemTemplate>
                                <asp:TextBox ID="txtBillNumber" runat="server" Text='<%# Bind("BILL_NUMBER") %>'
                                    AutoPostBack="true" OnTextChanged="txtField_TextChanged" placeholder="Bill #" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Cheque No." HeaderStyle-CssClass="col-cheque">
                            <ItemTemplate>
                                <asp:TextBox ID="txtChequeNumber" runat="server" Text='<%# Bind("CHEQUE_NUMBER") %>'
                                    AutoPostBack="true" OnTextChanged="txtField_TextChanged" placeholder="Cheque #" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="col-amount">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAmount" runat="server" Text='<%# Bind("AMOUNT", "{0:N2}") %>'
                                    OnTextChanged="txtAmount_TextChanged" AutoPostBack="true"
                                    placeholder="0.00" Style="text-align: right;" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Cost Centre" HeaderStyle-CssClass="col-cost">
                            <ItemTemplate>
                                <asp:TextBox ID="txtCostCentreCode" runat="server" Text='<%# Bind("COST_CENTRE_CODE") %>'
                                    AutoPostBack="true" OnTextChanged="txtField_TextChanged" placeholder="Cost Ctr" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action" HeaderStyle-CssClass="col-action">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDelete" runat="server"
                                    CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                                    CssClass="grid-icon delete-icon" ToolTip="Delete Row"
                                    OnClientClick="return confirm('Delete this row?');">✖</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="padding: 20px; text-align: center; background-color: #f9f9f9;">
                            No voucher details found. Click "Add Rows" to create entries.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <!-- Total Row -->
            <div class="total-row">
                <asp:LinkButton ID="btnAddRows" runat="server" CssClass="icon-btn add-btn"
                    OnClick="btnAddRows_Click" ToolTip="Add More Rows">
                    <i class="fas fa-plus-circle"></i> Add Rows
                </asp:LinkButton>
                <span class="total-amount">Total Amount:
                    <asp:Label ID="lblGrandTotal" runat="server" Font-Bold="true" Text="0.00" />
                </span>
            </div>

            <!-- Hidden Fields -->
            <asp:HiddenField ID="hfCurrentMode" runat="server" Value="ADD" />
            <asp:HiddenField ID="hfCompId" runat="server" Value="1" />
        </div>

        <!-- Search Voucher Modal -->
        <asp:Panel ID="pnlSearchVoucher" runat="server" CssClass="modal-dialog" Style="display: none; width: 800px;">
            <div class="modal-content">
                <div class="modal-header">
                    <h3>Search Vouchers</h3>
                </div>
                <div class="modal-body">
                    <div style="margin-bottom: 15px;">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 100px;">Voucher Key:</td>
                                <td style="width: 200px;">
                                    <asp:TextBox ID="txtSearchVoucherKey" runat="server" Width="180px" /></td>
                                <td style="width: 80px;">Book Type:</td>
                                <td style="width: 200px;">
                                    <asp:DropDownList ID="ddlSearchBookType" runat="server" Width="180px">
                                        <asp:ListItem Text="-- All --" Value="" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="text-align: center; padding-top: 10px;">
                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="icon-btn post-btn"
                                        OnClick="btnSearch_Click" ToolTip="Search"><i class="fas fa-search"></i> Search</asp:LinkButton>
                                    <asp:LinkButton ID="btnReset" runat="server" CssClass="icon-btn clear-btn"
                                        OnClick="btnReset_Click" ToolTip="Reset"><i class="fas fa-undo"></i> Reset</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div style="max-height: 300px; overflow-y: auto; border: 1px solid #ddd;">
                        <asp:GridView ID="gvSearchResults" runat="server" CssClass="gridview-style"
                            AutoGenerateColumns="False" Width="100%"
                            OnRowCommand="gvSearchResults_RowCommand" OnRowDataBound="gvSearchResults_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="VOUCHER_KEY" HeaderText="Voucher Key" />
                                <asp:BoundField DataField="GL_BOOK_TYPE" HeaderText="Book Type" />
                                <asp:BoundField DataField="VOUCHER_NUMBER" HeaderText="Number" />
                                <asp:BoundField DataField="TOTAL_AMOUNT" HeaderText="Total" DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkSelect" runat="server" CommandName="SelectVoucher"
                                            CommandArgument='<%# Eval("VOUCHER_KEY") %>'
                                            CssClass="grid-icon" Style="color: #4CAF50; text-decoration: none; font-weight: bold;">
                                            <i class="fas fa-check-circle"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div style="padding: 20px; text-align: center;">No vouchers found</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton ID="btnCloseSearch" runat="server" CssClass="icon-btn clear-btn"
                        OnClick="btnCloseSearch_Click" ToolTip="Close"><i class="fas fa-times"></i> Close</asp:LinkButton>
                </div>
            </div>
        </asp:Panel>

        <!-- Modal Popup Extender -->
        <ajaxToolkit:ModalPopupExtender ID="mpeSearchVoucher" runat="server"
            TargetControlID="btnCopyVoucher" PopupControlID="pnlSearchVoucher"
            BackgroundCssClass="modal-background" CancelControlID="btnCloseSearch" />
    </form>
</body>
</html>
