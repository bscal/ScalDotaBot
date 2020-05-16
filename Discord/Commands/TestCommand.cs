using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Discord.Commands
{
    public class TestCommand : ModuleBase<SocketCommandContext>
    {

        [Command("test")]
        [Summary("Test command.")]
        public Task TestAsync([Summary("The command to test")] SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            return ReplyAsync($"{userInfo.Username}");
        }

    }
}
