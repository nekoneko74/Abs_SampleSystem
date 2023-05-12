using System;
using System.Web.UI.WebControls;

namespace UserLoginSample
{
    /// <summary>
    /// メニューページ（UserMenu.aspx）
    /// </summary>
    public partial class UserMenu : WebPageBase
    {
        /// <summary>
        /// Webページが読み込まれた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // ユーザーのログイン状況をチェックする
            CheckSession();
            if (null != LoginUser)
            {
                // セッション維持OK
                LblWelcome.Text = "ようこそ" + LoginUser.DisplayName + "さん";

                // メニューリストを構築する
                BltLstMenuList.Items.Add(new ListItem("ユーザーメンテナンス", "./UserList.aspx"));
            }
        }
    }
}