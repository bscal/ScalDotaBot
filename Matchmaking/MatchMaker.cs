using InhouseBot.Mysql;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Matchmaking
{

    public static class MatchMaker
    {
        private const int NEEDED_COUNT = 10;

        private readonly static List<MatchPlayer> m_queue = new List<MatchPlayer>();

        public static MatchSettings m_match;

        public static void Update()
        {
            Console.WriteLine(m_queue.Count);
            if (m_queue.Count >= NEEDED_COUNT)
            {
                Console.WriteLine("2222");

                m_match = new MatchSettings();
                m_match.players = m_queue.GetRange(0, NEEDED_COUNT);

                bool found = FindBestTeam();
                if (!found) return;
                StartMatch();
            }
        }

        private static bool FindBestTeam()
        {
            Random rand = new Random();


            MatchPlayer p;
            int iterations = 0;
            int delta = 0;
            int minDeltaCount = 0;
            List<MatchPlayer> rTemp;
            List<MatchPlayer> dTemp;
            for (int i = 0; i < m_match.players.Count; i++)
            {
                p = m_match.players[i];
                var team = rand.Next(0, 1);
                if (team == 0 && m_match.radiant.Count < 5)
                {
                    m_match.radiant.Add(p);
                    m_match.radMMR += p.mmr;
                }
                else
                {
                    m_match.dire.Add(p);
                    m_match.direMMR += p.mmr;
                }
            }
            delta = Math.Abs(m_match.radMMR - m_match.direMMR);


            while (true)
            {
                rTemp = m_match.radiant;
                dTemp = m_match.dire;

                var i = rand.Next(0, 4);
                var j = rand.Next(0, 4);

                m_match.radMMR = m_match.radMMR - rTemp[i].mmr + dTemp[j].mmr;
                m_match.direMMR = m_match.radMMR - dTemp[j].mmr + rTemp[i].mmr;

                int temp = Math.Abs(m_match.radMMR - m_match.direMMR);

                Console.WriteLine($"Update: {m_match.radMMR} - {m_match.direMMR} | {temp}");
                m_match.radiant.ForEach((p) => { Console.Write($"{p.mmr} "); });
                m_match.dire.ForEach((p) => { Console.Write($"{p.mmr} "); });
                Console.WriteLine();

                if (temp < delta)
                {
                    delta = temp;
                    Console.WriteLine($"New Delta: {delta}");
                    MatchPlayer tr = rTemp[i];
                    MatchPlayer td = dTemp[j];
                    m_match.radiant[i] = td;
                    m_match.dire[j] = tr;
                    minDeltaCount = 0;
                }
                else
                {
                    m_match.radMMR = m_match.radMMR + rTemp[i].mmr - dTemp[j].mmr;
                    m_match.direMMR = m_match.radMMR + dTemp[j].mmr - rTemp[i].mmr;
                    if (iterations > 24)
                        minDeltaCount++;
                }

                iterations++;

                if (iterations > 49)
                    break;
                else if (minDeltaCount > 5)
                    break;
            }
            Console.WriteLine($"Final: {m_match.radMMR} - {m_match.direMMR} | Diff: {delta} | Iters: {iterations}");
            return true;
        }

        public static void StartMatch()
        {
            m_queue.RemoveRange(0, NEEDED_COUNT);

        }

        public static bool Join(MatchPlayer p)
        {
            m_queue.Add(p);
            p.joinSlot = m_queue.Count;
            return true;
        }

        public static void Leave(ulong discordId)
        {
            for (int i = 0; i < m_queue.Count; i++)
            {
                MatchPlayer p = m_queue[i];
                if (p.data.DiscordId == discordId)
                {
                    m_queue.RemoveAt(i);
                    break;
                }
            }
        }

        public static int QueueSize()
        {
            return m_queue.Count;
        }

    }

    public class MatchSettings
    {
        public List<MatchPlayer> players;
        public List<MatchPlayer> radiant = new List<MatchPlayer>();
        public int radMMR;
        public List<MatchPlayer> dire = new List<MatchPlayer>();
        public int direMMR;
    }

    public class MatchPlayer
    {
        public UserData data;
        public DateTime entered;
        public int mmr;

        public int joinSlot;

        public MatchPlayer() { }

        public MatchPlayer(UserData data)
        {
            this.data = data;
            this.entered = DateTime.Now;
        }
    }

    public class MMRComparer : IComparer<MatchPlayer>
    {
        public int Compare([AllowNull] MatchPlayer x, [AllowNull] MatchPlayer y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            return (x.mmr == y.mmr) ? 0 : (x.mmr > y.mmr) ? 1 : -1;
        }
    }
}
