using InhouseBot.Matchmaking;
using System;
using System.Collections.Generic;
using System.Text;

namespace InhouseBot.Tests
{
    class MatchMakerTest
    {
        public static void TestFinder()
        {
            MatchPlayer p0 = new MatchPlayer();
            p0.mmr = 1100;
            MatchPlayer p1 = new MatchPlayer();
            p1.mmr = 1500;
            MatchPlayer p2 = new MatchPlayer();
            p2.mmr = 2250;
            MatchPlayer p3 = new MatchPlayer();
            p3.mmr = 2700;
            MatchPlayer p4 = new MatchPlayer();
            p4.mmr = 4000;
            MatchPlayer p5 = new MatchPlayer();
            p5.mmr = 6500;
            MatchPlayer p6 = new MatchPlayer();
            p6.mmr = 5700;
            MatchPlayer p7 = new MatchPlayer();
            p7.mmr = 5150;
            MatchPlayer p8 = new MatchPlayer();
            p8.mmr = 4860;
            MatchPlayer p9 = new MatchPlayer();
            p9.mmr = 4000;


            MatchMaker.Join(p0);
            MatchMaker.Join(p1);
            MatchMaker.Join(p2);
            MatchMaker.Join(p3);
            MatchMaker.Join(p4);
            MatchMaker.Join(p5);
            MatchMaker.Join(p6);
            MatchMaker.Join(p7);
            MatchMaker.Join(p8);
            MatchMaker.Join(p9);

            MatchMaker.Update();

            return;
        }
    }
}
