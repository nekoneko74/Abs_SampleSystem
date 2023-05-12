using UserLoginSample.Model.DAO;
using UserLoginSample.Model.DTO;
using System;
using System.Text;

namespace UserLoginSample
{
    /// <summary>
    /// Webページの基底クラス
    /// </summary>
    /// <remarks>各Webページで共通の機能（ユーザーのログイン／ログアウト／セッション管理）などを提供します</remarks>
    public class WebPageBase : System.Web.UI.Page
    {
        /// <summary>
        /// ログインしているユーザーのIDを格納しているセッション情報の名称
        /// </summary>
        public const string LOGIN_USER_ID = "loginUserId";

        /// <summary>
        /// 現在ログインしているユーザーの情報
        /// </summary>
        protected User LoginUser = null;

        /// <summary>
        /// ユーザーのログイン状況をチェックする
        /// </summary>
        /// <returns>ログインしているユーザーの情報</returns>
        public User CheckSession()
        {
            // 現在ログインしているユーザーの情報をクリアしておく
            LoginUser = null;

            try
            {
                // ユーザーのログイン状況をチェックする
                string loginUserIdStr = (string)Session[WebPageBase.LOGIN_USER_ID];
                if (true != String.IsNullOrEmpty(loginUserIdStr))
                {
                    try
                    {
                        // ログインしているユーザーの情報を取得する
                        int loginUserId = Convert.ToInt32(loginUserIdStr);
                        LoginUser = UserDao.Select(loginUserId);
                        if (null == LoginUser)
                        {
                            // ログインしているとされているユーザーの情報が取得できなかった
                            //  ⇒ ログインしているはずのユーザー情報が削除されている？
                            throw new Exception("ログインユーザーの情報を取得できません。");
                        }
                        else if (true == LoginUser.DelFlg)
                        {
                            // ログインしているはずのユーザー情報が「削除」状態になっている？
                            throw new Exception("ログインユーザーが無効化されています。");
                        }
                    }
                    catch (FormatException ex)
                    {
                        throw new Exception("無効なログインユーザーID（" + loginUserIdStr + "）が設定されています。", ex);
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception("無効なログインユーザーID（" + loginUserIdStr + "）が設定されています。", ex);
                    }
                }
                // ユーザーはログインしていない
                else
                {
                    throw new Exception("未ログイン状態です。");
                }
            }
            catch (Exception ex) 
            {
                // ユーザーログイン画面に遷移する
                StringBuilder errMsgBuilder = new StringBuilder("セッションエラーです。<br>再度ログインしてください。");
                errMsgBuilder.Append(String.Format("<br><br>{0}", ex.Message));
                TransferSessionErrorPage(errMsgBuilder.ToString());
            }

            // ログインしているユーザーの情報を返す
            return LoginUser;
        }

        /// <summary>
        /// 新たなユーザーログイン情報をセッション情報に保存する
        /// </summary>
        /// <param name="loginUserId">ログインしているユーザーのID値</param>
        public void CreateSession(int loginUserId)
        {
            // セッション情報内の情報を一旦クリアする
            Session.RemoveAll();

            // セッション情報に「ログインユーザーID」を保持する
            Session[WebPageBase.LOGIN_USER_ID] = loginUserId.ToString();
        }

        /// <summary>
        /// ユーザーログイン情報を破棄する
        /// </summary>
        public void DestroySession()
        {
            // セッション情報内の情報を全てクリアする
            Session.RemoveAll();
        }

        /// <summary>
        /// エラーページに遷移（フォワード）する
        /// </summary>
        /// <param name="errInfo">エラー情報</param>
        /// <remarks>
        /// このメソッドをtryブロック内で使用しないようにしてください。
        /// 転送元スレッドの実行が中止されてSystem.Threading.ThreadAbortException例外がスローされる場合があり、
        /// この例外がcatchされることで思わぬエラー処理が実行される場合があります。
        /// </remarks>
        public void TransferErrorPage(ErrorInfo errInfo)
        {
            Session[UserErrorPage.ERROR_INFO_NAME] = errInfo;
            Server.Transfer("./UserErrorPage.aspx");
        }

        /// <summary>
        /// セッションエラー：エラーページに遷移（フォワード）する
        /// </summary>
        /// <param name="message">エラーメッセージ文字列</param>
        public void TransferSessionErrorPage(string message)
        {
            // エラーメッセージをセッション情報に格納してエラーページに遷移する
            ErrorInfo errInfo = new ErrorInfo();
            errInfo.Title = "セッションエラー";
            errInfo.Message = message;
            errInfo.LinkUrl = "./UserLogin.aspx";
            errInfo.LinkDisplayName = "ログイン画面に戻る";
            TransferErrorPage(errInfo);
        }

        /// <summary>
        /// システムエラー：例外発生時にエラーページに遷移（フォワード）する
        /// </summary>
        /// <param name="exception">発生している例外オブジェクト</param>
        public void TransferSystemErrorPage(Exception exception)
        {
            // エラーメッセージをセッション情報に格納してエラーページに遷移する
            string errMessage = exception.Message;
            if (null != exception.InnerException)
            {
                errMessage += "<br>" + exception.InnerException.Message;
            }
            TransferErrorPage(new ErrorInfo(errMessage, "システムエラー"));
        }
    }
}