using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Mysql
{
    class Database
    {

        MySqlConnection conn;

        public Database()
        {
            Console.WriteLine("Connecting database...");
            Console.WriteLine(Program.Document.Element("Mysql").Value);
            try
            {
                conn = new MySqlConnection {
                    ConnectionString = Program.Document.Element("Mysql").Value
                };
                conn.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
            }
            if (conn.Ping())
                Console.WriteLine("Successfully connected to database.");
        }

        async public Task Fetch(ulong steamid, Action<UserData> callback)
        {
            string sql = "SELECT * FROM users WHERE steamid = @steamid";
            MySqlCommand cmd = new MySqlCommand(sql);
            cmd.Parameters.AddWithValue("@steamid", steamid);

            var result = await cmd.ExecuteReaderAsync();

            if (!result.HasRows) return;

            UserData user = new UserData();
            user.steamId = (ulong)result.GetInt64(0);
            user.discordId = (ulong)result.GetInt64(1);

            callback.Invoke(user);
        }

        public void Update(ulong steamid, string table, Dictionary<string, string> values)
        {
            string sql = "UPDATE #table SET #values WHERE steamid = @steamid";
            sql = sql.Replace("#table", table);
            sql = sql.Replace("#values", SQLFormatDict(values));
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = MySqlHelper.EscapeString(sql);
                cmd.Parameters.AddWithValue("@steamid", steamid);
                Console.WriteLine(cmd.CommandText);
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Create()
        {

        }

        public void Remove()
        {

        }
        public void Close()
        {
            conn.Close();
        }

        public string Escape(string s)
        {
            return MySqlHelper.EscapeString(s);
        }

        public string SQLFormatDict(Dictionary<string, string> values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in values)
            {
                sb.Append(p.Key);
                sb.Append("=");
                sb.Append(p.Value);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
