using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GUI
{
    public partial class RegistrarDos : Form
    {
        public RegistrarDos()
        {
            InitializeComponent();
        }

        //private void btnCerrar_Click(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}

        //private void btnMaximizar_Click(object sender, EventArgs e)
        //{
        //    this.WindowState= FormWindowState.Maximized;
        //    btnMaximizar.Visible = false;
        //    btnRestaurar.Visible = true;
        //}

        //private void btnRestaurar_Click(object sender, EventArgs e)
        //{
        //    this.WindowState = FormWindowState.Normal;
        //    btnRestaurar.Visible = false;
        //    btnMaximizar.Visible = true;
        //}

        //private void btnMinimizar_Click(object sender, EventArgs e)
        //{
        //    this.WindowState = FormWindowState.Minimized;
        //}
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void txt_PResidencia_Enter(object sender, EventArgs e)
        {
            txt_PResidencia.Text = "";
            txt_PResidencia.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_PResidencia_Leave(object sender, EventArgs e)
        {
            txt_PResidencia.Text = "Pais de Residencia";
            txt_PResidencia.ForeColor = Color.Gray;
        }

        private void txt_CResidencia_Enter(object sender, EventArgs e)
        {
            txt_CResidencia.Text = "";
            txt_CResidencia.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_CResidencia_Leave(object sender, EventArgs e)
        {
            txt_CResidencia.Text = "Ciudad de Residencia";
            txt_CResidencia.ForeColor = Color.Gray;
        }
    }
}
