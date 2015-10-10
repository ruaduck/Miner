using System.Linq;
using System.Threading;
using ScriptSDK;
using ScriptSDK.API;
using ScriptSDK.Engines;
using ScriptSDK.Gumps;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;

namespace Tidus_Miner
{
    class Travel
    {
        public static bool Recall(uint runebookserial, int bookspot, string recalltype, bool osi)
        {
            Gump runegump;
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            var myItem = new UOEntity(new Serial(runebookserial)); //replace later with ID you want!
            myItem.DoubleClick(); // Open Runebook
            Stealth.Client.Wait(1000);
            if (!osi) { runegump = GetRunebookGump(0x554B87F3, myItem.Serial.Value); } // Choose gump as Runebook Gump
            else { runegump = GetRunebookGump(0x0059, myItem.Serial.Value); }
            var recall = bookspot*6 - 1;
            var sj = bookspot*6 + 1;
            var recallosi = bookspot + 49;
            #region RuneLocations

            if (!osi)
            {
                if (recalltype == "Recall")
                {
                    if (runegump.Serial.Value > 0)
                    {
                        runegump.Click(runegump.Buttons.First(e => e.PacketValue == recall));
                    }
                }
                else if (runegump.Serial.Value > 0)
                {
                    runegump.Click(runegump.Buttons.First(e => e.PacketValue == sj));
                }
            }
            else
            {
                if (recalltype == "Recall")
                {
                    if (runegump.Serial.Value > 0)
                    {
                        runegump.Click(runegump.Buttons.First(e => e.PacketValue == recallosi));
                    }
                }
                else if (runegump.Serial.Value > 0)
                {
                    runegump.Click(runegump.Buttons.First(e => e.PacketValue == sj));
                }
            }
            
            #endregion

            Stealth.Client.Wait(!osi ? 2000 : 3500);
            var loc2 = PlayerMobile.GetPlayer().Location; // LOC after recall
            return loc1 != loc2; // Compare Locs to see if you moved.
        }

        private static Gump GetRunebookGump(uint gumpType, uint runebookSerial)
        {
            if ((GumpHelper.GetGumpIndex(gumpType)) > -1)
            {
                GumpHelper.CloseGump(gumpType, false);
            }

            var runebook = new Item(new Serial(runebookSerial));
            runebook.DoubleClick();
            while ((GumpHelper.GetGumpIndex(gumpType)) < 0)
            {
                Thread.Sleep(50);
            }

            var g = new Gump(GumpHelper.GetGump(gumpType, false));
            return g;
        }
        
        

        public uint SetRunebookId()
        {
            Item runebook = null;
            var runebooks = Scanner.Find<Item>(0x22C5, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var book in runebooks)
            {
                runebook = book;
            }
            Stealth.Client.AddToSystemJournal(string.Format("{0} Runebooks found, {1} Serial of your Runebook", runebooks.Count, runebook));
            return runebook != null ? runebook.Serial.Value : 0;
        }
    }
}
