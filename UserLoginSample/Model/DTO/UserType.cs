namespace UserLoginSample.Model.DTO
{
    /// <summary>
    /// ユーザー種別
    /// </summary>
    public enum UserType : int
    {
        /// <summary>
        /// 一般ユーザー
        /// </summary>
        USER = 0,

        /// <summary>
        /// 管理者
        /// </summary>
        ADMIN = 9,
    }
}