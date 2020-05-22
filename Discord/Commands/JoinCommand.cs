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
    public class JoinCommand : ModuleBase<SocketCommandContext>
    {


        [Command("join")]
        [Summary("Joins the in-house queue")]
        public async Task JoinDefaultAsync()
        {
            SocketUser sender = Context.User;

            UserData user = new UserData(sender.Id);
            bool rs = await user.Fetch();

            if (rs)
            {
                Console.WriteLine(user.err);
                return;
            }

            MatchPlayer player = new MatchPlayer(user);
            bool didJoin = MatchMaker.Join(player);

            if (!didJoin)
                return;

            await ReplyAsync($"{sender.Username} has joined the queue! Position {player.joinSlot}");
        }
    }
}
