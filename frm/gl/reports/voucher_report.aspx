<%@ Page Language="C#" AutoEventWireup="true" CodeFile="voucher_report.aspx.cs" Inherits="voucher_report" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Voucher Print</title>
    <style>
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            margin: 20px;
            background: white;
        }

        /* Header container */
        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            border-bottom: 2px solid #0f7c57;
        }

        .print-btn, .close-btn {
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

            .print-btn:hover, .close-btn:hover {
                background-color: #333;
                transform: translateY(-1px);
            }

        /* Voucher Box */
        .voucher-box {
            border: 1px solid #ddd;
            padding: 20px;
            background: white;
            margin-top: 10px;
        }

        /* Company Header */
        .company-header {
            text-align: center;
            border-bottom: 2px solid #0f7c57;
            padding-bottom: 15px;
            margin-bottom: 20px;
        }

        .company-name {
            font-size: 18px;
            font-weight: bold;
            color: #0f7c57;
            margin: 0;
        }

        .company-address {
            font-size: 11px;
            color: #666;
            margin: 5px 0;
        }

        .voucher-title {
            text-align: center;
            font-size: 16px;
            font-weight: bold;
            text-decoration: underline;
            margin: 15px 0;
            text-transform: uppercase;
        }

        /* Voucher Info Table */
        .voucher-info {
            width: 100%;
            margin: 15px 0;
            border-collapse: collapse;
        }

            .voucher-info td {
                padding: 8px;
                border: 1px solid #ddd;
            }

                .voucher-info td:first-child {
                    width: 150px;
                    font-weight: bold;
                    background: #f5f5f5;
                }

        /* Details Table */
        .details-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }

            .details-table th {
                background: #0f7c57;
                color: white;
                padding: 10px;
                text-align: left;
                border: 1px solid #0a5e40;
            }

            .details-table td {
                padding: 8px;
                border: 1px solid #ddd;
                vertical-align: top;
            }

            .details-table tr:nth-child(even) {
                background: #f9f9f9;
            }

        .amount-column {
            text-align: right;
        }

        .total-row {
            background: #e6e6e6 !important;
            font-weight: bold;
        }

            .total-row td {
                font-weight: bold;
            }

        /* Amount in Words */
        .amount-words {
            margin: 20px 0;
            padding: 10px;
            background: #f5f5f5;
            border-left: 3px solid #0f7c57;
            font-size: 12px;
        }

        /* Signature Section */
        .signature {
            margin-top: 30px;
            display: flex;
            justify-content: space-between;
        }

        .signature-line {
            text-align: center;
            width: 200px;
        }

            .signature-line hr {
                margin: 30px 0 5px;
            }

        /* Footer */
        .footer {
            margin-top: 20px;
            text-align: center;
            font-size: 10px;
            color: #999;
            border-top: 1px solid #ddd;
            padding-top: 10px;
        }

        .status-posted {
            color: green;
            font-weight: bold;
        }

        .status-unposted {
            color: red;
            font-weight: bold;
        }

        @media print {
            .no-print {
                display: none;
            }

            .voucher-box {
                border: none;
                padding: 0;
            }

            .details-table th {
                background: #0f7c57;
                color: white;
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="no-print header-container">
            <h1 style="margin: 0;">Voucher Print</h1>
            <div class="no-print header-container">
    <div class="button-group">
        <asp:Button ID="btnPrint" runat="server" Text="🖨️ Print" CssClass="print-btn" OnClientClick="window.print();return false;" />
        <asp:Button ID="btnExportExcel" runat="server" Text="📊 Excel" CssClass="print-btn" OnClick="btnExportExcel_Click" />
        <asp:Button ID="btnClose" runat="server" Text="✖ Close" CssClass="close-btn" OnClick="btnClose_Click" />
    </div>
</div>
        </div>

        <div class="voucher-box">
            <!-- Company Header -->
            <div class="company-header">
                <div class="company-name">BAHRIA TOWN KARACHI</div>
            </div>

            <!-- Voucher Title -->
            <div class="voucher-title">
                <asp:Label ID="lblVoucherTitle" runat="server" Text="JOURNAL VOUCHER" />
            </div>

            <!-- Voucher Info -->
            <table class="voucher-info">
                <tr>
                    <td>Voucher Number:</td>
                    <td>
                        <asp:Label ID="lblVoucherNumber" runat="server" Font-Bold="true" /></td>
                    <td>Voucher Date:</td>
                    <td>
                        <asp:Label ID="lblVoucherDate" runat="server" /></td>
                </tr>
                <tr>
                    <td>Voucher Key:</td>
                    <td>
                        <asp:Label ID="lblVoucherKey" runat="server" /></td>
                    <td>Status:</td>
                    <td>
                        <asp:Label ID="lblStatus" runat="server" /></td>
                </tr>
            </table>

            <!-- Details Grid -->
            <asp:GridView ID="gvDetails" runat="server" CssClass="details-table"
                AutoGenerateColumns="False" ShowFooter="True" GridLines="None">
                <Columns>
                    <asp:TemplateField HeaderText="S.No" HeaderStyle-Width="50px">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <strong>TOTAL</strong>
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="GL_CODE" HeaderText="GL Code" HeaderStyle-Width="100px" />
                    <asp:BoundField DataField="GL_DESCRIPTION" HeaderText="Account Description / Particulars" HeaderStyle-Width="300px" />
                    <asp:TemplateField HeaderText="Debit" HeaderStyle-Width="120px" ItemStyle-CssClass="amount-column" FooterStyle-CssClass="amount-column">
                        <ItemTemplate>
                            <%# Convert.ToDecimal(Eval("DEBIT")).ToString("N2") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblTotalDebit" runat="server" Font-Bold="true" />
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Credit" HeaderStyle-Width="120px" ItemStyle-CssClass="amount-column" FooterStyle-CssClass="amount-column">
                        <ItemTemplate>
                            <%# Convert.ToDecimal(Eval("CREDIT")).ToString("N2") %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Label ID="lblTotalCredit" runat="server" Font-Bold="true" />
                        </FooterTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Amount in Words -->
            <div class="amount-words">
                <strong>Amount in Words:</strong>
                <asp:Label ID="lblAmountInWords" runat="server" />
            </div>

            <!-- Signature Section -->
            <div class="signature">
                <div class="signature-line">
                    <hr />
                    <span>Prepared By</span>
                </div>
                <div class="signature-line">
                    <hr />
                    <span>Checked By</span>
                </div>
                <div class="signature-line">
                    <hr />
                    <span>Authorized By</span>
                </div>
            </div>

            <!-- Footer -->
            <div class="footer">
                This is a computer generated document - No signature required<br />
                Printed on:
                <asp:Label ID="lblPrintDate" runat="server" />
            </div>
        </div>

        <asp:HiddenField ID="hfVoucherKey" runat="server" />
    </form>
</body>
</html>
