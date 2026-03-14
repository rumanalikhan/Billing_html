<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="water_reading_upload.aspx.cs"
    Inherits="water_reading_upload" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Water Reading Upload</title>
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
    </style>
</head>

<body>
<form id="form1" runat="server">
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
        <h2>Water Reading Upload</h2>

        <label>Select CSV File</label>
        <asp:FileUpload ID="fuCsv" runat="server" CssClass="asp-input" />

        <div class="btn-row">
            <asp:Button ID="btnUpload"
                        runat="server"
                        Text="Upload CSV"
                        CssClass="asp-button"
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
