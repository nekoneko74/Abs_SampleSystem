using UserLoginSample.Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace UserLoginSample.Model.DAO
{
    /// <summary>
    /// DAO：ユーザーマスタ（user）
    /// </summary>
    public class UserDao
    {
        /// <summary>
        /// ユーザーマスタ（user）テーブル上に「有効な管理者」が1人以上残っていることを確認する
        /// </summary>
        /// <param name="sqlConnection">データベース接続</param>
        /// <param name="sqlTransaction">実行中のトランザクション</param>
        /// <returns>有効な管理者の数</returns>
        /// <exception cref="Exception">有効な管理者が1名未満になっている</exception>
        protected static int CheckValidAdmin(SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            string countQuery = "SELECT COUNT(*) FROM [user] WHERE [del_flg] != 1 AND [type] = 9";
            SqlCommand sqlCommand = new SqlCommand(countQuery, sqlConnection, sqlTransaction);
            int numAdmin = (int)sqlCommand.ExecuteScalar();
            if (numAdmin <= 0)
            {
                throw new Exception("システムには1名以上の管理者が必要です。");
            }
            return numAdmin;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルから1レコードを論理削除する
        /// </summary>
        /// <param name="id">論理削除するレコードのid値</param>
        /// <param name="updateUserId">レコードを論理削除するユーザーのid値</param>
        /// <returns>論理削除されたレコードの数</returns>
        public static int Delete(int id, int updateUserId)
        {
            int result = 0;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                try
                {
                    // ユーザーマスタ（user）テーブルの1レコードに対して「削除フラグ」を「1：削除済み」に設定するSQL文を実行する
                    string deleteQuery = "UPDATE [user] SET [update_date] = CURRENT_TIMESTAMP, [update_user] = @update_user, [del_flg] = 1 WHERE [id] = @id";
                    SqlCommand sqlCommand = new SqlCommand(deleteQuery, sqlConnection, sqlTransaction);
                    sqlCommand.Parameters.Add("@update_user", SqlDbType.Int).Value = updateUserId;
                    sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    result = sqlCommand.ExecuteNonQuery();

                    // ユーザーマスタ（user）テーブル上に「有効な管理者」が1人以上残っていることを確認する
                    CheckValidAdmin(sqlConnection, sqlTransaction);

                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }

            // 論理削除されたレコードの数を返す
            return result;
        }

        /// <summary>
        /// 結果セットから1レコードをフェッチする
        /// </summary>
        /// <param name="reader">結果セットの読み取りオブジェクト</param>
        /// <returns>ユーザーマスタDTO</returns>
        protected static User Fetch(SqlDataReader reader)
        {
            // 結果セットから1レコードを読み取ってDTOに格納して返す
            User user = new User();
            user.Id = (int)reader["id"];
            user.Account = reader["account"] as string;
            user.Password = reader["password"] as string;
            user.DisplayName = reader["display_name"] as string;
            user.Type = (UserType)Enum.Parse(typeof(UserType), ((byte)reader["type"]).ToString());
            user.UpdateDate = reader["update_date"] as DateTime?;
            user.UpdateUser = (int)reader["update_user"];
            user.DelFlg = (1 == (byte)reader["del_flg"]) ? true : false;
            return user;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルに1レコードを挿入する
        /// </summary>
        /// <param name="user">挿入するレコードのデータ</param>
        /// <param name="createUserId">レコードを挿入するユーザーのid値</param>
        /// <returns>挿入されたレコードの数</returns>
        /// <exception cref="Exception">アカウント名が重複している</exception>
        public static int Insert(User user, int createUserId)
        {
            int result = 0;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // ユーザーマスタ（user）テーブルに1レコードを挿入するSQL文の実行準備を行う
                string insertQuery = "INSERT INTO [user]";
                insertQuery += " ( [account], [password], [display_name], [type], [update_date], [update_user], [del_flg] )";
                insertQuery += " VALUES( @account, @password, @display_name, @type, CURRENT_TIMESTAMP, @update_user, DEFAULT )";
                SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection);

                // 実行準備したSQL文の各SQLパラメータに対してDTOに格納されたフィールドの値を設定する
                sqlCommand.Parameters.Add("@account", SqlDbType.NVarChar).Value = user.Account;
                sqlCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.Password;
                sqlCommand.Parameters.Add("@display_name", SqlDbType.NVarChar).Value = user.DisplayName;
                sqlCommand.Parameters.Add("@type", SqlDbType.TinyInt).Value = user.Type;
                sqlCommand.Parameters.Add("@update_user", SqlDbType.Int).Value = createUserId;

                try
                {
                    // SQL文を実行して「影響を受けた＝挿入された」行数を取得する
                    result = sqlCommand.ExecuteNonQuery();

                    // 新たに生成されたID値を取得してユーザー情報のID値として返却する
                    SqlCommand sqlCommand2 = new SqlCommand("SELECT CAST(@@IDENTITY AS INT) AS [new_id]", sqlConnection);
                    using (SqlDataReader reader = sqlCommand2.ExecuteReader())
                    {
                        if (true == reader.Read())
                        {
                            int ord = reader.GetOrdinal("new_id");
                            user.Id = reader.GetInt32(ord);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // 一意制約違反が発生している…
                    if (2627 == ex.Number)
                    {
                        throw new Exception("同一のログインアカウントが存在しています。", ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // 挿入されたレコードの数を返す
            return result;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルから1レコードを取得する
        /// </summary>
        /// <param name="id">取得するレコードのid値</param>
        /// <returns>取得できたレコードを格納したDTO／取得できない場合にはnull値</returns>
        public static User Select(int id)
        {
            User result = null;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // ユーザーマスタ（user）テーブルから1レコードを取得するSQL文を実行する
                string selectQuery = "SELECT [id], [account], [password], [display_name], [type], [update_date], [update_user], [del_flg] FROM [user] WHERE [id] = @id";
                SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection);
                sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (true == reader.Read())
                    {
                        result = Fetch(reader);
                    }
                }
            }

            // 取得できたレコードを格納したDTOを返す
            return result;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルからレコードのリストを取得する
        /// </summary>
        /// <param name="account">アカウント名で検索する場合に指定する</param>
        /// <param name="type">ユーザー種別で検索する場合に指定する</param>
        /// <param name="displayName">表示名で検索する場合に指定する</param>
        /// <param name="delFlg">削除フラグが「1：削除済み」であるレコードを検索結果に含む場合に指定する</param>
        /// <returns>取得できたレコードのリスト</returns>
        public static List<User> Select(string account = null, int? type = null, string displayName = null, bool delFlg = false)
        {
            List<User> resultList = new List<User>();

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // ユーザーマスタ（user）テーブルからレコードのリストを取得するSQL文の実行準備を行う
                StringBuilder selectQuery = new StringBuilder("SELECT [id], [account], [password], [display_name], [type], [update_date], [update_user], [del_flg] FROM [user]");
                List<string> whereConds = new List<string>();
                if (true != String.IsNullOrEmpty(account))
                {
                    whereConds.Add("[account] LIKE @account");
                }
                if (null != type)
                {
                    whereConds.Add("[type] = @type");
                }
                if (true != String.IsNullOrEmpty(displayName))
                {
                    whereConds.Add("[display_name] LIKE @display_name");
                }
                if (true != delFlg)
                {
                    whereConds.Add("[del_flg] = 0");
                }
                if (0 < whereConds.Count)
                {
                    selectQuery.Append(" WHERE " + String.Join(" AND ", whereConds));
                }
                selectQuery.Append(" ORDER BY [del_flg] ASC, [account] ASC");
                SqlCommand sqlCommand = new SqlCommand(selectQuery.ToString(), sqlConnection);

                // SQLパラメータオブジェクトを生成してパラメータに値を設定する
                if (true != String.IsNullOrEmpty(account))
                {
                    sqlCommand.Parameters.Add("@account", SqlDbType.NVarChar).Value = "%" + account + "%";
                }
                if (null != type)
                {
                    sqlCommand.Parameters.Add("@type", SqlDbType.Int).Value = type;
                }
                if (true != String.IsNullOrEmpty(displayName))
                {
                    sqlCommand.Parameters.Add("@display_name", SqlDbType.NVarChar).Value = "%" + displayName + "%";
                }

                // SQL文を実行して、結果セットから読み取ったレコードをDTOに格納して値返却用のリストに格納する
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (true == reader.Read())
                    {
                        User result = Fetch(reader);
                        resultList.Add(result);
                    }
                }
            }

            // 取得できたレコードのリストを返す
            return resultList;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルから「アカウント名」の一致するレコードを取得する（削除されていないこと！）
        /// </summary>
        /// <param name="account">取得するレコードのアカウント名</param>
        /// <returns>取得できたレコードを格納したDTO／取得できない場合にはnull値</returns>
        public static User SelectByAccount(string account)
        {
            User result = null;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // ユーザーマスタ（user）テーブルから「アカウント名」の一致する有効なレコードを取得するSQL文を実行する
                string selectQuery = "SELECT [id], [account], [password], [display_name], [type], [update_date], [update_user], [del_flg] FROM [user] WHERE [del_flg] != 1 AND [account] = @account";
                SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection);
                sqlCommand.Parameters.Add("@account", SqlDbType.NVarChar).Value = account;
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (true == reader.Read())
                    {
                        result = Fetch(reader);
                    }
                }
            }

            // 取得できたレコードを格納したDTOを返す
            return result;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルの1レコードを論理削除状態から復活させる
        /// </summary>
        /// <param name="id">復活させるレコードのid値</param>
        /// <param name="updateUserId">レコードを復活させるユーザーのid値</param>
        /// <returns>復活させたレコードの数</returns>
        public static int Undelete(int id, int updateUserId)
        {
            int result = 0;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // ユーザーマスタ（user）テーブルの1レコードに対して「削除フラグ」を「0：未削除」に設定するSQL文を実行する
                string deleteQuery = "UPDATE [user] SET [update_date] = CURRENT_TIMESTAMP, [update_user] = @update_user, [del_flg] = 0 WHERE [id] = @id";
                SqlCommand sqlCommand = new SqlCommand(deleteQuery, sqlConnection);
                sqlCommand.Parameters.Add("@update_user", SqlDbType.Int).Value = updateUserId;
                sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
                result = sqlCommand.ExecuteNonQuery();
            }

            // 復活させたレコードの数を返す
            return result;
        }

        /// <summary>
        /// ユーザーマスタ（user）テーブルのレコードを更新する
        /// </summary>
        /// <param name="user">更新するレコードのデータ</param>
        /// <param name="id">更新するレコードのid値</param>
        /// <param name="updateUserId">レコードを更新するユーザーのid値</param>
        /// <returns>更新できたレコードの数</returns>
        public static int Update(User user, int id, int updateUserId)
        {
            int result = 0;

            // データベースに接続する
            string connectionString = ConnectionManager.SqlConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                try
                {
                    // ユーザーマスタ（user）テーブルのレコードを更新するSQL文を実行する準備を行う
                    string updateQuery = "UPDATE [user] SET [password] = @password, [display_name] = @display_name, [type] = @type, [update_date] = CURRENT_TIMESTAMP, [update_user] = @update_user WHERE [id] = @id";
                    SqlCommand sqlCommand = new SqlCommand(updateQuery, sqlConnection, sqlTransaction);

                    // 実行準備したSQL文の各SQLパラメータに対して必要な値を設定する
                    sqlCommand.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.Password;
                    sqlCommand.Parameters.Add("@display_name", SqlDbType.NVarChar).Value = user.DisplayName;
                    sqlCommand.Parameters.Add("@type", SqlDbType.TinyInt).Value = user.Type;
                    sqlCommand.Parameters.Add("@update_user", SqlDbType.Int).Value = updateUserId;
                    sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    // SQL文を実行して「影響を受けた＝更新された」行数を取得する
                    result = sqlCommand.ExecuteNonQuery();

                    // ユーザーマスタ（user）テーブル上に「有効な管理者」が1人以上残っていることを確認する
                    CheckValidAdmin(sqlConnection, sqlTransaction);

                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }

            // 更新できたレコードの数を返す
            return result;
        }
    }
}