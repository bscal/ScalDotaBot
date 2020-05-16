using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Discord.Commands
{
    class CommandHandler
    {
        private readonly DiscordSocketClient m_client;
        private readonly CommandService m_commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            m_client = client;
            m_commands = commands;
        }

        public async Task InstallCommandAsync()
        {
            // Hook the MessageReceived event into our command handler
            m_client.MessageReceived += HandleCommandAsync;


            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await m_commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage msgParam)
        {
            // Don't process the command if it was a system message
            var message = msgParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(m_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(m_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await m_commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            // if (!result.IsSuccess)
            // await context.Channel.SendMessageAsync(result.ErrorReason);
            return;
        }
    }
}
