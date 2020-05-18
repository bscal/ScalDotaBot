using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Mysql
{
    class UserData
    {
        public ulong DiscordId { get; set; }
        public ulong SteamId { get; set; }

        public UserData(ulong discordId)
        {
            DiscordId = discordId;
        }

        public void Create()
        {
            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "INSERT INTO users VALUES (@discord_id, @steam_id)";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                cmd.Parameters.AddWithValue("@steam_id", SteamId);
                Console.WriteLine(cmd.CommandText);
                int rows = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async public Task Fetch(Action<UserData> cb)
        {
            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM users WHERE discord_id = @discord_id";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                var rs = await cmd.ExecuteReaderAsync();
                DiscordId = (ulong)rs.GetInt64(1);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            cb.Invoke(this);
        }

        async public Task<bool> IsDiscordUsed(ulong discordId)
        {
            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM users WHERE discord_id = @discord_id";
                cmd.Parameters.AddWithValue("@discord_id", discordId);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null)
                    return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public void Update()
        {
            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "UPDATE users SET discord_id=@discord_id WHERE steam_id = @steam_id";
                cmd.Parameters.AddWithValue("@steam_id", SteamId);
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                Console.WriteLine(cmd.CommandText);
                int rows = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete()
        {
            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "DELETE FROM usersWHERE discord_id = @discord_id";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                Console.WriteLine(cmd.CommandText);
                int rows = cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
