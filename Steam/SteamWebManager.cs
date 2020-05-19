using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Steam
{

    class SteamWebManager
    {

        private SteamWebInterfaceFactory m_webFactory;
        private SteamUser m_interface;

        public SteamWebManager()
        {
            m_webFactory = new SteamWebInterfaceFactory(Program.Document.Element("SteamAPIKey").Value);

            // this will map to the ISteamUser endpoint
            // note that you have full control over HttpClient lifecycle here
            m_interface = m_webFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());
        }

        public async Task<string> GetUserName(ulong steamId)
        {
            var rs = await m_interface.GetPlayerSummaryAsync(steamId);
            return rs.Data.Nickname;
        }

        public async Task<PlayerSummaryModel> GetUserSummary(ulong steamId)
        {
            var rs = await m_interface.GetPlayerSummaryAsync(steamId);
            return rs.Data;
        }
    }
}
