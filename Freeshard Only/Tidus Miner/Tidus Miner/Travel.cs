using System;
using System.Linq;
using ScriptSDK.Gumps;
using StealthAPI;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;

namespace Tidus_Miner
{
    class Travel
    {

        public static bool Recall(Item runebookserial, int bookspot, string recalltype, bool osi)
        {
            var RUOconfig = new RuneBookConfig()
            {
                ScrollOffset = 2,
                DropOffset = 3,
                DefaultOffset = 4,
                RecallOffset = 5,
                GateOffset = 6,
                SacredOffset = 7,
                Jumper = 6
            };
            var OSIconfig = new RuneBookConfig()
            {
                ScrollOffset = 10,
                DropOffset = 200,
                DefaultOffset = 300,
                RecallOffset = 50,
                GateOffset = 100,
                SacredOffset = 75,
                Jumper = 1
            };
            Runebook RUOrb = new Runebook(runebookserial.Serial.Value, RUOconfig);
            Runebook OSIrb = new Runebook(runebookserial.Serial.Value, OSIconfig, "OSI");
            Stealth.Client.AddToSystemJournal(string.Format("Recalling to spot {0} using {1}", bookspot, recalltype));
            //Gump runegump;
            runebookserial.DoubleClick();
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            //runebookserial.DoubleClick(); // Open Runebook
            Stealth.Client.Wait(1000);
            Gump g;
            if (osi)
            {
                g = Gump.GetGump(0x0059); //OSI
                if (g == null)
                {
                    Stealth.Client.AddToSystemJournal("Gump is Null");
                }
                else
                {
                    foreach (var e in g.Buttons)
                    {
                        if (!e.PacketValue.Equals(OSIconfig.RecallOffset + bookspot-1) && !e.Graphic.Released.Equals(2103) &&
                            !e.Graphic.Pressed.Equals(2104)) continue;
                        if (recalltype == "Recall")
                        {
                            Stealth.Client.AddToSystemJournal(String.Format("{0} is my packet value",OSIconfig.RecallOffset + (bookspot-1)));
                            var recallButton = g.Buttons.First(i => i.PacketValue.Equals(OSIconfig.RecallOffset + (bookspot-1)));
                            recallButton.Click();
                            break;
                        }
                    }
                }
            }
            else
            {
                g = Gump.GetGump(0x554B87F3); //Freeshard
                if (g == null)
                {
                    Stealth.Client.AddToSystemJournal("Gump is Null");
                }
                else
                {
                    int go;
                    go = RUOconfig.RecallOffset + ((bookspot - 1) * RUOconfig.Jumper);
                    foreach (var e in g.Buttons)
                    {                       
                        if (!e.PacketValue.Equals(go) || !e.Graphic.Released.Equals(2103) ||
                            !e.Graphic.Pressed.Equals(2104)) continue;
                        if (recalltype == "Recall")
                        {
                            var recallButton =
                                g.Buttons.First(i => i.PacketValue.Equals(go));
                            recallButton.Click();
                        }
                        else
                        {

                        }
                    }
                }
            } 
            Stealth.Client.Wait(!osi ? 2000 : 3500);
            var loc2 = PlayerMobile.GetPlayer().Location; // LOC after recall
            return loc1 != loc2; // Compare Locs to see if you moved.
        }
    }
}
