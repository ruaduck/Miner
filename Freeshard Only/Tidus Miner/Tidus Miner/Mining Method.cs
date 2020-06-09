using System;
using System.Linq;
using ScriptSDK;
using ScriptSDK.Attributes;
using ScriptSDK.Data;
using ScriptSDK.Engines;
using ScriptSDK.Items;
using StealthAPI;

namespace Tidus_Miner
{
    class MiningMethod
    {
        public static ushort Ore;
        public static ushort Picktype;
        public static ushort[] Extras = { 0x0, 0x7DA, 0x4A7, 0x4A8, 0x4A9, 0x4AA };// Reg,Oak,Ash,Yew,Heartwood,Bloodwood

        public static void Mine(Serial pick, int distance)
        {

            var mypick = new UOEntity(new Serial(pick.Value));
            TileReader.Initialize(); //Initialize the TileReader
            var spots = TileReader.GetMiningSpots(distance); //Search all Trees in Range of 1 Tile  
            var targethelper = TargetHelper.GetTarget(); // Assign the TargetHelper refeence
            foreach (var spot in spots.Keys.SelectMany(key => spots[key]))
            {
                Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true); // Move to Tree
                for (var i = 0; i < 25; i++) // Do 25 times or until weight full
                {
                    if (Miner.Endtime < DateTime.Now) Miner.backgroundWorker1.CancelAsync();
                    if (Miner.backgroundWorker1.CancellationPending) break;
                    if (Checkweight())
                    {
                        Miner.Gohomeandunload();
                        Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true);
                    }
                    if (!CheckPicks())
                    {
                        Miner.Gohomeandreload();
                        Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true);
                    }
                    if ((mypick.DoubleClick()) && (targethelper.WaitForTarget(2000)))
                        // try to doubleclick and wait until tárget cursor appear
                        targethelper.TargetTo(spot.Tile, new Point3D(spot.X, spot.Y, spot.Z)); //target the tree
                    Stealth.Client.Wait(1100); //wait 1 second
                    if (!Miner.Speechhit) continue;
                    Miner.Speechhit = false;
                    break;
                }
            }
        }

        private static bool CheckPicks()
        {
            var items = Scanner.Find<Item>(Picktype, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            return items.Count >= 1;
        }

        private static
            bool Checkweight()
        {
            var weight = Stealth.Client.GetSelfWeight();
            return weight >= Miner.Maxweight;
        }
        public static void Unload(Item mycontainer)
        {
            Stealth.Client.Wait(1000);
            //Setvariables();
            Stealth.Client.newMoveXY((ushort)mycontainer.Location.X, (ushort)mycontainer.Location.Y, true, 1, true);
            mycontainer.DoubleClick();
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(Ore, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var item in items)
            {
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
            foreach (var move in Extras.Select(extra => Scanner.Find<Item>(extra, 0xFFFF, Stealth.Client.GetBackpackID(), true)).SelectMany(moves => moves))
            {
                Stealth.Client.MoveItem(move.Serial.Value, move.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
        }

        public static void Reload(Item mycontainer)
        {
            Stealth.Client.Wait(1000);
            //Setvariables();
            Stealth.Client.newMoveXY((ushort)mycontainer.Location.X, (ushort)mycontainer.Location.Y, true, 1, true);
            mycontainer.DoubleClick();
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(Picktype, 0xFFFF, mycontainer.Serial.Value, true);
            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, Stealth.Client.GetBackpackID(), 0, 0, 0);
                Stealth.Client.Wait(1000);
                if (index == 2)
                break;
            }
        }

    }
}
