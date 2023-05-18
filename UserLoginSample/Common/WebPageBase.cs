using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UserLoginSample.Model.DAO;
using UserLoginSample.Model.DTO;

namespace UserLoginSample
{
    /// <summary>
    /// Webページの基底クラス
    /// </summary>
    /// <remarks>各Webページで共通の機能（ユーザーのログイン／ログアウト／セッション管理）などを提供します</remarks>
    public class WebPageBase : System.Web.UI.Page
    {
        /// <summary>
        /// デバッグ出力への出力を行わないリクエストパラメータ名を列挙する
        /// </summary>
        protected static string[] excludeParamNames = { "ASP.NET_SessionId", "ALL_HTTP", "HTTP_ACCEPT", 
                                                        "HTTP_UPGRADE_INSECURE_REQUESTS", "HTTP_SEC_FETCH_DEST", "HTTP_SEC_FETCH_MODE", "HTTP_SEC_FETCH_SITE", "HTTP_SEC_FETCH_USER", "ALL_RAW", "APPL_MD_PATH", "APPL_PHYSICAL_PATH",
                                                        "AUTH_TYPE", "AUTH_USER", "AUTH_PASSWORD", "LOGON_USER", "REMOTE_USER", "CERT_COOKIE", "CERT_FLAGS", "CERT_ISSUER", "CERT_KEYSIZE", "CERT_SECRETKEYSIZE", "CERT_SERIALNUMBER",
                                                        "CERT_SERVER_ISSUER", "CERT_SERVER_SUBJECT", "CERT_SUBJECT", "CONTENT_LENGTH", "CONTENT_TYPE", "GATEWAY_INTERFACE", "HTTPS", "HTTPS_KEYSIZE", "HTTPS_SECRETKEYSIZE",
                                                        "HTTPS_SERVER_ISSUER", "HTTPS_SERVER_SUBJECT", "INSTANCE_ID", "INSTANCE_META_PATH", "LOCAL_ADDR", "PATH_INFO", "PATH_TRANSLATED", "QUERY_STRING", "REMOTE_ADDR", "REMOTE_HOST",
                                                        "REMOTE_PORT", "REQUEST_METHOD", "SCRIPT_NAME", "SERVER_NAME", "SERVER_PORT", "SERVER_PORT_SECURE", "SERVER_PROTOCOL", "SERVER_SOFTWARE", "URL", "HTTP_CONNECTION", "HTTP_ACCEPT",
                                                        "HTTP_ACCEPT_ENCODING", "HTTP_ACCEPT_LANGUAGE", "HTTP_COOKIE", "HTTP_CONTENT_LENGTH", "HTTP_CONTENT_TYPE", "HTTP_ORIGIN", "HTTP_HOST", "HTTP_REFERER", "HTTP_TE", "HTTP_USER_AGENT", 
                                                        "HTTP_UPGRADE_INSECURE_REQUESTS", "HTTP_SEC_FETCH_DEST", "HTTP_SEC_FETCH_MODE", "HTTP_SEC_FETCH_SITE", "HTTP_SEC_FETCH_USER",
                                                        "__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION", "__EVENTTARGET", "__EVENTARGUMENT" };

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
        /// 【ユーティリティ】オブジェクトの保持している値を文字列化して取得する
        /// </summary>
        /// <param name="instance">処理対象のオブジェクト</param>
        /// <returns>オブジェクトの保持している値を文字列化したもの</returns>
        public static string GetValueString(object instance)
        {
            string retStr = string.Empty;

            // 処理対象のオブジェクトが存在していない（nullである）
            if (null == instance)
            {
                retStr = "null";
            }
            // 処理対象の何らかのオブジェクトが存在している
            else
            {
                // 処理対象のオブジェクトの型情報を取得する
                Type type = instance.GetType();

                // 基本型／列挙型／文字列型である場合
                if (true == type.IsPrimitive || true == type.IsEnum || instance is string)
                {
                    retStr = String.Format("({0}) {1}", type.Name, instance);
                }
                // 配列／コレクションである場合
                else if (true == type.IsArray || instance is IEnumerable)
                {
                    // 配列／コレクションの各要素を文字列化する
                    List<string> arrayMembers = new List<string>();
                    int index = 0;
                    foreach (object inner in (IEnumerable)instance)
                    {
                        arrayMembers.Add(String.Format("{0} : {1}", index, GetValueString(inner)));
                        index++;
                    }
                    retStr = String.Format("({0}) [ {1} ]", type.Name, String.Join(", ", arrayMembers));
                }
                // オブジェクトである場合
                else if (instance is object)
                {
                    // オブジェクトのメンバ／プロパティを文字列化する
                    List<string> objectMembers = new List<string>();
                    foreach (FieldInfo fieldInfo in type.GetRuntimeFields())
                    {
                        // 自動実装プロパティ用の隠しメンバは除外する
                        Regex regExp = new Regex("\\A<.+>k__BackingField\\z");
                        if (true == regExp.IsMatch(fieldInfo.Name))
                        {
                            continue;
                        }
                        else
                        {
                            object value = fieldInfo.GetValue(instance);
                            objectMembers.Add(String.Format("{0} : {1}", fieldInfo.Name, GetValueString(value)));
                        }
                    }
                    foreach (PropertyInfo propInfo in type.GetProperties())
                    {
                        object value = propInfo.GetValue(instance);
                        objectMembers.Add(String.Format("{0} : {1}", propInfo.Name, GetValueString(value)));
                    }
                    retStr = String.Format("({0}) {{ {1} }}", type.Name, String.Join(", ", objectMembers));
                }
                else
                {
                    retStr = String.Format("({0}) {1}", type.Name, instance);
                }
            }

            // オブジェクトの保持している値を文字列化したものを返す
            return retStr;
        }

