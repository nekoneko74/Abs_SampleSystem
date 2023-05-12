using System;

namespace UserLoginSample.Model.DTO
{
    /// <summary>
    /// DTO：ユーザーマスタ（user）
    /// </summary>
    public class User
    {
        /// <summary>
        /// ユーザーID（プライマリキー）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ログインアカウント名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// ログインパスワード
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ユーザー表示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// ユーザー種別
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 最終更新ユーザーのID
        /// </summary>
        public int UpdateUser { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool DelFlg { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public User()
        {
            Id = -1;
            Account = string.Empty;
            Password = string.Empty;
            DisplayName = string.Empty;
            Type = UserType.USER;
            UpdateDate = null;
            UpdateUser = -1;
            DelFlg = false;
        }
    }
}