<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDetail.aspx.cs" Inherits="UserLoginSample.UserDetail" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ユーザー詳細</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                <asp:Label ID="LblMessage" runat="server" Text="" Visible="False"></asp:Label>
            </p>
            <h2><asp:Label ID="LblTitle" runat="server" Text="ユーザー詳細"></asp:Label></h2>
            <asp:Label ID="LblWelcome" runat="server" Text="ようこそ"></asp:Label>
            <asp:HyperLink ID="LnkLogout" runat="server" NavigateUrl="./UserLogout.aspx" Text="ログアウト"></asp:HyperLink>
        </div>
        <div>
            <asp:Table ID="TblUserInfo" runat="server">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="ユーザーID："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False" ColumnSpan="2">
                        <asp:Label ID="LblUserId" runat="server" Text="自動採番"></asp:Label>
                        <asp:Label ID="LblDelFlg" runat="server" Text="（削除済み）" Visible="false"></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="ログインアカウント："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:TextBox ID="TxtAccount" runat="server" MaxLength="20" ReadOnly="True"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:RequiredFieldValidator ID="Validator_TxtAccount_Required" runat="server" ControlToValidate="TxtAccount" ErrorMessage="ログインアカウントは必須項目です。" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="Validator_TxtAccount_MinLength" runat="server" ControlToValidate="TxtAccount" ErrorMessage="ログインアカウントは3文字上20文字以内で入力してください。" Display="Dynamic" ValidationExpression=".{3,20}"></asp:RegularExpressionValidator>
                        <asp:RegularExpressionValidator ID="Validator_TxtAccount_RegExp" runat="server" ControlToValidate="TxtAccount" ErrorMessage="ログインアカウントに使用可能な文字は「英文字（大／小）、数字、ハイフン、アンダーバー」のみです。なお先頭の文字は「英文字（大／小）」のみです。" ValidationExpression="[A-Za-z][A-Za-z0-9\-_]*"></asp:RegularExpressionValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="ログインパスワード："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:TextBox ID="TxtPassword" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:RequiredFieldValidator ID="Validator_TxtPassword_Required" runat="server" ControlToValidate="TxtPassword" ErrorMessage="ログインパスワードは必須項目です。" Enabled="false" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="Validator_TxtPassword_RegExp" runat="server" ControlToValidate="TxtPassword" ErrorMessage="ログインパスワードに使用可能な文字は「英大文字／英小文字、数字、記号」のみです。" ValidationExpression="\w*"></asp:RegularExpressionValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="パスワード（確認）："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:TextBox ID="TxtPassConfirm" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:CompareValidator ID="Validator_TxtPassConfirm_Compare" runat="server" ControlToCompare="TxtPassConfirm" ControlToValidate="TxtPassword" ErrorMessage="確認用パスワードが一致しません。"></asp:CompareValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="表示名："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:TextBox ID="TxtDisplayName" runat="server" MaxLength="50"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False">
                        <asp:RequiredFieldValidator ID="Validator_TxtDisplayName_Required" runat="server" ControlToValidate="TxtDisplayName" ErrorMessage="表示名は必須項目です。"></asp:RequiredFieldValidator>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="ユーザー種別："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False" ColumnSpan="2">
                        <asp:RadioButtonList ID="RdlUserType" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="最終更新日時："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False" ColumnSpan="2">
                        <asp:Label ID="LblLastUpdDate" runat="server" Text=""></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" HorizontalAlign="Right" Wrap="False" Text="最終更新者："></asp:TableCell>
                    <asp:TableCell runat="server" Wrap="False" ColumnSpan="2">
                        <asp:Label ID="LblLastUpdUser" runat="server" Text=""></asp:Label>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Button ID="BtnAction" runat="server" Text="登録" OnClick="BtnAction_Click" />
            <asp:Button ID="BtnCancel" runat="server" Text="キャンセル" OnClick="BtnCancel_Click" CausesValidation="False" />
            <asp:HiddenField ID="HidUserId" runat="server" />
            <asp:HiddenField ID="HidDelFlg" runat="server" />
            <asp:HiddenField ID="HidPageMode" runat="server" />
        </div>
    </form>
</body>
</html>