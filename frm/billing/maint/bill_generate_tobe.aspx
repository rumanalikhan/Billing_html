<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bill_generate_tobe.aspx.cs" Inherits="bill_generate_tobe" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Create Month Header</title>

    <style>
        body { margin:0; font-family:Calibri; background:#f7f7f7; }

        /* HEADER */ 
        .app-header { position: sticky; top: 0; background: #000; color: #fff; padding: 12px 20px; display: flex; align-items: center; } 
        
        /* LEFT / CENTER / RIGHT */ 
        .header-left, .header-right { flex: 1; display: flex; align-items: center; } 
        .header-left { justify-content: flex-start; } 
        .header-right { justify-content: flex-end; } 
        .header-center { flex: 1; text-align: center; font-size: 16px; font-weight: 600; letter-spacing: 0.5px; } 
        
        /* BUTTONS */ 
        .header-btns { background-color: #222; color: #fff !important; border: none; padding: 7px 12px; border-radius: 4px; cursor: pointer; font-size: 14px; text-decoration: none !important; } 
        .header-btns:hover { background-color: #444; } 
        
        /* USER LABEL */ 
        .header-user { font-size: 14px; font-weight: 600; margin-right: 10px; }

        /* CONTAINER */
        .container { display:flex; justify-content:center; padding:20px; }

        /* LEFT PANEL */ 
        .left-panel { background: #0f7c57; color: white; padding: 30px; border-radius: 8px; flex: 1; max-width: 300px; margin-right: 20px; } 
        .left-panel h2 { margin-bottom: 20px; } 
        .left-panel ul { list-style: disc; padding-left: 20px; } 
        .left-panel ul li { margin-bottom: 10px; }
        
        /* FORM PANEL */ 
        .form-panel { background: white; padding: 30px; border-radius: 8px; flex: 2; box-shadow: 0 2px 5px rgba(0,0,0,0.1); } 
        .form-panel h2 { font-size: 40px; margin-bottom: 30px; } 
        .row { display: flex; gap: 20px; margin-bottom: 20px; } 
        .col { flex: 1; } 
        label { display: block; padding-left: 8px; padding-bottom: 5px; font-size: 18px; font-weight: 600; } 
        .asp-input { width: 100%; height: 55px; padding: 10px; font-size: 22px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; } 
        .btn-row { margin-top: 30px; } 
        .asp-button { background: #2d2a26; color: white; border: none; padding: 10px 25px; font-size: 22px; border-radius: 4px; cursor: pointer; } 
        .asp-button:hover { background: #444; } 
        .status { font-size: 18px; font-weight: bold; }
    </style>

    <script type="text/javascript">
        function onIssueDateChange() {
            const issueVal = document.getElementById('<%= txtIssueDate.ClientID %>').value;
            if (!issueVal) return;

            // Date Reading = Issue Date
            const issue = new Date(issueVal);
            //document.getElementById('<%//= txtDateReading.ClientID %>').value = issueVal;

            // BG_NAME = previous month
            const prev = new Date(issue.getFullYear(), issue.getMonth() - 1, 1);
            const monthNames = ['JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC'];
            document.getElementById('<%= txtBGName.ClientID %>').value = monthNames[prev.getMonth()] + '-' + prev.getFullYear();

            // BILL_MONTH = YYYYMM
            const billMonth = document.getElementById('<%= txtBillMonth.ClientID %>').value = 
                prev.getFullYear().toString() + (prev.getMonth() + 1).toString().padStart(2, '0');

            // VALID_DATE = last day of issue month (LOCAL, NO UTC SHIFT)
            const lastDay = new Date(issue.getFullYear(), issue.getMonth() + 1, 0);

            const yyyy = lastDay.getFullYear();
            const mm = ("0" + (lastDay.getMonth() + 1)).slice(-2);
            const dd = ("0" + lastDay.getDate()).slice(-2);

            document.getElementById('<%= txtValidDate.ClientID %>').value = yyyy + "-" + mm + "-" + dd;
        }

        // Confirmation Popup
        function confirmPost() {
            return confirm(
                "⚠ WARNING!\n\n" +
                "This action will TRUNCATE existing bill data and generate a new bill.\n\n" +
                "Do you want to continue?"
            );
        }

        // Validate Due Date
        function validateDates() {
            const issue = document.getElementById('<%= txtIssueDate.ClientID %>').value;
            const due = document.getElementById('<%= txtDueDate.ClientID %>').value;

                    if (!issue || !due) return true;

            const issueDate = new Date(issue);
            const dueDate = new Date(due);

                    if (dueDate <= issueDate) {
                        alert("❌ Due Date must be greater than Issue Date.");
                        document.getElementById('<%= txtDueDate.ClientID %>').value = "";
                document.getElementById('<%= txtDueDate.ClientID %>').focus();
                return false;
            }
            return true;
        }

        function validateBeforePost() {
            const bgid = document.getElementById('<%= txtBGID.ClientID %>').value;

            if (!bgid) {
                alert("❌ BG ID is missing. Please reload the page.");
                return false;
            }

            return validateDates(); // keep existing date validation
        }
    </script>
</head>
    <body>
        <form id="Form1" runat="server">
            <%-- HEADER --%>
            <header class="app-header"> 
                <div class="header-left"> 
                    <asp:LinkButton ID="btnBack" runat="server" CssClass="header-btns" OnClick="btnBack_Click"> 
                        Go back 
                    </asp:LinkButton> 
                </div> 
                <div id="header_user" class="header-center" style="width:56%; float:left; text-align:center; color:white; font-weight:bold;"> 
                    <div>Create Month Header</div> 
                </div> 
                <div class="header-right">
                    <asp:Label ID="lblUser" runat="server" CssClass="header-user"></asp:Label> &nbsp;&nbsp; 
                    <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click"> Log off </asp:LinkButton>
                </div>
            </header>

            <%-- CONTAINER --%>
            <div class="container">
                <div class="left-panel">
                    <h3>Instructions</h3>
                    <ul>
                        <li>BG_ID auto generated</li>
                        <li>Select Issue & Due Date</li>
                        <li>Review calculated fields</li>
                        <li>Post will truncate & insert</li>
                    </ul>
                </div>

                <%-- FORM --%>
                <div class="form-panel">
                    <h2>Bill Generate (To Be)</h2>

                    <%-- ROW 1 --%>
                    <div class="row">
                        <div class="col">
                            <label>BG ID</label>
                            <asp:TextBox ID="txtBGID" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>

                        <%--<div class="col">
                            <label>Fetch ID</label>
                            <asp:Button ID="btnFetchID" runat="server" Text="Fetch ID" CssClass="asp-button" OnClick="btnFetchID_Click" />
                        </div>--%>
                    </div>

                    <%-- ROW 2 --%>
                    <div class="row">
                        <div class="col">
                            <label>Issue Date</label>
                            <asp:TextBox ID="txtIssueDate" runat="server" CssClass="asp-input" TextMode="Date" onchange="onIssueDateChange()" />
                        </div>
                        <div class="col">
                            <label>Due Date</label>
                            <asp:TextBox ID="txtDueDate" runat="server" CssClass="asp-input" TextMode="Date" OnChange="validateDates()" />
                        </div>
                        <div class="col">
                            <label>Valid Date</label>
                            <asp:TextBox ID="txtValidDate" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                    </div>

                    <%-- ROW 3 --%>
                    <div class="row">
                        <div class="col">
                            <label>BG_NAME</label>
                            <asp:TextBox ID="txtBGName" runat="server" CssClass="asp-input" />
                        </div>
                        <div class="col">
                            <label>Generate Date</label>
                            <asp:TextBox ID="txtGenerateDate" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                        <div class="col">
                            <label>Bill Month</label>
                            <asp:TextBox ID="txtBillMonth" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                    </div>

                    <%-- ROW 4 --%>
                    <%--<div class="row">
                        <div class="col">
                            <label>Date Reading</label>
                            <asp:TextBox ID="txtDateReading" runat="server" CssClass="asp-input" ReadOnly="true" Visible="false" />
                        </div>
                        <div class="col">
                            <label>Company ID</label>
                            <asp:TextBox ID="txtCompID" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                        <div class="col">
                            <label>Is Locked</label>
                            <asp:TextBox ID="txtIsLocked" runat="server" CssClass="asp-input" ReadOnly="true" />
                        </div>
                    </div>--%>

                    <asp:Label ID="lblStatus" runat="server" Font-Bold="true" />
                    <br /><br />
                    <asp:Button ID="btnPost" runat="server" Text="Post" CssClass="asp-button" OnClick="btnPost_Click" OnClientClick="return confirmPost() && validateBeforePost();" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="asp-button" OnClick="btnCancel_Click" />
                </div>
            </div>
        </form>
    </body>
</html>