        /// <summary>
        /// ロードイベント前のポストバックデータが全て読み込まれた段階で呼び出される
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            string methodName = String.Format("{0}::{1}", GetType().FullName, MethodBase.GetCurrentMethod().Name);
            string startMsg = String.Format(">>> {0} {1} ********************************************", "START", methodName);
            string endMsg = String.Format("<<< {0} {1} **********************************************", "END", methodName);
            Debug.WriteLine(startMsg);

            // デバッグ出力にプログラムの呼び出し情報を出力する
            Debug.WriteLine(String.Format("Program:{0}", this.AppRelativeVirtualPath));
            if (null != PreviousPage)
            {
                // フォワード元
                Debug.WriteLine(String.Format("Transfer by {0}", PreviousPage.AppRelativeVirtualPath));
            }
            else
            {
                // リクエストメソッド／リファラ
                Debug.WriteLine(String.Format("Method:{0}", Request.Params["REQUEST_METHOD"]));
                Debug.WriteLine(String.Format("Referer:{0}", Request.Params["HTTP_REFERER"]));
            }

            // クッキーの内容
            Debug.WriteLine(String.Format("Cookie:{0}", Request.Params["HTTP_COOKIE"]));

            // セッション情報
            Debug.WriteLine("*** Session ***");
            foreach (string key in Session.Keys)
            {
                Debug.WriteLine(String.Format("{0}:{1}", key, GetValueString(Session[key])));
            }

            // リクエストパラメータ
            Debug.WriteLine("*** Parameter ***");
            foreach (string key in Request.Params.AllKeys)
            {
                if (-1 == Array.IndexOf(WebPageBase.excludeParamNames, key))
                {
                    Debug.WriteLine(String.Format("{0}:{1}", key, GetValueString(Request.Params[key])));
                }
            }

            Debug.WriteLine(endMsg);
        }

        /// <summary>
        /// ページの処理が完了してメモリ上からアンロードされる際に呼び出される
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            string methodName = String.Format("{0}::{1}", GetType().FullName, MethodBase.GetCurrentMethod().Name);
            string startMsg = String.Format(">>> {0} {1} ********************************************", "START", methodName);
            string endMsg = String.Format("<<< {0} {1} **********************************************", "END", methodName);
            Debug.WriteLine(startMsg);

            // セッション情報
            Debug.WriteLine("*** Session ***");
            foreach (string key in Session.Keys)
            {
                Debug.WriteLine(String.Format("{0}:{1}", key, GetValueString(Session[key])));
            }

            Debug.WriteLine(endMsg);
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