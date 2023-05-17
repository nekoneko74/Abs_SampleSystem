using System.Text;

namespace UserLoginSample
{
    /// <summary> 
    /// エラーページ（UserErrorPage.aspx）の表示内容
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// エラーページのタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 遷移先WebページのURL
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 遷移先Webページの名称
        /// </summary>
        public string LinkDisplayName { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ErrorInfo()
        {
            Message = string.Empty;
            Title = string.Empty;
            LinkUrl = string.Empty;
            LinkDisplayName = string.Empty;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public ErrorInfo(string message) : this()
        {
            Message = message;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="title">エラーページのタイトル</param>
        public ErrorInfo(string message, string title) : this()
        {
            Message = message;
            Title = title;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="title">エラーページのタイトル</param>
        /// <param name="linkUrl">エラーページからさらに遷移する先のWebページのURL</param>
        public ErrorInfo(string message, string title, string linkUrl) : this()
        {
            Message = message;
            Title = title;
            LinkUrl = linkUrl;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="title">エラーページのタイトル</param>
        /// <param name="linkUrl">エラーページからさらに遷移する先のWebページのURL</param>
        /// <param name="linkDisplayName">エラーページからさらに遷移する先のWebページの名称</param>
        public ErrorInfo(string message, string title, string linkUrl, string linkDisplayName) : this()
        {
            Message = message;
            Title = title;
            LinkUrl = linkUrl;
            LinkDisplayName = linkDisplayName;
        }

        /// <summary>
        /// オブジェクトを表す文字列を取得する
        /// </summary>
        /// <returns>オブジェクトを表す文字列</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<<<");
            stringBuilder.AppendLine($" Message:{Message}");
            stringBuilder.AppendLine($" Title:{Title}");
            stringBuilder.AppendLine($" LinkUrl:{LinkUrl}");
            stringBuilder.AppendLine($" LinkDisplayName:{LinkDisplayName}");
            stringBuilder.Append(">>>");
            return stringBuilder.ToString();
        }
    }
}