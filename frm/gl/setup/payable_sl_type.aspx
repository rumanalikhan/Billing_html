<%@ Page Language="C#" AutoEventWireup="true" CodeFile="payable_sl_type.aspx.cs" Inherits="payable_sl_type" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Payable SL Type</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />

    <style>
        body { margin: 0; font-family: Segoe UI, sans-serif; background-color: #f4f6f8; }

        /* SNACKBAR STYLES */
        .snackbar {
            visibility: hidden;
            min-width: 300px;
            background-color: #333;
            color: #fff;
            text-align: center;
            border-radius: 4px;
            padding: 12px 20px;
            position: fixed;
            z-index: 10000;
            bottom: 30px;
            left: 50%;
            transform: translateX(-50%);
            font-size: 14px;
            font-weight: 500;
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
        }
        .snackbar.show {
            visibility: visible;
            animation: fadein 0.5s, fadeout 0.5s 2.5s;
        }
        .snackbar-success { background-color: #28a745; }
        .snackbar-error { background-color: #dc3545; }
        .snackbar-info { background-color: #17a2b8; }
        @keyframes fadein {
            from { bottom: 0; opacity: 0; }
            to { bottom: 30px; opacity: 1; }
        }
        @keyframes fadeout {
            from { bottom: 30px; opacity: 1; }
            to { bottom: 0; opacity: 0; }
        }

        .container { display: flex; height: 100vh; }

        /* SIDEBAR */
        .left-panel { background: #0f7c57; color: white; padding: 30px; border-radius: 8px; flex: 1; max-width: 300px; margin-right: 20px; height: auto; }
        .left-panel h3 { margin-bottom: 20px; }
        .left-panel ul { list-style: disc; padding-left: 20px; }
        .left-panel ul li { margin-bottom: 10px; }
        
        .card { background: white; padding: 25px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); margin-bottom: 20px; }
        
        /* HEADER */
        header { position: sticky; top: 0; background: #fff; border-bottom: 1px solid #e5e7eb; padding: 12px 20px; display: flex; gap: 12px; align-items: center; }
        .header-btns { background-color: black; color: white !important; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; font-size: 16px; transition: background-color 0.2s ease-in-out; text-decoration: none !important; display: inline-block; }
        .header-btns:hover { background-color: #333; }
        .header-border { border-top: 30px solid #000; width: 100%; margin-bottom: 0; }
        
        /* RIGHT CONTENT */
        .right-content { flex: 1; padding: 15px; background: #f4f4f4; overflow-y: auto; }

        /* FORM STYLES */
        .form-panel { max-width: 100%; margin: 20px auto; background: #ffffff; padding: 30px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08); }
        .panel-header { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 8px 15px; font-size: 18px; font-weight: 600; margin-bottom: 25px; border-radius: 4px; }

        .form-table { width: 100%; border-collapse: collapse; }
        .form-table td { padding: 12px 10px; vertical-align: middle; }
        .form-table .label-cell { font-weight: 600; width: 120px; white-space: nowrap; font-size: 14px; }
        .form-table .value-cell { width: 300px; }

        /* INPUTS */
        .asp-input { width: 100%; padding: 8px 10px; font-size: 14px; border: 1px solid #bfbfbf; border-radius: 4px; box-sizing: border-box; }
        .asp-input:focus { border-color: #2e7d32; box-shadow: 0 0 10px rgba(46, 125, 50, 0.3); outline: none; }
        .readonly-field { background-color: #f5f5f5; font-weight: 500; border-color: #d9d9d9; }

        /* SEARCH FIELD */
        .search-field { background-image: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="%23999" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>'); background-repeat: no-repeat; background-position: right 10px center; padding-right: 35px; }

        /* AUTOCOMPLETE */
        .ui-autocomplete { max-height: 200px; overflow-y: auto; overflow-x: hidden; z-index: 10000 !important; font-family: Segoe UI, sans-serif; font-size: 12px; border: 1px solid #999; background: white; }
        .ui-autocomplete .ui-menu-item { padding: 8px 10px; border-bottom: 1px solid #eee; }
        .ui-autocomplete .ui-menu-item:hover { background-color: #4CAF50; color: white; cursor: pointer; }
        .ui-helper-hidden-accessible { display: none; }

        /* BUTTONS */
        .button-group { display: flex; justify-content: flex-end; gap: 10px; margin-top: 30px; padding-top: 15px; border-top: 1px solid #e0e0e0; }
        .btn { padding: 10px 28px; border-radius: 6px; border: none; cursor: pointer; font-size: 14px; font-weight: 500; }
        .btn-save { background: white; color: #1f6f4a; border: 1px solid #1f6f4a; }
        .btn-cancel { background: white; color: #ea4242; border: 1px solid #ea4242; }
        .btn-save:hover { background: #1f6f4a; color: white; }
        .btn-cancel:hover { background: #ce1f1f; color: white; }

        /* BULK GRID */
        .bulk-grid-container {
            margin-top: 30px;
            overflow-x: auto;
        }
        .bulk-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
        }
        .bulk-grid th {
            background: #0f7c57;
            color: white;
            padding: 12px 8px;
            text-align: left;
            border: 1px solid #0a5e42;
        }
        .bulk-grid td {
            padding: 8px;
            border: 1px solid #ddd;
        }
        .bulk-grid input {
            width: 100%;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            font-family: Segoe UI, sans-serif;
        }
        .bulk-grid .glcode-input {
            background-image: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="%23999" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>');
            background-repeat: no-repeat;
            background-position: right 8px center;
            padding-right: 25px;
        }
        .bulk-grid .readonly-field {
            background-color: #f5f5f5;
        }
        .btn-remove-row {
            color: red;
            border: none;
            padding: 5px 10px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 20px;
        }
        .bulk-actions {
            margin-top: 15px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .btn-add-row {
            background: #0f7c57;
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 4px;
            cursor: pointer;
        }
        .btn-add-row:hover {
            background: #0a5e42;
        }
        .btn-bulk-submit {
            background: #0f7c57;
            color: white;
            border: none;
            padding: 8px 25px;
            border-radius: 4px;
            cursor: pointer;
        }
        .btn-bulk-submit:hover {
            background: #0a5e42;
        }
        .auto-id {
            background-color: #e8f0fe;
            font-weight: 600;
            color: #0f7c57;
            text-align: center;
            padding: 6px;
            border-radius: 4px;
            display: inline-block;
            width: 80%;
        }

        /* STATUS */
        .status-label { margin-top: 15px; padding: 10px; border-radius: 4px; text-align: center; font-weight: bold; }
        .status-success { background-color: #d4edda; color: #155724; }
        .status-error { background-color: #f8d7da; color: #721c24; }

        /* MODAL */
        .modal-background { background-color: rgba(0,0,0,0.5); position: fixed; top: 0; left: 0; width: 100%; height: 100%; z-index: 9999; }
        .modal-dialog { background-color: white; border-radius: 8px; box-shadow: 0 5px 15px rgba(0,0,0,0.3); padding: 20px; width: 350px; margin: 15% auto; }

        /* WIDTH HELPERS */
        .w-100 { width: 100px; }
        .w-150 { width: 150px; }
        .w-200 { width: 200px; }
        .w-250 { width: 250px; }
        .w-300 { width: 300px; }
        .w-full { width: 100%; }
        .auto-generated { background-color: #e8f0fe; font-weight: 600; color: #0f7c57; }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <!-- SNACKBAR -->
        <div id="snackbar" class="snackbar"></div>

        <!-- HEADER -->
        <div id="border_header" class="header-border"></div>
        <header>
            <div style="font-weight:bold; font-size:18px;">Payable SL Type</div>
            <div style="margin-left:auto; display:flex; gap:10px;">
                <asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
            </div>
        </header>

        <div class="container">
            <!-- LEFT PANEL -->
            <div class="left-panel">
                <h3>Payable SL Type Management</h3>
                <ul>
                    <li>GL Code will auto-fetch description & family</li>
                    <li>GL SL ID is auto-generated</li>
                    <li>Enter description for the SL Type</li>
                    <li>Use Bulk Update grid for multiple entries</li>
                    <li>Click Save Bulk to save all rows</li>
                </ul>
            </div>

            <!-- RIGHT CONTENT -->
            <div class="right-content">
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfCurrentMode" runat="server" Value="ADD" />
                <asp:HiddenField ID="hfSubLedgerId" runat="server" Value="0" />
                <asp:HiddenField ID="hfSelectedGLCode" runat="server" Value="" />

                <!-- MAIN FORM PANEL -->
                <div class="form-panel">
                    <div class="panel-header">Add Single SL Type</div>
                    
                    <table class="form-table">
                        <tr>
                            <td class="label-cell">GL Code <span style="color:red;">*</span></td>
                            <td class="value-cell">
                                <asp:TextBox ID="txtGLCode" runat="server" CssClass="asp-input search-field w-250" 
                                    placeholder="Type GL Code to search..." AutoPostBack="false" />
                            </td>
                            <td class="label-cell">Description</td>
                            <td>
                                <asp:TextBox ID="txtGLDesc" runat="server" CssClass="asp-input w-250 readonly-field" 
                                    ReadOnly="true" />
                            </td>
                        </tr>
                        
                        <tr>
                            <td class="label-cell">GL SL ID</td>
                            <td>
                                <asp:TextBox ID="txtGLSLId" runat="server" CssClass="asp-input w-150 readonly-field auto-generated" 
                                    ReadOnly="true" />
                            </td>
                            <td class="label-cell">Family</td>
                            <td>
                                <asp:TextBox ID="txtFamily" runat="server" CssClass="asp-input w-100 readonly-field" 
                                    ReadOnly="true" />
                            </td>
                        </tr>
                        
                        <tr>
                            <td class="label-cell">GL SL Description <span style="color:red;">*</span></td>
                            <td colspan="3">
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="asp-input w-full" 
                                    MaxLength="100" placeholder="Enter description for this SL Type" />
                            </td>
                        </tr>
                    </table>

                    <div class="button-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save Single" CssClass="btn btn-save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-cancel" OnClick="btnClear_Click" />
                    </div>
                </div>

                <!-- BULK UPDATE GRID -->
                <div class="form-panel">
                    <div class="panel-header">Bulk Update SL Types</div>
                    
                    <div class="bulk-grid-container">
                        <asp:GridView ID="gvBulkSL" runat="server" AutoGenerateColumns="false" CssClass="bulk-grid"
                            OnRowDataBound="gvBulkSL_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="GL SL ID" ItemStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblAutoId" runat="server" CssClass="auto-id" />
                                        <asp:HiddenField ID="hfRowIndex" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="GL Code">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtGLCode" runat="server" CssClass="glcode-input" 
                                            placeholder="Search GL Code..." />
                                        <asp:HiddenField ID="hfGLCode" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="GL Description">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtGLDesc" runat="server" CssClass="readonly-field" 
                                            ReadOnly="true" style="background:#f5f5f5;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Family" ItemStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtFamily" runat="server" CssClass="readonly-field" 
                                            ReadOnly="true" style="background:#f5f5f5;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="100" 
                                            placeholder="Enter description" style="width:100%;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="60px">
                                    <ItemTemplate>
                                        <button type="button" class="btn-remove-row" onclick="removeGridRow(this)">✕</button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    
                    <div class="bulk-actions">
                        <button type="button" class="btn-add-row" onclick="addGridRow()">+ Add Row</button>
                        <asp:Button ID="btnSaveBulk" runat="server" Text="Save All Rows" CssClass="btn-bulk-submit" OnClick="btnSaveBulk_Click" />
                    </div>
                </div>

                <!-- STATUS -->
                <div id="statusContainer" runat="server" class="status-label" visible="false">
                    <asp:Label ID="lblStatus" runat="server" />
                </div>
            </div>
        </div>

        <!-- Message Popup -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="modal-dialog" Style="display: none;">
            <div style="text-align: center;">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Font-Size="14px" />
                <br /><br />
                <asp:Button ID="btnMessageOk" runat="server" Text="OK" CssClass="btn btn-save" OnClick="btnMessageOk_Click" />
            </div>
        </asp:Panel>

        <ajaxToolkit:ModalPopupExtender ID="mpeMessage" runat="server"
            TargetControlID="btnMessageOk"
            PopupControlID="pnlMessage"
            BackgroundCssClass="modal-background"
            CancelControlID="btnMessageOk" />

        <script type="text/javascript">
            // SNACKBAR FUNCTION
            function showSnackbar(message, type) {
                var snackbar = document.getElementById("snackbar");
                snackbar.textContent = message;
                snackbar.className = "snackbar snackbar-" + type;
                snackbar.classList.add("show");
                setTimeout(function () {
                    snackbar.className = "snackbar";
                }, 3000);
            }

            // AutoComplete setup for single form GL Code search
            function setupGLAutoComplete() {
                $('#<%= txtGLCode.ClientID %>').autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            type: "POST",
                            url: "payable_sl_type.aspx/SearchGLCodes",
                            data: JSON.stringify({ searchTerm: request.term }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        label: item.GL_CODE + " - " + item.GL_DESCRP,
                                        value: item.GL_CODE,
                                        desc: item.GL_DESCRP,
                                        family: item.FAMILY
                                    };
                                }));
                            },
                            error: function (xhr, status, error) {
                                console.log("Search Error: " + error);
                            }
                        });
                    },
                    minLength: 2,
                    select: function (event, ui) {
                        $('#<%= txtGLCode.ClientID %>').val(ui.item.value);
                        $('#<%= txtGLDesc.ClientID %>').val(ui.item.desc);
                        $('#<%= txtFamily.ClientID %>').val(ui.item.family);
                        $('#<%= hfSelectedGLCode.ClientID %>').val(ui.item.value);
                        __doPostBack('<%= txtGLCode.UniqueID %>', '');
                        return false;
                    }
                });
            }

            // AutoComplete for grid rows
            function setupGridAutoComplete(elementId, rowIndex) {
                $('#' + elementId).autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            type: "POST",
                            url: "payable_sl_type.aspx/SearchGLCodes",
                            data: JSON.stringify({ searchTerm: request.term }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data) {
                                response($.map(data.d, function (item) {
                                    return {
                                        label: item.GL_CODE + " - " + item.GL_DESCRP,
                                        value: item.GL_CODE,
                                        desc: item.GL_DESCRP,
                                        family: item.FAMILY
                                    };
                                }));
                            },
                            error: function (xhr, status, error) {
                                console.log("Search Error: " + error);
                            }
                        });
                    },
                    minLength: 2,
                    select: function (event, ui) {
                        var glCodeInput = $('#' + elementId);
                        var row = glCodeInput.closest('tr');
                        glCodeInput.val(ui.item.value);
                        row.find('.readonly-field[id*="txtGLDesc"]').val(ui.item.desc);
                        row.find('.readonly-field[id*="txtFamily"]').val(ui.item.family);
                        row.find('.hf-glcode').val(ui.item.value);
                        return false;
                    }
                });
            }

            // Update auto-generated IDs for grid rows
            function updateAutoIds() {
                var grid = document.getElementById('<%= gvBulkSL.ClientID %>');
                var startId = <%= GetNextAvailableId() %>;
                
                for (var i = 1; i < grid.rows.length; i++) {
                    var row = grid.rows[i];
                    var autoIdLabel = row.querySelector('.auto-id');
                    if (autoIdLabel) {
                        autoIdLabel.textContent = startId + (i - 1);
                    }
                }
            }

            // Add new row to grid
            function addGridRow() {
                var grid = document.getElementById('<%= gvBulkSL.ClientID %>');
                var tbody = grid.tBodies[0];
                var lastRow = tbody.rows[tbody.rows.length - 1];
                var newRow = lastRow.cloneNode(true);
                
                $(newRow).find('input[type="text"]').val('');
                $(newRow).find('.readonly-field').val('');
                $(newRow).find('.hf-glcode').val('');
                
                var autoIdLabel = newRow.querySelector('.auto-id');
                if (autoIdLabel) {
                    autoIdLabel.textContent = '';
                }
                
                var rowCount = tbody.rows.length;
                $(newRow).find('input, .auto-id').each(function() {
                    var oldId = this.id;
                    if (oldId) {
                        this.id = oldId + '_' + rowCount;
                    }
                });
                
                tbody.appendChild(newRow);
                updateAutoIds();
                
                var newGlCodeInput = $(newRow).find('.glcode-input')[0];
                if (newGlCodeInput && newGlCodeInput.id) {
                    setupGridAutoComplete(newGlCodeInput.id, rowCount);
                }
            }

            // Remove row from grid
            function removeGridRow(btn) {
                var grid = document.getElementById('<%= gvBulkSL.ClientID %>');
                var tbody = grid.tBodies[0];
                if (tbody.rows.length <= 1) {
                    showSnackbar('At least one row is required', 'error');
                    return;
                }
                var row = btn.parentNode.parentNode;
                tbody.removeChild(row);
                updateAutoIds();
            }

            function validateForm() {
                var glCode = $('#<%= txtGLCode.ClientID %>').val().trim();
                if (glCode === '') {
                    showSnackbar('Please select a GL Code', 'error');
                    $('#<%= txtGLCode.ClientID %>').focus();
                    return false;
                }

                var description = $('#<%= txtDescription.ClientID %>').val().trim();
                if (description === '') {
                    showSnackbar('Please enter GL SL Description', 'error');
                    $('#<%= txtDescription.ClientID %>').focus();
                    return false;
                }

                return true;
            }

            $(document).ready(function () {
                setupGLAutoComplete();
                updateAutoIds();
                
                $('.glcode-input').each(function() {
                    if (this.id) {
                        setupGridAutoComplete(this.id, 0);
                    }
                });

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
                    setupGLAutoComplete();
                    updateAutoIds();
                    $('.glcode-input').each(function() {
                        if (this.id) {
                            setupGridAutoComplete(this.id, 0);
                        }
                    });
                });
            }
        </script>
    </form>
</body>
</html>