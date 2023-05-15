using UserLoginSample.Model.DAO;
using UserLoginSample.Model.DTO;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UserLoginSample;

namespace UserLoginSample
{
    /// <summary>
    /// ユーザー詳細ページ（UserDetail.aspx）
    /// </summary>
    public partial class UserDetail : WebPageBase
    {
        /// <summary>
        /// 処理対象のユーザーを指定するhttpパラメータ名
        /// </summary>
        public const string PARAM_NAME_ID = "id";

        /// <summary>
        /// 「キャンセル」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            // ユーザー一覧ページに戻る
            Response.Redirect("./UserList.aspx");
        }

        /// <summary>
        /// 「登録／更新／削除／再登録」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAction_Click(object sender, EventArgs e)
        {
            try
            {
                // 処理モードが「削除」モードである場合
                if (PageMode.DELETE == HidPageMode.Value)
                {
                    // 現在表示中のユーザー情報が「削除済み」である場合
                    if ("1" == HidDelFlg.Value)
                    {
                        Undelete();
                    }
                    // 現在表示中のユーザー情報は「削除済み」ではない場合
                    else
                    {
                        Delete();
                    }
                }
                // 処理モードが「編集」モードである場合
                else if (PageMode.EDIT == HidPageMode.Value)
                {
                    Update();
                }
                // 処理モードが「新規登録」モードである
                else if (PageMode.ADD == HidPageMode.Value)
                {
                    Insert();
                }
            }
            catch (Exception ex)
            {
                // 処理が継続できないのでエラー表示後に一覧画面に戻る
                ErrorInfo errInfo = new ErrorInfo();
                errInfo.Message = ex.Message;
                if (null != ex.InnerException)
                {
                    errInfo.Message += "<br>" + ex.InnerException.Message;
                }
                errInfo.LinkUrl = "./UserList.aspx";
                errInfo.LinkDisplayName = "ユーザー一覧に戻る";
                TransferErrorPage(errInfo);
            }
        }

