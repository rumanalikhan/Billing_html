<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="water_bill_posting.aspx.cs"
    Inherits="water_bill_posting" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Water Bill Posting</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        body {
            margin: 0;
            font-family: Arial, sans-serif;
            background: #f7f7f7;
            color: #333;
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
            font-size: 42px;
            margin-bottom: 25px;
        }

        .row {
            display: flex;
            gap: 20px;
            margin-bottom: 20px;
        }

        .col {
            flex: 1;
        }

        label {
            font-size: 22px;
            font-weight: 600;
            padding-left: 8px;
        }

        .asp-input {
            width: 100%;
            height: 65px;
            font-size: 24px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

        .readonly-box {
            border: none !important;
            background: transparent;
            pointer-events: none;
            font-weight: bold;
        }

        .btn-row {
            display: flex;
            margin-top: 25px;
        }

        .btn-left {
            display: flex;
            gap: 15px;
        }

        .btn-right {
            margin-left: auto;
        }

        .asp-button {
            background: #2d2a26;
            color: #fff;
            padding: 15px 30px;
            font-size: 24px;
            border-radius: 4px;
            border: none;
            cursor: pointer;
        }

        #lblStatus {
            font-size: 22px;
            font-weight: bold;
        }

        .posted {
            color: green;
        }

        .pending {
            color: red;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">
<div class="container">

    <!-- LEFT PANEL -->
    <div class="left-panel">
        <h2>Posting Flow</h2>
        <ul>
            <li>Review bills</li>
            <li>Post to ledger</li>
            <li>Finalize</li>
        </ul>
    </div>

    <!-- FORM PANEL -->
    <div class="form-panel">
        <h2>Water Bill Posting</h2>

        <!-- BUTTONS -->
        <div class="btn-row">
            <div class="btn-left">
                <asp:Button ID="btnPost"
                            runat="server"
                            Text="Post Bills"
                            CssClass="asp-button"
                            OnClick="btnPost_Click" />

                <asp:Button ID="btnCancel"
                            runat="server"
                            Text="Cancel"
                            CssClass="asp-button"
                            OnClick="btnCancel_Click" />
            </div>
        </div>

        <!-- STATUS -->
        <div class="row">
            <div class="col">
                <asp:Label ID="lblStatus" runat="server" />
            </div>
        </div>

        <!-- SUMMARY -->
        <div class="row">
            <div class="col">
                <label>Posted Bills</label>
                <asp:TextBox ID="txtPosted"
                             runat="server"
                             CssClass="asp-input readonly-box posted"
                             ReadOnly="true" />
            </div>
        </div>

    </div>
</div>
</form>
</body>
</html>
