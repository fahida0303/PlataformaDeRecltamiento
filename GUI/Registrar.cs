using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ENTITY;
using DAL;

namespace GUI
{
    public partial class Registrar : Form
    {
        private byte[] fotoBytes = null;

        // 👉 Repositorio para acceder a la BD
        private readonly UsuarioRepository repo = new UsuarioRepository();

        public Registrar()
        {
            InitializeComponent();
            // El ComboBox y otras cosas que ya tengas...

            // 👉 Hacer que el botón sea hijo del PictureBox
            btnSeleccionarFoto.Parent = pbFoto;
            btnSeleccionarFoto.BackColor = Color.Transparent;

            // 👉 Asegurar que cubre toda el área del icono
            btnSeleccionarFoto.Location = new Point(0, 0);
            btnSeleccionarFoto.Size = pbFoto.Size;

            // 👉 Configurar que sea totalmente invisible
            btnSeleccionarFoto.FlatStyle = FlatStyle.Flat;
            btnSeleccionarFoto.FlatAppearance.BorderSize = 0;
            btnSeleccionarFoto.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSeleccionarFoto.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSeleccionarFoto.ForeColor = Color.Transparent;
            btnSeleccionarFoto.Text = string.Empty;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("Ingrese el ID / documento.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                MessageBox.Show("Ingrese la fecha de nacimiento (Ej: 2003-05-22).");
                return false;
            }

            if (!DateTime.TryParse(txtFechaNacimiento.Text, out _))
            {
                MessageBox.Show("La fecha no es válida. Use un formato como 2003-05-22.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre completo.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("Ingrese el correo.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                MessageBox.Show("Ingrese la contraseña.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmar.Text))
            {
                MessageBox.Show("Confirme la contraseña.");
                return false;
            }

            if (txtContraseña.Text != txtConfirmar.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden.");
                return false;
            }

            if (cmbTipoUsuario.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el tipo de usuario (Candidato/Reclutador).");
                return false;
            }

            if (fotoBytes == null)
            {
                MessageBox.Show("Seleccione una foto de perfil.");
                return false;
            }

            return true;
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label_Nombres_Click(object sender, EventArgs e)
        {

        }

        private void btnSeleccionarFoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar foto de perfil";
                ofd.Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Image img = Image.FromFile(ofd.FileName);
                    pbFoto.Image = img;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, img.RawFormat);
                        fotoBytes = ms.ToArray();
                    }
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // 1. Validar los datos del primer formulario
            if (!ValidarFormulario())
                return;

            // 2. Construir el objeto Usuario con los datos del PASO 1
            DateTime fechaNacimiento;
            DateTime.TryParse(txtFechaNacimiento.Text, out fechaNacimiento);

            // tipo de usuario elegido en el combo
            string tipoSeleccionado = cmbTipoUsuario.SelectedItem.ToString(); // "Candidato" o "Reclutador"

            Usuario nuevo = new Usuario()
            {
                Nombre = txtNombre.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                Contrasena = txtContraseña.Text.Trim(),
                Estado = "Activo",
                Documento = txtDocumento.Text.Trim(),
                FechaNacimiento = fechaNacimiento,
                TipoUsuario = tipoSeleccionado,
                Foto = fotoBytes,
                TelegramId = null,
                TelegramUsername = null,
                WhatsappNumber = null
            };

            // 3. Ir al segundo formulario SIEMPRE, pasándole Usuario + TipoUsuario
            Principal padre = this.ParentForm as Principal;
            if (padre != null)
            {
                padre.AbrirFormulario(new RegistrarDos(nuevo, tipoSeleccionado));
            }
            else
            {
                RegistrarDos formPaso2 = new RegistrarDos(nuevo, tipoSeleccionado);
                formPaso2.Show();
                this.Hide();
            }

        }
    }
}
