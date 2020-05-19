using Discord;
using Discord.Commands;
using Discord.WebSocket;
using InhouseBot.Mysql;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InhouseBot.Discord.Commands
{

    public class LinkCommand : ModuleBase<SocketCommandContext>
    {

        [Command("link")]
        [Summary("Links a Discord Id to a Steam Id")]
        public Task LinkDefaultAsync()
        {
            return Error();
        }

        [Command("link")]
        [Summary("Links a Discord Id to a Steam Id")]
        public async Task LinkAsync(string idArg)
        {
            var sender = Context.User;
            UserData user = new UserData(sender.Id);
            SteamID steamId = new SteamID();

            if (idArg.Contains("STEAM_"))
                steamId.SetFromString(idArg, EUniverse.Public);
            else if (ulong.TryParse(idArg, out ulong id))
                steamId.SetFromUInt64(id);
            else if (Regex.Match(idArg, @"^\[{1}\b[A-Z]\:{1}[0-9]\b").Success)
                steamId.SetFromSteam3String(idArg);
            else
                await Error();

            user.SteamId = steamId.ConvertToUInt64();

            bool errored = await user.Create();

            var persona = await Program.SteamWebManager.GetUserName(steamId.ConvertToUInt64());
            Console.WriteLine($"{persona}");


            if (errored)
                await ReplyAsync(user.err);
            else
                await ReplyAsync($"Account Linked! {sender.Mention} -> {persona}");
        }

        private Task<IUserMessage> Error()
        {
            return ReplyAsync("Wrong syntax. Use !link steamid. Example SteamIds:\n" +
            "   steamID: STEAM_1:0:23512094\n" +
            "   steamID3: [U:1:47024188]\n" +
            "   steamID64: 76561198007289916");
        }

    }
}
