namespace UserLoginSample
{
    /// <summary>
    /// データベース接続文字列を管理するクラス
    /// </summary>
    internal class ConnectionManager
    {
        /// <summary>
        /// データベース接続文字列
        /// </summary>
        public static readonly string SqlConnectionString = @"Data Source=localhost;Initial Catalog=SampleSystem;Integrated Security=False; User ID=sa; Password=himitu;";
    }
}