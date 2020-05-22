using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Mysql
{
    public class UserData
    {
        public ulong DiscordId { get; set; }
        public ulong SteamId { get; set; }

        public string err;

        public UserData(ulong discordId)
        {
            DiscordId = discordId;
        }

        /// <inheritdoc cref="Create(ulong)"/>
        public async Task<bool> Create()
        {
            return await Create(SteamId);
        }

        /// <summary>
        /// Creates a new user in the database <br></br>
        /// returns true if an error occurred. Will set UserData.err with error message.
        /// </summary>
        public async Task<bool> Create(ulong steamId)
        {
            SteamId = steamId;

            if (IsValid())
                return Error("SteamId is not set.");

            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "INSERT INTO users (discord_id, steam_id) VALUES (@discord_id, @steam_id)";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                cmd.Parameters.AddWithValue("@steam_id", SteamId);
                Console.WriteLine(cmd.CommandText);
                int rows = await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.Code == 1062)
                    return Error("Duplicate ID in use.");
                return Error("Could not link account.");
            }
            return false;
        }

        /// <summary>
        /// Queries database for a steam id linked to the given UserData's discord id
        /// </summary>
        public async Task<bool> Fetch()
        {
            if (IsValid())
                return Error("UserData is not valid.");

            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "SELECT steam_id FROM users WHERE discord_id = @discord_id";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                var rs = await cmd.ExecuteReaderAsync();
                await rs.ReadAsync();
                SteamId = (ulong)rs.GetInt64(0);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }
            return false;
        }

        public void Update()
        {
            if (IsValid())
                Console.WriteLine("UserData is not valid");

            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "UPDATE users SET discord_id=@discord_id, steam_id = @steam_id WHERE steam_id = @steam_id";
                cmd.Parameters.AddWithValue("@steam_id", SteamId);
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete()
        {
            if (DiscordId == 0)
                Console.WriteLine("UserData is not valid");

            try
            {
                MySqlCommand cmd = Program.Database.Conn.CreateCommand();
                cmd.CommandText = "DELETE FROM users WHERE discord_id = @discord_id";
                cmd.Parameters.AddWithValue("@discord_id", DiscordId);
                Console.WriteLine(cmd.CommandText);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool IsValid()
        {
            return DiscordId != 0 && SteamId != 0;
        }

        private bool Error(string err)
        {
            this.err = err;
            return true;
        }
    }
}
