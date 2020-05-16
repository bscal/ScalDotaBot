using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InhouseBot.Steam
{

    class SteamBot
    {

        private DotaClient m_client;
        private bool m_isReady = false;

        public void StartSteam()
        {
            Console.WriteLine("Starting Steam bot...");

            //m_client = DotaClient.Create(
            //    Program.Document.Element("SteamUsername").Value,
            //    Program.Document.Element("SteamPassword").Value,
            //    new DotaClientParams()
            //    );

            //m_client.OnGameFinished += Client_OnGameFinished;

            //m_client.Connect();
            Thread Temp = new Thread(() => {
                m_client = new DotaClient();
                m_client.Start(new string[] {Program.Document.Element("SteamUsername").Value, Program.Document.Element("SteamPassword").Value });
            });
            Temp.Start();

            m_isReady = true;
            Console.WriteLine("Steam started.");
        }

        private void Client_OnGameFinished(DotaGameResult results)
        {
        }

    }
}
