using Discord;
using Discord.Commands;
using Discord.WebSocket;
using InhouseBot.Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InhouseBot.Discord
{
    class DiscordBot
    {

        private DiscordSocketClient m_client;
        private CommandHandler m_cmdHandler;

        public async Task StartBot()
        {
            Console.WriteLine("Starting Discord bot...");
            m_client = new DiscordSocketClient();

            XElement discordToken = Program.Document.Element("DiscordToken");

            await m_client.LoginAsync(TokenType.Bot, discordToken.Value);
            await m_client.StartAsync();

            Console.WriteLine("Discord bot logged in.");

            m_cmdHandler = new CommandHandler(m_client, new CommandService());
            await m_cmdHandler.InstallCommandAsync();

            Console.WriteLine("Listening for commands...");
        }

    }
}
