<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserErrorPage.aspx.cs" Inherits="UserLoginSample.UserErrorPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>エラーが発生しました</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                <asp:Label ID="LblErrorMessage" runat="server" Text="エラーが発生しました<br>（エラー情報が設定されていません）"></asp:Label>
            </p>
            <p>
                <asp:HyperLink ID="LnkToNextPage" runat="server" NavigateUrl="./UserLogin.aspx">トップページへ</asp:HyperLink>
            </p>
        </div>
    </form>
</body>
</html>