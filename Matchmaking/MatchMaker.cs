using InhouseBot.Mysql;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace InhouseBot.Matchmaking
{

    public class MatchMaker
    {
        private const int NEEDED_COUNT = 10;

        private readonly static List<MatchPlayer> m_queue = new List<MatchPlayer>();

        private static bool m_makingMatch;
        public static MatchSettings m_match;

        private MatchMaker() { }

        public static void Update()
        {
            Console.WriteLine(m_queue.Count);
            if (m_queue.Count >= NEEDED_COUNT)
            {
                Console.WriteLine("2222");
                m_makingMatch = true;

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
            int rMmrTemp;
            int dMmmrTemp;
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
                m_match.radMMR -= rTemp[i].mmr;
                m_match.direMMR += rTemp[i].mmr;
                m_match.direMMR -= dTemp[j].mmr;
                m_match.radMMR += dTemp[j].mmr;
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
                    m_match.radMMR += rTemp[i].mmr;
                    m_match.direMMR -= rTemp[i].mmr;
                    m_match.direMMR += dTemp[j].mmr;
                    m_match.radMMR -= dTemp[j].mmr;
                    if (iterations > 24)
                        minDeltaCount++;
                }

                iterations++;

                if (iterations > 50)
                    break;
                else if (minDeltaCount > 5)
                    break;
            }
            Console.WriteLine($"Final: {m_match.radMMR} - {m_match.direMMR} | Diff: {delta} | Iters: {iterations}");
            return true;
        }

        public static void StartMatch()
        {
            m_makingMatch = false;

            m_queue.RemoveRange(0, NEEDED_COUNT);

        }

        public static bool Join(MatchPlayer p)
        {
            m_queue.Add(p);

            return true;
        }

        public static void Leave(MatchPlayer p)
        {
            m_queue.Remove(p);
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

    public struct MatchPlayer
    {
        public UserData data;
        public DateTime entered;
        public int mmr;

        public override bool Equals(object obj)
        {
            if (this == null || obj == null) return false;

            if (GetType() != typeof(MatchPlayer) || obj.GetType() != typeof(MatchPlayer)) return false;

            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return mmr.GetHashCode();
        }

        public static bool operator ==(MatchPlayer left, MatchPlayer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MatchPlayer left, MatchPlayer right)
        {
            return !(left == right);
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
