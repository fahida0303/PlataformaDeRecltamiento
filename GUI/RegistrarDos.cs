using DAL;
using ENTITY;
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

namespace GUI
{
    public partial class RegistrarDos : Form
    {
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();
        private readonly CandidatoRepository candidatoRepo = new CandidatoRepository();
        private readonly EmpresaRepository empresaRepo = new EmpresaRepository();
        private readonly ReclutadorRepository reclutadorRepo = new ReclutadorRepository();

        // Usuario del Paso 1
        private readonly Usuario _usuarioBase;
        // Tipo de usuario elegido en el Paso 1 ("Candidato" o "Reclutador")
        private readonly string _tipoUsuario;

        // Rutas de archivos seleccionados
        private string rutaCedula = null;
        private string rutaCV = null;

        public RegistrarDos(Usuario usuario, string tipoUsuario)
        {
            InitializeComponent();

            _usuarioBase = usuario;
            _tipoUsuario = tipoUsuario;

            ConfigurarBotonesInvisibles();
            ConfigurarUIporRol();

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // 2) Constructor vacío SOLO para el diseñador (no lo uses en código)
        public RegistrarDos()
        {
            InitializeComponent();
        }

        private void ConfigurarUIporRol()
        {
            // Usamos el string que viene del combo en el Paso 1
            bool esReclutador = _tipoUsuario != null &&
                _tipoUsuario.Equals("Reclutador", StringComparison.OrdinalIgnoreCase);

            // Si es reclutador → mostramos campos extra
            labelCargo.Visible = esReclutador;
            txtCargo.Visible = esReclutador;
            labelEmpresa.Visible = esReclutador;
            txtEmpresa.Visible = esReclutador;

        }

        private void ConfigurarBotonesInvisibles()
        {
            // CÉDULA
            btnSeleccionarCedula.Parent = pbCedula;
            btnSeleccionarCedula.BackColor = Color.Transparent;
            btnSeleccionarCedula.FlatStyle = FlatStyle.Flat;
            btnSeleccionarCedula.FlatAppearance.BorderSize = 0;
            btnSeleccionarCedula.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSeleccionarCedula.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSeleccionarCedula.ForeColor = Color.Transparent;
            btnSeleccionarCedula.Text = string.Empty;
            btnSeleccionarCedula.Location = new Point(0, 0);
            btnSeleccionarCedula.Size = pbCedula.Size;

            // CV
            btnSeleccionarCV.Parent = pbCV;
            btnSeleccionarCV.BackColor = Color.Transparent;
            btnSeleccionarCV.FlatStyle = FlatStyle.Flat;
            btnSeleccionarCV.FlatAppearance.BorderSize = 0;
            btnSeleccionarCV.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSeleccionarCV.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSeleccionarCV.ForeColor = Color.Transparent;
            btnSeleccionarCV.Text = string.Empty;
            btnSeleccionarCV.Location = new Point(0, 0);
            btnSeleccionarCV.Size = pbCV.Size;
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

        private void btnSeleccionarCedula_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar archivo de cédula";
                ofd.Filter = "Imágenes o PDF|*.png;*.jpg;*.jpeg;*.bmp;*.pdf";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rutaCedula = ofd.FileName;

                    try
                    {
                        string ext = Path.GetExtension(ofd.FileName).ToLower();
                        if (ext != ".pdf") // si es imagen, la mostramos
                        {
                            pbCedula.Image = Image.FromFile(ofd.FileName);
                        }
                    }
                    catch
                    {
                        // ignoramos error, dejamos el icono
                    }
                }
            }
        }



        private void btnSeleccionarCV_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar archivo de CV";
                ofd.Filter = "Documentos|*.pdf;*.doc;*.docx;*.png;*.jpg;*.jpeg";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rutaCV = ofd.FileName;

