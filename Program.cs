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

        public static XElement Document { get; private set; }
        public static Database Database { get; private set; }

        private bool m_isRunning = false;

        static void Main(string[] args)
        {
            Document = XElement.Load("credentials.xml");
            new Program().AsyncMain().GetAwaiter().GetResult();
        }

        async Task AsyncMain()
        {
            m_isRunning = true;

            Database = new Database();
//             Database.Update(123, "tabletest", new Dictionary<string, string>
//             {
//                 { "key1", "value1" },
//                 { "key2", "value2" }
//             });

            DiscordBot discordBot = new DiscordBot();
            await discordBot.StartBot();

            SteamBot steamBot = new SteamBot();
            steamBot.StartSteam();

            while (m_isRunning)
            {
                await Task.Delay(100);
            }


        }
    }
}
