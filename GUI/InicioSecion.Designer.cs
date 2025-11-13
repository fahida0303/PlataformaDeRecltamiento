namespace GUI
{
    partial class InicioSecion
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
            this.panel_IniciarSesion = new System.Windows.Forms.Panel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.bttContinuar = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.linkLabel_Olvidar = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_Contraseña = new System.Windows.Forms.Label();
            this.label_NombreUsuario = new System.Windows.Forms.Label();
            this.label_IniciarSesion = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.BarraTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).BeginInit();
            this.panelContenedor.SuspendLayout();
            this.panel_IniciarSesion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.panelContenedor.BackgroundImage = global::GUI.Properties.Resources.montifer2;
            this.panelContenedor.Controls.Add(this.panel_IniciarSesion);
            this.panelContenedor.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelContenedor.ForeColor = System.Drawing.Color.Brown;
            this.panelContenedor.Location = new System.Drawing.Point(0, 35);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1300, 615);
            this.panelContenedor.TabIndex = 2;
            // 
            // panel_IniciarSesion
            // 
            this.panel_IniciarSesion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_IniciarSesion.BackColor = System.Drawing.Color.Transparent;
            this.panel_IniciarSesion.BackgroundImage = global::GUI.Properties.Resources.image_removebg_preview__4_;
            this.panel_IniciarSesion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_IniciarSesion.Controls.Add(this.textBox2);
            this.panel_IniciarSesion.Controls.Add(this.pictureBox5);
            this.panel_IniciarSesion.Controls.Add(this.bttContinuar);
            this.panel_IniciarSesion.Controls.Add(this.textBox1);
            this.panel_IniciarSesion.Controls.Add(this.pictureBox4);
            this.panel_IniciarSesion.Controls.Add(this.linkLabel1);
            this.panel_IniciarSesion.Controls.Add(this.label1);
            this.panel_IniciarSesion.Controls.Add(this.pictureBox3);
            this.panel_IniciarSesion.Controls.Add(this.linkLabel_Olvidar);
            this.panel_IniciarSesion.Controls.Add(this.pictureBox2);
            this.panel_IniciarSesion.Controls.Add(this.pictureBox1);
            this.panel_IniciarSesion.Controls.Add(this.label_Contraseña);
            this.panel_IniciarSesion.Controls.Add(this.label_NombreUsuario);
            this.panel_IniciarSesion.Controls.Add(this.label_IniciarSesion);
            this.panel_IniciarSesion.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.panel_IniciarSesion.Location = new System.Drawing.Point(161, 30);
            this.panel_IniciarSesion.Name = "panel_IniciarSesion";
            this.panel_IniciarSesion.Size = new System.Drawing.Size(1040, 556);
            this.panel_IniciarSesion.TabIndex = 0;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::GUI.Properties.Resources.JobsyLogo_removebg_preview;
            this.pictureBox5.Location = new System.Drawing.Point(25, 33);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(237, 108);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 14;
            this.pictureBox5.TabStop = false;
            // 
            // bttContinuar
            // 
            this.bttContinuar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttContinuar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bttContinuar.FlatAppearance.BorderSize = 0;
            this.bttContinuar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttContinuar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttContinuar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttContinuar.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttContinuar.ForeColor = System.Drawing.Color.White;
            this.bttContinuar.Location = new System.Drawing.Point(712, 402);
            this.bttContinuar.Name = "bttContinuar";
            this.bttContinuar.Size = new System.Drawing.Size(146, 29);
            this.bttContinuar.TabIndex = 13;
            this.bttContinuar.Text = "Continuar";
            this.bttContinuar.UseVisualStyleBackColor = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(64)))), ((int)(((byte)(108)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.Menu;
            this.textBox1.Location = new System.Drawing.Point(630, 189);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(292, 25);
            this.textBox1.TabIndex = 11;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::GUI.Properties.Resources.Copia_de_Montifer__6_1;
            this.pictureBox4.Location = new System.Drawing.Point(68, 125);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(363, 332);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel1.Font = new System.Drawing.Font("Cascadia Code Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.ForeColor = System.Drawing.Color.Gainsboro;
            this.linkLabel1.LinkColor = System.Drawing.Color.Lavender;
            this.linkLabel1.Location = new System.Drawing.Point(830, 451);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(96, 17);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Registrarse";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.Font = new System.Drawing.Font("Cascadia Code Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1.Location = new System.Drawing.Point(651, 451);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "¿No tienes una cuenta? ";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::GUI.Properties.Resources.image_removebg_preview__5_;
            this.pictureBox3.Location = new System.Drawing.Point(686, 386);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(194, 59);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 6;
            this.pictureBox3.TabStop = false;
            // 
            // linkLabel_Olvidar
            // 
            this.linkLabel_Olvidar.AutoSize = true;
            this.linkLabel_Olvidar.Font = new System.Drawing.Font("Cascadia Code Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel_Olvidar.LinkColor = System.Drawing.Color.Lavender;
            this.linkLabel_Olvidar.Location = new System.Drawing.Point(754, 342);
            this.linkLabel_Olvidar.Name = "linkLabel_Olvidar";
            this.linkLabel_Olvidar.Size = new System.Drawing.Size(208, 17);
            this.linkLabel_Olvidar.TabIndex = 5;
            this.linkLabel_Olvidar.TabStop = true;
            this.linkLabel_Olvidar.Text = "¿Olvidaste tu contraseña?";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox2.Image = global::GUI.Properties.Resources.image_removebg_preview__4_1;
            this.pictureBox2.Location = new System.Drawing.Point(602, 274);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(347, 51);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox1.Image = global::GUI.Properties.Resources.image_removebg_preview__4_1;
            this.pictureBox1.Location = new System.Drawing.Point(602, 176);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(347, 51);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label_Contraseña
            // 
            this.label_Contraseña.AutoSize = true;
            this.label_Contraseña.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Contraseña.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_Contraseña.Location = new System.Drawing.Point(607, 249);
            this.label_Contraseña.Name = "label_Contraseña";
            this.label_Contraseña.Size = new System.Drawing.Size(110, 22);
            this.label_Contraseña.TabIndex = 2;
            this.label_Contraseña.Text = "Contraseña";
            // 
            // label_NombreUsuario
            // 
            this.label_NombreUsuario.AutoSize = true;
            this.label_NombreUsuario.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_NombreUsuario.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_NombreUsuario.Location = new System.Drawing.Point(607, 151);
            this.label_NombreUsuario.Name = "label_NombreUsuario";
            this.label_NombreUsuario.Size = new System.Drawing.Size(180, 22);
            this.label_NombreUsuario.TabIndex = 1;
            this.label_NombreUsuario.Text = "Nombre de Usuario";
            // 
            // label_IniciarSesion
            // 
            this.label_IniciarSesion.AutoSize = true;
            this.label_IniciarSesion.Font = new System.Drawing.Font("Cascadia Code", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_IniciarSesion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_IniciarSesion.Location = new System.Drawing.Point(681, 61);
            this.label_IniciarSesion.Name = "label_IniciarSesion";
            this.label_IniciarSesion.Size = new System.Drawing.Size(195, 30);
            this.label_IniciarSesion.TabIndex = 0;
            this.label_IniciarSesion.Text = "Iniciar Sesión";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(64)))), ((int)(((byte)(108)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.SystemColors.Menu;
            this.textBox2.Location = new System.Drawing.Point(630, 286);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(292, 25);
            this.textBox2.TabIndex = 15;
            // 
            // InicioSecion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 650);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.BarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InicioSecion";
            this.Text = "UsuarioTipo";
            this.BarraTitulo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).EndInit();
            this.panelContenedor.ResumeLayout(false);
            this.panel_IniciarSesion.ResumeLayout(false);
            this.panel_IniciarSesion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BarraTitulo;
        private System.Windows.Forms.PictureBox btnMinimizar;
        private System.Windows.Forms.PictureBox btnCerrar;
        private System.Windows.Forms.PictureBox btnRestaurar;
        private System.Windows.Forms.PictureBox btnMaximizar;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.Panel panel_IniciarSesion;
        private System.Windows.Forms.Label label_IniciarSesion;
        private System.Windows.Forms.Label label_NombreUsuario;
        private System.Windows.Forms.Label label_Contraseña;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.LinkLabel linkLabel_Olvidar;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bttContinuar;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.TextBox textBox2;
    }
}