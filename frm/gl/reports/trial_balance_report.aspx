<%@ Page Language="C#" AutoEventWireup="true" CodeFile="trial_balance_report.aspx.cs" Inherits="trial_balance" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Trial Balance Report</title>
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
            margin-bottom: 20px;
            border: 1px solid #dee2e6;
            border-radius: 8px;
        }

        .report-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 11px;
            table-layout: fixed;
        }

            .report-table th {
                background: #0f7c57;
                color: white;
                border: 1px solid #0a5e42;
                padding: 8px 4px;
                text-align: center;
                font-weight: bold;
                font-size: 11px;
            }

            .report-table td {
                border: 1px solid #e0e0e0;
                padding: 6px 4px;
                vertical-align: top;
            }

            .report-table tr:nth-child(even) {
                background-color: #f9f9f9;
            }

            .report-table tr:hover {
                background: #f2f2f2;
            }

        .indent-2 {
            padding-left: 20px;
        }

        .indent-3 {
            padding-left: 40px;
        }

        .indent-4 {
            padding-left: 60px;
        }

        .report-table th:nth-child(1) {
            width: 8%;
        }

        .report-table th:nth-child(2) {
            width: 28%;
        }

        .report-table th:nth-child(3) {
            width: 8%;
        }

        .report-table th:nth-child(4) {
            width: 8%;
        }

        .report-table th:nth-child(5) {
            width: 8%;
        }

        .report-table th:nth-child(6) {
            width: 8%;
        }

        .report-table th:nth-child(7) {
            width: 8%;
        }

        .report-table th:nth-child(8) {
            width: 8%;
        }

        .level-1 {
            font-weight: bold;
            background-color: #e8f0fe !important;
        }

        .level-2 td:nth-child(2) {
            padding-left: 20px !important;
        }

        .level-3 td:nth-child(2) {
            padding-left: 40px !important;
        }

        .level-4 td:nth-child(2) {
            padding-left: 60px !important;
        }

        .text-right {
            text-align: right !important;
        }

        .credit-balance {
            color: red !important;
            font-weight: bold !important;
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
            margin-top: 30px;
            text-align: center;
            font-size: 10px;
            color: #999;
            border-top: 1px solid #ddd;
            padding-top: 10px;
            clear: both;
            position: relative;
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
                margin-bottom: 30px;
                page-break-after: avoid;
            }

            .report-table {
                width: 100%;
                font-size: 9px;
                page-break-inside: auto;
            }

                .report-table tr {
                    page-break-inside: avoid;
                    page-break-after: auto;
                }

                .report-table th {
                    background: #0f7c57 !important;
                    -webkit-print-color-adjust: exact;
                    print-color-adjust: exact;
                }

            .footer {
                position: relative !important;
                bottom: auto !important;
                margin-top: 40px !important;
                page-break-before: avoid;
                page-break-after: avoid;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="no-print">
            <div class="header-container">
                <h1>Trial Balance Report</h1>
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

            <div class="report-header">
                <div>Trial Balance Report</div>
                <div>
                    <asp:Label ID="lblReportDateTime" runat="server" /></div>
            </div>

            <div class="filter-section no-print">
                <div class="filter-group">
                    <label>Opening As On:</label>
                    <asp:TextBox ID="txtOpeningDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>From Date:</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>To Date:</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" />
                </div>
                <div class="filter-group">
                    <label>Posting Status:</label>
                    <asp:DropDownList ID="ddlPostingStatus" runat="server">
                        <asp:ListItem Text="Posted Only" Value="Posted" Selected="True" />
                        <asp:ListItem Text="Unposted Only" Value="Unposted" />
                        <asp:ListItem Text="All Vouchers" Value="All" />
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <asp:CheckBox ID="chkShowZeroOpening" runat="server" Text="Show accounts with zero opening balance" />
                </div>
                <div class="button-container">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn-search" />
                </div>
            </div>

            <div class="report-period">
                OPENING AS ON:
                <asp:Label ID="lblOpeningDate" runat="server" />
                &nbsp;|&nbsp;
                FOR THE PERIOD:
                <asp:Label ID="lblFromDate" runat="server" />
                TO
                <asp:Label ID="lblToDate" runat="server" />
            </div>

            <div class="table-wrapper">
                <table class="report-table" style="width: 100%;">
                    <thead>
                        <tr style="background-color: #0f7c57; color: white;">
                            <th rowspan="2" style="width: 8%;">CODE</th>
                            <th rowspan="2" style="width: 28%;">TITLE</th>
                            <th colspan="2" style="width: 16%;">OPENING BALANCE</th>
                            <th colspan="2" style="width: 16%;">PERIOD</th>
                            <th colspan="2" style="width: 16%;">CLOSING BALANCE</th>
                        </tr>
                        <tr style="background-color: #0f7c57; color: white;">
                            <th style="width: 8%;">DEBIT</th>
                            <th style="width: 8%;">CREDIT</th>
                            <th style="width: 8%;">DEBIT</th>
                            <th style="width: 8%;">CREDIT</th>
                            <th style="width: 8%;">DEBIT</th>
                            <th style="width: 8%;">CREDIT</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptReport" runat="server" OnItemDataBound="rptReport_ItemDataBound">
                            <ItemTemplate>
                                <tr class='<%# GetRowClass(Container.DataItem) %>'>
                                    <td><%# Eval("GL_CODE") %></td>
                                    <td><%# Eval("GL_DESCRP") %></td>
                                    <td class="text-right"><%# Convert.ToDecimal(Eval("OPENING_DEBIT")).ToString("N2") %></td>
                                    <td class="text-right">
                                        <asp:Label ID="lblOpeningCredit" runat="server" Text='<%# Convert.ToDecimal(Eval("OPENING_CREDIT")).ToString("N2") %>' />
                                    </td>
                                    <td class="text-right"><%# Convert.ToDecimal(Eval("PERIOD_DEBIT")).ToString("N2") %></td>
                                    <td class="text-right"><%# Convert.ToDecimal(Eval("PERIOD_CREDIT")).ToString("N2") %></td>
                                    <td class="text-right"><%# Convert.ToDecimal(Eval("CLOSING_DEBIT")).ToString("N2") %></td>
                                    <td class="text-right">
                                        <asp:Label ID="lblClosingCredit" runat="server" Text='<%# Convert.ToDecimal(Eval("CLOSING_CREDIT")).ToString("N2") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
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
