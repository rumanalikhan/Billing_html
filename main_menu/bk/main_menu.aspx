<%@ Page Language="C#" AutoEventWireup="true" CodeFile="main_menu.aspx.cs" Inherits="main_menu" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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

        .menu-button {background-color: white;color: #333;border: none;font-size: 16px;font-weight: 700;border-radius: 20px;cursor: pointer;box-shadow: 0 4px 6px rgba(0,0,0,0.2);transition: all 0.2s ease-in-out;text-align: center;width:250px;height:120px;}
        .menu-button:hover {background-color: #f0f0f0;box-shadow: 0 -4px 6px rgba(0,0,0,0.3);}

        .back-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}
        .logoff-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}

        .label-large {padding-bottom: 2px;padding-left: 10px;font-size: 25px;font-weight: 600;}

        .header-btns {background-color: black;color: white !important;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;font-size: 16px;transition: background-color 0.2s ease-in-out;text-decoration: none !important;display: inline-block;}

        .header-btns:hover {background-color: #333;text-decoration: none !important;}

        .header-border {border-top: 30px solid #000;width: 100%;margin-bottom: 0;}
    </style>
<%--    <link rel="stylesheet" type="text/css" href="~/css/style.css" />--%>
</head>
<body>
    <form id="form1" runat="server">
        <div id="border_header" class="header-border"></div>
        <header>
            <div id="brand_name" CssClass="header-border" style="width:10%; ">BAHRIA TOWN KARACHI ERP</div>
    
            <div id="header_actions_goback" style="width:10%; float:left;">
                <asp:LinkButton ID="back_btn" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Go back to previous page</asp:LinkButton>
            </div>

            <div id="header_user" style="width:56%; float:left; text-align:center; color:white; font-weight:bold;">
                <asp:Label ID="lblUser" runat="server" ForeColor="Blue"></asp:Label>
            </div>

            <div id="header_actions_logoff" style="width:22%; text-align:right; float:left;">
                <asp:LinkButton ID="btnLogoff" runat="server" CssClass="header-btns" OnClick="btnLogoff_Click">Log off</asp:LinkButton>
            </div>
        </header>

        <div class="wrap" style="width:900px; height:390px;">
            <div class="section" style="width:900px; height:170px;">
                <h2 style="padding-bottom: 10px; padding-left: 10px; padding-top: 5px; font-size: 25px; font-weight: 600;">Administration</h2>

                <div class="grid" style="width:900px; height:110px;">
                    <asp:Button ID="btnUsers" runat="server" CssClass="menu-button" Text="User & Roles" OnClick="btnUsers_Click" />
                </div>
            </div>

            <div class="section" style="width:900px; height:170px;">
                <h2 style="padding-bottom: 10px; padding-left: 10px; padding-top: 5px; font-size: 25px; font-weight: 600;">Applications</h2>
                <div class="grid" style="width:297px; height:110px; float:left;">
                    <asp:Button ID="btnmain_menu_gl" runat="server" CssClass="menu-button" Text="GL Accounting System" OnClick="btnmain_menu_gl_Click" />
                </div>
                <div class="grid" style="width:297px; height:110px; float:left;">
                    <asp:Button ID="btnmain_menu_bs" runat="server" CssClass="menu-button" Text="Billing System" OnClick="btnmain_menu_bs_Click" />
                </div>
                <div class="grid" style="width:297px; height:110px; float:left;">
                    <asp:Button ID="btnMeterMgmt" runat="server" CssClass="menu-button" Text="Meter Management System" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
