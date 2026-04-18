<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sub_ledger_report.aspx.cs" Inherits="sub_ledger_report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Sub Ledger Report</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            background: #f4f6f8;
            padding: 20px;
            margin: 0;
            min-height: 100vh;
        }

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

        .report-container {
            width: 100%;
            background: white;
            padding: 20px;
            border-radius: 8px;
        }

        .company-header {
            text-align: center;
            margin-bottom: 12px;
        }

        .company-name {
            font-size: 20px;
            font-weight: bold;
            color: #0f7c57;
        }

        .company-sub {
            font-size: 12px;
            color: #555;
            margin-top: 5px;
        }

        .report-header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
            font-size: 11px;
            color: #555;
            border-bottom: 1px solid #ddd;
            padding-bottom: 8px;
        }

        .filter-section {
            background: #f8f9fa;
            padding: 15px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: flex-end;
            flex-wrap: wrap;
            border: 1px solid #dee2e6;
            border-radius: 8px;
        }

        .filter-group {
            display: flex;
            flex-direction: column;
        }

        .filter-group label {
            font-weight: 600;
            font-size: 12px;
            margin-bottom: 5px;
        }

        .filter-group input, .filter-group select {
            padding: 6px 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            font-size: 12px;
        }

        .button-container {
            margin-left: auto;
            display: flex;
            gap: 10px;
        }

        .btn-search, .btn-excel {
            padding: 6px 20px;
            cursor: pointer;
            border: none;
            border-radius: 4px;
            font-weight: bold;
            font-size: 12px;
            transition: all 0.2s ease-in-out;
        }

        .btn-search {
            background: #0f7c57;
            color: white;
        }

        .btn-search:hover {
            background: #0a5e42;
        }

        .btn-excel {
            background: #1f6f4a;
            color: white;
        }

        .btn-excel:hover {
            background: #0a5e42;
        }

        .btn-clear {
            padding: 6px 20px;
            cursor: pointer;
            border: none;
            border-radius: 4px;
            font-weight: bold;
            font-size: 12px;
            background: #6c757d;
            color: white;
        }

        .btn-clear:hover {
            background: #5a6268;
        }

        .sl-info {
            background: #e8f0fe;
            padding: 10px 15px;
            margin-bottom: 15px;
            border-left: 3px solid #0f7c57;
            font-size: 12px;
            border-radius: 4px;
        }

        .period-info {
            text-align: center;
            font-size: 12px;
            font-weight: bold;
            margin-bottom: 15px;
            padding: 8px;
            background: #dcdcdc;
            border-radius: 4px;
        }

        .table-wrapper {
            overflow-x: auto;
            margin-top: 10px;
            border: 1px solid #dee2e6;
            border-radius: 8px;
        }

        .report-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 11px;
            table-layout: fixed;
            min-width: 900px;
        }

        .report-table th {
            background: #0f7c57;
            color: white;
            border: 1px solid #0a5e42;
            padding: 10px 6px;
            text-align: center;
            font-weight: bold;
            font-size: 11px;
        }

        .report-table td {
            border: 1px solid #e0e0e0;
            padding: 8px 6px;
            vertical-align: top;
        }

        .report-table tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .report-table tr:hover {
            background: #f2f2f2;
        }

        .report-table th:nth-child(1) { width: 12%; }
        .report-table th:nth-child(2) { width: 10%; }
        .report-table th:nth-child(3) { width: 25%; }
        .report-table th:nth-child(4) { width: 10%; }
        .report-table th:nth-child(5) { width: 10%; }
        .report-table th:nth-child(6) { width: 10%; }
        .report-table th:nth-child(7) { width: 13%; }

        .total-row {
            background: #dcdcdc !important;
            font-weight: bold;
        }

        .total-row td {
            border-top: 2px solid #0f7c57;
            border-bottom: 2px solid #0f7c57;
            background: #dcdcdc;
        }

        .text-right {
            text-align: right !important;
        }

        .status {
            margin-top: 15px;
            padding: 10px;
            background: #f8f9fa;
            border-radius: 4px;
            font-size: 12px;
            text-align: center;
            border-left: 3px solid #0f7c57;
        }

        .footer {
            margin-top: 20px;
            text-align: center;
            font-size: 10px;
            color: #999;
            border-top: 1px solid #ddd;
            padding-top: 10px;
        }

        @media print {
            body {
                background: white;
                padding: 0;
                margin: 0;
            }

            .no-print {
                display: none !important;
            }

            .header-container {
                display: none !important;
            }

            .report-container {
                padding: 5px;
                width: 100%;
            }

            .table-wrapper {
                overflow: visible !important;
                border: none;
                width: 100%;
            }

            .report-table {
                width: 100%;
                font-size: 9px;
            }

            .report-table th {
                background: #0f7c57 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            @page {
                size: A4 landscape;
                margin: 0.5cm;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="no-print">
            <div class="header-container">
                <h1>Sub Ledger Report</h1>
                <div class="button-group">
                    <asp:Button ID="btnPrint" runat="server" Text="🖨️ Print Report" CssClass="print-btn" OnClientClick="window.print();return false;" />
                    <asp:Button ID="btnExportExcel" runat="server" Text="📊 Export Excel" CssClass="print-btn" OnClick="btnExportExcel_Click" />
                    <asp:Button ID="btnBack" runat="server" Text="< Go Back" CssClass="back-btn" OnClick="btnBack_Click" />
                </div>
            </div>
        </div>

        <div class="report-container">
            <div class="company-header">
                <div class="company-name">BAHRIA TOWN KARACHI</div>
                <div class="company-sub">GL ACCOUNTING SYSTEM</div>
            </div>

            <div class="report-header">
                <div>Sub Ledger Report</div>
                <div><asp:Label ID="lblReportDateTime" runat="server" /></div>
            </div>

            <div class="filter-section no-print">
                <div class="filter-group">
                    <label>From Date:</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" Width="130px" />
                </div>
                <div class="filter-group">
                    <label>To Date:</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" Width="130px" />
                </div>
                <div class="filter-group">
                    <label>Sub Ledger Code:</label>
                    <asp:TextBox ID="txtSLCode" runat="server" placeholder="Enter SL Code" Width="150px" />
                </div>
                <div class="filter-group">
                    <label>Posting Status:</label>
                    <asp:DropDownList ID="ddlPostingStatus" runat="server">
                        <asp:ListItem Text="Posted Only" Value="Posted" Selected="True" />
                        <asp:ListItem Text="Unposted Only" Value="Unposted" />
                        <asp:ListItem Text="All Vouchers" Value="All" />
                    </asp:DropDownList>
                </div>
                <div class="button-container">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn-search" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" CssClass="btn-clear" />
                </div>
            </div>

            <div class="period-info" id="periodInfo" runat="server" visible="false">
                FROM: <asp:Label ID="lblFromDate" runat="server" /> TO: <asp:Label ID="lblToDate" runat="server" />
            </div>

            <div class="sl-info" id="slInfo" runat="server" visible="false">
                <strong>Sub Ledger:</strong> <asp:Label ID="lblSLCode" runat="server" /> - <asp:Label ID="lblSLName" runat="server" /><br />
                <strong>Printed On:</strong> <asp:Label ID="lblPrintDate" runat="server" />
            </div>

            <div class="table-wrapper">
                <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="report-table"
                    OnRowDataBound="gvReport_RowDataBound" ShowHeader="true" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="VOUCHER_KEY" HeaderText="VOUCHER KEY" />
                        <asp:BoundField DataField="TRANS_DATE" HeaderText="DATE" />
                        <asp:BoundField DataField="PARTICULARS" HeaderText="PARTICULARS" />
                        <asp:BoundField DataField="CHEQUE_NUMBER" HeaderText="CHQ/SLIP" />
                        <asp:TemplateField HeaderText="DEBIT" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <%# Convert.ToDecimal(Eval("DEBIT")).ToString("N2") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CREDIT" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <%# Convert.ToDecimal(Eval("CREDIT")).ToString("N2") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BALANCE" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:Label ID="lblBalance" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="padding: 40px; text-align: center;">No transactions found for this Sub Ledger</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <div class="footer">
                This is a computer generated document - No signature required<br />
                Page 1 of 1
            </div>

            <div class="status no-print">
                <asp:Label ID="lblStatus" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>