using System;

namespace UserLoginSample
{
    /// <summary>
    /// 詳細Webページの処理モード
    /// </summary>
    public class PageMode
    {
        /// <summary>
        /// 処理モード：新規登録
        /// </summary>
        public const string ADD = "ADD";

        /// <summary>
        /// 処理モード：削除／再登録
        /// </summary>
        public const string DELETE = "DELETE";

        /// <summary>
        /// 処理モード：編集
        /// </summary>
        public const string EDIT = "EDIT";

        /// <summary>
        /// 処理モード：閲覧（デフォルト）
        /// </summary>
        public const string VIEW = "VIEW";

        /// <summary>
        /// 処理モードを指定するhttpパラメータ名
        /// </summary>
        public const string PARAM_NAME = "mode";

        /// <summary>
        /// 処理モード
        /// </summary>
        protected string modeValue;

        /// <summary>
        /// 処理モード
        /// </summary>
        public string Value
        {
            get
            {
                return modeValue;
            }
            set
            {
                // フィールド「modeValue」に格納される文字列を正規化する
                if (true != String.IsNullOrEmpty(value))
                {
                    switch (value.ToUpper())
                    {
                        case DELETE:
                        case EDIT:
                        case ADD:
                            modeValue = value.ToString();
                            break;
                        case VIEW:
                        default:
                            modeValue = PageMode.VIEW;
                            break;
                    }
                }
                else
                {
                    modeValue = PageMode.VIEW;
                }
            }
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public PageMode()
        {
            modeValue = PageMode.VIEW;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pageMode">初期状態</param>
        public PageMode(string pageMode) : this()
        {
            Value = pageMode;
        }
    }
}