using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUI
{
    public partial class Eleccion : Form
    {
        private Principal _principal;

        // Constructor SIN parámetros (lo usa el diseñador)
        public Eleccion()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // Constructor CON parámetro (lo usa Principal)
        public Eleccion(Principal principal) : this()
        {
            _principal = principal;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {
            // Solo para que el diseñador no dé error
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Botón "Entrar como candidato"
            if (_principal != null)
            {
                // Ya no pasamos "candidato", el tipo se obtiene después del login
                _principal.AbrirFormulario(new InicioSecion());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Botón "Entrar como reclutador"
            if (_principal != null)
            {
                // Igual aquí: solo abrimos la pantalla de login
                _principal.AbrirFormulario(new InicioSecion());
            }
        }
    }
}
