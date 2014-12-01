using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace vv
{
    public partial class fMain : Form
    {
        [DllImport("wtsapi32.dll")]
        private static extern bool WTSRegisterSessionNotification(IntPtr hWnd, int dwFlags);

        [DllImport("wtsapi32.dll")]
        private static extern bool WTSUnRegisterSessionNotification(IntPtr hWnd);

        private const int NotifyForThisSession = 0; // This session only

        private const int SessionChangeMessage = 0x02B1;
        private const int SessionLockParam = 0x7;
        private const int SessionUnlockParam = 0x8;

        private const string MENSAJE1 = "Mire a la distancia por 20 segundos, luego reanude su trabajo.";
        private const string MENSAJE2 = "Cierre los ojos durante 20 segundos, luego reanude su trabajo.";
        private const string MENSAJE3 = "Estire los brazos 5 segundos, luego reanude su trabajo.";
        private const string MENSAJE4 = "Gire el cuello durante 5 segundos, luego reanude su trabajo.";

        private const string TITULO = "Evite la fatiga visual";

        private int nmensaje = 0;
        private int t = 0;
        private int max = 600; //1200 segundos = 20 minutos, 900 = 15 min, 600 = 10 min

        public fMain()
        {
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            WTSRegisterSessionNotification(this.Handle, NotifyForThisSession);
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));

            InstallOnStartupFolder();

            MostrarMensajeInicio();
        }

        private void MostrarMensajeDescanso()
        {
            string mensaje;

            nmensaje++;

            switch (nmensaje)
            {
                case 1:
                    mensaje = MENSAJE1;
                    break;
                case 2:
                    mensaje = MENSAJE2;
                    break;
                case 3:
                    mensaje = MENSAJE3;
                    break;
                default:
                    mensaje = MENSAJE4;
                    nmensaje = 0;
                    break;
            }

            this.notifyIcon1.ShowBalloonTip(20000, TITULO, mensaje, ToolTipIcon.Info);
            SoundPlayer sndplayr = new SoundPlayer(vv.Properties.Resources.onebell);
            sndplayr.Play();
        }

        private void MostrarMensajeInicio()
        {
            this.notifyIcon1.ShowBalloonTip(20000, TITULO, "Recuerde descansar los ojos cada 10 minutos y tomar un descanso largo cada hora.", ToolTipIcon.Info);
        }

        void OnSessionLock()
        {
            t = 0;
            this.timer1.Enabled = false;
        }

        void OnSessionUnlock()
        {
            t = 0;
            this.timer1.Enabled = true;
        }

        protected override void WndProc(ref Message m)
        {
            // check for session change notifications
            if (m.Msg == SessionChangeMessage)
            {
                if (m.WParam.ToInt32() == SessionLockParam)
                    OnSessionLock(); // Do something when locked
                else if (m.WParam.ToInt32() == SessionUnlockParam)
                    OnSessionUnlock(); // Do something when unlocked
            }

            base.WndProc(ref m);
            return;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            t++;
            if (t > this.max)
            {
                this.timer1.Enabled = false;
                t = 0;
                this.MostrarMensajeDescanso();
            }
        }

        private void notifyIcon1_BalloonTipClosed(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            fAbout f = new fAbout();
            f.Show();
        }

        private void fMain_MouseMove(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InstallOnStartupFolder()
        {
            string fileName = "vv - Evite la fatiga visual.appref-ms";

            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            string destinePath = Path.Combine(startupPath, fileName);

            string originalPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            originalPath = Directory.GetParent(originalPath).FullName;
            originalPath = Path.Combine(originalPath, "Vida Amarilla");
            originalPath = Path.Combine(originalPath, fileName);

            //Copy link
            try
            {
                File.Copy(originalPath, destinePath);
            }
            catch (Exception)
            {
                
            }

        }

    }
}
