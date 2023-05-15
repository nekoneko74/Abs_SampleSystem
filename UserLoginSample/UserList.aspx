<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="UserLoginSample.UserList" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ユーザー一覧</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                <asp:Label ID="LblMessage" runat="server" Text="" Visible="False"></asp:Label>
            </p>
            <h2><asp:Label ID="LblTitle" runat="server" Text="ユーザー一覧"></asp:Label></h2>
            <asp:Label ID="LblWelcome" runat="server" Text="ようこそ"></asp:Label>
            <asp:HyperLink ID="LnkLogout" runat="server" NavigateUrl="./UserLogout.aspx" Text="ログアウト"></asp:HyperLink>
        </div>
        <div>
            <table>
                <tr>
                    <td colspan="2">検索条件</td>
                </tr>
                <tr>
                    <td style="text-align: right;">アカウント名：</td>
                    <td><asp:TextBox ID="TxtAccount" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="text-align: right;">表示名</td>
                    <td><asp:TextBox ID="TxtDisplayName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="text-align: right;">ユーザー種別</td>
                    <td><asp:DropDownList ID="DrLstType" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><asp:CheckBox ID="ChkDelFlg" runat="server" Text="検索結果に削除データを含む" /></td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: right;">
                        <asp:Button ID="BtnSearch" runat="server" OnClick="BtnSearch_Click" Text="検索" />
                        <asp:Button ID="BtnClear" runat="server" OnClick="BtnClear_Click" Text="クリア" />
                    </td>
                </tr>
            </table>
            <p></p>
            <asp:GridView ID="GrdvUserList" runat="server" EmptyDataText="該当するユーザーデータは存在しません" ShowHeaderWhenEmpty="True" AllowPaging="True" OnRowEditing="GrdvUserList_RowEditing" OnRowDeleting="GrdvUserList_RowDeleting" BorderStyle="Solid" OnPageIndexChanging="GrdvUserList_PageIndexChanging" AutoGenerateColumns="False" OnRowDataBound="GrdvUserList_RowDataBound">
                <Columns>
                    <asp:CommandField InsertVisible="False" ShowCancelButton="False" ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="Account" HeaderText="アカウント" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayName" HeaderText="表示名" ReadOnly="True" />
                    <asp:BoundField DataField="Type" HeaderText="ユーザー種別" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="UpdateDate" DataFormatString="{0:yyyy/MM/dd HH:mm:ss}" HeaderText="最終更新日時" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DelFlg" HeaderText="削除データ" ReadOnly="True">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            <p>
                <asp:Button ID="BtnAddNew" runat="server" Text="新規登録" OnClick="BtnAddNew_Click" />
            </p>
        </div>
    </form>
</body>
</html>