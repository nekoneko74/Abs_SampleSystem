using System;
using UserLoginSample;

namespace UserLoginSample
{
    /// <summary>
    /// ログアウトページ（UserLogout.aspx）
    /// </summary>
    public partial class UserLogout : WebPageBase
    {
        /// <summary>
        /// Webページが読み込まれた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // セッション情報を全て破棄する
            DestroySession();
        }
    }
}