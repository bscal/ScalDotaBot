using InhouseBot.Discord;
using InhouseBot.Discord.Commands;
using InhouseBot.Mysql;
using InhouseBot.Steam;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InhouseBot
{
    class Program
    {

        public static XElement Document { get; } = XElement.Load("credentials.xml");
        public static Database Database { get; private set; }
        public static SteamBot SteamBot { get; private set; }
        public static DiscordBot DiscordBot { get; private set; }
        public static SteamWebManager SteamWebManager { get; private set; }

        private bool m_isRunning = false;

        static void Main(string[] args)
        {
            new Program().AsyncMain().GetAwaiter().GetResult();
        }

        async Task AsyncMain()
        {
            m_isRunning = true;

            Database = new Database();

            DiscordBot = new DiscordBot();
            await DiscordBot.StartBot();

            SteamWebManager = new SteamWebManager();

            SteamBot = new SteamBot();
            SteamBot.StartSteam();

            while (m_isRunning)
            {
                await Task.Delay(100);
            }
        }
    }
}
