<%@ Page Language="C#" AutoEventWireup="true" CodeFile="gl_vouch.aspx.cs" Inherits="gl_vouch" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>General Ledger Voucher</title>
    <style>
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            height: 100vh;
            overflow: hidden;
        }

        .voucher-container {
            width: 100%;
            height: 100vh;
            background-color: white;
            padding: 20px 30px;
            margin: 0;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            overflow-y: auto;
            box-sizing: border-box;
        }

        h2 {
            margin-top: 0;
            color: #333;
            border-bottom: 2px solid #4CAF50;
            padding-bottom: 10px;
        }

        .voucher-header {
            background-color: #f9f9f9;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            border: 1px solid #ddd;
        }

        .header-row {
            margin-bottom: 10px;
            overflow: hidden;
        }

        .header-label {
            float: left;
            width: 100px;
            font-weight: bold;
            line-height: 30px;
        }

        .header-field {
            float: left;
            margin-right: 20px;
        }

        .voucher-details {
            margin-bottom: 20px;
        }

        .voucher-grid {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 15px;
        }

            .voucher-grid th {
                background-color: #4CAF50;
                color: white;
                padding: 10px;
                font-weight: normal;
                text-align: left;
            }

            .voucher-grid td {
                padding: 8px;
                border-bottom: 1px solid #ddd;
            }

            .voucher-grid input {
                width: 100%;
                padding: 5px;
                border: 1px solid #ccc;
                border-radius: 3px;
                box-sizing: border-box;
            }

                .voucher-grid input:focus {
                    border-color: #4CAF50;
                    outline: none;
                }

        .total-row {
            background-color: #f2f2f2;
            font-weight: bold;
        }

            .total-row td {
                padding: 10px;
            }

        .action-buttons {
            margin: 20px 0;
            text-align: right;
        }

        .btn {
            padding: 10px 20px;
            margin-left: 10px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-primary {
            background-color: #4CAF50;
            color: white;
        }

            .btn-primary:hover {
                background-color: #45a049;
            }

        .btn-secondary {
            background-color: #f0f0f0;
            color: #333;
        }

            .btn-secondary:hover {
                background-color: #e0e0e0;
            }

        .btn-danger {
            background-color: #f44336;
            color: white;
        }

            .btn-danger:hover {
                background-color: #da190b;
            }

        .message-box {
            padding: 10px;
            margin: 10px 0;
            border-radius: 4px;
        }

        .success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .info {
            background-color: #d1ecf1;
            color: #0c5460;
            border: 1px solid #bee5eb;
        }

        .footer {
            margin-top: 20px;
            text-align: right;
            color: #666;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="voucher-container">
            <h2>General Ledger Voucher</h2>

            <!-- Message Display -->
            <asp:Label ID="lblMessage" runat="server" CssClass="message-box" Visible="false"></asp:Label>

            <!-- Voucher Header Section -->
            <div class="voucher-header">
                <div class="header-row">
                    <div class="header-label">Voucher No:</div>
                    <div class="header-field">
                        <asp:TextBox ID="txtVoucherNo" runat="server" Width="150px"
                            placeholder="Auto-generated" ReadOnly="true"></asp:TextBox>
                    </div>

                    <div class="header-label" style="width: 80px;">Book Type:</div>
                    <div class="header-field">
                        <asp:DropDownList ID="ddlVoucherType" runat="server" Width="150px">
                            <asp:ListItem Text="Journal Voucher" Value="JV" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Payment Voucher" Value="PV"></asp:ListItem>
                            <asp:ListItem Text="Receipt Voucher" Value="RV"></asp:ListItem>
                            <asp:ListItem Text="Contra Voucher" Value="CV"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="header-row">
                    <div class="header-label">Date:</div>
                    <div class="header-field">
                        <asp:TextBox ID="txtDate" runat="server" Width="150px" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="header-label" style="width: 80px;">Narration:</div>
                    <div class="header-field">
                        <asp:TextBox ID="txtNarration" runat="server" Width="300px"
                            placeholder="Brief description of transaction"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Voucher Details Grid -->
            <h3>Voucher Entries</h3>
            <asp:GridView ID="gvVoucherEntries" runat="server"
                CssClass="voucher-grid"
                AutoGenerateColumns="false"
                ShowFooter="true"
                DataKeyNames="RowID"
                OnRowCommand="gvVoucherEntries_RowCommand"
                OnRowEditing="gvVoucherEntries_RowEditing"
                OnRowUpdating="gvVoucherEntries_RowUpdating"
                OnRowCancelingEdit="gvVoucherEntries_RowCancelingEdit"
                OnRowDeleting="gvVoucherEntries_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="GL Code">
                        <ItemTemplate>
                            <asp:Label ID="lblGLCode" runat="server" Text='<%# Eval("GLCode") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditGLCode" runat="server" Text='<%# Eval("GLCode") %>' Width="80px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtNewGLCode" runat="server" placeholder="e.g., 1010" Width="80px"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Account Name">
                        <ItemTemplate>
                            <asp:Label ID="lblAccountName" runat="server" Text='<%# Eval("AccountName") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlEditAccount" runat="server" Width="150px" SelectedValue='<%# Eval("GLCode") %>'>
                                <asp:ListItem Text="Cash Account" Value="1010"></asp:ListItem>
                                <asp:ListItem Text="Bank Account" Value="1020"></asp:ListItem>
                                <asp:ListItem Text="Accounts Receivable" Value="1030"></asp:ListItem>
                                <asp:ListItem Text="Accounts Payable" Value="2010"></asp:ListItem>
                                <asp:ListItem Text="Sales Revenue" Value="4010"></asp:ListItem>
                                <asp:ListItem Text="Rent Expense" Value="5020"></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:DropDownList ID="ddlNewAccount" runat="server" Width="150px">
                                <asp:ListItem Text="Select Account" Value=""></asp:ListItem>
                                <asp:ListItem Text="Cash Account" Value="1010"></asp:ListItem>
                                <asp:ListItem Text="Bank Account" Value="1020"></asp:ListItem>
                                <asp:ListItem Text="Accounts Receivable" Value="1030"></asp:ListItem>
                                <asp:ListItem Text="Accounts Payable" Value="2010"></asp:ListItem>
                                <asp:ListItem Text="Sales Revenue" Value="4010"></asp:ListItem>
                                <asp:ListItem Text="Rent Expense" Value="5020"></asp:ListItem>
                            </asp:DropDownList>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditDescription" runat="server" Text='<%# Eval("Description") %>' Width="150px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtNewDescription" runat="server" placeholder="Description" Width="150px"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Debit (Dr)">
                        <ItemTemplate>
                            <asp:Label ID="lblDebit" runat="server" Text='<%# Eval("Debit", "{0:N2}") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditDebit" runat="server" Text='<%# Eval("Debit", "{0:N2}") %>' Width="80px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtNewDebit" runat="server" placeholder="0.00" Text="0.00" Width="80px"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Credit (Cr)">
                        <ItemTemplate>
                            <asp:Label ID="lblCredit" runat="server" Text='<%# Eval("Credit", "{0:N2}") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEditCredit" runat="server" Text='<%# Eval("Credit", "{0:N2}") %>' Width="80px"></asp:TextBox>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:TextBox ID="txtNewCredit" runat="server" placeholder="0.00" Text="0.00" Width="80px"></asp:TextBox>
                        </FooterTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server"
                                CommandName="Edit"
                                Text="Edit"
                                ForeColor="Blue">
                            </asp:LinkButton>
                            |
                <asp:LinkButton ID="btnDelete" runat="server"
                    CommandName="Delete"
                    CommandArgument='<%# Container.DataItemIndex %>'
                    Text="Delete"
                    OnClientClick="return confirm('Delete this entry?');"
                    ForeColor="Red">
                </asp:LinkButton>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:LinkButton ID="btnUpdate" runat="server"
                                CommandName="Update"
                                Text="Update"
                                ForeColor="Green">
                            </asp:LinkButton>
                            |
                <asp:LinkButton ID="btnCancel" runat="server"
                    CommandName="Cancel"
                    Text="Cancel"
                    ForeColor="Gray">
                </asp:LinkButton>
                        </EditItemTemplate>
                        <FooterTemplate>
                            <asp:LinkButton ID="btnAdd" runat="server"
                                CommandName="AddNew"
                                Text="Add Line"
                                ForeColor="Green"
                                Font-Bold="true">
                            </asp:LinkButton>
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Totals Section -->
            <table class="voucher-grid">
                <tr class="total-row">
                    <td colspan="3" style="text-align: right;"><strong>Totals:</strong></td>
                    <td>
                        <asp:Label ID="lblTotalDebit" runat="server" Text="0.00"></asp:Label></td>
                    <td>
                        <asp:Label ID="lblTotalCredit" runat="server" Text="0.00"></asp:Label></td>
                    <td></td>
                </tr>
                <tr class="total-row" style="background-color: #e8f5e8;">
                    <td colspan="3" style="text-align: right;"><strong>Difference:</strong></td>
                    <td colspan="2">
                        <asp:Label ID="lblDifference" runat="server" Text="0.00" Font-Bold="true"></asp:Label>
                    </td>
                    <td></td>
                </tr>
            </table>

            <!-- Action Buttons -->
            <div class="action-buttons">
                <asp:Button ID="btnAddSample" runat="server" Text="Add Sample Data"
                    CssClass="btn btn-secondary" OnClick="btnAddSample_Click" />
                <asp:Button ID="btnSave" runat="server" Text="Save Voucher"
                    CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear"
                    CssClass="btn btn-secondary" OnClick="btnClear_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel"
                    CssClass="btn btn-danger" OnClick="btnCancel_Click" />
            </div>

            <!-- Footer Info -->
            <div class="footer">
                Note: Debits must equal Credits. Difference should be 0.00 to save.
            </div>
        </div>
    </form>
</body>
</html>