        /// <summary>
        /// ユーザー情報を削除する
        /// </summary>
        /// <exception cref="Exception">ユーザー情報の削除を行うことができなかった</exception>
        protected void Delete()
        {
            try
            {
                try
                {
                    // ログインユーザーが「管理者」でない場合には削除を行わせない
                    if (UserType.ADMIN != LoginUser.Type)
                    {
                        throw new Exception("ユーザー情報の削除には管理者権限が必要です。");
                    }

                    // 処理対象のユーザー情報に削除フラグを設定する
                    int userId = Convert.ToInt32(HidUserId.Value);
                    if (1 == UserDao.Delete(userId, LoginUser.Id))
                    {
                        // 更新されたデータを現在の処理モード（＝削除モード）で開き直す
                        List<string> paramValues = new List<string>();
                        paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, HidPageMode.Value }));
                        paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, userId.ToString() }));
                        string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
                        Response.Redirect(url);
                    }
                    // 処理対象のユーザー情報が存在していない？
                    else
                    {
                        throw new Exception("処理対象のユーザー情報が存在していません。");
                    }
                }
                catch (FormatException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
                catch (OverflowException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ユーザー情報を更新することができません。", ex);
            }
        }

        /// <summary>
        /// ユーザー情報を挿入する
        /// </summary>
        /// <exception cref="Exception">ユーザー情報の挿入を行うことができなかった</exception>
        protected void Insert()
        {
            try
            {
                // 新しいユーザー情報を生成して挿入処理を行う
                User user = new User();
                user.Account = TxtAccount.Text;
                user.Password = TxtPassword.Text;
                user.DisplayName = TxtDisplayName.Text;
                user.Type = (UserType)Enum.Parse(typeof(UserType), RdlUserType.SelectedValue);
                UserDao.Insert(user, LoginUser.Id);

                // 挿入されたデータを「編集モード」で開き直す
                List<string> paramValues = new List<string>();
                paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, PageMode.EDIT }));
                paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, user.Id.ToString() }));
                string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
                Response.Redirect(url);
            }
            catch (Exception ex)
            {
                throw new Exception("ユーザー情報を登録することができません。", ex);
            }
        }

        /// <summary>
        /// 処理対象のユーザー情報を読み込んでWebページ上に表示する
        /// </summary>
        /// <param name="userIdStr">処理対象のユーザーID（文字列）</param>
        /// <exception cref="Exception">処理対象のユーザー情報を読み込むことができなかった</exception>
        protected void LoadUserInfo(string userIdStr)
        {
            try
            {
                try
                {
                    // 処理対象のユーザー情報を取得する
                    int userId = Convert.ToInt32(userIdStr);
                    User user = UserDao.Select(userId);
                    if (null == user)
                    {
                        // 指定されたユーザーIDを持つレコードを読み込むことが出来なかった
                        throw new Exception(userId.ToString() + "は無効なユーザーIDです。");
                    }

                    // 最終更新者のレコードを取得する
                    User lastUpdUser = UserDao.Select(user.UpdateUser);
                    if (null == lastUpdUser)
                    {
                        // 最終更新者のレコードが取得できない！
                        throw new Exception("最終更新者（id=" + user.UpdateUser.ToString() + "）の情報を読み込むことが出来ませんでした。");
                    }

                    // 取得できたユーザーのデータをWebフォーム上に設定する
                    LblUserId.Text = String.Format("({0})", user.Id);
                    LblDelFlg.Visible = (true == user.DelFlg) ? true : false;
                    TxtAccount.Text = user.Account;
                    TxtPassword.Text = string.Empty;
                    TxtPassConfirm.Text = string.Empty;
                    TxtDisplayName.Text = user.DisplayName;
                    RdlUserType.SelectedValue = user.Type.ToString();
                    LblLastUpdDate.Text = String.Format("{0:yyyy/MM/dd HH:mm:ss}", user.UpdateDate);
                    LblLastUpdUser.Text = lastUpdUser.DisplayName;
                    HidDelFlg.Value = (true == user.DelFlg) ? "1" : "0";
                    HidUserId.Value = user.Id.ToString();
                }
                catch (FormatException ex)
                {
                    throw new Exception("無効なユーザーID（" + userIdStr + "）が指定されました。", ex);
                }
                catch (OverflowException ex)
                {
                    throw new Exception("無効なユーザーID（" + userIdStr + "）が指定されました。", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ユーザー情報を読み込むことが出来ませんでした", ex);
            }
        }

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

                if (true != IsPostBack)
                {
                    try
                    {
                        // 「ユーザー種別」の選択用ラジオボタンを用意する
                        RdlUserType.Items.Add(new ListItem("一般ユーザー", UserType.USER.ToString()));
                        RdlUserType.Items.Add(new ListItem("管理者", UserType.ADMIN.ToString()));

                        // 外部からの呼び出しパラメータ（処理モード、処理対象のユーザーID）を取得する
                        PageMode pageMode = new PageMode(Request.Params[PageMode.PARAM_NAME]);
                        HidPageMode.Value = pageMode.Value;
                        string userIdStr = Request.Params[UserDetail.PARAM_NAME_ID];

                        // 削除モードの場合の初期化処理を行う
                        if (PageMode.DELETE == HidPageMode.Value)
                        {
                            // 処理対象のユーザー情報を読み込む
                            LoadUserInfo(userIdStr);

                            // タイトルと処理ボタンの名前を設定する
                            string action = ("1" == HidDelFlg.Value) ? "再登録" : "削除";
                            Title = "ユーザー詳細：" + action;
                            LblTitle.Text = "ユーザー詳細：" + action;
                            BtnAction.Text = action;

                            // 各入力欄を操作不能にする
                            TxtPassword.ReadOnly = true;
                            TxtPassConfirm.ReadOnly = true;
                            TxtDisplayName.ReadOnly = true;
                            RdlUserType.Enabled = false;

                            // 「削除」ボタンはログインユーザーが「管理者」である場合にのみ有効にする
                            // 　⇒ 即ち、ログインユーザーが「管理者」以外の場合に無効にする
                            if ("1" != HidDelFlg.Value && UserType.ADMIN != LoginUser.Type)
                            {
                                BtnAction.Enabled = false;
                            }
                        }
                        // 編集モードの場合の初期化処理を行う
                        else if (PageMode.EDIT == HidPageMode.Value)
                        {
                            // 処理対象のユーザー情報を読み込む
                            LoadUserInfo(userIdStr);

                            // タイトルと処理ボタンの名前を設定する
                            Title = "ユーザー詳細：編集";
                            LblTitle.Text = "ユーザー詳細：編集";
                            BtnAction.Text = "更新";

                            // 既に削除されているデータは操作させない
                            if ("1" == HidDelFlg.Value)
                            {
                                TxtPassword.ReadOnly = true;
                                TxtPassConfirm.ReadOnly = true;
                                TxtDisplayName.ReadOnly = true;
                                RdlUserType.Enabled = false;
                                BtnAction.Enabled = false;
                            }
                        }
                        // 新規登録モードの場合の初期化処理を行う
                        else if (PageMode.ADD == HidPageMode.Value)
                        {
                            // タイトルと処理ボタンの名前を設定する
                            Title = "ユーザー詳細：新規登録";
                            LblTitle.Text = "ユーザー詳細：新規登録";
                            BtnAction.Text = "登録";

                            // 新規登録時にのみ行うこと
                            // ・アカウントを入力可能にする
                            // ・パスワードの必須チェックを有効にする
                            // ・ユーザー種別の初期選択状態を「一般ユーザー」にする
                            // ・最終更新日時／最終更新者の表示行を非表示にする
                            TxtAccount.ReadOnly = false;
                            Validator_TxtPassword_Required.Enabled = true;
                            RdlUserType.SelectedValue = UserType.USER.ToString();
                            TblUserInfo.Rows[6].Visible = false;
                            TblUserInfo.Rows[7].Visible = false;
                        }
                        // 閲覧モードの場合の初期化処理を行う
                        else
                        {
                            // 処理対象のユーザー情報を読み込む
                            LoadUserInfo(userIdStr);

                            // タイトルと処理ボタンの名前を設定する
                            Title = "ユーザー詳細：閲覧";
                            LblTitle.Text = "ユーザー詳細：閲覧";

                            // データは操作させない
                            TxtPassword.ReadOnly = true;
                            TxtPassConfirm.ReadOnly = true;
                            TxtDisplayName.ReadOnly = true;
                            RdlUserType.Enabled = false;
                            BtnAction.Enabled = false;
                            BtnAction.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 処理が継続できないのでエラー表示後に一覧画面に戻る
                        ErrorInfo errInfo = new ErrorInfo();
                        errInfo.Message = ex.Message;
                        if (null != ex.InnerException)
                        {
                            errInfo.Message += "<br>" + ex.InnerException.Message;
                        }
                        errInfo.LinkUrl = "./UserList.aspx";
                        errInfo.LinkDisplayName = "ユーザー一覧に戻る";
                        TransferErrorPage(errInfo);
                    }
                }
            }
        }

        /// <summary>
        /// ユーザー情報を再登録する
        /// </summary>
        /// <exception cref="Exception">ユーザー情報の再登録を行うことができなかった</exception>
        protected void Undelete()
        {
            try
            {
                try
                {
                    // 処理対象のユーザー情報の削除フラグに「0：未削除」を設定する
                    int userId = Convert.ToInt32(HidUserId.Value);
                    if (1 == UserDao.Undelete(userId, LoginUser.Id))
                    {
                        // 挿入されたデータを現在の処理モード（＝削除モード）で開き直す
                        List<string> paramValues = new List<string>();
                        paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, HidPageMode.Value }));
                        paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, userId.ToString() }));
                        string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
                        Response.Redirect(url);
                    }
                    // 処理対象のユーザー情報が存在していない？
                    else
                    {
                        throw new Exception("処理対象のユーザー情報が存在していません。");
                    }
                }
                catch (FormatException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
                catch (OverflowException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ユーザー情報を更新することができません。", ex);
            }
        }

        /// <summary>
        /// ユーザー情報を更新する
        /// </summary>
        /// <exception cref="Exception">ユーザー情報の更新を行うことができなかった</exception>
        protected void Update()
        {
            try
            {
                try
                {
                    // 処理対象のユーザー情報を読み込む
                    int userId = Convert.ToInt32(HidUserId.Value);
                    User user = UserDao.Select(userId);
                    if (null == user)
                    {
                        // 処理対象のユーザー情報を読み込むことが出来なかった
                        throw new Exception("ユーザー情報を読み込むことができませんでした（" + userId.ToString() + "は無効なユーザーIDです）。");
                    }

                    // 削除済みのデータを更新しようとした
                    if (true == user.DelFlg)
                    {
                        throw new Exception("削除済みのユーザー情報を更新することはできません。");
                    }

                    // フォーム上で入力された情報を取得してユーザー情報を更新する
                    if (true != String.IsNullOrEmpty(TxtPassword.Text))
                    {
                        // パスワードは入力されたときにのみ更新を行う
                        user.Password = TxtPassword.Text;
                    }
                    user.DisplayName = TxtDisplayName.Text;
                    user.Type = (UserType)Enum.Parse(typeof(UserType), RdlUserType.SelectedValue);
                    if (1 == UserDao.Update(user, userId, LoginUser.Id))
                    {
                        // 更新されたデータを現在の処理モード（＝編集モード）で開き直す
                        List<string> paramValues = new List<string>();
                        paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, HidPageMode.Value }));
                        paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, user.Id.ToString() }));
                        string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
                        Response.Redirect(url);
                    }
                    // 処理対象のユーザー情報が存在していない？
                    else
                    {
                        throw new Exception("処理対象のユーザー情報が存在していません。");
                    }
                }
                catch (FormatException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
                catch (OverflowException ex)
                {
                    throw new Exception("無効なユーザーID（" + HidUserId.Value + "）が指定されました。", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ユーザー情報を更新することができません。", ex);
            }
        }
    }
}