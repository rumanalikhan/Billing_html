<%@ Page Language="C#" AutoEventWireup="true" CodeFile="coa.aspx.cs" Inherits="coa" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chart of Accounts</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .level { margin-bottom: 20px; padding: 15px; border: 1px solid #ddd; border-radius: 5px; }
        .level h3 { margin-top: 0; color: #333; }
        .account-grid { width: 100%; border-collapse: collapse; margin-top: 10px; }
        .account-grid th, .account-grid td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        .account-grid th { background-color: #f2f2f2; }
        .opening-balance { background-color: #f9f9f9; padding: 10px; margin: 10px 0; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Chart of Accounts</h2>
            
            <!-- Level-1 -->
            <div class="level">
                <h3>Level-1</h3>
                <asp:RadioButtonList ID="rblLevel1" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Assets" Value="Assets"></asp:ListItem>
                    <asp:ListItem Text="Liabilities" Value="Liabilities"></asp:ListItem>
                    <asp:ListItem Text="Capital" Value="Capital"></asp:ListItem>
                    <asp:ListItem Text="Revenue" Value="Revenue"></asp:ListItem>
                    <asp:ListItem Text="Expenses" Value="Expenses"></asp:ListItem>
                </asp:RadioButtonList>
            </div>

            <!-- Level-2 -->
            <div class="level">
                <h3>Level-2</h3>
                <asp:GridView ID="gvLevel2" runat="server" CssClass="account-grid" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Code" HeaderText="Code" />
                        <asp:BoundField DataField="ParentCode" HeaderText="Parent Code" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Opening Balance Section -->
            <div class="opening-balance">
                <h3>Account Details</h3>
                <table>
                    <tr>
                        <td><strong>Account Code:</strong></td>
                        <td>£100:0001</td>
                    </tr>
                    <tr>
                        <td><strong>Opening Balance:</strong></td>
                        <td>
                            <asp:TextBox ID="txtOpeningBalance" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>

            <!-- Level-3 -->
            <div class="level">
                <h3>Level-3</h3>
                <asp:GridView ID="gvLevel3" runat="server" CssClass="account-grid" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Code" HeaderText="Code" />
                        <asp:BoundField DataField="ParentCode" HeaderText="Parent Code" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Level-4 -->
            <div class="level">
                <h3>Level-4</h3>
                <asp:GridView ID="gvLevel4" runat="server" CssClass="account-grid" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Code" HeaderText="Code" />
                        <asp:BoundField DataField="ParentCode" HeaderText="Parent Code" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Buttons -->
            <div>
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
        </div>
    </form>
</body>
</html>