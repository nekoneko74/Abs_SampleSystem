<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserLogin.aspx.cs" Inherits="UserLoginSample.UserLogin" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ユーザーログイン</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                <asp:Label ID="LblMessage" runat="server" Text="" Visible="False"></asp:Label>
            </p>
            <h2>ユーザーログイン</h2>
        </div>
        <div>
            <table>
                <tr>
                    <td style="text-align: right;">ログインアカウント：</td>
                    <td><asp:TextBox ID="TxtLoginAccount" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="text-align: right;">パスワード：</td>
                    <td><asp:TextBox ID="TxtLoginPassword" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2" style=" text-align: center;">
                        <asp:Button ID="BtnLogin" runat="server" Text="ログイン" OnClick="BtnLogin_Click" />
                        <asp:Button ID="BtnClear" runat="server" Text="クリア" OnClick="BtnClear_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>