                    try
                    {
                        string ext = Path.GetExtension(ofd.FileName).ToLower();
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            pbCV.Image = Image.FromFile(ofd.FileName);
                        }
                    }
                    catch
                    {
                        // ignoramos error
                    }
                }
            }
        }
        private bool ValidarPaso2()
        {
            if (string.IsNullOrWhiteSpace(rutaCedula))
            {
                MessageBox.Show("Debe añadir un archivo de Cédula.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(rutaCV))
            {
                MessageBox.Show("Debe añadir un archivo de CV.");
                return false;
            }

            bool esReclutador = _tipoUsuario.Equals("Reclutador", StringComparison.OrdinalIgnoreCase);

            if (esReclutador)
            {
                if (string.IsNullOrWhiteSpace(txtCargo.Text))
                {
                    MessageBox.Show("Ingrese el cargo del reclutador.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEmpresa.Text))
                {
                    MessageBox.Show("Ingrese el nombre de la empresa.");
                    return false;
                }
            }

            return true;
        }

        private void label_PaisRecidencia_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarPaso2())
                return;

            // 1️⃣ INSERTAR USUARIO en tabla Usuario
            var respUsuario = usuarioRepo.Insertar(_usuarioBase);

            if (!respUsuario.Estado)
            {
                MessageBox.Show("Error al registrar el usuario:\n" + respUsuario.Mensaje,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Usuario usuarioGuardado = respUsuario.Entidad;
            int idUsuario = usuarioGuardado.IdUsuario; // SCOPE_IDENTITY() en el repo

            bool esReclutador = _tipoUsuario != null &&
                _tipoUsuario.Equals("Reclutador", StringComparison.OrdinalIgnoreCase);

            // Leemos los archivos (cedula y CV) en byte[]
            byte[] cvBytes = null;
            try
            {
                cvBytes = File.ReadAllBytes(rutaCV);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo leer el archivo de CV:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Si en algún momento quieres guardar la cédula en BD:
            // byte[] cedulaBytes = null;
            // try
            // {
            //     cedulaBytes = File.ReadAllBytes(rutaCedula);
            // }
            // catch { }

            if (!esReclutador)
            {
                // 2️⃣ CANDIDATO: insertar en tabla Candidato (hojaDeVida = bytes del CV)
                Candidato cand = new Candidato
                {
                    IdUsuario = idUsuario,
                    Nombre = usuarioGuardado.Nombre,
                    Correo = usuarioGuardado.Correo,
                    Contrasena = usuarioGuardado.Contrasena,
                    Estado = usuarioGuardado.Estado,
                    HojaDeVida = cvBytes,   // 🔹 AHORA ES BYTE[]
                    Tipox = null,
                    NivelFormacion = null,
                    Experiencia = null
                };

                var respCand = candidatoRepo.Insertar(cand);
                if (!respCand.Estado)
                {
                    MessageBox.Show("Usuario creado, pero error al registrar datos de candidato:\n" +
                                    respCand.Mensaje,
                                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // 3️⃣ RECLUTADOR: Empresa + Reclutador

                string nombreEmp = txtEmpresa.Text.Trim();

                // (a) Ver si la empresa ya existe por nombre
                Empresa empresaFinal = null;
                var respEmpTodas = empresaRepo.ObtenerTodos();

                if (respEmpTodas.Estado && respEmpTodas.Lista != null)
                {
                    foreach (var emp in respEmpTodas.Lista)
                    {
                        if (emp.Nombre.Equals(nombreEmp, StringComparison.OrdinalIgnoreCase))
                        {
                            empresaFinal = emp;
                            break;
                        }
                    }
                }

                if (empresaFinal == null)
                {
                    // (b) Crear empresa nueva (datos mínimos)
                    Empresa nuevaEmp = new Empresa
                    {
                        Nombre = nombreEmp,
                        Sector = "Sin especificar",
                        Direccion = null,
                        CorreoContacto = $"{nombreEmp.Replace(" ", "").ToLower()}@empresa.com"
                    };

                    var respEmp = empresaRepo.Insertar(nuevaEmp);
                    if (!respEmp.Estado)
                    {
                        MessageBox.Show("Error al registrar la empresa:\n" + respEmp.Mensaje,
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    empresaFinal = respEmp.Entidad;
                }

                // (c) Insertar en tabla RECLUTADOR
                Reclutador rec = new Reclutador
                {
                    IdUsuario = idUsuario,
                    Nombre = usuarioGuardado.Nombre,
                    Correo = usuarioGuardado.Correo,
                    Contrasena = usuarioGuardado.Contrasena,
                    Estado = usuarioGuardado.Estado,
                    Cargo = txtCargo.Text.Trim(),
                    IdEmpresa = empresaFinal.IdEmpresa
                };

                var respRec = reclutadorRepo.Insertar(rec);
                if (!respRec.Estado)
                {
                    MessageBox.Show("Usuario creado, pero error al registrar datos de reclutador:\n" +
                                    respRec.Mensaje,
                                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // 4️⃣ Final: volver al login
            MessageBox.Show("Registro completado correctamente.",
                            "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Principal principal = this.ParentForm as Principal;
            if (principal != null)
            {
                principal.AbrirFormulario(new InicioSecion());
            }
            else
            {
                InicioSecion login = new InicioSecion();
                login.Show();
                this.Close();
            }
        }
    }

}
