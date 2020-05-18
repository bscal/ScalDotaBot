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

        public MySqlConnection Conn { get; private set; }

        public Database()
        {
            Console.WriteLine("Connecting database...");
            Console.WriteLine(Program.Document.Element("Mysql").Value);
            try
            {
                Conn = new MySqlConnection {
                    ConnectionString = Program.Document.Element("Mysql").Value
                };
                Conn.Open();
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
            Console.WriteLine("Successfully connected to database.");
        }

        public static string SQLFormatDict(Dictionary<string, string> values)
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
