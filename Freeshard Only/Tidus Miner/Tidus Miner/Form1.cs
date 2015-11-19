using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using ScriptSDK;
using ScriptSDK.API;
using ScriptSDK.Items;
using ScriptSDK.Mobiles;
using Tidus_Miner.Properties;

namespace Tidus_Miner
{
    public partial class Miner : Form
    {
        public static Item Housecontainer;
        public static Item Pickcontainer;
        public Serial PickSerial;
        public static DateTime Endtime;
        public static int Minutes; //Minutes to Run
        public static int Homerune; // Rune Home
        public static int Bankrune;  //  Bank Rune
        public static int Firstrune; // First Lumberjacking Rune
        public static int Lastrune; // Last Lumberjacking Rune
        public static bool Osi; // OSI = True; RebirthUO = False;
        public static int Runetouse; //current rune on recall
        public static uint Runebook;
        public static int Minearea; //  Area to look for tree
        public static int Maxweight;
        public static bool Speechhit;
        public static string Method = "Not Set";

        public Miner()
        {
            Stealth.Client.ClilocSpeech += OnClilocSpeech;
            InitializeComponent();
            cancelbutton.Enabled = false;
            lumberjackbutton.Enabled = false;
        }
        private void lumberjackbutton_Click(object sender, EventArgs e)
        {
            if (SetInputs())
            {
                Endtime = DateTime.Now.AddMinutes((Minutes));
                Start();
                cancelbutton.Enabled = true;
                lumberjackbutton.Enabled = false;
            }
            else MessageBox.Show(@"You missed some required fields or didn't enter in digits in those fields");
        }
        private void Start()
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.RunWorkerAsync();
        }
        private void cancelbutton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            cancelbutton.Enabled = false;
            lumberjackbutton.Enabled = true;
        }
        private void startsetup_Click(object sender, EventArgs e)
        {
            startsetup.Enabled = false;
            Setup();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Miningloop();
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SetInputs())
            {
                RecallSetup();
                RunebookSetup();
                DialogResult axedialogResult = MessageBox.Show(@"Do you have your Axe Equipped?", @"Getting Axe ID", MessageBoxButtons.YesNo);
                switch (axedialogResult)
                {
                    case DialogResult.Yes:
                        PickSerial = PickSetup();
                        break;
                    case DialogResult.No:
                    MessageBox.Show(@"Equip your Axe and start script again.");
                    Close();
                    break;
                }
                Invoke((MethodInvoker)
                    delegate
                    {
                        recallstatus.Text = Travel.Recall(Runebook, Homerune, Method, Osi)
                            ? "Recalled"
                            : "Recall Failed";
                    });
                ContainerSetup();
                MiningMethod.Unload(Housecontainer);
                Invoke((MethodInvoker)delegate
                {
                    Text = PlayerMobile.GetPlayer().Name + @" - Miner";
                    lumberjackbutton.Enabled = true;

                });
            }

            else
            {
                startsetup.Enabled = true;
                MessageBox.Show(@"You missed some required fields or didn't enter in digits in those fields");
            }
        }
        private static void ContainerSetup()
        {
            MessageBox.Show(@"Select your Ore Container");
            Housecontainer = new Item(new Serial(Getcontainer()));
            Housecontainer.DoubleClick();
            MessageBox.Show(@"Select your Pick Container");
            Pickcontainer = new Item(new Serial(Getcontainer()));
        }
        private void RunebookSetup()
        {
            Travel travel = new Travel();
            #region Get Runebook Info
            DialogResult runebookDialogResult = MessageBox.Show(@"Do you have only 1 runebook in your backpack?", @"Getting Axe ID", MessageBoxButtons.YesNo);
            if (runebookDialogResult == DialogResult.Yes)
            {
                Runebook = travel.SetRunebookId();
                Invoke((MethodInvoker)
                        delegate { Runebooktbox.Text = Runebook == 0 ? "Error" : Runebook.ToString(); });
            }
            else if (runebookDialogResult == DialogResult.No)
            {
                MessageBox.Show(@"Get your runebooks correct and start script again.");
                Close();
            }
            #endregion
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
        private static void RecallSetup()
        {
            #region Recall Method
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

            #endregion
        }
        private Serial PickSetup()
        {
            Serial axe = null;
            #region Get Axe Info

                    axe = PlayerMobile.GetPlayer().Paperdoll.TwoHanded.Serial;
                    Invoke((MethodInvoker)
                        delegate { axetextbox.Text = axe.ToString(); });
            return axe;

            #endregion
        }
        private void Miningloop()
        {
            while (Endtime > DateTime.Now)
            {

                for (Runetouse = Firstrune; Runetouse < Lastrune + 1; Runetouse++)
                {
                    Invoke((MethodInvoker)delegate { gumptext.Text = string.Format("Recalling to spot {0}", Runetouse); });
                    if (!Travel.Recall(Runebook, Runetouse, Method, Osi))
                    {
                        Invoke((MethodInvoker)
                        delegate
                        {
                            recallstatus.Text = Method + @" Failed. Trying Next Rune";
                        });
                        continue;

                    }
                    MiningMethod.Mine(AxeSerial, Minearea);
                    if (Endtime < DateTime.Now) backgroundWorker1.CancelAsync();
                    if (backgroundWorker1.CancellationPending) break;
                    Invoke((MethodInvoker)
                        delegate { gumptext.Text = @"getting next rune"; });
                }
                if (backgroundWorker1.CancellationPending) break;
            }
            GoHome();
            MiningMethod.Unload(Housecontainer);
            MessageBox.Show(string.Format("We have ran for {0} minutes. Thank you! ", endtimebox.Text));
            Invoke((MethodInvoker)
                delegate { lumberjackbutton.Enabled = true; });
        }
        private static void OnClilocSpeech(object sender, ClilocSpeechEventArgs e)
        {
            switch (e.Text)
            {
                case "There's not enough wood here to harvest.":
                    Speechhit = true;
                    break;

                default:
                    break;
            }
        }
        public bool SetInputs()
        {
            Maxweight = Stealth.Client.GetSelfMaxWeight() - 30;
            Invoke((MethodInvoker)
                delegate { Osi = comboBox1.Text == @"OSI"; });
            if (endtimebox == null || homerunebox == null || bankrunebox == null || firstrunebox == null ||
                lastrunebox == null)
            {
                return false;
            }
            if (!int.TryParse(endtimebox.Text, out Minutes)) return false;
            if (!int.TryParse(homerunebox.Text, out Homerune)) return false;
            if (!int.TryParse(bankrunebox.Text, out Bankrune)) return false;
            if (!int.TryParse(treeareatbox.Text, out Minearea)) return false;
            return int.TryParse(firstrunebox.Text, out Firstrune) && int.TryParse(lastrunebox.Text, out Lastrune);
        }
        public void Setup()
        {
            backgroundWorker2.RunWorkerAsync();
        }
        public static void Gohomeandunload()
        {

            while (!GoHome())
            {
                Thread.Sleep(50);
            }

            MiningMethod.Unload(Housecontainer);
            Travel.Recall(Runebook, Runetouse, Method, Osi);
        }
        public static void Gohomeandreload()
        {

            while (!GoHome())
            {
                Thread.Sleep(50);
            }

            MiningMethod.Reload(Pickcontainer);
            Travel.Recall(Runebook, Runetouse, Method, Osi);
        }

        public static bool GoHome()
        {
            return Travel.Recall(Runebook, Homerune, Method, Osi);
        }
    }

}
