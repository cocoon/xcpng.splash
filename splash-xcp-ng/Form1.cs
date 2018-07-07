using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace splash_xcp_ng
{
    public partial class Form1 : Form
    {
        private System.IO.Stream stream;
        private System.Reflection.Assembly assembly;
        const string exe = "XenCenterMain.exe";

        private BackgroundWorker bworker = new BackgroundWorker();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private Process proc;

        public Form1()
        {
            InitializeComponent();
            Style();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Launch();
        }

        private void Style()
        {
            Image bitmap;
            assembly = System.Reflection.Assembly.LoadFrom(Application.ExecutablePath);
            stream = assembly.GetManifestResourceStream("splash_xcp_ng.Resources.splash.bmp");
            bitmap = Image.FromStream(stream);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            this.BackgroundImage = bitmap;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
            this.CenterToScreen();
        }

        private void Launch()
        {
            if (!File.Exists(exe))
            {
                MessageBox.Show("[ERROR] Application not found: " + exe);
                Exit();
                return;
            }


            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = String.Empty;

            bworker.WorkerSupportsCancellation = true;
            bworker.DoWork += new DoWorkEventHandler(Start);
            bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bworkerCompleted);
            bworker.RunWorkerAsync();

            //FixMe: Add some safty counter to exit if something goes wrong
            while (proc == null || string.IsNullOrEmpty(proc.MainWindowTitle))
            {
                System.Threading.Thread.Sleep(100);
                if(proc != null) proc.Refresh();
            }

            //MessageBox.Show(proc.MainWindowTitle);

            Exit();

        }

        private void Start(object sender, DoWorkEventArgs e)
        {
                proc = Process.Start(startInfo);
        }

        private void bworkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void Exit()
        {
            if (bworker != null && bworker.WorkerSupportsCancellation) bworker.CancelAsync();
            System.Environment.Exit(1);
        }


    }
}
