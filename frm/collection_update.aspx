<%@ Page Language="C#" AutoEventWireup="true" CodeFile="collection_update.aspx.cs" Inherits="frm_billing_Default" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Electric Collection Upload</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f7f7f7;
        }

        header{position:sticky;top:0;background:#fff;border-bottom:1px solid #e5e7eb;padding:12px 20px;display:flex;gap:12px;align-items:center;}

        /* HEADER */
        /*.app-header {background: #000;color: #fff;padding: 12px 20px;display: flex;align-items: center;}
        .header-left, .header-right { flex: 1; }
        .header-center { flex: 1; text-align: center; font-weight: 600; }
        .header-btns {background: #222; color: #fff !important; padding: 7px 12px; border-radius: 4px; text-decoration: none;}*/

        /* ===== HEADER ===== */
        header{position:sticky; top:0; background:#fff; border-bottom:1px solid #e5e7eb; padding:12px 20px; display:flex; gap:12px; align-items:center; }
        .header-border{border-top:30px solid #000; width:100%; }
        .header-btns{background-color:black; color:white !important; border:none; padding:8px 16px; border-radius:4px; cursor:pointer; font-size:16px; text-decoration:none !important; }
        .header-btns:hover{background-color:#333; }

        .container {
            display: flex;
            justify-content: center;
            padding: 40px;
        }

        .left-panel {
            background: #1e6fa7;
            color: white;
            padding: 30px;
            border-radius: 8px;
            flex: 1;
            max-width: 300px;
            margin-right: 20px;
        }

        .form-panel {
            background: white;
            padding: 30px;
            border-radius: 8px;
            flex: 2;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        h2 {
            font-size: 40px;
            margin-bottom: 25px;
        }

        label {
            font-size: 22px;
            font-weight: 600;
        }

        .asp-input {
            width: 100%;
            height: 55px;
            font-size: 22px;
            margin-top: 10px;
        }

        .row {
            display: flex;
            gap: 20px;
            margin-bottom: 20px;
        }

        .col {
            flex: 1;
        }

        .btn-row {
            margin-top: 25px;
            display: flex;
            gap: 15px;
        }

        .asp-button {
            background: #2d2a26;
            color: #fff;
            padding: 15px 30px;
            font-size: 22px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }

        #lblStatus {
            font-size: 22px;
            font-weight: bold;
            margin-top: 20px;
        }

        /* ===== LOADER STYLES ===== */
        .overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.6);
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        }

        .loader-box {
            background: #fff;
            padding: 30px 45px;
            border-radius: 8px;
            text-align: center;
            font-size: 22px;
            font-weight: bold;
            color: #0f7c57;
            box-shadow: 0 4px 10px rgba(0,0,0,0.3);
        }

        .spinner {
            margin: 15px auto 0;
            width: 50px;
            height: 50px;
            border: 6px solid #ddd;
            border-top: 6px solid #0f7c57;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }

        .back-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}
        .logoff-btn {background-color: black;color: white;border: none;padding: 8px 16px;border-radius: 4px;cursor: pointer;}

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>

    <script>
        function showLoader() {
            document.getElementById("overlay").style.display = "flex";
        }
    </script>
</head>

<body>
<form id="form1" runat="server">

    <div id="border_header" class="header-border"></div>
    <!-- HEADER -->
    <header>
        <div style="width:10%;">
            <asp:LinkButton ID="btnBack" runat="server"
                CssClass="header-btns"
                OnClick="btnBack_Click">
                Go Back
            </asp:LinkButton>
        </div>
        
        <div style="width:10%; font-weight:600;">
            
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

    <!-- ===== LOADING OVERLAY ===== -->
    <div id="overlay" class="overlay">
        <div class="loader-box">
            Uploading & Processing CSV<br />
            Please wait...
            <div class="spinner"></div>
        </div>
    </div>    

    <div class="container">

        <!-- LEFT -->
        <div class="left-panel">
            <h2>Upload Flow</h2>
            <ul>
                <li>Select CSV</li>
                <li>Truncate table</li>
                <li>Insert readings</li>
            </ul>
        </div>

        <!-- FORM -->
        <div class="form-panel">
            <h2>Electric Collection Upload</h2>

            <%-- Radio Buttons --%>
            <div class="row">
                <div class="col">
                    <label>Bill Type</label>
                    <asp:RadioButtonList
                        ID="rbBillType"
                        runat="server"
                        RepeatDirection="Horizontal"
                        CssClass="bill-radio" />
                </div>
            </div>

            <div class="row">
                <div class="col">
                    <label>Process Month</label>
                    <asp:TextBox ID="txtProcessMonth"
                                    runat="server"
                                    CssClass="asp-input"
                                    placeholder="YYYYMM" />
                </div>
            </div>
            <br />

            <label>Select CSV File</label>
            <asp:FileUpload ID="fuCsv"
                            runat="server"
                            CssClass="asp-input" />

            <div class="btn-row">
                <asp:Button ID="btnUpload"
                            runat="server"
                            Text="Upload CSV"
                            CssClass="asp-button"
                            OnClientClick="showLoader();"
                            OnClick="btnUpload_Click" />

                <asp:Button ID="btnCancel"
                            runat="server"
                            Text="Cancel"
                            CssClass="asp-button"
                            OnClick="btnCancel_Click" />
            </div>

            <asp:Label ID="lblStatus" runat="server" />

        </div>
    </div>
</form>
</body>
</html>

