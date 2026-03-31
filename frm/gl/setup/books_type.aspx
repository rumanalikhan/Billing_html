<%@ Page Language="C#" AutoEventWireup="true" CodeFile="books_type.aspx.cs" Inherits="books_type" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Books Type Management</title>
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
        .form-table .label-cell { font-weight: 600; width: 140px; white-space: nowrap; font-size: 14px; }
        .form-table .value-cell { width: 300px; }

        /* INPUTS */
        .asp-input { width: 100%; padding: 8px 10px; font-size: 14px; border: 1px solid #bfbfbf; border-radius: 4px; box-sizing: border-box; }
        .asp-input:focus { border-color: #2e7d32; box-shadow: 0 0 10px rgba(46, 125, 50, 0.3); outline: none; }
        .readonly-field { background-color: #f5f5f5; font-weight: 500; border-color: #d9d9d9; }

        /* DROPDOWN */
        .book-type-dropdown { width: 250px; padding: 8px 10px; font-size: 14px; border: 1px solid #bfbfbf; border-radius: 4px; background-color: white; }

        /* BUTTONS */
        .button-group { display: flex; justify-content: flex-end; gap: 10px; margin-top: 30px; padding-top: 15px; border-top: 1px solid #e0e0e0; }
        .btn { padding: 10px 28px; border-radius: 6px; border: none; cursor: pointer; font-size: 14px; font-weight: 500; transition: all 0.3s; }
        .btn-save { background: white; color: #1f6f4a; border: 1px solid #1f6f4a; }
        .btn-cancel { background: white; color: #ea4242; border: 1px solid #ea4242; }
        .btn-save:hover { background: #1f6f4a; color: white; }
        .btn-cancel:hover { background: #ce1f1f; color: white; }

        /* GRID */
        .grid-container { margin-top: 20px; overflow-x: auto; }
        .gridview-style { width: 100%; border-collapse: collapse; font-size: 12px; }
        .gridview-style th { background: #0f7c57; color: white; padding: 10px; text-align: left; border: 1px solid #0a5e40; }
        .gridview-style td { padding: 8px; border: 1px solid #e0e0e0; }
        .gridview-style tr:nth-child(even) { background: #f9f9f9; }
        .gridview-style tr:hover { background: #f5f5f5; }

        /* STATUS SNACKBAR STYLES */
        .status-label { 
            margin-top: 15px; 
            padding: 12px 20px; 
            border-radius: 4px; 
            text-align: center; 
            font-weight: bold; 
            font-size: 14px;
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

        /* MODAL */
        .modal-background { background-color: rgba(0,0,0,0.5); position: fixed; top: 0; left: 0; width: 100%; height: 100%; z-index: 9999; }
        .modal-dialog { background-color: white; border-radius: 8px; box-shadow: 0 5px 15px rgba(0,0,0,0.3); padding: 20px; width: 350px; margin: 15% auto; text-align: center; }
        .modal-dialog button { margin-top: 15px; }

        /* WIDTH HELPERS */
        .w-150 { width: 150px; }
        .w-200 { width: 200px; }
        .w-250 { width: 250px; }
        .w-300 { width: 300px; }
        .auto-generated { background-color: #e8f0fe; font-weight: 600; color: #0f7c57; }
        
        /* Hide grid */
        .hidden-grid { display: none; }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- HEADER -->
        <div id="border_header" class="header-border"></div>
        <header>
            <div style="font-weight:bold; font-size:18px;">Books Type Management</div>
            <div style="margin-left:auto; display:flex; gap:10px;">
                <asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
            </div>
        </header>

        <div class="container">
            <!-- LEFT PANEL -->
            <div class="left-panel">
                <h3>Books Type Management</h3>
                <ul>
                    <li>Select Book Type from master list</li>
                    <li>Book Type ID auto-generates per type</li>
                    <li>Enter GL Code manually</li>
                    <li>Click Save to create mapping</li>
                </ul>
                <div style="margin-top: 20px; padding: 10px; background: rgba(255,255,255,0.2); border-radius: 5px;">
                    <small><strong>Note:</strong> Book Type ID starts from 1 for each book type and increments automatically</small>
                </div>
            </div>

            <!-- RIGHT CONTENT -->
            <div class="right-content">
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfCurrentMode" runat="server" Value="ADD" />
                <asp:HiddenField ID="hfBookTypeId" runat="server" Value="0" />
                <asp:HiddenField ID="hfCurrentBookType" runat="server" Value="" />

                <!-- MAIN FORM PANEL -->
                <div class="form-panel">
                    <div class="panel-header">Book Type Details</div>
                    
                    <table class="form-table">
                        <tr>
                            <td class="label-cell">Book Type <span style="color:red;">*</span>:</td>
                            <td class="value-cell">
                                <asp:DropDownList ID="ddlBookType" runat="server" CssClass="book-type-dropdown" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlBookType_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td class="label-cell"></td>
                            <td class="value-cell"></td>
                        </tr>
                        
                        <tr>
                            <td class="label-cell">Book Type ID <span style="color:red;">*</span>:</td>
                            <td class="value-cell">
                                <asp:TextBox ID="txtBookTypeId" runat="server" CssClass="asp-input w-150 readonly-field auto-generated" 
                                    ReadOnly="true" />
                            </td>
                            <td class="label-cell"></td>
                            <td class="value-cell"></td>
                        </tr>
                        
                        <tr>
                            <td class="label-cell">GL Code <span style="color:red;">*</span>:</td>
                            <td colspan="3">
                                <asp:TextBox ID="txtGLCode" runat="server" CssClass="asp-input w-300" 
                                    MaxLength="15" placeholder="Enter GL Code (e.g., 111003)" />
                            </td>
                        </tr>
                    </table>

                    <div class="button-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save Book Type" CssClass="btn btn-save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear Form" CssClass="btn btn-cancel" OnClick="btnClear_Click" />
                    </div>

                    <!-- STATUS SNACKBAR -->
                    <div id="statusContainer" runat="server" class="status-label" visible="false" style="display: none;">
                        <asp:Label ID="lblStatus" runat="server" />
                    </div>
                </div>

                <!-- EXISTING BOOK TYPES GRID (HIDDEN) -->
                <div class="card hidden-grid" id="gridContainer" runat="server">
                    <h3>Existing Book Type Mappings</h3>
                    <div class="grid-container">
                        <asp:GridView ID="gvBookTypes" runat="server" CssClass="gridview-style" AutoGenerateColumns="False" 
                            OnRowCommand="gvBookTypes_RowCommand" DataKeyNames="BOOK_TYPE_ID">
                            <Columns>
                                <asp:BoundField DataField="BOOK_TYPE_ID" HeaderText="ID" ItemStyle-Width="60px" />
                                <asp:BoundField DataField="BOOK_TYPE" HeaderText="Book Type" ItemStyle-Width="80px" />
                                <asp:BoundField DataField="GL_CODE" HeaderText="GL Code" ItemStyle-Width="100px" />
                                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditRow" 
                                            CommandArgument='<%# Eval("BOOK_TYPE_ID") + "|" + Eval("BOOK_TYPE") %>' Text="Edit" 
                                            Style="color: #2196F3; text-decoration: none;" />
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteRow" 
                                            CommandArgument='<%# Eval("BOOK_TYPE_ID") %>' Text="Delete" 
                                            Style="color: #f44336; text-decoration: none; margin-left: 10px;"
                                            OnClientClick="return confirm('Delete this mapping?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div style="padding: 20px; text-align: center;">No book type mappings found</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <!-- Message Popup -->
        <asp:Panel ID="pnlMessage" runat="server" CssClass="modal-dialog" Style="display: none;">
            <div>
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" Font-Size="14px" />
                <br />
                <asp:Button ID="btnMessageOk" runat="server" Text="OK" CssClass="btn btn-save" OnClick="btnMessageOk_Click" />
            </div>
        </asp:Panel>

        <ajaxToolkit:ModalPopupExtender ID="mpeMessage" runat="server"
            TargetControlID="btnMessageOk"
            PopupControlID="pnlMessage"
            BackgroundCssClass="modal-background"
            CancelControlID="btnMessageOk" />
    </form>
</body>
</html>