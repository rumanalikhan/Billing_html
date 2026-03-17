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
            header{position:sticky;top:0;background:#fff;border-bottom:1px solid #e5e7eb;padding:12px 20px;display:flex;gap:12px;align-items:center;}
            .back-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}
            .logoff-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}

            .label-large {padding-bottom: 2px;padding-left: 10px;font-size: 25px;font-weight: 600;}

            .header-btns {background-color: black;color: white !important;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;font-size: 16px;transition: background-color 0.2s ease-in-out;text-decoration: none !important;display: inline-block;}
            .header-btns:hover {background-color: #333;text-decoration: none !important;}
            .header-border {border-top: 30px solid #000;width: 100%;margin-bottom: 0;}
            
            /* RIGHT CONTENT */
            .right-content { flex: 1; padding: 15px; background: #f4f4f4; }

            /* SECTION TITLE */
            .section-title { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 4px 8px; font-weight: 600; margin-bottom: 6px; }

            /* LEVEL ROW - LEVEL BLOCK */
            .level-row { display: flex; gap: 10px; margin-bottom: 12px; }
            .level-block { flex: 1; background: white; border: 1px solid #bdbdbd; }

            /* GRID */
            .grid-style { width: 100%; border-collapse: collapse; font-size: 12px; }
            .grid-style th { background: #d9d9d9; border: 1px solid #bfbfbf; padding: 4px; text-align: left; }
            .grid-style td { border: 1px solid #e0e0e0; padding: 4px; }
            .grid-style tr:hover { background: #f2f2f2; }

            /* DETAIL PANEL */
            .detail-panel { max-width: 900px; margin: 20px auto;  /* Center horizontally */
                            background: #ffffff; padding: 30px 40px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08); }

            .panel-header { background: #dcdcdc; border: 1px solid #bdbdbd; padding: 4px 8px; font-size: 18px; font-weight: 600; margin-bottom: 20px; text-align: center; }

            .detail-grid { display: grid; grid-template-columns: 140px 1fr 120px 150px 120px 1fr; row-gap: 14px; column-gap: 20px; align-items: center; }
            .detail-grid label { font-weight: 600; }

            /* INPUTS */
            .labels-input { padding-left: 6px; }

            .asp-input { width: 100%; padding: 3px; font-size: 12px; border: 1px solid #bfbfbf; }
            .asp-input:focus { border-color: #2e7d32; box-shadow: 0 0 10px rgba(46, 125, 50, 0.3); }
            
            .input-small { width: 60px; text-align: center; }
            .input-medium { width: 220px; }
            .input-large { width: 100%; }

            .readonly-field { background-color: #f5f5f5; font-weight: 600; pointer-events: none; }

            /* BUTTONS */
            .button-group { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
            
            .btn { padding: 8px 18px; border-radius: 6px; border: none; cursor: pointer; }

            .btn-save { background: white; color: #1f6f4a; border: #1f6f4a 1px solid; }
            .btn-cancel { background: white; color: #ea4242; border: #ea4242 1px solid; }
            .btn-print { background: white; color: #2c3e50; border: #2c3e50 1px solid; }
            
            .btn-save:hover { background: #1f6f4a; color: white; }
            .btn-cancel:hover { background: #ce1f1f; color: white; }
            .btn-print:hover { background: #2c3e50; color: white; }            
            
        </style>

        <%-- Script for Confirmation Popup --%>
        <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    </head>

    <body>
        <form id="form1" runat="server">

            <%-- HEADER --%>            
            <div id="border_header" class="header-border"></div>
            <header>
                <div id="brand_name" CssClass="header-border" style="width:10%; ">Chart of Accounts</div>
    
                <div id="header_actions_goback" style="width:10%; float:left;">
                    <asp:LinkButton ID="btnGoBack" runat="server" CssClass="header-btns" OnClick="btnGoBack_Click">Go Back</asp:LinkButton>
                </div>

                <div id="header_user" style="width:56%; float:left; text-align:center; color:white; font-weight:bold;">
                    <asp:Label ID="lbl" runat="server" ForeColor="Blue"></asp:Label>
                </div>

                <div id="header_actions_logoff" style="width:22%; text-align:right; float:left;">
                    <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
                </div>
            </header>

            <%-- USER LABEL --%>
            <div style="text-align:center; font-weight:bold; margin:10px">
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue"></asp:Label>
            </div>

            <div class="container">
                <!-- LEFT PANEL -->
                <div class="left-panel">
                    <h3>Chart of Accounts</h3>
                    <ul>
                        <li>Select Level 1 (Assets / etc.)</li>
                        <li>Add or Modify Account</li>
                        <li>Use Save to commit changes</li>
                    </ul>
                </div>

                <!-- RIGHT CONTENT -->
                <div class="right-content">

                    <!-- LEVEL 1 -->
                    <div class="card">
                        <h1>Chart of Accounts</h1>
                        <h3>Level 1</h3>
                        <asp:RadioButtonList
                            ID="rbBillType"
                            runat="server"
                            RepeatDirection="Horizontal"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="rbBillType_SelectedIndexChanged">
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
                                        <asp:BoundField DataField="Opening_Balance" HeaderText="Opening Balance" />
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
                            <!-- Row 1 -->
                            <label class="labels-input">Parent</label>
                            <asp:TextBox ID="txtParent" runat="server" CssClass="asp-input input-medium readonly-field" ReadOnly="true" />

                            <label class="labels-input">Family</label>
                            <asp:TextBox ID="txtFamily" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                            <label class="labels-input">Active</label>
                            <asp:TextBox ID="txtAI" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                            <!-- Row 2 -->
                            <label class="labels-input">Code</label>
                            <asp:TextBox ID="txtCode" runat="server" CssClass="asp-input input-medium"/>

                            <label class="labels-input">Level</label>
                            <asp:TextBox ID="txtAccLevel" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                            <label class="labels-input">General Details</label>
                            <asp:TextBox ID="txtGenDetail" runat="server" CssClass="asp-input input-small readonly-field" ReadOnly="true" />

                            <!-- Row 3 -->
                            <label class="labels-input">Opening Balance</label>
                            <asp:TextBox ID="txtOB" runat="server" CssClass="asp-input input-medium" />

                            <label class="labels-input">Description</label>
                            <asp:TextBox ID="txtDesc" runat="server" CssClass="asp-input input-medium" />                             

                            <div></div>
                            <div></div>

                        </div>                        

                        <%-- BUTTONS --%>
                        <div class="button-group">
                            <asp:Button ID="btnSave" runat="server" Text="Save" 
                                CssClass="asp-button btn btn-save" OnClick="btnSave_Click" UseSubmitBehavior="false" CausesValidation="false" />

                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                                CssClass="asp-button btn btn-cancel" OnClick="btnCancel_Click" />

                            <asp:Button ID="btnPrint" runat="server" Text="Print" 
                                CssClass="asp-button btn btn-print" OnClick="btnPrint_Click" />
                        </div>

                        <!-- STATUS --> 
                        <div class="row"> 
                            <div class="col"> 
                                <asp:Label ID="lblStatus" runat="server" /> 
                            </div> 
                        </div>

                    </div>
                </div>       
                   
            </div>

            <%-- CONFIRMATION POPUP SCRIPT --%>
            <script>
                window.onload = function () {
                    var btn = document.getElementById('<%= btnSave.ClientID %>');

                    btn.onclick = function (e) {

                        e.preventDefault();   // STOP normal postback
                        e.stopPropagation();  // EXTRA protection

                        Swal.fire({
                            title: 'Are you sure?',
                            text: 'Do you want to save this record?',
                            icon: 'question',
                            showCancelButton: true,
                            confirmButtonText: 'Yes, Save it!',
                            cancelButtonText: 'Cancel'
                        }).then(function (result) {

                            if (result.isConfirmed) {
                                // manually trigger postback
                                __doPostBack('<%= btnSave.UniqueID %>', '');
                            }
                        });

                        return false; // DOUBLE safety
                    };
                };
            </script>

        </form>
    </body>
</html>
  