using UserLoginSample.Model.DAO;
using UserLoginSample.Model.DTO;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UserLoginSample;

namespace UserLoginSample
{
    /// <summary>
    /// ユーザー一覧ページ（UserList.aspx）
    /// </summary>
    public partial class UserList : WebPageBase
    {
        /// <summary>
        /// 「新規登録」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnAddNew_Click(object sender, EventArgs e)
        {
            // ユーザー詳細ページを「新規登録モード」で呼び出す
            List<string> paramValues = new List<string>();
            paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, PageMode.ADD }));
            string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
            Response.Redirect(url);
        }

        /// <summary>
        /// 「クリア」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnClear_Click(object sender, EventArgs e)
        {
            // 検索条件をクリアする
            TxtAccount.Text = string.Empty;
            TxtDisplayName.Text = string.Empty;
            DrLstType.SelectedValue = "-1";
            ChkDelFlg.Checked = false;

            // 検索処理を行う
            Search();
        }

        /// <summary>
        /// 「検索」ボタンがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            // 検索処理を行う
            Search();
        }

        /// <summary>
        /// ユーザー一覧グリッドビューでページング処理が行われた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvUserList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // 検索処理を行い、新しいページを表示する
            Search(e.NewPageIndex);
        }

        /// <summary>
        /// ユーザー一覧グリッドビューの行にデータがバインドされる際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvUserList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // 各行の「ユーザー種別」「削除データ」列の表示内容をカスタマイズする
            if (DataControlRowType.DataRow == e.Row.RowType)
            {
                User rowData = (User)e.Row.DataItem;
                e.Row.Cells[3].Text = (UserType.ADMIN == rowData.Type) ? "管理者" : "一般";
                e.Row.Cells[5].Text = (true == rowData.DelFlg) ? "削除" : string.Empty;
            }
        }

        /// <summary>
        /// ユーザー一覧グリッドビューの「削除」リンクがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvUserList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // 選択された行に該当するレコードの「ID値」をパラメータにしてユーザー詳細ページを「削除」モードで呼び出す
            string userId = e.Keys[0].ToString();
            List<string> paramValues = new List<string>();
            paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, PageMode.DELETE }));
            paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, userId }));
            string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
            Response.Redirect(url);
        }

        /// <summary>
        /// ユーザー一覧グリッドビューの「編集」リンクがクリックされた際に呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GrdvUserList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // 選択された行に該当するレコードの「ID値」をパラメータにしてユーザー詳細ページを「編集」モードで呼び出す
            string userId = GrdvUserList.DataKeys[e.NewEditIndex].Values[0].ToString();
            List<string> paramValues = new List<string>();
            paramValues.Add(String.Join("=", new string[] { PageMode.PARAM_NAME, PageMode.EDIT }));
            paramValues.Add(String.Join("=", new string[] { UserDetail.PARAM_NAME_ID, userId }));
            string url = "./UserDetail.aspx?" + String.Join("&", paramValues);
            Response.Redirect(url);
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
                    // 「ユーザー種別」のドロップダウンリストボックスに選択肢を設定する
                    DrLstType.Items.Add(new ListItem("選択してください", "-1"));
                    DrLstType.Items.Add(new ListItem("一般ユーザー", UserType.USER.ToString("d")));
                    DrLstType.Items.Add(new ListItem("管理者", UserType.ADMIN.ToString("d")));

                    // （初期）検索処理を行う
                    Search();
                }
            }
        }

        /// <summary>
        /// ユーザー一覧の検索を行う
        /// </summary>
        /// <param name="pageIndex">ユーザー一覧グリッドビューにバインド（表示）するページの番号</param>
        protected void Search(int pageIndex = 0)
        {
            // 入力されている検索条件を取得する
            string account = (true != String.IsNullOrEmpty(TxtAccount.Text)) ? TxtAccount.Text : null;
            string displayName = (true != String.IsNullOrEmpty(TxtDisplayName.Text)) ? TxtDisplayName.Text : null;
            int? type = null;
            int typeSelect = Convert.ToInt32(DrLstType.SelectedValue);
            if ((int)UserType.ADMIN == typeSelect || (int)UserType.USER == typeSelect)
            {
                type = typeSelect;
            }
            bool incldueDeleted = ChkDelFlg.Checked;

            // ユーザーの一覧を取得してグリッドビューにデータバインドする
            List<User> userList = UserDao.Select(account, type, displayName, incldueDeleted);
            GrdvUserList.DataSource = userList;
            GrdvUserList.PageIndex = pageIndex;
            GrdvUserList.DataKeyNames = new string[] { "Id" };
            GrdvUserList.DataBind();
        }
    }
}