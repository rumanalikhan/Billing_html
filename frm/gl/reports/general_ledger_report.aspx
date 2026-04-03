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
            padding: 0;
            margin: 0;
            min-height: 100vh;
        }

        .report-container {
            width: 100%;
            min-height: 100vh;
            background: white;
            padding: 20px;
        }

        .company-header {
            text-align: center;
            margin-bottom: 25px;
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

        .report-title {
            font-size: 16px;
            font-weight: bold;
            text-align: center;
            margin: 15px 0 10px;
            text-decoration: underline;
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

        .btn-search, .btn-print {
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

        .btn-print {
            background: white;
            color: #2c3e50;
            border: 1px solid #2c3e50;
        }

            .btn-print:hover {
                background: #2c3e50;
                color: white;
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

        .total-row td {
            border-top: 2px solid #0f7c57;
            border-bottom: 2px solid #0f7c57;
            background: #dcdcdc;
        }



        .report-table th {
            background: #d9d9d9;
            border: 1px solid #bfbfbf;
            padding: 10px 6px;
            text-align: left;
            font-weight: 600;
            font-size: 11px;
        }

        .report-table td {
            border: 1px solid #e0e0e0;
            padding: 8px 6px;
            vertical-align: top;
        }

        .report-table tr:hover {
            background: #f2f2f2;
        }

        /* Fixed column widths */
        .report-table th:nth-child(1) {
            width: 8%;
        }
        /* GL CODE */
        .report-table th:nth-child(2) {
            width: 12%;
        }
        /* GL DESCRIPTION */
        .report-table th:nth-child(3) {
            width: 8%;
        }
        /* BOOK TYPE */
        .report-table th:nth-child(4) {
            width: 5%;
        }
        /* GL FORM */
        .report-table th:nth-child(5) {
            width: 5%;
        }
        /* VOUCHER DATE */
        .report-table th:nth-child(6) {
            width: 16%;
        }
        /* NARATION */
        .report-table th:nth-child(7) {
            width: 10%;
        }
        /* CHEQUE NO */
        .report-table th:nth-child(8) {
            width: 5%;
        }
        /* BILL NO */
        .report-table th:nth-child(9) {
            width: 5%;
        }
        /* OPENING */
        .report-table th:nth-child(10) {
            width: 8%;
        }
        /* DEBIT */
        .report-table th:nth-child(11) {
            width: 8%;
        }
        /* CREDIT */
        .report-table th:nth-child(12) {
            width: 8%;
        }
        /* RUNNING BALANCE */

        .total-row {
            background: #dcdcdc !important;
            font-weight: bold;
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

        @media print {
    body {
        background: white;
        padding: 0;
        margin: 0;
    }
    
    .filter-section, .no-print {
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
        background: #d9d9d9 !important;
        -webkit-print-color-adjust: exact;
        print-color-adjust: exact;
    }
    
    .total-row td {
        background: #dcdcdc !important;
        -webkit-print-color-adjust: exact;
        print-color-adjust: exact;
    }
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="report-container">
            <div class="company-header">
                <div class="company-name">BAHRIA TOWN KARACHI</div>
                <div class="company-sub">GL ACCOUNTING SYSTEM</div>
            </div>

            <div class="report-title">GENERAL LEDGER REPORT</div>

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
                    <asp:Button ID="btnPrint" runat="server" Text="Print" OnClientClick="window.print();return false;" CssClass="btn-print" />
                </div>
            </div>

            <div class="report-period">
                FROM:
                <asp:Label ID="lblFromDate" runat="server" />
                TO:
                <asp:Label ID="lblToDate" runat="server" />
                <asp:Label ID="lblAccountRange" runat="server" />
            </div>

            <div class="table-wrapper">
                <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="false" CssClass="report-table"
                    OnRowDataBound="gvReport_RowDataBound" ShowHeader="true" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="GL_CODE" HeaderText="GL CODE" ItemStyle-CssClass="text-right" HeaderStyle-CssClass="text-left" />
                        <asp:BoundField DataField="GL_DESCRP" HeaderText="GL DESCRIPTION" />
                        <asp:BoundField DataField="BOOK_TYPE" HeaderText="BOOK TYPE" />
                        <asp:BoundField DataField="GL_FORM_NUMBER" HeaderText="GL FORM" />
                        <asp:BoundField DataField="VOUCHER_DATE" HeaderText="VOUCHER DATE" />
                        <asp:BoundField DataField="NARATION" HeaderText="NARATION" />
                        <asp:BoundField DataField="CHEQUE_NUMBER" HeaderText="CHEQUE NO" />
                        <asp:BoundField DataField="BILL_NUMBER" HeaderText="BILL NO" />
                        <asp:BoundField DataField="OPENING_BALANCE" HeaderText="OPENING" ItemStyle-CssClass="text-right" />
                        <asp:BoundField DataField="DEBIT" HeaderText="DEBIT" ItemStyle-CssClass="text-right" />
                        <asp:BoundField DataField="CREDIT" HeaderText="CREDIT" ItemStyle-CssClass="text-right" />
                        <asp:TemplateField HeaderText="RUNNING BALANCE" ItemStyle-CssClass="text-right">
                            <ItemTemplate>
                                <asp:Label ID="lblRunningBalance" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="status no-print">
                <asp:Label ID="lblStatus" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
