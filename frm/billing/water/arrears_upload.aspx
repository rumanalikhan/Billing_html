<%@ Page Language="C#" AutoEventWireup="true" CodeFile="arrears_upload.aspx.cs" Inherits="arrears_upload" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <title>Water Arrears Upload</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f7f7f7;
        }

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

    <!-- ===== LOADING OVERLAY ===== -->
    <div id="overlay" class="overlay">
        <div class="loader-box">
            Uploading CSV<br />
            Please wait...
            <div class="spinner"></div>
        </div>
    </div>

    <div class="container">

        <!-- LEFT -->
        <div class="left-panel">
            <h2>Upload Flow</h2>
            <ul>
                <li>Columns: Res Id, Arrears</li>
                <li>No comma required</li>
                <li>File Format .CSV</li>
            </ul>
        </div>

        <!-- FORM -->
        <div class="form-panel">
            <h2>Water Arrears Upload</h2>

            <label>Select Arrears CSV File</label>
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
