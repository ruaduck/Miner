using ScriptSDK.Engines;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;
using ScriptSDK.Targets;
using StealthAPI;

namespace Tidus_Miner
{
    class MiningMethod : Miner
    {
        

        public static Item ChoosePick()
        {
            return Scanner.Find<Item>(MiningEquipment, 0xFFFF, Stealth.Client.GetBackpackID(), true)[0];
        }
        public static bool CheckPicks()
        {
            var items = Scanner.Find<Item>(MiningEquipment, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            return items.Count >= 1;
        }

        public static bool Checkweight()
        {
            var max = Stealth.Client.GetSelfMaxWeight();
            var weight = Stealth.Client.GetSelfWeight();
            if (weight < (max)) return false;
            OreToIngot();
            Stealth.Client.Wait(1000);
            weight = Stealth.Client.GetSelfWeight();
            return weight >= (max);
        }

        public static void OreToIngot()
        {
            var player = PlayerMobile.GetPlayer();
            var ores = Scanner.Find<Item>(Ore, 0xFFFF, Stealth.Client.GetBackpackID(), false);
            var forge = Scanner.Find<Item>(Forge, 0xFFFF, Stealth.Client.GetBackpackID(), false);
            foreach (var ore in ores)
            {
                ore.DoubleClick();               
                var target = new EntityTarget(1000);
                target.Action(forge[0]);
            }
        }

        public static void Unload(Item mycontainer)
        {
            Stealth.Client.Wait(1000);
            Stealth.Client.newMoveXY((ushort)mycontainer.Location.X, (ushort)mycontainer.Location.Y, true, 1, true);
            mycontainer.DoubleClick();
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(Ingot, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var item in items)
            {
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
            
        }

        public static void Reload(Item mycontainer)
        {
            Stealth.Client.Wait(1000);
            Stealth.Client.newMoveXY((ushort)mycontainer.Location.X, (ushort)mycontainer.Location.Y, true, 1, true);
            mycontainer.DoubleClick();
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(MiningEquipment, 0xFFFF, mycontainer.Serial.Value, true);
            for (var i = 0; i < items.Count; i++)
            {
                Stealth.Client.MoveItem(items[i].Serial.Value, items[i].Amount, Stealth.Client.GetBackpackID(), 0, 0, 0);
                Stealth.Client.Wait(1000);
                if (i == 0)
                break;
            }
        }
        public static void BankUnload(Item mycontainer)
        {
            Stealth.Client.SendTextToUO("Bank");
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(Ingot, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var item in items)
            {
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
        }
        public static void BankReload(Item mycontainer)
        {
            Stealth.Client.SendTextToUO("Bank");
            Stealth.Client.Wait(1000);
            mycontainer.DoubleClick();
            Stealth.Client.Wait(1000);
            var items = Scanner.Find<Item>(MiningEquipment, 0xFFFF, mycontainer.Serial.Value, true);
            for (var i = 0; i < items.Count; i++)
            {
                Stealth.Client.MoveItem(items[i].Serial.Value, items[i].Amount, Stealth.Client.GetBackpackID(), 0, 0, 0);
                Stealth.Client.Wait(1000);
                if (i == 2)
                    break;
            }
        }
        

    }
}
