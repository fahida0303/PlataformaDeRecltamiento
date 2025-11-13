namespace GUI
{
    partial class UsuarioTipoCandidato
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BarraTitulo = new System.Windows.Forms.Panel();
            this.btnMaximizar = new System.Windows.Forms.PictureBox();
            this.btnMinimizar = new System.Windows.Forms.PictureBox();
            this.btnCerrar = new System.Windows.Forms.PictureBox();
            this.btnRestaurar = new System.Windows.Forms.PictureBox();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.Menu = new System.Windows.Forms.Panel();
            this.lb_TipoUsuario = new System.Windows.Forms.Label();
            this.lb_NombreUsuario = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.bttCalendario = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bttInicio = new System.Windows.Forms.Button();
            this.bttConvocatorias = new System.Windows.Forms.Button();
            this.pBox_JobsyLogo = new System.Windows.Forms.PictureBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.BarraTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).BeginInit();
            this.panelContenedor.SuspendLayout();
            this.Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_JobsyLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // BarraTitulo
            // 
            this.BarraTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.BarraTitulo.Controls.Add(this.btnMaximizar);
            this.BarraTitulo.Controls.Add(this.btnMinimizar);
            this.BarraTitulo.Controls.Add(this.btnCerrar);
            this.BarraTitulo.Controls.Add(this.btnRestaurar);
            this.BarraTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.BarraTitulo.Location = new System.Drawing.Point(0, 0);
            this.BarraTitulo.Name = "BarraTitulo";
            this.BarraTitulo.Size = new System.Drawing.Size(1300, 35);
            this.BarraTitulo.TabIndex = 0;
            this.BarraTitulo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BarraTitulo_MouseDown);
            // 
            // btnMaximizar
            // 
            this.btnMaximizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximizar.Image = global::GUI.Properties.Resources.cuadricula22;
            this.btnMaximizar.Location = new System.Drawing.Point(1225, 4);
            this.btnMaximizar.Name = "btnMaximizar";
            this.btnMaximizar.Size = new System.Drawing.Size(25, 25);
            this.btnMaximizar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMaximizar.TabIndex = 4;
            this.btnMaximizar.TabStop = false;
            this.btnMaximizar.Visible = false;
            this.btnMaximizar.Click += new System.EventHandler(this.btnMaximizar_Click);
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimizar.Image = global::GUI.Properties.Resources.minimizar_signo;
            this.btnMinimizar.Location = new System.Drawing.Point(1190, 5);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(25, 25);
            this.btnMinimizar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimizar.TabIndex = 1;
            this.btnMinimizar.TabStop = false;
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrar.Image = global::GUI.Properties.Resources.boton_x;
            this.btnCerrar.Location = new System.Drawing.Point(1263, 5);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(25, 25);
            this.btnCerrar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.TabStop = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestaurar.Image = global::GUI.Properties.Resources.cuadricula;
            this.btnRestaurar.Location = new System.Drawing.Point(1225, 4);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(25, 25);
            this.btnRestaurar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnRestaurar.TabIndex = 3;
            this.btnRestaurar.TabStop = false;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(195)))), ((int)(((byte)(242)))));
            this.panelContenedor.BackgroundImage = global::GUI.Properties.Resources.Copia_de_Montifer_pages_to_jpg_0001;
            this.panelContenedor.Controls.Add(this.panel6);
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(305, 35);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(995, 615);
            this.panelContenedor.TabIndex = 2;
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(173)))));
            this.Menu.BackgroundImage = global::GUI.Properties.Resources.Captura_de_pantalla_2025_11_08_040129;
            this.Menu.Controls.Add(this.lb_TipoUsuario);
            this.Menu.Controls.Add(this.lb_NombreUsuario);
            this.Menu.Controls.Add(this.panel7);
            this.Menu.Controls.Add(this.bttCalendario);
            this.Menu.Controls.Add(this.panel4);
            this.Menu.Controls.Add(this.panel2);
            this.Menu.Controls.Add(this.pictureBox2);
            this.Menu.Controls.Add(this.pictureBox1);
            this.Menu.Controls.Add(this.panel1);
            this.Menu.Controls.Add(this.bttInicio);
            this.Menu.Controls.Add(this.bttConvocatorias);
            this.Menu.Controls.Add(this.pBox_JobsyLogo);
            this.Menu.Dock = System.Windows.Forms.DockStyle.Left;
            this.Menu.Location = new System.Drawing.Point(0, 35);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(305, 615);
            this.Menu.TabIndex = 1;
            // 
            // lb_TipoUsuario
            // 
            this.lb_TipoUsuario.AutoSize = true;
            this.lb_TipoUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(198)))), ((int)(((byte)(222)))));
            this.lb_TipoUsuario.Font = new System.Drawing.Font("Cascadia Code Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_TipoUsuario.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lb_TipoUsuario.Location = new System.Drawing.Point(60, 580);
            this.lb_TipoUsuario.Name = "lb_TipoUsuario";
            this.lb_TipoUsuario.Size = new System.Drawing.Size(80, 17);
            this.lb_TipoUsuario.TabIndex = 20;
            this.lb_TipoUsuario.Text = "Candidato";
            // 
            // lb_NombreUsuario
            // 
            this.lb_NombreUsuario.AutoSize = true;
            this.lb_NombreUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(198)))), ((int)(((byte)(222)))));
            this.lb_NombreUsuario.Font = new System.Drawing.Font("Cascadia Code", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_NombreUsuario.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lb_NombreUsuario.Location = new System.Drawing.Point(60, 564);
            this.lb_NombreUsuario.Name = "lb_NombreUsuario";
            this.lb_NombreUsuario.Size = new System.Drawing.Size(64, 17);
            this.lb_NombreUsuario.TabIndex = 19;
            this.lb_NombreUsuario.Text = "Usuario";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.panel7.Location = new System.Drawing.Point(12, 316);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(282, 2);
            this.panel7.TabIndex = 17;
            // 
            // bttCalendario
            // 
            this.bttCalendario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttCalendario.FlatAppearance.BorderSize = 0;
            this.bttCalendario.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttCalendario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttCalendario.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttCalendario.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.bttCalendario.Location = new System.Drawing.Point(12, 270);
            this.bttCalendario.Name = "bttCalendario";
            this.bttCalendario.Size = new System.Drawing.Size(282, 50);
            this.bttCalendario.TabIndex = 11;
            this.bttCalendario.Text = "Calendario";
            this.bttCalendario.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.panel4.Location = new System.Drawing.Point(13, 268);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(281, 2);
            this.panel4.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.panel2.Location = new System.Drawing.Point(12, 218);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(282, 2);
            this.panel2.TabIndex = 3;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox2.Image = global::GUI.Properties.Resources.icono_Usuariotipo_removebg_preview;
            this.pictureBox2.Location = new System.Drawing.Point(3, 539);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(216, 76);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 14;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::GUI.Properties.Resources.tuerca;
            this.pictureBox1.Location = new System.Drawing.Point(225, 553);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.panel1.Location = new System.Drawing.Point(12, 170);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 2);
            this.panel1.TabIndex = 2;
            // 
            // bttInicio
            // 
            this.bttInicio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttInicio.FlatAppearance.BorderSize = 0;
            this.bttInicio.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttInicio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttInicio.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttInicio.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.bttInicio.Location = new System.Drawing.Point(12, 170);
            this.bttInicio.Name = "bttInicio";
            this.bttInicio.Size = new System.Drawing.Size(282, 50);
            this.bttInicio.TabIndex = 1;
            this.bttInicio.Text = "Inicio";
            this.bttInicio.UseVisualStyleBackColor = false;
            // 
            // bttConvocatorias
            // 
            this.bttConvocatorias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttConvocatorias.FlatAppearance.BorderSize = 0;
            this.bttConvocatorias.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(19)))), ((int)(((byte)(69)))), ((int)(((byte)(105)))));
            this.bttConvocatorias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttConvocatorias.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttConvocatorias.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.bttConvocatorias.Location = new System.Drawing.Point(11, 220);
            this.bttConvocatorias.Name = "bttConvocatorias";
            this.bttConvocatorias.Size = new System.Drawing.Size(282, 50);
            this.bttConvocatorias.TabIndex = 5;
            this.bttConvocatorias.Text = "Convocatorias";
            this.bttConvocatorias.UseVisualStyleBackColor = false;
            // 
            // pBox_JobsyLogo
            // 
            this.pBox_JobsyLogo.Image = global::GUI.Properties.Resources.jobsitransparentet;
            this.pBox_JobsyLogo.Location = new System.Drawing.Point(-92, -36);
            this.pBox_JobsyLogo.Margin = new System.Windows.Forms.Padding(0);
            this.pBox_JobsyLogo.Name = "pBox_JobsyLogo";
            this.pBox_JobsyLogo.Size = new System.Drawing.Size(483, 242);
            this.pBox_JobsyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBox_JobsyLogo.TabIndex = 0;
            this.pBox_JobsyLogo.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel6.Location = new System.Drawing.Point(33, 20);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(937, 577);
            this.panel6.TabIndex = 3;
            // 
            // UsuarioTipoCandidato
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 650);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.Menu);
            this.Controls.Add(this.BarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "UsuarioTipoCandidato";
            this.Text = "UsuarioTipo";
            this.BarraTitulo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).EndInit();
            this.panelContenedor.ResumeLayout(false);
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_JobsyLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BarraTitulo;
        private System.Windows.Forms.Panel Menu;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.PictureBox btnMinimizar;
        private System.Windows.Forms.PictureBox btnCerrar;
        private System.Windows.Forms.PictureBox btnRestaurar;
        private System.Windows.Forms.PictureBox btnMaximizar;
        private System.Windows.Forms.PictureBox pBox_JobsyLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bttInicio;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button bttConvocatorias;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label lb_NombreUsuario;
        private System.Windows.Forms.Label lb_TipoUsuario;
        private System.Windows.Forms.Button bttCalendario;
        private System.Windows.Forms.Panel panel6;
    }
}