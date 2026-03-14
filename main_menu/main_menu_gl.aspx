<%@ Page Language="C#" AutoEventWireup="true" CodeFile="main_menu_gl.aspx.cs" Inherits="main_menu_main_menu_gl" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>General Ledger System</title>
    <style>
        /* Your existing styles remain the same */
        :root { 
            --gap: 16px; 
            --radius: 16px; 
            --shadow: 0 6px 18px rgba(0,0,0,.08); 
        }
        
        body {
            margin: 0;
            font-family: system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif;
            background: #f6f7f9;
            min-height: 100vh;
        }
        
        .header-border {
            border-top: 30px solid #000;
            width: 100%;
            margin-bottom: 0;
        }
        
        header {
            position: sticky;
            top: 0;
            background: #fff;
            border-bottom: 1px solid #e5e7eb;
            padding: 12px 20px;
            display: flex;
            gap: 12px;
            align-items: center;
            justify-content: space-between;
        }
        
        .brand {
            font-weight: 600;
            font-size: 18px;
            color: #000;
        }
        
        .header-btns {
            background-color: black;
            color: white !important;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            transition: background-color 0.2s ease-in-out;
            text-decoration: none !important;
            display: inline-block;
        }
        
        .header-btns:hover {
            background-color: #333;
            text-decoration: none !important;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        
        h1 {
            font-size: 2.5rem;
            margin-bottom: 10px;
        }
        
        .subtitle {
            font-size: 1.2rem;
            opacity: 0.9;
        }
        
        .menu-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
            gap: 25px;
        }
        
        .menu-card {
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: var(--shadow);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }
        
        .menu-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
        }
        
        .card-header {
            color: white;
            padding: 20px;
            text-align: center;
            font-size: 1.4rem;
            font-weight: 600;
        }
        
        .setup-header {
            background: #3498db;
        }
        
        .transaction-header {
            background: #2ecc71;
        }
        
        .reporting-header {
            background: #e74c3c;
        }
        
        .menu-items {
            padding: 20px;
        }
        
        .menu-item {
            padding: 15px;
            margin: 12px 0;
            background: #f8f9fa;
            border-radius: 8px;
            display: flex;
            align-items: center;
            transition: background-color 0.2s;
            cursor: pointer;
            text-decoration: none;
            color: inherit;
            border: none;
            width: 100%;
            text-align: left;
        }
        
        .menu-item:hover {
            background: #e9ecef;
        }
        
        .menu-item i {
            margin-right: 15px;
            font-size: 1.2rem;
            width: 25px;
            text-align: center;
        }
        
        .setup-item i {
            color: #3498db;
        }
        
        .transaction-item i {
            color: #2ecc71;
        }
        
        .reporting-item i {
            color: #e74c3c;
        }
        
        .footer {
            text-align: center;
            margin-top: 40px;
            padding: 20px;
            color: #6c757d;
            font-size: 0.9rem;
        }
        
        @media (max-width: 768px) {
            .menu-grid {
                grid-template-columns: 1fr;
            }
            
            h1 {
                font-size: 2rem;
            }
        }
    </style>
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="header-border"></div>
        <header>
            <div class="brand">BAHRIA TOWN KARACHI ERP</div>
            
            <div style="display: flex; gap: 12px; align-items: center;">
                <asp:LinkButton ID="back_btn" runat="server" CssClass="header-btns" OnClick="back_btn_Click">
                    Go back to previous page
                </asp:LinkButton>
                
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue" Font-Bold="true"></asp:Label>
                
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">
                    Log off
                </asp:LinkButton>
            </div>
        </header>
        
        <div class="container">
            <div style="text-align: center; margin-bottom: 30px; padding: 20px;">
                <h1>General Ledger System</h1>
                <p class="subtitle">Comprehensive financial management solution</p>
            </div>
            
            <div class="menu-grid">
                <!-- Setup GL Card -->
                <div class="menu-card">
                    <div class="card-header setup-header">
                        <i class="fas fa-cog"></i> Setup GL
                    </div>
                    <div class="menu-items">
                        <asp:LinkButton ID="lnkCOA" runat="server" CssClass="menu-item setup-item" OnClick="SetupMenuItem_Click">
                            <i class="fas fa-list-alt"></i>
                            <span>Chart of Accounts</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnSubLedger" runat="server" CssClass="menu-item setup-item" OnClick="SetupMenuItem_Click">
                            <i class="fas fa-list-ol"></i>
                            <span>Chart of Accounts (Sub Ledger)</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnBooksType" runat="server" CssClass="menu-item setup-item" OnClick="SetupMenuItem_Click">
                            <i class="fas fa-book"></i>
                            <span>Books Type</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnCostCenter" runat="server" CssClass="menu-item setup-item" OnClick="SetupMenuItem_Click">
                            <i class="fas fa-bullseye"></i>
                            <span>Cost Center</span>
                        </asp:LinkButton>
                    </div>
                </div>
                
                <!-- Transaction Card -->
                <div class="menu-card">
                    <div class="card-header transaction-header">
                        <i class="fas fa-exchange-alt"></i> Transaction
                    </div>
                    <div class="menu-items">
                        <asp:LinkButton ID="btnBankReceiveables" runat="server" CssClass="menu-item transaction-item" OnClick="TransactionMenuItem_Click">
                            <i class="fas fa-university"></i>
                            <span>Bank Receiveables</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnBankPayables" runat="server" CssClass="menu-item transaction-item" OnClick="TransactionMenuItem_Click">
                            <i class="fas fa-credit-card"></i>
                            <span>Bank Payables</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnCashReceiveables" runat="server" CssClass="menu-item transaction-item" OnClick="TransactionMenuItem_Click">
                            <i class="fas fa-money-bill-wave"></i>
                            <span>Cash Receiveables</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnCashPayables" runat="server" CssClass="menu-item transaction-item" OnClick="TransactionMenuItem_Click">
                            <i class="fas fa-money-check"></i>
                            <span>Cash Payables</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnJournalVoucher" runat="server" CssClass="menu-item transaction-item" OnClick="TransactionMenuItem_Click">
                            <i class="fas fa-file-invoice-dollar"></i>
                            <span>Journal Voucher</span>
                        </asp:LinkButton>
                    </div>
                </div>
                
                <!-- Reporting Card -->
                <div class="menu-card">
                    <div class="card-header reporting-header">
                        <i class="fas fa-chart-bar"></i> Reporting
                    </div>
                    <div class="menu-items">
                        <asp:LinkButton ID="btnAuditTrial" runat="server" CssClass="menu-item reporting-item" OnClick="ReportingMenuItem_Click">
                            <i class="fas fa-search-dollar"></i>
                            <span>Audit Trial</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnJournalGeneral" runat="server" CssClass="menu-item reporting-item" OnClick="ReportingMenuItem_Click">
                            <i class="fas fa-book"></i>
                            <span>Journal General</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnJournalGeneralSub" runat="server" CssClass="menu-item reporting-item" OnClick="ReportingMenuItem_Click">
                            <i class="fas fa-book-open"></i>
                            <span>Journal General Sub Ledger</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnTrialBalance" runat="server" CssClass="menu-item reporting-item" OnClick="ReportingMenuItem_Click">
                            <i class="fas fa-balance-scale"></i>
                            <span>Trial Balance</span>
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnTrialBalanceSub" runat="server" CssClass="menu-item reporting-item" OnClick="ReportingMenuItem_Click">
                            <i class="fas fa-balance-scale-left"></i>
                            <span>Trial Balance Sub Ledger</span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>