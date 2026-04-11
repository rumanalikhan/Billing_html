<%@ Page Language="C#" AutoEventWireup="true" CodeFile="view_voucher.aspx.cs" Inherits="view_voucher" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Voucher Search & Print</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <style>
        body { margin: 0; font-family: Segoe UI, sans-serif; background-color: #f4f6f8; }

        .container { display: flex; 
    min-height: calc(100vh - 80px); }

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
        .right-content { flex: 1; padding: 15px; background: #f4f4f4;  }

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
        .voucher-type-dropdown { width: 200px; padding: 8px 10px; font-size: 14px; border: 1px solid #bfbfbf; border-radius: 4px; background-color: white; }

        /* BUTTONS */
        .button-group { display: flex; justify-content: flex-end; gap: 10px; margin-top: 30px; padding-top: 15px; border-top: 1px solid #e0e0e0; }
        .btn { padding: 10px 28px; border-radius: 6px; border: none; cursor: pointer; font-size: 14px; font-weight: 500; transition: all 0.3s; }
        .btn-search { background: white; color: #0f7c57; border: 1px solid #0f7c57; }
        .btn-print { background: white; color: #2196F3; border: 1px solid #2196F3; }
        .btn-cancel { background: white; color: #ea4242; border: 1px solid #ea4242; }
        .btn-search:hover { background: #0f7c57; color: white; }
        .btn-print:hover { background: #2196F3; color: white; }
        .btn-cancel:hover { background: #ce1f1f; color: white; }

        /* GRID */
        .grid-container { margin-top: 20px; overflow-x: auto; }
        .gridview-style { width: 100%; border-collapse: collapse; font-size: 12px; }
        .gridview-style th { background: #0f7c57; color: white; padding: 10px; text-align: left; border: 1px solid #0a5e40; }
        .gridview-style td { padding: 8px; border: 1px solid #e0e0e0; }
        .gridview-style tr:nth-child(even) { background: #f9f9f9; }
        .gridview-style tr:hover { background: #f5f5f5; }

        /* STATUS */
        .status-label { margin-top: 15px; padding: 10px; border-radius: 4px; text-align: center; font-weight: bold; }
        .status-success { background-color: #d4edda; color: #155724; }
        .status-error { background-color: #f8d7da; color: #721c24; }
        .status-info { background-color: #d1ecf1; color: #0c5460; }

        /* MODAL */
        .modal-background { background-color: rgba(0,0,0,0.5); position: fixed; top: 0; left: 0; width: 100%; height: 100%; z-index: 9999; }
        .modal-dialog { background-color: white; border-radius: 8px; box-shadow: 0 5px 15px rgba(0,0,0,0.3); padding: 20px; width: 350px; margin: 15% auto; text-align: center; }
        .modal-dialog button { margin-top: 15px; }

        .w-300 { width: 300px; }
        .action-link { color: #0f7c57; text-decoration: none; font-weight: bold; }
        .action-link:hover { text-decoration: underline; }
        
        .status-posted { color: green; font-weight: bold; }
        .status-unposted { color: red; font-weight: bold; }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- HEADER -->
        <div id="border_header" class="header-border"></div>
        <header>
            <div style="font-weight:bold; font-size:18px;">Voucher Search & Print</div>
            <div style="margin-left:auto; display:flex; gap:10px;">
                <asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
            </div>
        </header>

        <div class="container">
            <!-- LEFT PANEL -->
            <div class="left-panel">
                <h3>Voucher Search</h3>
                <ul>
                    <li>Enter Voucher Key to search</li>
                    <li>Format: 1-GJV-407 or 1-CPV-100</li>
                    <li>Select Voucher Type to filter</li>
                    <li>Click Search to find voucher</li>
                    <li>Click Print to view and print</li>
                </ul>
                <div style="margin-top: 20px; padding: 10px; background: rgba(255,255,255,0.2); border-radius: 5px;">
                    <small><strong>Note:</strong> You can search by full or partial voucher key</small>
                </div>
            </div>

            <!-- RIGHT CONTENT -->
            <div class="right-content">
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfVoucherKey" runat="server" Value="" />

                <!-- MAIN FORM PANEL -->
                <div class="form-panel">
                    <div class="panel-header">Search Voucher</div>
                    
                    <table class="form-table">
                        <tr>
                            <td class="label-cell">Voucher Key:</td>
                            <td class="value-cell">
                                <asp:TextBox ID="txtVoucherKey" runat="server" CssClass="asp-input w-300" 
                                    placeholder="e.g., 1-GJV-407 or 1-CPV-100" />
                            </td>
                            <td class="label-cell">Voucher Type:</td>
                            <td class="value-cell">
                                <asp:DropDownList ID="ddlVoucherType" runat="server" CssClass="voucher-type-dropdown">
                                    <asp:ListItem Text="-- All --" Value="" />
                                    <asp:ListItem Text="GJV - Journal Voucher" Value="GJV" />
                                    <asp:ListItem Text="CPV - Cash Payment Voucher" Value="CPV" />
                                    <asp:ListItem Text="CRV - Cash Receipt Voucher" Value="CRV" />
                                    <asp:ListItem Text="GPV - General Payment Voucher" Value="GPV" />
                                    <asp:ListItem Text="GRV - General Receipt Voucher" Value="GRV" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>

                    <div class="button-group">
                        <asp:Button ID="btnSearch" runat="server" Text="🔍 Search" CssClass="btn btn-search" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-cancel" OnClick="btnClear_Click" />
                    </div>

                    <!-- STATUS -->
                    <div id="statusContainer" runat="server" class="status-label" visible="false">
                        <asp:Label ID="lblStatus" runat="server" />
                    </div>
                </div>

                <!-- SEARCH RESULTS GRID -->
                <div class="card">
                    <h3>Search Results</h3>
                    <div class="grid-container">
                        <asp:GridView ID="gvResults" runat="server" CssClass="gridview-style" AutoGenerateColumns="False" 
                            OnRowCommand="gvResults_RowCommand" EmptyDataText="No vouchers found. Please try different search criteria.">
                            <Columns>
                                <asp:BoundField DataField="VOUCHER_KEY" HeaderText="Voucher Key" ItemStyle-Width="180px" />
                                <asp:BoundField DataField="BOOK_TYPE" HeaderText="Type" ItemStyle-Width="60px" />
                                <asp:BoundField DataField="VOUCHER_NUMBER" HeaderText="Number" ItemStyle-Width="80px" />
                                <asp:BoundField DataField="VOUCHER_DATE" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" />
                                <asp:TemplateField HeaderText="Status" ItemStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TOTAL_AMOUNT" HeaderText="Total Amount" DataFormatString="{0:N2}" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Right" />
                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPrint" runat="server" CommandName="PrintVoucher" 
                                            CommandArgument='<%# Eval("VOUCHER_KEY") + "|" + Eval("BOOK_TYPE") %>'
                                            Text="Print" CssClass="action-link" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
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
                <asp:Button ID="btnMessageOk" runat="server" Text="OK" CssClass="btn btn-search" OnClick="btnMessageOk_Click" />
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