<%@ Page Language="C#" AutoEventWireup="true" CodeFile="menu_maintenance.aspx.cs" Inherits="menu_maintenance" %>

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bahria Town Karachi (Maintenance Billing)</title>

    <style>
        body{
            margin:0;
            font-family:system-ui,-apple-system,Segoe UI,Roboto,Arial,sans-serif;
            background:#f6f7f9;
        }

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
            background:black;
            color:white !important;
            border:none;
            padding:8px 16px;
            border-radius:4px;
            cursor:pointer;
            font-size:16px;
            text-decoration:none !important;
        }

        .wrap{
            max-width:1100px;
            margin:24px auto;
            padding:0 16px;
        }

        /* ===== MAIN 3 COLUMN LAYOUT ===== */
        .columns{
            display:grid;
            grid-template-columns:repeat(3, 250px);
            gap:30px;
            justify-content:center;
            margin-top:20px;
        }

        .column{
            display:flex;
            flex-direction:column;
            gap:16px;
        }

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

    <div class="header-border"></div>

    <header>
        <div style="width:10%; font-weight:600;">MAINTENANCE BILLING</div>

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

    <div class="wrap">
        <h2 style="font-size:25px;font-weight:600;text-align:center;">
            Maintenance Menu
        </h2>

        <div class="columns">

            <!-- COLUMN 1 -->
            <div class="column">
                <asp:Button ID="btnResidentialInfo" runat="server" CssClass="menu-button"
                    Text="Residential Information" OnClick="btnResidentialInfo_Click" />

                <asp:Button ID="btnBarcodeMaintenance" runat="server" CssClass="menu-button"
                    Text="Barcode Maintenance" OnClick="btnBarcodeMaintenance_Click" />

                <asp:Button ID="btnBarcodeElectric" runat="server" CssClass="menu-button"
                    Text="Barcode Electric" OnClick="btnBarcodeElectric_Click" />

                <asp:Button ID="btnBarcodeWater" runat="server" CssClass="menu-button"
                    Text="Barcode Water" OnClick="btnBarcodeWater_Click" />

                <asp:Button ID="btnBarcodeGas" runat="server" CssClass="menu-button"
                    Text="Barcode Gas" OnClick="btnBarcodeGas_Click" />
            </div>

            <!-- COLUMN 2 -->
            <div class="column">
                <asp:Button ID="btnBillGenerateTobe" runat="server" CssClass="menu-button"
                    Text="Create Month Header" OnClick="btnBillGenerateTobe_Click" />

                <asp:Button ID="btnGenerateBills" runat="server" CssClass="menu-button"
                    Text="Generate Bills" OnClick="btnGenerateBills_Click" />

                <asp:Button ID="btnUpdateBills" runat="server" CssClass="menu-button"
                    Text="Update Bills" OnClick="btnUpdateBills_Click" />

                <asp:Button ID="btnUploadArrears" runat="server" CssClass="menu-button"
                    Text="Upload Arrears" OnClick="btnUploadArrears_Click" />

                <asp:Button ID="btnUploadAdvances" runat="server" CssClass="menu-button"
                    Text="Upload Advances" OnClick="btnUploadAdvances_Click" />
            </div>

            <!-- COLUMN 3 -->
            <div class="column">
                <asp:Button ID="btnPostingBills" runat="server" CssClass="menu-button"
                    Text="Posting Bills" OnClick="btnPostingBills_Click" />
            </div>

        </div>
    </div>

</form>
</body>
</html>
