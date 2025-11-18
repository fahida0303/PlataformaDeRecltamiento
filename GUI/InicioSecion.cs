using ENTITY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GUI
{
    public partial class InicioSecion : Form
    {
       
        private readonly string _connectionString =@"Server=LAPTOP-UVS73RFU\SQLEXPRESS;Database=JobsyDB;User Id=usr_ass;Password=psr_ass;Encrypt=False;TrustServerCertificate=True;";


        public InicioSecion()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        /// <summary>
        /// Valida usuario/contraseña y devuelve un objeto Usuario (o null si falla)
        /// </summary>


        private dynamic ValidarLogin(string correo, string contrasena)
        {

            string query = @"
                SELECT 
                    idUsuario,
                    nombre,
                    correo,
                    [contraseña],
                    estado,
                    tipoUsuario,
                    documento,
                    fechaNacimiento,
                    foto
                FROM Usuario
                WHERE correo = @correo
                  AND [contraseña] = @pass
                  AND estado = 'Activo';
            ";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@pass", contrasena);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Usuario
                    {
                        IdUsuario = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Correo = reader.GetString(2),
                        Contrasena = reader.GetString(3),
                        Estado = reader.GetString(4),
                        TipoUsuario = reader.GetString(5),
                        Documento = reader.IsDBNull(6) ? null : reader.GetString(6),
                        FechaNacimiento = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                        Foto = reader.IsDBNull(8) ? null : (byte[])reader[8],
                        TelegramId = null,
                        TelegramUsername = null,
                        WhatsappNumber = null
                    };
                }

                return null;
            }
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

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel_IniciarSesion_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bttContinuar_Click(object sender, EventArgs e)
        {
            string correo = textBoxUsuario.Text.Trim();
            string contrasena = textBoxContrasena.Text.Trim();

            Usuario usuario = ValidarLogin(correo, contrasena);

            if (usuario == null)
            {
                MessageBox.Show("Correo o contraseña incorrectos.");
                return;
            }

            // Estamos dentro de Principal, cargamos el dashboard en su panel
            Principal principal = this.ParentForm as Principal;

            if (principal != null)
            {
                if (usuario.TipoUsuario == "Candidato")
                {
                    principal.AbrirFormulario(new UsuarioTipoCandidato(usuario));
                }
                else if (usuario.TipoUsuario == "Reclutador")
                {
                    principal.AbrirFormulario(new UsuarioTipoReclutador(usuario));
                }
                else
                {
                    MessageBox.Show("Tipo de usuario desconocido en base de datos.");
                }
            }
            else
            {
                // Caso raro: InicioSecion abierto solo
                Principal p = new Principal();
                p.Show();
                this.Hide();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Obtener referencia al formulario principal
            Principal principal = this.ParentForm as Principal;

            if (principal != null)
            {
                // Cargar Registrar dentro del panel contenedor
                principal.AbrirFormulario(new Registrar());
            }
            else
            {
                // Si por alguna razón NO estás dentro de Principal
                Registrar reg = new Registrar();
                reg.Show();
                this.Hide();
            }
        }
    }
}
