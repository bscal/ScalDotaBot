using InhouseBot.Discord;
using InhouseBot.Discord.Commands;
using InhouseBot.Steam;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InhouseBot
{
    class Program
    {

        public static XElement Document { get; private set; }

        private bool m_isRunning = false;

        static void Main(string[] args)
        {
            Document = XElement.Load("credentials.xml");
            new Program().AsyncMain().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        async Task AsyncMain()
        {
            m_isRunning = true;

            DiscordBot discordBot = new DiscordBot();
            await discordBot.StartBot();

            SteamBot steamBot = new SteamBot();
            steamBot.StartSteam();

            while (m_isRunning)
            {
                await Task.Delay(10);
            }


        }
    }
}
