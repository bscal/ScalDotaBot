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
        public DotaClient Client { get { return m_client; } }

    public void StartSteam()
        {
            Console.WriteLine("Starting Steam bot...");

            Thread Temp = new Thread(() => {
                m_client = new DotaClient();
                m_client.Start(new string[] { Program.Document.Element("SteamUsername").Value, Program.Document.Element("SteamPassword").Value });
            });
            Temp.Start();
        }

    }
}
