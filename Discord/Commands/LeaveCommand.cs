using Discord.Commands;
using Discord.WebSocket;
using InhouseBot.Matchmaking;
using InhouseBot.Mysql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Discord.Commands
{
    public class LeaveCommand : ModuleBase<SocketCommandContext>
    {

        [Command("leave")]
        [Summary("Leaves the in-house queue")]
        public async Task LeaveDefaultAsync()
        {
            MatchMaker.Leave(Context.User.Id);
            await ReplyAsync($"{Context.User.Username} has left the queue!");
        }
    }
}
