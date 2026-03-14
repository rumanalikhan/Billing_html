<%@ Page Language="C#" AutoEventWireup="true" CodeFile="getstart.aspx.cs" Inherits="getstart" %>
<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
  <meta charset="UTF-8" />
  <title>Bahria Town Karachi</title>
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />

<%--  <style>
    body {margin:0;font-family:Arial, sans-serif;color:#1a1a1a;}
    header {background:#2d2a26;color:white;padding:10px 40px;display:flex;align-items:center;justify-content:space-between;}
    header nav a {color:white;margin:0 10px;text-decoration:none;font-size:14px;}
    header nav a:hover {text-decoration:underline;}
    .hero {background:#f4f4f2;padding:50px 40px;display:flex;align-items:center;justify-content:space-between;}
    .hero-text {max-width:50%;}
    .hero-text h1 {font-size:36px;margin-bottom:10px;}
    .hero-text p {font-size:18px;margin-bottom:20px;}
    .hero-text .aspNetButton {padding:10px 20px;border:none;border-radius:4px;cursor:pointer;font-size:16px;margin-right:10px;}
    .btn-dark {background:black;color:white;}
    .btn-light {background:white;color:black;border:1px solid #000;}
    .hero img {max-width:40%;border-radius:4px;}
    .steps {padding:40px;display:grid;grid-template-columns:repeat(auto-fit, minmax(250px,1fr));gap:20px;}
    .step {border-top:2px solid #000;padding-top:10px;}
    .step h2 {font-size:24px;margin:10px 0;}
    .step p {font-size:14px;line-height:1.5;}
  </style>--%>

  <link rel="stylesheet" type="text/css" href="~/css/style.css" />
</head>
<body>
  <form id="form1" runat="server">
    <header>
    </header>
    <section class="hero">
      <div class="hero-text">
        <h1>Bahria Town Karachi</h1>
        <p>Login to applications or go for complains</p>
        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="aspNetButton btn-dark" OnClick="btnLogin_Click" />
        <asp:Button ID="btnComplains" runat="server" Text="Complains" CssClass="aspNetButton btn-light" OnClick="btnComplains_Click" />
      </div>
    </section>

    <section class="steps">
      <div class="step">
        <h2>1</h2>
        <p><strong>Start Utility Billing System</strong><br />
        You’ll have 6 type of billing applications to access</p>
      </div>
      <div class="step">
        <h2>2</h2>
        <p><strong>Meter management System</strong><br />
        Issue new or replace old meters</p>
      </div>
      <div class="step">
        <h2>3</h2>
        <p><strong>GL Accounting System</strong><br />
        Update your ledgers here or client's account.</p>
      </div>
    </section>
  </form>
</body>
</html>
