using UserLoginSample.Model.DAO;
using UserLoginSample.Model.DTO;
using System;

namespace UserLoginSample
{
    /// <summary>
    /// ユーザーログインページ（UserLogin.aspx）
    /// </summary>
    public partial class UserLogin : WebPageBase
    {
        /// <summary>
        /// 「クリア」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnClear_Click(object sender, EventArgs e)
        {
            // ログインアカウントとパスワードの入力欄をクリアする
            TxtLoginAccount.Text = string.Empty;
            TxtLoginPassword.Text = string.Empty;
        }

        /// <summary>
        /// 「ログイン」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            // 入力されたパラメータを取得する
            string loginAccount = TxtLoginAccount.Text;
            string loginPassword = TxtLoginPassword.Text;
            if (true == String.IsNullOrEmpty(loginAccount) || true == String.IsNullOrEmpty(loginPassword))
            {
                // 入力が不足していることをメッセージ表示する
                LblMessage.Text = Messages.E9002;
                LblMessage.Visible = true;
            }
            else
            {
                // ログイン処理を試みる
                User user = UserDao.SelectByAccount(loginAccount);
                if (user is User)
                {
                    // ログインに成功した
                    if (loginPassword == user.Password)
                    {
                        // セッション情報に「ログイン種別＝ユーザー」と「ユーザーID」を保持する
                        CreateSession(user.Id);

                        // ユーザーメニュー画面に遷移する
                        Response.Redirect("./UserMenu.aspx");
                    }
                    // パスワードが一致しない
                    else
                    {
                        // ログイン処理に失敗したことをメッセージ表示する
                        LblMessage.Text = Messages.E9001;
                        LblMessage.Visible = true;
                    }
                }
                // 指定されたログインアカウントを持つユーザー情報が存在しない
                else
                {
                    // ログイン処理に失敗したことをメッセージ表示する
                    LblMessage.Text = Messages.E9001;
                    LblMessage.Visible = true;
                }
            }
        }
        /// <summary>
        /// Webページが読み込まれた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // メッセージ表示をクリアする
            LblMessage.Text = string.Empty;
            LblMessage.Visible = false;
        }
    }
}