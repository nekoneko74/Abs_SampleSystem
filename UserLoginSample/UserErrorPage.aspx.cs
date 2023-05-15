using System;

namespace UserLoginSample
{
    /// <summary>
    /// エラーページ（UserErrorPage.aspx）
    /// </summary>
    public partial class UserErrorPage : System.Web.UI.Page
    {
        /// <summary>
        /// セッション情報内でのエラー情報の名称
        /// </summary>
        public const string ERROR_INFO_NAME = "ErrorInfo";

        /// <summary>
        /// Webページが読み込まれた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // セッション情報からエラー情報を取得する
            ErrorInfo errInfo = Session[UserErrorPage.ERROR_INFO_NAME] as ErrorInfo;
            if (null != errInfo)
            {
                // 設定されたエラー情報から画面を構成する
                if (true != String.IsNullOrEmpty(errInfo.Title))
                {
                    Title = errInfo.Title;
                }
                if (true != String.IsNullOrEmpty(errInfo.Message))
                {
                    LblErrorMessage.Text = errInfo.Message;
                }
                if (true != String.IsNullOrEmpty(errInfo.LinkUrl))
                {
                    LnkToNextPage.NavigateUrl = errInfo.LinkUrl;
                }
                if (true != String.IsNullOrEmpty(errInfo.LinkDisplayName))
                {
                    LnkToNextPage.Text = errInfo.LinkDisplayName;
                }
            }

            // エラー情報をセッション情報から削除する
            Session.Remove(UserErrorPage.ERROR_INFO_NAME);
        }
    }
}