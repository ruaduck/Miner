using ScriptSDK.Engines;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;
using ScriptSDK.Targets;
using StealthAPI;

namespace Tidus_Miner
{
    class MiningMethod : Miner
    {
        public static int Reg;
        public static int Agapite;
        public static int Blaze;
        public static int Bronze;
        public static int Copper;
        public static int DullCopper;
        public static int Electrum;
        public static int Golden;
        public static int Ice;
        public static int Platinum;
        public static int ShadowIron;
        public static int Toxic;
        public static int Valorite;
        public static int Verite;

        private static void ResourceCount(Item ingot)
        {

            var color = Stealth.Client.GetColor(ingot.Serial.Value);
            switch (color)
            {
                case (ushort)Enums.MiningColors.Reg:
                    Reg += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Agapite:
                    Agapite += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Blaze:
                    Blaze += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Bronze:
                    Bronze += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Copper:
                    Copper += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.DullCopper:
                    DullCopper += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Electrum:
                    Electrum += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Golden:
                    Golden += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Ice:
                    Ice += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Platinum:
                    Platinum += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.ShadowIron:
                    ShadowIron += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Toxic:
                    Toxic += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Valorite:
                    Valorite += ingot.Amount;
                    break;
                case (ushort)Enums.MiningColors.Verite:
                    Verite += ingot.Amount;
                    break;
            }
        }

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
            var items = Scanner.Find<Item>(IronOre, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var item in items)
            {
                ResourceCount(item);
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
            if (backgroundWorker3 != null)
            {
                backgroundWorker3.RunWorkerAsync();
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
            var items = Scanner.Find<Item>(IronOre, 0xFFFF, Stealth.Client.GetBackpackID(), true);
            foreach (var item in items)
            {
                ResourceCount(item);
                Stealth.Client.MoveItem(item.Serial.Value, item.Amount, mycontainer.Serial.Value, 0, 0, 0);
                Stealth.Client.Wait(1000);
            }
            if (backgroundWorker3 != null)
            {
                backgroundWorker3.RunWorkerAsync();
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
