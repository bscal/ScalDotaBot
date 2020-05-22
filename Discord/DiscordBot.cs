using Discord;
using Discord.Commands;
using Discord.WebSocket;
using InhouseBot.Discord.Commands;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using InhouseBot.Matchmaking;

namespace InhouseBot.Discord
{
    class DiscordBot
    {

        private const string STATUS_CHANNEL = "inhouse-status";

        private DiscordSocketClient m_client;
        private CommandHandler m_cmdHandler;
        private System.Timers.Timer m_timer;

        private SocketGuild m_guild;
        private SocketTextChannel m_channel;
        private ulong m_messageId = 0;

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

            m_client.GuildAvailable += OnGuild;

            Console.WriteLine("Listening for commands...");
        }

        private async Task OnGuild(SocketGuild g)
        {
            m_guild = g;
            using var e = m_guild.TextChannels.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Name == STATUS_CHANNEL)
                {
                    m_channel = e.Current;
                }
            }
            Console.WriteLine($"{g.Name} : {m_channel.Name}");

            m_timer = new System.Timers.Timer(10 * 1000);
            m_timer.Elapsed += OnElapsed;
            m_timer.Enabled = true;
            m_timer.AutoReset = true;
            m_timer.Start();

            // Cleans chat.
            var messages = await m_channel.GetMessagesAsync(5).FlattenAsync();
            foreach (IMessage m in messages)
            {
                await m_channel.DeleteMessageAsync(m.Id);
            }
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            await Update();

        }

        private async Task Update()
        {
            //m_guild = m_client.GetGuild(m_guildId);
            //m_channel = m_guild.GetTextChannel(m_channelId);
            Console.WriteLine("Updating status...");
            if (m_messageId != 0)
                await m_channel.DeleteMessageAsync(m_messageId);

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.DarkRed;
            embed.AddField("Players in Queue: ", MatchMaker.QueueSize());

            var rs = await m_channel.SendMessageAsync(embed: embed.Build());
            m_messageId = rs.Id;
        }

    }
}
