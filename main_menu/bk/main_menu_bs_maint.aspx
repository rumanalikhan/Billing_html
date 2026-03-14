<%@ Page Language="C#" AutoEventWireup="true" CodeFile="main_menu_bs.aspx.cs" Inherits="main_menu_bs" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bahria Town Karachi</title>
    <style>
      :root { --gap: 16px; --radius: 16px; --shadow: 0 6px 18px rgba(0,0,0,.08); }
      body{margin:0;font-family:system-ui,-apple-system,Segoe UI,Roboto,Arial,sans-serif;background:#f6f7f9;}
      header{position:sticky;top:0;background:#fff;border-bottom:1px solid #e5e7eb;padding:12px 20px;display:flex;gap:12px;align-items:center;}
      .brand{font-weight:600;}
      .wrap{max-width:1100px;margin:24px auto;padding:0 16px;}
      .section{margin-bottom:28px;}
      .section h2{font-size:14px;font-weight:700;color:#6b7280;margin:0 0 10px;}
      .grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(180px,1fr));gap:var(--gap);}
      .tile{background:#fff;border-radius:var(--radius);box-shadow:var(--shadow);padding:16px;cursor:pointer;border:1px solid transparent;transition:.15s;}
      .tile:hover{transform:translateY(-2px);border-color:#d1d5db;}
      .tile .title{font-weight:600;margin-bottom:6px;}
      .tile .desc{font-size:12px;color:#6b7280;}
    </style>
    <link rel="stylesheet" type="text/css" href="~/css/style.css" />
</head>
<body>
    <header>
        <div id="brand_name" style="width:10%;">UTILITY Billing SYSTEM</div>
    
        <div id="header_actions_goback" style="width:80%; float:left;">
            <button class="back-btn" onclick="history.back()">Go back to previous page</button>
        </div>
    
        <div id="header_actions_logoff" style="width:10%; text-align:right; float:left;">
            <button class="logoff-btn" onclick="btnLogoff_Click()">Log off</button>
        </div>
    </header>

    <form id="form1" runat="server">
        <div class="wrap">
            <div class="section">
                <h2>Setup</h2>
                <div class="grid">
                    <asp:LinkButton ID="lnkCoa" runat="server" CssClass="tile">
<%--                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="tile" OnClick="lnkCoa_Click">--%>
                        <div class="title">Maintenance</div>
                        <div class="desc">GL master</div>
                    </asp:LinkButton>

                    <asp:LinkButton ID="lnkSubLedger" runat="server" CssClass="tile">
<%--                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="tile" OnClick="lnkSubLedger_Click">--%>
                        <div class="title">Electric</div>
                        <div class="desc">AR/AP</div>
                    </asp:LinkButton>

                    <asp:LinkButton ID="lnkBookType" runat="server" CssClass="tile">
<%--                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="tile" OnClick="lnkBookType_Click">--%>
                        <div class="title">Water</div>
                        <div class="desc">Define books</div>
                    </asp:LinkButton>

                    <asp:LinkButton ID="lnkCostCenter" runat="server" CssClass="tile">
<%--                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="tile" OnClick="lnkCostCenter_Click">--%>
                        <div class="title">Gas</div>
                        <div class="desc">Dimensions</div>
                    </asp:LinkButton>
                </div>
            </div>

            <div class="section">
                <h2>Transactions</h2>
                <div class="grid">
                    <asp:LinkButton ID="lnkBPV" runat="server" CssClass="tile">
<%--                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="tile" OnClick="lnkBPV_Click">--%>
                        <div class="title">Rental Billing</div>
                        <div class="desc">Outgoing bank</div>
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
