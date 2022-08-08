using tsvcheck;
using PokemonStandardLibrary;
using PokemonStandardLibrary.CommonExtension;

namespace tsvcheckGUI
{
    public partial class Form1 : Form
    {

        
        public Form1()
        {
            InitializeComponent();
        }

        private CancellationTokenSource cts;

        private async void button1_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();

            button1.Enabled = false;
            button2.Enabled = true;

            LogBoxWriteLine("Calculating...");


            uint g7tid = (uint)nupG7TID.Value;
            List<uint> ivs = new List<Decimal>{nupH.Value, nupA.Value,nupB.Value,nupC.Value, nupD.Value, nupS.Value }.Select(x=>decimal.ToUInt32(x)).ToList();
            Nature nature = comboBoxNature.Text.ConvertToNature();
            bool isUsum = radioButtonUSUM.Checked;
            int min = (int)nupMin.Value;
            int max = (int)nupMax.Value;


            TSVChecker checker = new TSVChecker(g7tid, ivs, nature, isUsum, min, max, this);
            await Task.Run(() =>
            {
                var sw = new System.Diagnostics.Stopwatch();
                // 計測開始
                sw.Start();
                checker.Check(cts.Token);
                // 計測停止
                sw.Stop();

                InvokeLogBoxWriteLine("Time elapsed:");
                TimeSpan ts = sw.Elapsed;
                InvokeLogBoxWriteLine($"{sw.ElapsedMilliseconds}ms");

                ResetProgress();
            });

            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            button2.Enabled = false;
            button1.Enabled = true;
        }

        public void ResetProgress()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                progressBar1.Value = 0;
                progressLabel.Text = $"{progressBar1.Value}/{progressBar1.Maximum}";
            }));

        }

        public void IncrementProgress()
        {
            this.Invoke((MethodInvoker)(() =>
            { 
                progressBar1.Value++;
                progressLabel.Text = $"{progressBar1.Value}/{progressBar1.Maximum}";
            }));
        }

        public void LogBoxWriteLine(string str)
        {
            LogBox.Text += str + "\r\n";
            LogBox.ScrollToCaret();
        }

        public void InvokeLogBoxWriteLine(string str)
        {
            this.Invoke((MethodInvoker)(() => 
            {
                LogBox.Text += str + "\r\n";
                LogBox.ScrollToCaret();
            }));
        }

    }
}