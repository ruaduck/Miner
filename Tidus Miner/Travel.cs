using StealthAPI;
using ScriptSDK.Mobiles;
using System.Threading;

namespace Tidus_Miner
{
    class Travel
    {
        public static bool Recall(int bookspot)
        {
            Stealth.Client.AddToSystemJournal($"Recalling to spot {bookspot} using Recall");
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            Stealth.Client.Wait(1000);
            while (!Miner.Runebook.Entries[bookspot - 1].Recall()) Thread.Sleep(50);
            Stealth.Client.Wait(!Miner.Osi ? 2000 : 3500);
            return loc1 != PlayerMobile.GetPlayer().Location; // Compare Locs to see if you moved.
        }
        public static bool SacredJourney(int bookspot)
        {
            Stealth.Client.AddToSystemJournal($"Recalling to spot {bookspot} using Sacred Journey");
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            Stealth.Client.Wait(1000);
            while (!Miner.Runebook.Entries[bookspot - 1].Sacred()) Thread.Sleep(50);
            Stealth.Client.Wait(!Miner.Osi ? 2000 : 3500);
            return loc1 != PlayerMobile.GetPlayer().Location; // Compare Locs to see if you moved.
        }
        public static bool Scroll(int bookspot)
        {
            Stealth.Client.AddToSystemJournal($"Recalling to spot {bookspot} using Sacred Journey");
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            Stealth.Client.Wait(1000);
            while (!Miner.Runebook.Entries[bookspot - 1].UseScroll()) Thread.Sleep(50);
            Stealth.Client.Wait(!Miner.Osi ? 2000 : 3500);
            return loc1 != PlayerMobile.GetPlayer().Location; // Compare Locs to see if you moved.
        }
    }
}
