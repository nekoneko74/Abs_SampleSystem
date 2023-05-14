<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserMenu.aspx.cs" Inherits="UserLoginSample.UserMenu" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>メニュー</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                <asp:Label ID="LblMessage" runat="server" Text="" Visible="False"></asp:Label>
            </p>
            <h2><asp:Label ID="LblTitle" runat="server" Text="メニュー"></asp:Label></h2>
            <asp:Label ID="LblWelcome" runat="server" Text="ようこそ"></asp:Label>
            <asp:HyperLink ID="LnkLogout" runat="server" NavigateUrl="./UserLogout.aspx" Text="ログアウト"></asp:HyperLink>
        </div>
        <div>
            <asp:BulletedList ID="BltLstMenuList" runat="server" BulletStyle="Square" DisplayMode="HyperLink"></asp:BulletedList>
        </div>
    </form>
</body>
</html>