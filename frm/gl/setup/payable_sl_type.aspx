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
        .form-panel { max-width: 800px; margin: 20px auto; background: #ffffff; padding: 30px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08); }
        .panel-header { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 8px 15px; font-size: 18px; font-weight: 600; margin-bottom: 25px; border-radius: 4px; }

        .form-table { width: 100%; border-collapse: collapse; }
        .form-table td { padding: 12px 10px; vertical-align: middle; }
        .form-table .label-cell { font-weight: 600; width: 120px; white-space: nowrap; font-size: 14px; }
        .form-table .value-cell { width: 300px; }
        .form-table .readonly-cell { background-color: #f5f5f5; padding: 8px 12px; border-radius: 4px; border: 1px solid #e0e0e0; }

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
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

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
                    <li>GL Code will auto-fetch description</li>
                    <li>GL SL ID is auto-generated</li>
                    <li>Enter description for the SL Type</li>
                    <li>Family auto-populates from GL account</li>
                    <li>Click Save to create new SL Type</li>
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
                    <div class="panel-header">Payable SL Type Details</div>
                    
                    <table class="form-table">
                        <!-- Row 1: GL Code with autocomplete -->
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
                        
                        <!-- Row 2: GL SL ID (Auto-generated) and Family -->
                        <tr>
                            <td class="label-cell">GL SL ID</td>
                            <td class="value-cell">
                                <asp:TextBox ID="txtGLSLId" runat="server" CssClass="asp-input w-150 readonly-field auto-generated" 
                                    ReadOnly="true" />
                            </td>
                            <td class="label-cell">Family</td>
                            <td>
                                <asp:TextBox ID="txtFamily" runat="server" CssClass="asp-input w-100 readonly-field" 
                                    ReadOnly="true" />
                            </td>
                        </tr>
                        
                        <!-- Row 3: GL SL Description -->
                        <tr>
                            <td class="label-cell">GL SL Description <span style="color:red;">*</span></td>
                            <td colspan="3">
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="asp-input w-full" 
                                    MaxLength="100" placeholder="Enter description for this SL Type" />
                            </td>
                        </tr>
                    </table>

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
            // AutoComplete setup for GL Code search
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

            function validateForm() {
                var glCode = $('#<%= txtGLCode.ClientID %>').val().trim();
                if (glCode === '') {
                    alert('Please select a GL Code');
                    $('#<%= txtGLCode.ClientID %>').focus();
                    return false;
                }

                var description = $('#<%= txtDescription.ClientID %>').val().trim();
                if (description === '') {
                    alert('Please enter GL SL Description');
                    $('#<%= txtDescription.ClientID %>').focus();
                    return false;
                }

                return true;
            }

            $(document).ready(function () {
                setupGLAutoComplete();

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
                });
            }
        </script>
    </form>
</body>
</html>