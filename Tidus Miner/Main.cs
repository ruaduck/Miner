using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ScriptSDK;
using StealthAPI;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;
using Tidus_Miner.Properties;
using System.Collections.Generic;
using ScriptSDK.Engines;
using ScriptSDK.Attributes;
using ScriptSDK.Data;
using System.Linq;
using System.IO;

namespace Tidus_Miner
{
    public partial class Miner : Form
    {
        public static StaticItemRealXY mytile;
        public static List<StaticItemRealXY> bannedtile = new List<StaticItemRealXY>();
        public static string BannedCSV = $@"{Application.StartupPath}\BannedTiles.csv";
        public static List<ushort> MiningEquipment { get; set; }
        public static List<ushort> Ore { get; set; }
        public static List<ushort> Colors { get; set; }
        public static List<ushort> Forge { get; set; }
        public static ushort Ingot = (ushort)Enums.Ingots.Ingot;
        public static Item Housecontainer { get; set; }
        public static Item Pickcontainer { get; set; }
        public Serial PickSerial { get; set; }
        public static DateTime Endtime { get; set; }
        public static DateTime Starttime { get; set; }
        public int Minutes //Minutes to Run
        {
            get => Convert.ToInt32(endtimebox.Text); 
            set => endtimebox.Text = Convert.ToString(value);
        }
        public static int Homerune { get; set; }  // Rune Home
 
