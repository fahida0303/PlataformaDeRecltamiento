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
    public partial class Registrar : Form
    {
        public Registrar()
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

        private void ID(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_ID_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_Nombres_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_Nombres_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_Apellidos_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_Apellidos_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_FechaNacimiento_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }
        

        private void txt_FechaNacimiento_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_Gmail_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_Gmail_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_Contraseña_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_Contraseña_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txt_ConfirContraseña_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txt_ConfirContraseña_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }

        private void txtID2_Enter(object sender, EventArgs e)
        {
            txt_ID.Text = "";
            txt_ID.ForeColor = Color.White; // O el color real de tu texto
        }

        private void txtID2_Leave(object sender, EventArgs e)
        {
            txt_ID.Text = "ID";
            txt_ID.ForeColor = Color.Gray;
        }
    }
}
