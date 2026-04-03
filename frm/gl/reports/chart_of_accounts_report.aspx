<%@ Page Language="C#" AutoEventWireup="true" CodeFile="chart_of_accounts_report.aspx.cs" Inherits="chart_of_accounts_report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Chart of Accounts Report</title>
    <style>
        body { font-family: 'Segoe UI', Arial, sans-serif, Arial, sans-serif; margin: 20px; background: white; }
        
        /* Header container - flexbox for same row */
        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        
        h1 { 
            color: black; 
            margin: 0;
            font-size: 24px;
        }
        
        .button-group {
            display: flex;
            gap: 10px;
        }
        
        .print-btn, .back-btn { 
            background-color: black; 
            color: white; 
            padding: 10px 20px; 
            border: none; 
            cursor: pointer; 
            border-radius: 4px; 
            font-size: 14px;
            font-weight: bold;
            transition: all 0.3s;
        }
        
        .print-btn:hover, .back-btn:hover { 
            background-color: #333;
            transform: translateY(-1px);
        }
        
        table { 
            border-collapse: collapse; 
            width: 100%; 
            margin-top: 20px; 
            font-family: 'Segoe UI', Arial, sans-serif;
        }
        
        th, td { 
            border: 1px solid #ddd; 
            padding: 8px; 
            text-align: left; 
        }
        
        th { 
            background-color: #0f7c57; 
            color: white; 
            font-weight: bold; 
            font-family: Arial, sans-serif;
        }
        
        td:nth-child(4) {  /* Description column */
            white-space: pre;  /* Preserve spaces and tabs */
            font-family: 'Segoe UI', Arial, sans-serif;
        }
        
        tr:nth-child(even) { 
            background-color: #f9f9f9; 
        }
        
        .status { 
            margin-top: 20px; 
            padding: 10px; 
            background: #f0f0f0; 
            border-radius: 4px; 
        }
        
        @media print {
            .no-print { display: none; }
            table { page-break-inside: avoid; }
            .header-container {
                background: none;
                padding: 0;
                margin-bottom: 20px;
            }
            h1 { color: black; }
            td:nth-child(4) {
                white-space: pre;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="no-print">
            <!-- Header with black background, white text -->
            <div class="header-container">
                <h1>Chart of Accounts Report</h1>
                <div class="button-group">
                    <asp:Button ID="btnPrint" runat="server" Text="🖨️ Print Report" CssClass="print-btn" OnClientClick="window.print();return false;" />
                    <asp:Button ID="btnBack" runat="server" Text="< Go Back" CssClass="back-btn" OnClick="btnBack_Click" />
                </div>
            </div>
        </div>
        
        <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" Width="100%">
            <Columns>
                <asp:BoundField DataField="FAMILY_NAME" HeaderText="Family" />
                <asp:BoundField DataField="LEVELL" HeaderText="Level" />
                <asp:BoundField DataField="GL_CODE" HeaderText="Account Code" />
                <asp:BoundField DataField="GL_DESCRP" HeaderText="Description" HtmlEncode="false" />
                <asp:BoundField DataField="OPENING_BALANCE" HeaderText="Opening Balance" DataFormatString="{0:N2}" />
            </Columns>
        </asp:GridView>
        
        <div class="status no-print">
            <asp:Label ID="lblStatus" runat="server" ForeColor="Green"></asp:Label>
        </div>
    </form>
</body>
</html>