        public static int Bankrune  // Bank Rune
        {
            get; set;
        } 
        public static int Firstrune; // First Mining Spot
        public static int Lastrune; // Last Mining Spot
        public static bool Osi { get; set; } // OSI = True; RebirthUO = False;
        public static int Runetouse; //current rune on recall
        public static Runebook Runebook;
        public static int Minearea;//  Area to look for tree
        public static int Maxweight { get; set; }
        public static bool Speechhit { get; set; }
        public static bool SkipOre { get; set; }
        public static string Method = "Not Set";
        public static bool Encumbered { get; set; }
        public static bool Actionperform { get; set; }
        public static bool Loadused { get; set; }
        public static bool Beetle { get; set; }
        public static bool BankCrystal { get; set; }
        public static Item Bankstorage { get; set; }
        public static BackgroundWorker backgroundWorker2 = new BackgroundWorker();
        public static BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        public Miner()
        {
            Stealth.Client.ClilocSpeech += OnClilocSpeech;
            LoadBannedTiles();
            InitializeComponent();
            InitializeBackgroundWorker();
            Osi = false;
            cancelbutton.Enabled = false;
            mine_btn.Enabled = false;

        }
        private void LoadBannedTiles()
        {
            if (File.Exists(BannedCSV))
            {
                using (StreamReader sr = new StreamReader(BannedCSV))
                {
                    while (!sr.EndOfStream)
                    {
                        StaticItemRealXY tile;
                        
                        string[] rows = sr.ReadLine().Split(',');                                                
                        tile.Color = (ushort)Convert.ToInt32(rows[0]);
                        tile.Tile = (ushort)Convert.ToInt32(rows[1]);
                        tile.X = (ushort)Convert.ToInt32(rows[2]);
                        tile.Y = (ushort)Convert.ToInt32(rows[3]);
                        tile.Z = (byte)Convert.ToInt32(rows[4]);
                        bannedtile.Add(tile);
                    }

                }

            }
        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork +=
                 new DoWorkEventHandler(BackgroundWorker2_DoWork);
        }
        private void Mine_btn_Click(object sender, EventArgs e)
        {
            if (SetInputs())
            {
                Endtime = DateTime.Now.AddMinutes((Minutes));
                if (Osi) 
                {
                    Setups.SetOSIInfo();
                }
                else
                {
                    Setups.SetFSInfo();
                }
                Start();
                cancelbutton.Enabled = true;
                mine_btn.Enabled = false;
            }
            else MessageBox.Show(@"You missed some required fields or didn't enter in digits in those fields");
        }
        
        
        private void Start()
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();
        }
        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            cancelbutton.Enabled = false;
            mine_btn.Enabled = true;
        }
        private void Startsetup_Click(object sender, EventArgs e)
        {
            startsetup.Enabled = false;
            Setup();
        }
      
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!MiningMethod.CheckPicks())
            {
                if (BankCrystal) { MiningMethod.BankReload(Pickcontainer); }
                else
                {
                    Gohomeandreload();
                }

            }
            Miningloop();
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SetInputs())
            {
                Setups.BankCrystalSetup();
                Setups.RecallSetup();
                Setups.RunebookSetup();
                
                if (!BankCrystal)
                {
                    _ = Invoke((MethodInvoker)
                    delegate
                    {
                        Gosomewehere(Homerune);
                    });
                    Setups.ContainerSetup();
                    MiningMethod.Unload(Housecontainer);
                }
                Invoke((MethodInvoker)delegate
                {
                    Text = PlayerMobile.GetPlayer().Name + @" - Miner";
                    mine_btn.Enabled = true;

                });
            }

            else
            {
                startsetup.Enabled = true;
                MessageBox.Show(@"You missed some required fields or didn't enter in digits in those fields");
            }
        }
                
        private void Miningloop()
        {
            while (Endtime > DateTime.Now)
            {

                for (Runetouse = Firstrune; Runetouse < Lastrune + 1; Runetouse++)
                {
                    var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
                    _ = Invoke((MethodInvoker)
                    delegate
                    {
                        Gosomewehere(Runetouse);
                    }); 
                    if (loc1 == PlayerMobile.GetPlayer().Location) continue;                    
                    Mine(Minearea);
                    if (Endtime < DateTime.Now) backgroundWorker1.CancelAsync();
                    if (backgroundWorker1.CancellationPending) break;
                }
                if (backgroundWorker1.CancellationPending) break;
            }
            if (!BankCrystal)
            { 
                GoHome();
                MiningMethod.Unload(Housecontainer);
            }
            else
            {
                Stealth.Client.SendTextToUO("Bank");
                MiningMethod.BankUnload(Bankstorage);
                GoHome();
            }
            OutputToCSV();
            MessageBox.Show(string.Format("We have ran for {0} minutes. Thank you! ", endtimebox.Text));
            Invoke((MethodInvoker)
                delegate { mine_btn.Enabled = true; });
        }
        private static void OutputToCSV()
        {
            string newFileName = $@"{Application.StartupPath}\BannedTiles.csv";
            foreach (var tile in bannedtile)
            {
                string mytext = $"{tile.Color},{tile.Tile},{tile.X},{tile.Y},{tile.Z}{Environment.NewLine}";
                File.AppendAllText(newFileName, mytext);
            }

            
        }
        private static void OnClilocSpeech(object sender, ClilocSpeechEventArgs e)
        {
            switch (e.Text)
            {
                case "There is no metal here to mine.":
                    Speechhit = true;
                    break;
                case "There is not enough metal-bearing ore in this pile to make an ingot.":
                    SkipOre = true;
                    break;
                case "That is too far away.":
                    Speechhit = true;
                    bannedtile.Add(mytile);
                    break;
                case "Target cannot be seen.":
                    Speechhit = true;
                    bannedtile.Add(mytile);
                    break;
                case "You can't mine there.":
                    Speechhit = true;
                    bannedtile.Add(mytile);
                    break;
                default:
                    break;
            }
        }
        public bool SetInputs()
        {
            Maxweight = Stealth.Client.GetSelfMaxWeight() - 30;
            if (endtimebox == null || homerunebox == null || bankrunebox == null || firstrunebox == null ||
                lastrunebox == null)
            {
                return false;
            }
            Homerune = Convert.ToInt32(homerunebox.Text);
            Bankrune = Convert.ToInt32(bankrunebox.Text);
            if (!int.TryParse(treeareatbox.Text, out Minearea)) return false;
            return int.TryParse(firstrunebox.Text, out Firstrune) && int.TryParse(lastrunebox.Text, out Lastrune);
        }
        public void Setup()
        {
            backgroundWorker2.RunWorkerAsync();
        }
        public void Mine(int distance)
        {
            TileReader.Initialize(); //Initialize the TileReader
            var spots = TileReader.GetMiningSpots(distance); //Search all spots in Range of n Tile  
            var targethelper = TargetHelper.GetTarget(); // Assign the TargetHelper reference
            foreach (var spot in spots.Keys.SelectMany(key => spots[key]))
            {
                mytile = spot;
                if (!bannedtile.Contains(mytile))
                {
                    Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true); // Move to Tree
                    for (var i = 0; i < 15; i++) // Do 25 times or until weight full
                    {
                        //Stealth.Client.AddToSystemJournal($"Tile: {spot.Tile}  3D Point: {spot.X},{spot.Y},{spot.Z}");

                        if (Endtime < DateTime.Now) backgroundWorker1.CancelAsync();
                        if (backgroundWorker1.CancellationPending) break;
                        if (MiningMethod.Checkweight())
                        {
                            if (BankCrystal) { MiningMethod.BankUnload(Bankstorage); }
                            else
                            {
                                Gohomeandunload();
                                Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true);
                            }

                        }
                        if (!MiningMethod.CheckPicks())
                        {
                            if (BankCrystal) { MiningMethod.BankReload(Pickcontainer); }
                            else
                            {
                                Gohomeandreload();
                                Stealth.Client.newMoveXY(spot.X, spot.Y, true, 1, true);
                            }

                        }
                        var mypick = MiningMethod.ChoosePick();
                        if ((mypick.DoubleClick()) && (targethelper.WaitForTarget(2000)))
                            // try to doubleclick and wait until tárget cursor appear
                            targethelper.TargetTo(new Point3D(spot.X, spot.Y, spot.Z)); //target the tree
                        Stealth.Client.Wait(1100); //wait 1 second
                        if (!Speechhit) continue;
                        Speechhit = false;
                        break;
                    }
                }
                if (backgroundWorker1.CancellationPending) break;
            }
        }
        private void Gosomewehere(int rune)
        {
            switch (Method)
            {
                case "Recall":
                    recallstatus.Text = Travel.Recall(rune)
                        ? "Recalled"
                        : "Recall Failed";
                    break;
                case "Sacred Journey":
                    recallstatus.Text = Travel.SacredJourney(rune)
                        ? "Sacred Journey"
                        : "Sacred Journey Failed";
                    break;
                default:
                    recallstatus.Text = Travel.Recall(rune)
                        ? "Recalled"
                        : "Recall Failed";
                    break;
                    //recallstatus.Text = Travel.Scroll(Homerune)
                    //    ? "Scroll Recall"
                    //    : "Scroll Recall Failed";
            }

        }
        private void Gohomeandunload()
        {

            while (!GoHome())
            {
                Thread.Sleep(50);
            }

            MiningMethod.Unload(Housecontainer);
            Travel.Recall(Runetouse);
        }
        private void Gohomeandreload()
        {

            while (!GoHome())
            {
                Thread.Sleep(50);
            }

            MiningMethod.Reload(Pickcontainer);
            Travel.Recall(Runetouse);
        }

        public bool GoHome()
        {
            var loc1 = PlayerMobile.GetPlayer().Location;// LOC before recall
            Gosomewehere(Homerune);
            return loc1 != PlayerMobile.GetPlayer().Location; // Compare Locs to see if you moved.
        }

    }

}
