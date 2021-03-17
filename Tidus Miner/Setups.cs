using System.Threading;
using System.Windows.Forms;
using ScriptSDK;
using StealthAPI;
using ScriptSDK.Items;
using Tidus_Miner.Properties;
using System.Collections.Generic;

namespace Tidus_Miner
{
    class Setups : Miner
    {
        

        private static uint Getitem()
        {
            Stealth.Client.ClientRequestObjectTarget();
            while (!Stealth.Client.ClientTargetResponsePresent())
            {
                Thread.Sleep(50);
            }
            var myitem = Stealth.Client.ClientTargetResponse().ID;
            return myitem;
        }
        private static uint Getcontainer()
        {
            Stealth.Client.ClientRequestObjectTarget();
            while (!Stealth.Client.ClientTargetResponsePresent())
            {
                Thread.Sleep(50);
            }
            var container = Stealth.Client.ClientTargetResponse().ID;
            return container;
        }
        private static RuneBookConfig Config()
        {
            RuneBookConfig config;
            if (Osi)
            {
                config = new RuneBookConfig()
                {
                    ScrollOffset = 10,
                    DropOffset = 200,
                    DefaultOffset = 300,
                    RecallOffset = 50,
                    GateOffset = 100,
                    SacredOffset = 75,
                    Jumper = 1
                };
            }
            else
            {
                config = new RuneBookConfig()
                {
                    ScrollOffset = 2,
                    DropOffset = 3,
                    DefaultOffset = 4,
                    RecallOffset = 5,
                    GateOffset = 6,
                    SacredOffset = 7,
                    Jumper = 6
                };
            }
            return config;
        }
        public static void BankCrystalSetup()
        {
            var BankCrystalDialogResult =
            MessageBox.Show("Do you have a Bank Crystal?",
                @"Bank Crystal Method", MessageBoxButtons.YesNo);
            switch (BankCrystalDialogResult)
            {
                case DialogResult.Yes:
                    BankCrystal = true;
                    Stealth.Client.SendTextToUO("Bank");
                    MessageBox.Show(@"Select your Ore/Iron Container in your Bank");
                    Bankstorage = new Item(new Serial(Getitem()));
                    Bankstorage.DoubleClick();
                    PickContainerSetup();
                    break;
                case DialogResult.No:
                    BankCrystal = false;
                    break;
            }
        }
        public static void PickContainerSetup()
        {
            MessageBox.Show(@"Select your Pick Container");
            Pickcontainer = new Item(new Serial(Getcontainer()));
            Pickcontainer.DoubleClick();
        }

        public static void ContainerSetup()
        {
            MessageBox.Show(@"Select your Ore/Iron Container");
            Housecontainer = new Item(new Serial(Getcontainer()));
            Housecontainer.DoubleClick();
            PickContainerSetup();
        }

        public static void RunebookSetup()
        {
            MessageBox.Show(@"Select your Runebook");
            var config = Config();
            Runebook = new Runebook(new Serial(Getitem()),Config(), (uint)Enums.RunebookGump.EVO);
            Runebook.Parse();
        }
        public static void RecallSetup()
        {
            var recallDialogResult =
                MessageBox.Show(Resources.Miner_RecallSetup_,
                    @"Recall Method", MessageBoxButtons.YesNo);
            switch (recallDialogResult)
            {
                case DialogResult.Yes:
                    Method = "Recall";
                    break;
                case DialogResult.No:
                    Method = "Sacred Journey";
                    break;
            }
        }
        public static void SetFSInfo()
        {
            MiningEquipment = new List<ushort>
            {
                (ushort)Enums.MiningEquipment.Pickaxe,
                (ushort)Enums.MiningEquipment.Shovel
            };           
            Ore = new List<ushort>
            {
                (ushort)Enums.Ore.Large,
                (ushort)Enums.Ore.Medium,
                (ushort)Enums.Ore.Small,
                (ushort)Enums.Ore.Tiny
            };
            IronOre = new List<ushort>
            {
                (ushort)Enums.Ore.Large,
                (ushort)Enums.Ore.Medium,
                (ushort)Enums.Ore.Small,
                (ushort)Enums.Ore.Tiny,
                (ushort)Enums.Ingots.Ingot
            };
            Forge = new List<ushort>
            {
                (ushort)Enums.Forges.MobileForge,
                (ushort)Enums.Forges.OrcishForge
            };      
        }
        public static void SetOSIInfo()
        {
            MiningEquipment = new List<ushort>
            {
                (ushort)Enums.MiningEquipment.Pickaxe,
                (ushort)Enums.MiningEquipment.Shovel
            };            
            Ore = new List<ushort>
            {
                (ushort)Enums.Ore.Large,
                (ushort)Enums.Ore.Medium,
                (ushort)Enums.Ore.Small,
                (ushort)Enums.Ore.Tiny
            };            
            IronOre= new List<ushort>
            {
                (ushort)Enums.Ore.Large,
                (ushort)Enums.Ore.Medium,
                (ushort)Enums.Ore.Small,
                (ushort)Enums.Ore.Tiny,
                (ushort)Enums.Ingots.Ingot
            };      
            Forge = new List<ushort>
            {
                (ushort)Enums.Forges.StaticForge, 
            };

        }
    }
}
