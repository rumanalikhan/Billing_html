<%@ Page Language="C#" AutoEventWireup="true" CodeFile="chart_of_accounts_report.aspx.cs" Inherits="chart_of_accounts_report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Chart of Accounts Report</title>
    <style>
        body { 
            font-family: 'Segoe UI', Arial, sans-serif; 
            margin: 0; 
            padding: 0; 
            background: white;
        }
        
        /* Full width - no container */
        .print-container {
            width: 100%;
            background: white;
        }
        
        /* Header container */
        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: white;
            padding: 10px 20px;
            border-bottom: 1px solid #ddd;
        }
        
        /* Company Header for Report */
        .company-header {
            text-align: center;
            padding: 20px;
            background: white;
        }
        
        .company-name {
            font-size: 22px;
            font-weight: bold;
            color: #0f7c57;
            margin: 0;
        }
        
        .company-sub {
            font-size: 12px;
            color: #666;
            margin: 5px 0;
        }
        
        .report-title {
            text-align: center;
            font-size: 18px;
            font-weight: bold;
            text-decoration: underline;
            margin: 15px 0;
            text-transform: uppercase;
        }
        
        .report-date {
            text-align: right;
            font-size: 11px;
            color: #666;
            margin-bottom: 15px;
            padding-right: 20px;
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
            padding: 8px 20px; 
            border: none; 
            cursor: pointer; 
            border-radius: 4px; 
            font-size: 14px;
            font-weight: bold;
            transition: all 0.3s;
        }
        
        .print-btn:hover, .back-btn:hover { 
            background-color: #333;
        }
        
        /* Full width table */
        table { 
            border-collapse: collapse; 
            width: 100%; 
            font-family: 'Segoe UI', Arial, sans-serif;
        }
        
        th, td { 
            border: 1px solid #ddd; 
            padding: 8px 10px; 
            text-align: left; 
        }
        
        th { 
            background-color: #0f7c57; 
            color: white; 
            font-weight: bold; 
        }
        
        td:nth-child(4) {  /* Description column */
            white-space: pre;
        }
        
        tr:nth-child(even) { 
            background-color: #f9f9f9; 
        }
        
        .status { 
            margin-top: 20px; 
            padding: 10px 20px; 
            background: #f0f0f0; 
        }
        
        .footer {
            margin-top: 30px;
            text-align: center;
            font-size: 10px;
            color: #999;
            border-top: 1px solid #ddd;
            padding: 10px 0;
        }
        
        /* PRINT STYLES */
        @media print {
            body {
                background: white;
                padding: 0;
                margin: 0;
            }
            .no-print {
                display: none;
            }
            .company-header {
                border-bottom: 2px solid #0f7c57;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            th {
                background-color: #0f7c57;
                color: white;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
            table {
                page-break-inside: auto;
            }
            tr {
                page-break-inside: avoid;
            }
            thead {
                display: table-header-group;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="print-container">
            <!-- Buttons - No Print -->
            <div class="no-print header-container">
                <h1 style="margin:0;">Chart of Accounts Report</h1>
                <div class="button-group">
                    <asp:Button ID="btnPrint" runat="server" Text="🖨️ Print" CssClass="print-btn" OnClientClick="window.print();return false;" />
                    <asp:Button ID="btnBack" runat="server" Text="← Back" CssClass="back-btn" OnClick="btnBack_Click" />
                </div>
            </div>
            
            <!-- Company Header - Prints -->
            <div class="company-header">
                <div class="company-name">BAHRIA TOWN KARACHI</div>
                <div class="company-sub">GL ACCOUNTING SYSTEM</div>
            </div>
            
            <!-- Report Title -->
            <div class="report-title">
                CHART OF ACCOUNTS REPORT
            </div>
            
            <!-- Report Date -->
            <div class="report-date">
                Printed on: <asp:Label ID="lblPrintDate" runat="server" />
            </div>
            
            <!-- Grid View - Full Width -->
            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" Width="100%">
                <Columns>
                    <asp:BoundField DataField="FAMILY_NAME" HeaderText="Family" />
                    <asp:BoundField DataField="LEVELL" HeaderText="Level" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="GL_CODE" HeaderText="Account Code" />
                    <asp:BoundField DataField="GL_DESCRP" HeaderText="Description" HtmlEncode="false" />
                    <asp:BoundField DataField="OPENING_BALANCE" HeaderText="Opening Balance" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
            
            <!-- Footer -->
            <div class="footer">
                This is a computer generated document - No signature required
            </div>
            
            <!-- Status Message -->
            <div class="status no-print">
                <asp:Label ID="lblStatus" runat="server" ForeColor="Green"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>