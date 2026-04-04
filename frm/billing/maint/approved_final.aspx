<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="approved_final.aspx.cs"
    Inherits="approved_final" %>

<!DOCTYPE html>
<html lang="en">
    <head id="Head1" runat="server">
        <meta charset="UTF-8" />
        <title>Final Approval</title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />

        <style>
            body { margin: 0; font-family: Arial, sans-serif; background: #f7f7f7; color: #333; }

            /* HEADER */
            header{ position:sticky;top:0;background:#fff;border-bottom:1px solid #e5e7eb;padding:12px 20px;display:flex;gap:12px;align-items:center;}
            .back-btn { background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}
            .logoff-btn { background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}

            .label-large { padding-bottom: 2px;padding-left: 10px;font-size: 25px;font-weight: 600;}

            .header-btns { background-color: black;color: white !important;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;font-size: 16px;transition: background-color 0.2s ease-in-out;text-decoration: none !important;display: inline-block;}
            .header-btns:hover { background-color: #333;text-decoration: none !important;}
            .header-border { border-top: 30px solid #000;width: 100%;margin-bottom: 0;}
            /*----------------------------------------------------------------------*/

            /* CONTAINER */
            .container { display: flex; justify-content: center; padding: 40px; }

            /* LEFT PANEL */
            .left-panel { background: #0f7c57; color: white; padding: 30px; border-radius: 8px; flex: 1; max-width: 300px; margin-right: 20px; }
            .left-panel h2 { margin-bottom: 20px; }
            .left-panel ul { list-style: disc; padding-left: 20px; }
            .left-panel ul li { margin-bottom: 10px; }

            /* FORM PANEL */
            .form-panel { background: white; padding: 30px; border-radius: 8px; flex: 2; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
            .form-panel h2 { font-size: 40px; font-weight: 600; margin-bottom: 20px; } 
            
            .row { display: flex; gap: 20px; margin-bottom: 20px; }
            .col { flex: 1; }
            
            label { display: block; padding-left: 10px; padding-bottom: 5px; font-size: 22px; font-weight: 600; }

            /* BUTTONS */
            .btn-row { margin-top: 10px; display: flex; align-items: center; }

            /*.btn-left { display: flex; gap: 15px; }*/
            .btn-right { margin-left: auto; }
            .asp-button { background: #2d2a26; color: white; border: none; padding: 10px 13px; font-size: 18px; border-radius: 4px; cursor: pointer; }
            .asp-button:hover { background: #444; }

            #lblStatus { font-size: 22px; font-weight: bold; padding-top: 20px; }

            /* MESSAGE PANEL */
            .msg-panel { margin-top: 25px; padding: 20px; border: 1px solid #ccc; border-radius: 6px; background: #f9f9f9; }
            .msg-row { display: flex; margin-bottom: 12px; }
            .msg-col { flex: 1; font-size: 20px; font-weight: 600; }
            .msg-value { color: #0f7c57; font-weight: bold; }

            .readonly-box { border: none !important; background: transparent; box-shadow: none; 
                pointer-events: none;   /* cursor & typing completely off */ }

            /* GRID */
            .grid-wrapper {
                width: 100%;
                overflow-x: auto;
                overflow-y: auto;      /* vertical scroll if needed */
                max-height: 355px;     /* adjust to show ~10 rows */
                padding-bottom: 0;
                margin-bottom: 10px;
                border: 1px solid #ccc;
                border-radius: 4px;
            }

            /* Prevent column squeezing */
            .grid-style { min-width: 1000px; }

            .grid-style { border-collapse: collapse; width: 100%; font-family: Segoe UI, Arial; font-size: 13px; }
            .grid-style th { background-color: #2f4050; color: white; text-align: center; padding: 6px; border: 1px solid #d0d0d0; }
            .grid-style td { padding: 6px; border: 1px solid #d0d0d0; }
            .grid-style tr:hover { background: #f2f2f2; }
            .grid-style tr:nth-child(even) { background-color: #f7f7f7; }
            
            .grid-icon { font-size: 18px; margin-right: 10px; text-decoration: none; font-weight: bold; }

            .group-header { background-color: #1f4e79 !important; font-weight: bold; font-size: 14px; }

            .subgroup-header { background-color: #305f8c !important; font-weight: bold; }

            .modified-value { background-color: #fff3cd; font-weight: bold; }

            .before-value { background-color: #92f1b1; font-weight: bold; }


            .approve-icon { color: green; padding: 0 10px 0 10px; }
            .approve-icon:hover { color: darkgreen; }

            .reject-icon { color: red; }
            .reject-icon:hover { color: darkred; }

            #pnlRejectRemarks {
                margin-top: 15px;
            }

            /* ================= RESPONSIVE ================= */
            @media (max-width: 1100px) {

                .container {
                    flex-direction: column;
                    padding: 20px;
                }

                .left-panel {
                    max-width: 100%;
                    margin-right: 0;
                    margin-bottom: 20px;
                }

                .form-panel {
                    width: 100%;
                    padding: 20px;
                }

                header {
                    flex-wrap: wrap;
                    gap: 8px;
                }

                #brand_name,
                #header_actions_goback,
                #header_user,
                #header_actions_logoff {
                    width: 100% !important;
                    text-align: center !important;
                    float: none !important;
                }
            }

            @media (max-width: 768px) {

                .grid-style {
                    font-size: 11px;
                }

                .grid-style th,
                .grid-style td {
                    padding: 6px 4px;
                }

                .grid-icon {
                    font-size: 16px;
                }

                .form-panel h2 {
                    font-size: 28px;
                }
            }
        </style>
    </head>

    <body>
        <form id="form1" runat="server">
            <%-- HEADER --%>            
            <div id="border_header" class="header-border"></div>
            <header>
                <div id="brand_name" CssClass="header-border" style="width:10%; ">Final Approval</div>
    
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
                    <h2>Approval Process</h2>
                    <ol>
                        <li>
                            To Approve
                            <ul>
                                <li>&#10004;</li>
                            </ul>
                        </li>
                        <li>
                            To Reject
                            <ul>
                                <li>&#10006;</li>
                            </ul>
                        </li>
                    </ol>
                </div>

                <!-- FORM PANEL -->
                <div class="form-panel">
                    <h2>Final Approval</h2>

                    <%-- GRIDS --%>
                    <%--<div style="max-height:500px; overflow-y:auto; padding-bottom: 20px; margin-bottom: 10px;">--%>
                    <div class="grid-wrapper">
                        <asp:GridView ID="gvResults" runat="server"
                            CssClass="grid-style"
                            AutoGenerateColumns="false"
                            DataKeyNames="REFCODE"
                            OnRowCommand="gvResults_RowCommand"
                            OnRowCreated="gvResults_RowCreated"
                            OnRowDataBound="gvResults_RowDataBound">

                            <Columns>
                                <asp:BoundField DataField="REFCODE" HeaderText="Ref.Code" />
                                <asp:BoundField DataField="RESNAME" HeaderText="Client Name" />
                                <asp:BoundField DataField="BILCAT" HeaderText="Category" />
                                <asp:BoundField DataField="BILAMNTBDDT" HeaderText="Before Due DT" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="BILAMNTADDT" HeaderText="After Due DT" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="BILAMNTBDDT_TOBE" HeaderText="Before Due DT" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="BILAMNTADDT_TOBE" HeaderText="After Due DT" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="REMARKS_APPROVEDBY" HeaderText="Billing Remarks" />

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnRowApprove"
                                            runat="server"
                                            CommandName="ApproveRow"
                                            CommandArgument='<%# Eval("REFCODE") %>'
                                            CssClass="grid-icon approve-icon"
                                            ToolTip="Approve"
                                            visible="false">
                                            &#10004;
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="btnRowReject"
                                            runat="server"
                                            CommandName="RejectRow"
                                            CommandArgument='<%# Eval("REFCODE") %>'
                                            CssClass="grid-icon reject-icon"
                                            ToolTip="Reject"
                                            visible="false">
                                            &#10006;
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                            </Columns>
                        </asp:GridView>                        
                    </div>

                    <!-- REJECT REMARKS PANEL -->
                    <asp:Panel ID="pnlRejectRemarks" runat="server" Visible="false" CssClass="msg-panel">
                        <asp:Label ID="lblRejectPrompt" runat="server" style="font-weight:bold;" Text="Enter Remarks for Rejection:" />
                        <br />
                        <asp:TextBox ID="txtRejectRemarks" runat="server" style="" TextMode="MultiLine" Rows="2" Columns="80" />
                        <br /><br />
                        <asp:Button ID="btnSubmitReject" runat="server" Text="Submit Rejection" CssClass="asp-button" OnClick="btnSubmitReject_Click" />
                        <asp:HiddenField ID="hfRejectRefCode" runat="server" />
                    </asp:Panel>
                    
                    <!-- STATUS -->
                    <div class="row">
                        <div class="col">
                            <asp:Label ID="lblStatus" runat="server" />
                        </div>
                    </div>

                    <!-- MESSAGE SUMMARY -->
                    <asp:Panel ID="pnlMessages" runat="server"
                               CssClass="msg-panel"
                               Visible="false">
                    </asp:Panel>
                    
                </div>
            </div>
        </form>
    </body>
</html>

  