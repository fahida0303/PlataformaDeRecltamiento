using ENTITY;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUI
{
    public partial class UsuarioTipoCandidato : Form
    {
        private readonly Usuario _usuarioActual;
        private readonly int _idUsuario;
        private readonly string _tipoUsuario;

        private readonly string _connectionString =
            @"Server=LAPTOP-UVS73RFU\SQLEXPRESS;Database=JobsyDB;User Id=usr_ass;Password=psr_ass;Encrypt=False;TrustServerCertificate=True;";

        // Botón de postulación en Convocatorias
        private Button btnPostularConvocatoria;

        // ========= CONSTRUCTOR PRINCIPAL =========
        public UsuarioTipoCandidato(Usuario usuario)
        {
            InitializeComponent();

            _usuarioActual = usuario;
            _idUsuario = usuario.IdUsuario;
            _tipoUsuario = usuario.TipoUsuario;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            ConfigurarSubPaneles();
            ConfigurarMenuLateral();

            // UI INICIO
            InicializarUI();

            // UI CONVOCATORIAS
            InicializarUIConvocatorias();

            // UI POSTULACIONES
            InicializarUIPostulaciones();
        }

        // Constructor vacío SOLO para el diseñador
        public UsuarioTipoCandidato()
        {
            InitializeComponent();
        }

        // ===================== PANEL INICIO =====================

        private void InicializarUI()
        {
            string nombre = string.IsNullOrWhiteSpace(_usuarioActual?.Nombre)
                ? "candidato"
                : _usuarioActual.Nombre.ToLower();

            lblTituloInicio.Text = $"Hola, {nombre} 👋";
            lblTituloInicio.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTituloInicio.ForeColor = Color.FromArgb(20, 50, 90);
            lblTituloInicio.TextAlign = ContentAlignment.MiddleCenter;
            lblTituloInicio.AutoSize = true;

            panelInicio.BackColor = Color.FromArgb(230, 245, 255);

            panelCards.BackColor = Color.Transparent;
            panelCards.BorderStyle = BorderStyle.None;

            EstilizarCard(cardConvocatorias);
            EstilizarCard(cardPostulaciones);
            EstilizarCard(cardEntrevistas);

            EstilizarDataGridView(dgvUltimasPostulaciones);

            AjustarLayoutInicio();
            this.Resize += (s, e) =>
            {
                AjustarLayoutInicio();
                AjustarLayoutConvocatorias();
                AjustarLayoutPostulaciones();
            };

            panelCards.BringToFront();
            cardConvocatorias.BringToFront();
            cardPostulaciones.BringToFront();
            cardEntrevistas.BringToFront();
            lblTituloInicio.BringToFront();
            dgvUltimasPostulaciones.BringToFront();

            MostrarPanel(panelInicio);
            CargarDashboardInicio();
        }

        private void EstilizarCard(Panel card)
        {
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.None;
            card.Padding = new Padding(10);

            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (Pen p = new Pen(Color.FromArgb(210, 220, 230), 1))
                {
                    Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                    g.DrawRectangle(p, rect);
                }
            };

            var labels = card.Controls
                             .OfType<Label>()
                             .OrderBy(l => l.Top)
                             .ToList();

            if (labels.Count >= 1)
            {
                Label titulo = labels[0];
                titulo.Dock = DockStyle.Top;
                titulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                titulo.ForeColor = Color.FromArgb(40, 70, 120);
                titulo.TextAlign = ContentAlignment.MiddleLeft;
                titulo.Padding = new Padding(10, 10, 10, 0);
            }

            if (labels.Count >= 2)
            {
                Label valor = labels[1];
                valor.Dock = DockStyle.Fill;
                valor.Font = new Font("Segoe UI", 28, FontStyle.Bold);
                valor.ForeColor = Color.FromArgb(0, 90, 170);
                valor.TextAlign = ContentAlignment.MiddleLeft;
                valor.Padding = new Padding(10, 0, 0, 15);
            }
        }

        private void EstilizarDataGridView(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(80, 140, 200);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 34;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 230, 240);
        }

        private void AjustarLayoutInicio()
        {
            if (!panelInicio.IsHandleCreated) return;

            int ladoIzquierdo = 260;
            int margenDerecho = 40;

            int topTitulo = 25;
            int anchoDisponibleTitulo = panelInicio.Width - ladoIzquierdo - margenDerecho;
            if (anchoDisponibleTitulo < 300) anchoDisponibleTitulo = 300;

            lblTituloInicio.AutoSize = true;
            int centroX = ladoIzquierdo + anchoDisponibleTitulo / 2;
            lblTituloInicio.Location = new Point(
                centroX - lblTituloInicio.Width / 2,
                topTitulo
            );

            int cardsLeftGlobal = ladoIzquierdo;
            int cardsTopGlobal = lblTituloInicio.Bottom + 25;
            int cardsWidth = panelInicio.Width - ladoIzquierdo - margenDerecho;
            if (cardsWidth < 700) cardsWidth = 700;

            int cardsHeight = 170;

            panelCards.SetBounds(cardsLeftGlobal, cardsTopGlobal, cardsWidth, cardsHeight);

            int innerMargin = 15;
            int cardWidth = (cardsWidth - innerMargin * 4) / 3;
            cardWidth = Math.Max(cardWidth, 220);
            int cardHeight = cardsHeight - 2 * innerMargin;

            cardConvocatorias.SetBounds(innerMargin, innerMargin, cardWidth, cardHeight);
            cardPostulaciones.SetBounds(innerMargin * 2 + cardWidth, innerMargin, cardWidth, cardHeight);
            cardEntrevistas.SetBounds(
               innerMargin * 3 + cardWidth * 2,
               innerMargin,
               cardWidth,
               cardHeight
           );

            int topGrid = panelCards.Bottom + 30;
            int leftGrid = ladoIzquierdo;
            int widthGrid = panelInicio.Width - ladoIzquierdo - margenDerecho;
            if (widthGrid < 400) widthGrid = 400;

            int bottomMarginGrid = 40;
            int heightGrid = panelInicio.Height - topGrid - bottomMarginGrid;
            if (heightGrid < 180) heightGrid = 180;

            dgvUltimasPostulaciones.SetBounds(leftGrid, topGrid, widthGrid, heightGrid);
        }

        // ===================== PANEL CONVOCATORIAS =====================

        private void InicializarUIConvocatorias()
        {
            panelConvocatorias.BackColor = Color.FromArgb(230, 245, 255);

            if (lblTituloConvocatorias != null)
            {
                lblTituloConvocatorias.Text = "Convocatorias disponibles";
                lblTituloConvocatorias.Font = new Font("Segoe UI", 24, FontStyle.Bold);
                lblTituloConvocatorias.ForeColor = Color.FromArgb(20, 50, 90);
                lblTituloConvocatorias.AutoSize = true;
                lblTituloConvocatorias.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (panelConvocatoriasGrid != null)
            {
                panelConvocatoriasGrid.BackColor = Color.White;
                panelConvocatoriasGrid.Padding = new Padding(15);
                panelConvocatoriasGrid.BorderStyle = BorderStyle.None;
                panelConvocatoriasGrid.Dock = DockStyle.None;
                panelConvocatoriasGrid.Anchor =
                    AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                panelConvocatoriasGrid.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (Pen p = new Pen(Color.FromArgb(210, 220, 230), 1))
                    {
                        Rectangle rect = new Rectangle(0, 0, panelConvocatoriasGrid.Width - 1, panelConvocatoriasGrid.Height - 1);
                        g.DrawRectangle(p, rect);
                    }
                };
            }

            EstilizarDataGridView(dgvConvocatorias);
            dgvConvocatorias.RowTemplate.Height = 60;
            dgvConvocatorias.DefaultCellStyle.Padding = new Padding(8, 12, 8, 12);
            dgvConvocatorias.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvConvocatorias.Dock = DockStyle.Fill;

            // Botón POSTULARME
            btnPostularConvocatoria = new Button();
            btnPostularConvocatoria.Text = "Postularme";
            btnPostularConvocatoria.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnPostularConvocatoria.BackColor = Color.FromArgb(0, 120, 215);
            btnPostularConvocatoria.ForeColor = Color.White;
            btnPostularConvocatoria.FlatStyle = FlatStyle.Flat;
            btnPostularConvocatoria.FlatAppearance.BorderSize = 0;
            btnPostularConvocatoria.Cursor = Cursors.Hand;
            btnPostularConvocatoria.Enabled = false;
            btnPostularConvocatoria.Anchor = AnchorStyles.Bottom;

            btnPostularConvocatoria.Click += BtnPostularConvocatoria_Click;

            panelConvocatorias.Controls.Add(btnPostularConvocatoria);

            dgvConvocatorias.SelectionChanged += (s, e) =>
            {
                btnPostularConvocatoria.Enabled = dgvConvocatorias.CurrentRow != null;
            };

            AjustarLayoutConvocatorias();
        }

        private void AjustarLayoutConvocatorias()
        {
            if (!panelConvocatorias.IsHandleCreated || panelConvocatoriasGrid == null) return;

            int margenIzquierdo = 25;
            int margenDerecho = 25;
            int margenSuperiorTitulo = 20;
            int espacioTituloGrid = 10;
            int margenInferior = 30;
            int altoBoton = 55;
            int espacioBotonGrid = 15;

            int topTitulo = margenSuperiorTitulo;
            if (lblTituloConvocatorias != null)
            {
                lblTituloConvocatorias.Location = new Point(margenIzquierdo, topTitulo);
            }

            int topGrid = (lblTituloConvocatorias != null
                ? lblTituloConvocatorias.Bottom + espacioTituloGrid
                : 60);

            int leftGrid = margenIzquierdo;
            int widthGrid = panelConvocatorias.ClientSize.Width - margenIzquierdo - margenDerecho;

            int heightGrid = panelConvocatorias.ClientSize.Height
                             - topGrid
                             - margenInferior
                             - altoBoton
                             - espacioBotonGrid;

            if (heightGrid < 150) heightGrid = 150;

            panelConvocatoriasGrid.SetBounds(leftGrid, topGrid, widthGrid, heightGrid);

            if (btnPostularConvocatoria != null)
            {
                int btnWidth = 240;
                int btnHeight = altoBoton;

                int x = (panelConvocatorias.ClientSize.Width - btnWidth) / 2;
                int y = panelConvocatorias.ClientSize.Height - btnHeight - margenInferior;

                btnPostularConvocatoria.SetBounds(x, y, btnWidth, btnHeight);
                btnPostularConvocatoria.BringToFront();
            }
        }

        // ===================== PANEL POSTULACIONES =====================

        private void InicializarUIPostulaciones()
        {
            panelPostulaciones.BackColor = Color.FromArgb(230, 245, 255);

            if (lblTituloPostulaciones != null)
            {
                lblTituloPostulaciones.Text = "Mis postulaciones";
                lblTituloPostulaciones.Font = new Font("Segoe UI", 24, FontStyle.Bold);
                lblTituloPostulaciones.ForeColor = Color.FromArgb(20, 50, 90);
                lblTituloPostulaciones.AutoSize = true;
                lblTituloPostulaciones.TextAlign = ContentAlignment.MiddleLeft;
            }

            if (panelPostulacionesGrid != null)
            {
                panelPostulacionesGrid.BackColor = Color.White;
                panelPostulacionesGrid.Padding = new Padding(15);
                panelPostulacionesGrid.BorderStyle = BorderStyle.None;
                panelPostulacionesGrid.Dock = DockStyle.None;
                panelPostulacionesGrid.Anchor =
                    AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                panelPostulacionesGrid.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    using (Pen p = new Pen(Color.FromArgb(210, 220, 230), 1))
                    {
                        Rectangle rect = new Rectangle(0, 0, panelPostulacionesGrid.Width - 1, panelPostulacionesGrid.Height - 1);
                        g.DrawRectangle(p, rect);
                    }
                };
            }

            EstilizarDataGridView(dgvPostulaciones);
            dgvPostulaciones.RowTemplate.Height = 60;
            dgvPostulaciones.DefaultCellStyle.Padding = new Padding(8, 12, 8, 12);
            dgvPostulaciones.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvPostulaciones.Dock = DockStyle.Fill;

            AjustarLayoutPostulaciones();
        }

        private void AjustarLayoutPostulaciones()
        {
            if (!panelPostulaciones.IsHandleCreated || panelPostulacionesGrid == null) return;

            int margenIzquierdo = 25;
            int margenDerecho = 25;
            int margenSuperiorTitulo = 20;
            int espacioTituloGrid = 10;
            int margenInferior = 25;

            int topTitulo = margenSuperiorTitulo;
            if (lblTituloPostulaciones != null)
            {
                lblTituloPostulaciones.Location = new Point(margenIzquierdo, topTitulo);
            }

            int topGrid = (lblTituloPostulaciones != null
                ? lblTituloPostulaciones.Bottom + espacioTituloGrid
                : 60);

            int leftGrid = margenIzquierdo;
            int widthGrid = panelPostulaciones.ClientSize.Width - margenIzquierdo - margenDerecho;
            int heightGrid = panelPostulaciones.ClientSize.Height - topGrid - margenInferior;

            if (heightGrid < 150) heightGrid = 150;

            panelPostulacionesGrid.SetBounds(leftGrid, topGrid, widthGrid, heightGrid);
        }

        // ===================== MENÚ LATERAL =====================

        private void ConfigurarMenuLateral()
        {
            btnInicio.Click += (s, e) =>
            {
                MostrarPanel(panelInicio);
                CargarDashboardInicio();
            };

            btnConvocatorias.Click += (s, e) =>
            {
                MostrarPanel(panelConvocatorias);
                CargarConvocatoriasParaCandidato();
                AjustarLayoutConvocatorias();
            };

            btnPostulaciones.Click += (s, e) =>
            {
                MostrarPanel(panelPostulaciones);
                CargarPostulacionesCandidato();
                AjustarLayoutPostulaciones();
            };

            btnCalendario.Click += (s, e) =>
            {
                MostrarPanel(panelCalendario);
                CargarCalendarioCandidato();
            };
        }

        private void ConfigurarSubPaneles()
        {
            panelInicio.Dock = DockStyle.Fill;
            panelConvocatorias.Dock = DockStyle.Fill;
            panelPostulaciones.Dock = DockStyle.Fill;
            panelCalendario.Dock = DockStyle.Fill;

            panelInicio.Visible = true;
            panelConvocatorias.Visible = false;
            panelPostulaciones.Visible = false;
            panelCalendario.Visible = false;
        }

        private void MostrarPanel(Panel panel)
        {
            panelInicio.Visible = false;
            panelConvocatorias.Visible = false;
            panelPostulaciones.Visible = false;
            panelCalendario.Visible = false;

            panel.Visible = true;
            panel.BringToFront();
        }

        // ===================== DASHBOARD INICIO =====================

        private void CargarDashboardInicio()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    int convocatoriasAbiertas;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Convocatoria WHERE estado = 'Abierta';", con))
                    {
                        convocatoriasAbiertas = (int)cmd.ExecuteScalar();
                    }

                    int postulacionesTotales;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Postulacion WHERE idCandidato = @idCandidato;", con))
                    {
                        cmd.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        postulacionesTotales = (int)cmd.ExecuteScalar();
                    }

                    int entrevistasPendientes;
                    string sqlEntrevistas = @"
                        SELECT COUNT(*) 
                        FROM Reunion
                        WHERE idCandidato = @idCandidato
                          AND fecha >= CAST(GETDATE() AS date)
                          AND estadoConfirmacion IN ('Pendiente', 'Programada');";

                    using (SqlCommand cmd = new SqlCommand(sqlEntrevistas, con))
                    {
                        cmd.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        entrevistasPendientes = (int)cmd.ExecuteScalar();
                    }

                    string sqlLista = @"
                        SELECT TOP 5 
                               p.idPostulacion,
                               c.titulo AS Convocatoria,
                               p.estado,
                               p.fecha_postulacion
                        FROM Postulacion p
                        INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                        WHERE p.idCandidato = @idCandidato
                        ORDER BY p.fecha_postulacion DESC;";

                    DataTable tabla = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlLista, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        da.Fill(tabla);
                    }

                    lblConvocatoriasAbiertas.Text = convocatoriasAbiertas.ToString();
                    lblPostulacionesTotales.Text = postulacionesTotales.ToString();
                    lblEntrevistasPendientes.Text = entrevistasPendientes.ToString();

                    dgvUltimasPostulaciones.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el inicio del candidato:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== CONVOCATORIAS =====================

        private void CargarConvocatoriasParaCandidato()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT 
                            idConvocatoria,
                            titulo,
                            descripcion,
                            fechaPublicacion,
                            fechaLimite,
                            estado
                        FROM Convocatoria
                        WHERE estado = 'Abierta'
                        ORDER BY fechaPublicacion DESC;";

                    DataTable tabla = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                    {
                        da.Fill(tabla);
                    }

                    dgvConvocatorias.DataSource = tabla;

                    if (dgvConvocatorias.Columns["idConvocatoria"] != null)
                        dgvConvocatorias.Columns["idConvocatoria"].HeaderText = "ID";

                    if (dgvConvocatorias.Columns["titulo"] != null)
                        dgvConvocatorias.Columns["titulo"].HeaderText = "Título";

                    if (dgvConvocatorias.Columns["descripcion"] != null)
                        dgvConvocatorias.Columns["descripcion"].HeaderText = "Descripción";

                    if (dgvConvocatorias.Columns["fechaPublicacion"] != null)
                        dgvConvocatorias.Columns["fechaPublicacion"].HeaderText = "Publicación";

                    if (dgvConvocatorias.Columns["fechaLimite"] != null)
                        dgvConvocatorias.Columns["fechaLimite"].HeaderText = "Fecha límite";

                    if (dgvConvocatorias.Columns["estado"] != null)
                        dgvConvocatorias.Columns["estado"].HeaderText = "Estado";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las convocatorias:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPostularConvocatoria_Click(object sender, EventArgs e)
        {
            if (dgvConvocatorias.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una convocatoria antes de postularte.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idConvocatoria;
            try
            {
                idConvocatoria = Convert.ToInt32(
                    dgvConvocatorias.CurrentRow.Cells["idConvocatoria"].Value);
            }
            catch
            {
                idConvocatoria = Convert.ToInt32(
                    dgvConvocatorias.CurrentRow.Cells[0].Value);
            }

            var confirm = MessageBox.Show(
                "¿Deseas postularte a esta convocatoria?",
                "Confirmar postulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    string sqlCheck = @"
                        SELECT COUNT(*) 
                        FROM Postulacion
                        WHERE idCandidato = @idCandidato
                          AND idConvocatoria = @idConvocatoria;";

                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        cmdCheck.Parameters.AddWithValue("@idConvocatoria", idConvocatoria);
                        int yaExiste = (int)cmdCheck.ExecuteScalar();

                        if (yaExiste > 0)
                        {
                            MessageBox.Show("Ya te has postulado a esta convocatoria.",
                                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    string sqlInsert = @"
                        INSERT INTO Postulacion
                            (idCandidato, idConvocatoria, estado, fecha_postulacion)
                        VALUES
                            (@idCandidato, @idConvocatoria, 'En revisión', GETDATE());";

                    using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        cmdInsert.Parameters.AddWithValue("@idConvocatoria", idConvocatoria);
                        cmdInsert.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Te has postulado correctamente.",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarDashboardInicio();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al postularse:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== POSTULACIONES =====================

        private void CargarPostulacionesCandidato()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT 
                            p.idPostulacion,
                            c.titulo       AS Convocatoria,
                            p.estado,
                            p.fecha_postulacion,
                            c.fechaLimite
                        FROM Postulacion p
                        INNER JOIN Convocatoria c 
                            ON p.idConvocatoria = c.idConvocatoria
                        WHERE p.idCandidato = @idCandidato
                        ORDER BY p.fecha_postulacion DESC;";

                    DataTable tabla = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@idCandidato", _idUsuario);
                        da.Fill(tabla);
                    }

                    dgvPostulaciones.DataSource = tabla;

                    if (dgvPostulaciones.Columns["idPostulacion"] != null)
                        dgvPostulaciones.Columns["idPostulacion"].HeaderText = "ID";

                    if (dgvPostulaciones.Columns["Convocatoria"] != null)
                        dgvPostulaciones.Columns["Convocatoria"].HeaderText = "Convocatoria";

                    if (dgvPostulaciones.Columns["estado"] != null)
                        dgvPostulaciones.Columns["estado"].HeaderText = "Estado";

                    if (dgvPostulaciones.Columns["fecha_postulacion"] != null)
                        dgvPostulaciones.Columns["fecha_postulacion"].HeaderText = "Fecha de postulación";

                    if (dgvPostulaciones.Columns["fechaLimite"] != null)
                        dgvPostulaciones.Columns["fechaLimite"].HeaderText = "Fecha límite";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tus postulaciones:\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== CALENDARIO (por hacer) =====================

        private void CargarCalendarioCandidato()
        {
            // TODO
        }

        // ===================== COSAS DE VENTANA =====================

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private static extern void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private static extern void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void BarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void bttInicio_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelInicio);
            CargarDashboardInicio();
        }

        private void bttConvocatorias_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelConvocatorias);
            CargarConvocatoriasParaCandidato();
            AjustarLayoutConvocatorias();
        }

        private void bttPostulaciones_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelPostulaciones);
            CargarPostulacionesCandidato();
            AjustarLayoutPostulaciones();
        }

        private void btnCalendario_Click(object sender, EventArgs e)
        {
            MostrarPanel(panelCalendario);
            CargarCalendarioCandidato();
        }
    }
}
