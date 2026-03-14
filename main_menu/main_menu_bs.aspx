<%@ Page Language="C#" AutoEventWireup="true" CodeFile="main_menu_bs.aspx.cs" Inherits="main_menu_bs" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bahria Town Karachi</title>

    <style>
        :root { --gap: 16px; --radius: 16px; --shadow: 0 6px 18px rgba(0,0,0,.08); }

        body{
            margin:0;
            font-family:system-ui,-apple-system,Segoe UI,Roboto,Arial,sans-serif;
            background:#f6f7f9;
        }

        /* ===== HEADER ===== */
        header{
            position:sticky;
            top:0;
            background:#fff;
            border-bottom:1px solid #e5e7eb;
            padding:12px 20px;
            display:flex;
            gap:12px;
            align-items:center;
        }

        .header-border{
            border-top:30px solid #000;
            width:100%;
        }

        .header-btns{
            background-color:black;
            color:white !important;
            border:none;
            padding:8px 16px;
            border-radius:4px;
            cursor:pointer;
            font-size:16px;
            text-decoration:none !important;
        }

        .header-btns:hover{
            background-color:#333;
        }

        /* ===== BODY ===== */
        .wrap{
            max-width:1100px;
            margin:24px auto;
            padding:0 16px;
        }

        .section{
            margin-bottom:28px;
        }

        /* ===== GRID FIX (NO OVERLAP) ===== */
        .grid{
            display:grid;
            grid-template-columns:repeat(auto-fit, 250px);
            gap:16px;
            justify-content:center;
        }

        /* ===== MENU BUTTON ===== */
        .menu-button{
            width:250px;
            height:120px;
            background:#fff;
            color:#333;
            border:none;
            font-size:16px;
            font-weight:700;
            border-radius:20px;
            cursor:pointer;
            box-shadow:0 4px 6px rgba(0,0,0,0.2);
            transition:all 0.2s ease-in-out;
        }

        .menu-button:hover{
            background:#f0f0f0;
            box-shadow:0 -4px 6px rgba(0,0,0,0.3);
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <!-- BLACK TOP BAR -->
    <div class="header-border"></div>

    <!-- HEADER -->
    <header>
        <div style="width:10%; font-weight:600;">
            BILLING SYSTEM
        </div>

        <div style="width:10%;">
            <asp:LinkButton ID="btnBack" runat="server"
                CssClass="header-btns"
                OnClick="btnBack_Click">
                Go back to previous page
            </asp:LinkButton>
        </div>

        <div style="width:56%; text-align:center;">
            <asp:Label ID="lblUser" runat="server" ForeColor="Blue"></asp:Label>
        </div>

        <div style="width:22%; text-align:right;">
            <asp:LinkButton ID="btnLogoff" runat="server"
                CssClass="header-btns"
                OnClick="btnLogoff_Click">
                Log off
            </asp:LinkButton>
        </div>
    </header>

    <!-- BODY -->
    <div class="wrap">

        <div class="section">
            <h2 style="font-size:25px;font-weight:600;padding:10px;text-align:center;">
                Billing Modules
            </h2>

            <div class="grid">

                <asp:Button ID="btnMaintenance" runat="server"
                    CssClass="menu-button"
                    Text="Maintenance Billing"
                    OnClick="btnMaintenance_Click" />

                <asp:Button ID="btnElectric" runat="server"
                    CssClass="menu-button"
                    Text="Electric Billing"
                    OnClick="btnElectric_Click" />

                <asp:Button ID="btnWater" runat="server"
                    CssClass="menu-button"
                    Text="Water Billing"
                    OnClick="btnWater_Click" />

                <asp:Button ID="btnGas" runat="server"
                    CssClass="menu-button"
                    Text="Gas Billing"
                    OnClick="btnGas_Click" />

                <asp:Button ID="btnRental" runat="server"
                    CssClass="menu-button"
                    Text="Rental Billing"
                    OnClick="btnRental_Click" />

                <asp:Button ID="btnBNB" runat="server"
                    CssClass="menu-button"
                    Text="BNB Billing"
                    OnClick="btnBNB_Click" />

                <asp:Button ID="btnGenSummary" runat="server"
                    CssClass="menu-button"
                    Text="Generate Summary"
                    OnClick="btnGenSummary_Click" />

                <asp:Button ID="btnCollectionUpdate" runat="server"
                    CssClass="menu-button"
                    Text="Collection Update"
                    OnClick="btnCollectionUpdate_Click" />
            </div>
        </div>

    </div>

</form>
</body>
</html>
