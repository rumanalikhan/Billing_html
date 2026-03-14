<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
  <meta charset="UTF-8" />
  <title>Login to Database</title>
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />

  <style>
    body {margin:0;font-family:Arial, sans-serif;background:#f7f7f7;color:#333;}
    header {background:#2d2a26;color:white;padding:15px 40px;font-size:18px;font-weight:bold;}
    .container {display:flex;justify-content:center;padding:40px;}
    .left-panel {background:#0f7c57;color:white;padding:30px;border-radius:8px;flex:1;max-width:300px;margin-right:20px;}
    .left-panel h2 {margin-bottom:20px;}
    .left-panel ul {list-style:disc;padding-left:20px;}
    .left-panel ul li {margin-bottom:10px;}


    .form-panel {background:white;padding:30px;border-radius:8px;flex:2;box-shadow:0 2px 5px rgba(0,0,0,0.1);height: 500px;}


    .form-panel h2 {margin-bottom:20px;}
    .form-group {margin-bottom:20px;}
    /*label {display:block;font-weight:bold;margin-bottom:5px;}*/


    /*label-large {padding-bottom: 2px; padding-left: 10px; font-size: 25px; font-weight: 600;}*/

    input, select, 
    
    .asp-input {width:100%;padding:10px;border:1px solid #ccc;border-radius:4px;box-sizing:border-box;height:70px;font-size: 30px;}


    .captcha {display:flex;align-items:center;margin-top:10px;}
    .captcha input {width:auto;margin-right:10px;}
    button, .asp-button {background:#2d2a26;color:white;border:none;padding:12px 20px;font-size:16px;border-radius:4px;cursor:pointer; font-size:30px;}
    button:disabled, .asp-button:disabled {background:#999;cursor:not-allowed;}
    #message {margin-top:8px;font-size:14px;color:#c00;}
    .spinner {display:inline-block;width:14px;height:14px;border:2px solid #fff;border-top-color:transparent;border-radius:50%;vertical-align:middle;margin-left:8px;animation:spin .8s linear infinite;}
    @keyframes spin {to {transform:rotate(360deg);}}
  </style>

</head>
<body>
  <header></header>

  <form id="form1" runat="server">
    <div class="container">
      <!-- Left Panel -->
      <div class="left-panel">
        <h2>Login to access database</h2>
        <p>Protect your password</p>
        <ul>
          <li>Never share your password</li>
          <li>Create strong password like Aa1$</li>
        </ul>
        <p><strong>Login to database</strong></p>
      </div>

      <!-- Right Form Panel -->
      <div class="form-panel">
<%--        <h2>Get access</h2>--%>
        <h2 style="padding-bottom: 0px; padding-left: 0px; font-size: 50px; font-weight: 600;">Get access</h2>

        <div class="form-group">
          <label for="txtUser" style="padding-bottom: 2px; padding-left: 10px; font-size: 25px; font-weight: 600;">Login ID</label>
          <asp:TextBox ID="txtUser" runat="server" CssClass="asp-input"></asp:TextBox>
        </div>

        <div class="form-group" style="padding-top: 15px;">
          <label for="txtPass" style="padding-bottom: 2px; padding-left: 10px; font-size: 25px; font-weight: 600;">Password</label>
          <asp:TextBox ID="txtPass" runat="server" CssClass="asp-input" TextMode="Password"></asp:TextBox>
        </div>

        <div class="form-group" style="padding-top: 20px;">
          <asp:Button ID="btnSubmit" runat="server" CssClass="asp-button" Text="Submit" OnClick="btnSubmit_Click" Enabled="false" />
          <asp:Literal ID="litSpinner" runat="server" EnableViewState="false"></asp:Literal>
        </div>

        <div id="message">
          <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
        </div>
      </div>
    </div>
  </form>

  <script>
      (function () {
          var user = document.getElementById('<%= txtUser.ClientID %>');
        var pass = document.getElementById('<%= txtPass.ClientID %>');
        var submitBtn = document.getElementById('<%= btnSubmit.ClientID %>');

        function validate() {
            submitBtn.disabled = !(user.value.trim() && pass.value.trim());
        }

        user.addEventListener('input', validate);
        pass.addEventListener('input', validate);

        // initial validation (handles autofill)
        validate();
    })();
  </script>
</body>
</html>
