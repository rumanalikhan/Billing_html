<%@ Page Language="C#" AutoEventWireup="true" CodeFile="chart_of_accounts.aspx.cs" Inherits="chart_of_accounts" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Chart of Accounts</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body { margin: 0; font-family: Segoe UI, sans-serif; background-color: #f4f6f8; }

        .container { display: flex; height: 100vh; }

        /* SIDEBAR */
        .left-panel { background: #0f7c57; color: white; padding: 30px; border-radius: 8px; flex: 1; max-width: 300px; margin-right: 20px; height: auto; }
        .left-panel h2 { margin-bottom: 20px; }
        .left-panel ul { list-style: disc; padding-left: 20px; }
        .left-panel ul li { margin-bottom: 10px; }
        
        .main { flex: 1; background: #f4f6f9; padding: 25px; overflow-y: auto; }
        .card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); margin-bottom: 20px; }

        .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
        .form-group { display: flex; flex-direction: column; }
        
        /* HEADER */
        header { position: sticky; top: 0; background: #fff; border-bottom: 1px solid #e5e7eb; padding: 12px 20px; display: flex; gap: 12px; align-items: center; }
        .back-btn { background-color: black; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; }
        .logoff-btn { background-color: black; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; }

        .label-large { padding-bottom: 2px; padding-left: 10px; font-size: 25px; font-weight: 600; }

        .header-btns { background-color: black; color: white !important; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; font-size: 16px; transition: background-color 0.2s ease-in-out; text-decoration: none !important; display: inline-block; }
        .header-btns:hover { background-color: #333; text-decoration: none !important; }
        .header-border { border-top: 30px solid #000; width: 100%; margin-bottom: 0; }
        
        /* RIGHT CONTENT */
        .right-content { flex: 1; padding: 15px; background: #f4f4f4; }

        /* SECTION TITLE */
        .section-title { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 4px 8px; font-weight: 600; margin-bottom: 6px; }

        /* LEVEL ROW - LEVEL BLOCK */
        .level-row { display: flex; gap: 10px; margin-bottom: 12px; }
        .level-block { flex: 1; background: white; border: 1px solid #bdbdbd; border-radius: 5px; overflow: hidden; }

        /* GRID */
        .grid-style { width: 100%; border-collapse: collapse; font-size: 12px; }
        .grid-style th { background: #0f7c57; color: white; padding: 8px; text-align: left; border: 1px solid #0a5e40; }
        .grid-style td { padding: 6px 8px; border: 1px solid #e0e0e0; }
        .grid-style tr:hover { background: #f2f2f2; cursor: pointer; }
        .grid-style tr:nth-child(even) { background: #f9f9f9; }

        /* DETAIL PANEL */
        .detail-panel { max-width: 900px; margin: 20px auto; background: #ffffff; padding: 30px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08); }
        .panel-header { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 8px 15px; font-size: 18px; font-weight: 600; margin-bottom: 25px; border-radius: 4px; text-align: center; }

        .detail-grid { display: grid; grid-template-columns: 140px 1fr 120px 150px 120px 1fr; row-gap: 14px; column-gap: 20px; align-items: center; }
        .detail-grid label { font-weight: 600; }

        /* INPUTS */
        .labels-input { padding-left: 6px; }

        .asp-input { width: 100%; padding: 8px 10px; font-size: 14px; border: 1px solid #bfbfbf; border-radius: 4px; box-sizing: border-box; }
        .asp-input:focus { border-color: #2e7d32; box-shadow: 0 0 10px rgba(46, 125, 50, 0.3); outline: none; }
        
        .input-small { width: 80px; text-align: center; }
        .input-medium { width: 220px; }
        .input-large { width: 100%; }

        .readonly-field { background-color: #f5f5f5; font-weight: 500; }

        /* BUTTONS */
        .button-group { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
        
        .btn { padding: 10px 28px; border-radius: 6px; border: none; cursor: pointer; font-size: 14px; font-weight: 500; }
        .btn-save { background: white; color: #1f6f4a; border: 1px solid #1f6f4a; }
        .btn-cancel { background: white; color: #ea4242; border: 1px solid #ea4242; }
        .btn-print { background: white; color: #2c3e50; border: 1px solid #2c3e50; }
        .btn-save:hover { background: #1f6f4a; color: white; }
        .btn-cancel:hover { background: #ce1f1f; color: white; }
        .btn-print:hover { background: #2c3e50; color: white; }

        /* STATUS */
        .status-msg { margin-top: 15px; padding: 10px; border-radius: 4px; text-align: center; font-weight: bold; }
        .status-success { background-color: #d4edda; color: #155724; }
        .status-error { background-color: #f8d7da; color: #721c24; }
        
        @media print { .no-print { display: none; } }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</head>

<body>
    <form id="form1" runat="server">

        <%-- HEADER --%>            
        <div id="border_header" class="header-border"></div>
        <header>
            <div style="font-weight:bold; font-size:18px;">Chart of Accounts</div>
            <div style="margin-left:auto; display:flex; gap:10px;">
                <asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true" />
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
            </div>
        </header>

        <div class="container">
            <!-- LEFT PANEL -->
            <div class="left-panel">
                <h3>Chart of Accounts</h3>
                <ul>
                    <li>Select Level 1 (Assets / etc.)</li>
                    <li>Click on any account to edit</li>
                    <li>Use Save to commit changes</li>
                </ul>
            </div>

            <!-- RIGHT CONTENT -->
            <div class="right-content">

                <!-- LEVEL 1 Radio Buttons -->
                <div class="card">
                    <h3>Level 1</h3>
                    <asp:RadioButtonList ID="rbBillType" runat="server"
                        RepeatDirection="Horizontal"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="rbBillType_SelectedIndexChanged">
                        <asp:ListItem Text="Assets" Value="A" />
                        <asp:ListItem Text="Liability" Value="L" />
                        <asp:ListItem Text="Capital" Value="C" />
                        <asp:ListItem Text="Revenue" Value="R" />
                        <asp:ListItem Text="Expenses" Value="E" />
                    </asp:RadioButtonList>
                </div>

                <!-- LEVEL 2 / 3 / 4 -->
                <div class="level-row">
                    <div class="level-block">
                        <div class="section-title">Level 2</div>
                        <div style="max-height:250px; overflow-y:auto;">
                            <asp:GridView ID="gvLevel2" runat="server"
                                CssClass="grid-style"
                                AutoGenerateColumns="false"
                                DataKeyNames="Code"
                                OnSelectedIndexChanged="gvLevel2_SelectedIndexChanged">
                                <Columns>
                                    <asp:BoundField DataField="Code" HeaderText="Code" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:CommandField ShowSelectButton="true" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="level-block">
                        <div class="section-title">Level 3</div>
                        <div style="max-height:250px; overflow-y:auto;">
                            <asp:GridView ID="gvLevel3" runat="server"
                                CssClass="grid-style"
                                AutoGenerateColumns="false"
                                DataKeyNames="Code"
                                OnSelectedIndexChanged="gvLevel3_SelectedIndexChanged">
                                <Columns>
                                    <asp:BoundField DataField="Code" HeaderText="Code" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:CommandField ShowSelectButton="true" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="level-block">
                        <div class="section-title">Level 4</div>
                        <div style="max-height:250px; overflow-y:auto;">
                            <asp:GridView ID="gvLevel4" runat="server"
                                CssClass="grid-style"
                                AutoGenerateColumns="false"
                                DataKeyNames="Code"
                                OnSelectedIndexChanged="gvLevel4_SelectedIndexChanged">
                                <Columns>
                                    <asp:BoundField DataField="Code" HeaderText="Code" />
                                    <asp:BoundField DataField="Description" HeaderText="Description" />
                                    <asp:CommandField ShowSelectButton="true" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- UPDATE TRANSACTION -->
                <div class="panel-header">Update Transaction</div>
                <div class="detail-panel">
                    <div class="detail-grid">
                        <label>Parent</label>
                        <asp:TextBox ID="txtParent" runat="server" CssClass="asp-input input-medium readonly-field" ReadOnly="true" />

                        <label>Family</label>
                        <asp:TextBox ID="txtFamily" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                        <label>Active</label>
                        <asp:TextBox ID="txtAI" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                        <label>Code</label>
                        <asp:TextBox ID="txtCode" runat="server" CssClass="asp-input input-medium"/>

                        <label>Level</label>
                        <asp:TextBox ID="txtAccLevel" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                        <label>General Details</label>
                        <asp:TextBox ID="txtGenDetail" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                        <label>Opening Balance</label>
                        <asp:TextBox ID="txtOB" runat="server" CssClass="asp-input input-medium" />

                        <label>Description</label>
                        <asp:TextBox ID="txtDesc" runat="server" CssClass="asp-input input-medium" />                             

                        <div></div>
                        <div></div>
                    </div>                        

                    <%-- BUTTONS --%>
                    <div class="button-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-cancel" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="btn btn-print" OnClick="btnPrint_Click" />
                    </div>

                    <!-- STATUS --> 
                    <div id="statusContainer" runat="server" class="status-msg" visible="false">
                        <asp:Label ID="lblStatus" runat="server" />
                    </div>
                </div>
            </div>       
        </div>

    </form>
</body>
</html>