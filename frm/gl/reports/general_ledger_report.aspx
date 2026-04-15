<%@ Page Language="C#" AutoEventWireup="true" CodeFile="general_ledger_report.aspx.cs" Inherits="general_ledger_report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>General Ledger Report</title>
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

        /* Header container - flexbox for same row like Chart of Accounts */
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

        .report-period {
            text-align: center;
            font-size: 12px;
            font-weight: bold;
            margin-bottom: 20px;
            padding: 10px;
            background: #dcdcdc;
            border-radius: 4px;
        }

        .filter-section {
            background: #f8f9fa;
            padding: 15px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
            border: 1px solid #dee2e6;
            border-radius: 8px;
        }

        .filter-group {
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .filter-group label {
            font-weight: 600;
            font-size: 12px;
        }

        .filter-group input {
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

        .btn-search {
            padding: 6px 20px;
            cursor: pointer;
            border: none;
            border-radius: 4px;
            font-weight: bold;
            font-size: 12px;
            transition: all 0.2s ease-in-out;
            background: #0f7c57;
            color: white;
        }

        .btn-search:hover {
            background: #0a5e42;
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
            table-layout: auto;
            min-width: 1100px;
        }

        .report-table th {
            background: #0f7c57;
            color: white;
            border: 1px solid #0a5e42;
            padding: 10px 6px;
            text-align: left;
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

        /* Fixed column widths */
        .report-table th:nth-child(1) { width: 8%; }
        .report-table th:nth-child(2) { width: 12%; }
        .report-table th:nth-child(3) { width: 8%; }
        .report-table th:nth-child(4) { width: 5%; }
        .report-table th:nth-child(5) { width: 5%; }
        .report-table th:nth-child(6) { width: 16%; }
        .report-table th:nth-child(7) { width: 10%; }
        .report-table th:nth-child(8) { width: 5%; }
        .report-table th:nth-child(9) { width: 5%; }
        .report-table th:nth-child(10) { width: 8%; }
        .report-table th:nth-child(11) { width: 8%; }
        .report-table th:nth-child(12) { width: 8%; }

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
                table-layout: auto;
                font-size: 9px;
            }

            .report-table th,
            .report-table td {
                border: 1px solid #000 !important;
                padding: 4px;
            }

            .report-table th {
                background: #0f7c57 !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            .total-row td {
                background: #dcdcdc !important;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }

            .footer {
                position: fixed;
                bottom: 0;
                width: 100%;
            }
        }
    </style>
    <script type="text/javascript">
        function displayPageNumbers() {
            var pageNumber = document.getElementById('pageNumber');
            var totalPages = document.getElementById('totalPages');
            if (pageNumber && totalPages) {
                pageNumber.innerText = '1';
                totalPages.innerText = '1';
            }
        }
        window.onload = displayPageNumbers;
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header like Chart of Accounts -->
       <div class="no-print">
    <div class="header-container">
        <h1>General Ledger Report</h1>
        <div class="button-group">
            <asp:Button ID="btnPrint" runat="server" Text="🖨️ Print Report" CssClass="print-btn" OnClientClick="window.print();return false;" />
            <asp:Button ID="btnExcel" runat="server" Text="📊 Export Excel" CssClass="print-btn" OnClick="btnExcel_Click" />
            <asp:Button ID="btnBack" runat="server" Text="< Go Back" CssClass="back-btn" OnClick="btnBack_Click" />
        </div>
    </div>
</div>

        <div class="report-container">
            <div class="company-header">
                <div class="company-name">BAHRIA TOWN KARACHI</div>
                <div class="company-sub">GL ACCOUNTING SYSTEM</div>
            </div>

            <!-- Report Header with Date/Time -->
            <div class="report-header">
                <div>General Ledger Report</div>
                <div><asp:Label ID="lblReportDateTime" runat="server" /></div>
            </div>

            <div class="filter-section no-print">
                <div class="filter-group">
                    <label>From Date:</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>To Date:</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>From Account:</label>
                    <asp:TextBox ID="txtFromAccount" runat="server" Width="100px" placeholder="Optional" />
                </div>
                <div class="filter-group">
                    <label>To Account:</label>
                    <asp:TextBox ID="txtToAccount" runat="server" Width="100px" placeholder="Optional" />
                </div>
                <div class="button-container">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn-search" />
                </div>
            </div>

            <div class="report-period">
                FROM: <asp:Label ID="lblFromDate" runat="server" />
                TO: <asp:Label ID="lblToDate" runat="server" />
                <asp:Label ID="lblAccountRange" runat="server" />
            </div>

            <div class="table-wrapper">
                <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="report-table"
                    OnRowDataBound="gvReport_RowDataBound" ShowHeader="true" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="GL_CODE" HeaderText="GL CODE" />
                        <asp:BoundField DataField="GL_DESCRP" HeaderText="GL DESCRIPTION" />
                        <asp:BoundField DataField="BOOK_TYPE" HeaderText="BOOK TYPE" />
                        <asp:BoundField DataField="GL_FORM_NUMBER" HeaderText="GL FORM" />
                        <asp:BoundField DataField="VOUCHER_DATE" HeaderText="VOUCHER DATE" />
                        <asp:BoundField DataField="NARATION" HeaderText="NARATION" />
                        <asp:BoundField DataField="CHEQUE_NUMBER" HeaderText="CHEQUE NO" />
                        <asp:BoundField DataField="BILL_NUMBER" HeaderText="BILL NO" />
                        <asp:BoundField DataField="OPENING_BALANCE" HeaderText="OPENING" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="DEBIT" HeaderText="DEBIT" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="CREDIT" HeaderText="CREDIT" ItemStyle-HorizontalAlign="Right" />
                        <asp:TemplateField HeaderText="RUNNING BALANCE" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:Label ID="lblRunningBalance" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Footer with Page Number -->
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