namespace GUI
{
    partial class RegistrarDos
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
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.btnMaximizar = new System.Windows.Forms.PictureBox();
            this.btnMinimizar = new System.Windows.Forms.PictureBox();
            this.btnCerrar = new System.Windows.Forms.PictureBox();
            this.btnRestaurar = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pBox_DocCV = new System.Windows.Forms.PictureBox();
            this.label_IniciarSesion = new System.Windows.Forms.Label();
            this.label_Datos = new System.Windows.Forms.Label();
            this.txt_PResidencia = new System.Windows.Forms.TextBox();
            this.bttSalvarContinuar = new System.Windows.Forms.Button();
            this.label_AñadirCV = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txt_CResidencia = new System.Windows.Forms.TextBox();
            this.pBox_DocID = new System.Windows.Forms.PictureBox();
            this.label_AñadirID = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.BarraTitulo.SuspendLayout();
            this.panelContenedor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_DocCV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_DocID)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
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
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(195)))), ((int)(((byte)(242)))));
            this.panelContenedor.BackgroundImage = global::GUI.Properties.Resources.Copia_de_Montifer_pages_to_jpg_0001;
            this.panelContenedor.Controls.Add(this.panel1);
            this.panelContenedor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelContenedor.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelContenedor.Location = new System.Drawing.Point(0, 35);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1300, 615);
            this.panelContenedor.TabIndex = 2;
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
            // pictureBox7
            // 
            this.pictureBox7.Image = global::GUI.Properties.Resources.barra_2;
            this.pictureBox7.Location = new System.Drawing.Point(67, 163);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(275, 42);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 9;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::GUI.Properties.Resources.boton_1;
            this.pictureBox8.Location = new System.Drawing.Point(233, 498);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(257, 63);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox8.TabIndex = 10;
            this.pictureBox8.TabStop = false;
            // 
            // pBox_DocCV
            // 
            this.pBox_DocCV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBox_DocCV.Image = global::GUI.Properties.Resources.documento_transparente1;
            this.pBox_DocCV.Location = new System.Drawing.Point(419, 264);
            this.pBox_DocCV.Name = "pBox_DocCV";
            this.pBox_DocCV.Size = new System.Drawing.Size(172, 148);
            this.pBox_DocCV.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBox_DocCV.TabIndex = 11;
            this.pBox_DocCV.TabStop = false;
            // 
            // label_IniciarSesion
            // 
            this.label_IniciarSesion.AutoSize = true;
            this.label_IniciarSesion.Font = new System.Drawing.Font("Cascadia Code", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_IniciarSesion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_IniciarSesion.Location = new System.Drawing.Point(290, 47);
            this.label_IniciarSesion.Name = "label_IniciarSesion";
            this.label_IniciarSesion.Size = new System.Drawing.Size(156, 30);
            this.label_IniciarSesion.TabIndex = 12;
            this.label_IniciarSesion.Text = "Registrarse";
            // 
            // label_Datos
            // 
            this.label_Datos.AutoSize = true;
            this.label_Datos.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Datos.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_Datos.Location = new System.Drawing.Point(78, 112);
            this.label_Datos.Name = "label_Datos";
            this.label_Datos.Size = new System.Drawing.Size(72, 27);
            this.label_Datos.TabIndex = 16;
            this.label_Datos.Text = "Datos";
            // 
            // txt_PResidencia
            // 
            this.txt_PResidencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(65)))), ((int)(((byte)(108)))));
            this.txt_PResidencia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_PResidencia.Font = new System.Drawing.Font("Cascadia Code", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_PResidencia.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.txt_PResidencia.Location = new System.Drawing.Point(90, 175);
            this.txt_PResidencia.Name = "txt_PResidencia";
            this.txt_PResidencia.Size = new System.Drawing.Size(230, 16);
            this.txt_PResidencia.TabIndex = 22;
            this.txt_PResidencia.Text = "Pais de Residencia";
            // 
            // bttSalvarContinuar
            // 
            this.bttSalvarContinuar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttSalvarContinuar.FlatAppearance.BorderSize = 0;
            this.bttSalvarContinuar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttSalvarContinuar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(207)))), ((int)(((byte)(188)))));
            this.bttSalvarContinuar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttSalvarContinuar.Font = new System.Drawing.Font("Cascadia Code", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttSalvarContinuar.Location = new System.Drawing.Point(244, 514);
            this.bttSalvarContinuar.Name = "bttSalvarContinuar";
            this.bttSalvarContinuar.Size = new System.Drawing.Size(231, 29);
            this.bttSalvarContinuar.TabIndex = 23;
            this.bttSalvarContinuar.Text = "Salvar y Continuar";
            this.bttSalvarContinuar.UseVisualStyleBackColor = false;
            // 
            // label_AñadirCV
            // 
            this.label_AñadirCV.AutoSize = true;
            this.label_AñadirCV.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AñadirCV.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_AñadirCV.Location = new System.Drawing.Point(457, 415);
            this.label_AñadirCV.Name = "label_AñadirCV";
            this.label_AñadirCV.Size = new System.Drawing.Size(100, 22);
            this.label_AñadirCV.TabIndex = 24;
            this.label_AñadirCV.Text = "Añadir CV";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GUI.Properties.Resources.barra_2;
            this.pictureBox1.Location = new System.Drawing.Point(366, 163);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(275, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // txt_CResidencia
            // 
            this.txt_CResidencia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(65)))), ((int)(((byte)(108)))));
            this.txt_CResidencia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_CResidencia.Font = new System.Drawing.Font("Cascadia Code", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_CResidencia.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.txt_CResidencia.Location = new System.Drawing.Point(389, 175);
            this.txt_CResidencia.Name = "txt_CResidencia";
            this.txt_CResidencia.Size = new System.Drawing.Size(230, 16);
            this.txt_CResidencia.TabIndex = 26;
            this.txt_CResidencia.Text = "Ciudad de Residencia";
            // 
            // pBox_DocID
            // 
            this.pBox_DocID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBox_DocID.Image = global::GUI.Properties.Resources.documento_transparente1;
            this.pBox_DocID.Location = new System.Drawing.Point(104, 264);
            this.pBox_DocID.Name = "pBox_DocID";
            this.pBox_DocID.Size = new System.Drawing.Size(172, 148);
            this.pBox_DocID.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBox_DocID.TabIndex = 27;
            this.pBox_DocID.TabStop = false;
            // 
            // label_AñadirID
            // 
            this.label_AñadirID.AutoSize = true;
            this.label_AñadirID.Font = new System.Drawing.Font("Cascadia Code", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AñadirID.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_AñadirID.Location = new System.Drawing.Point(136, 415);
            this.label_AñadirID.Name = "label_AñadirID";
            this.label_AñadirID.Size = new System.Drawing.Size(100, 22);
            this.label_AñadirID.TabIndex = 28;
            this.label_AñadirID.Text = "Añadir ID";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(195)))), ((int)(((byte)(242)))));
            this.panel1.BackgroundImage = global::GUI.Properties.Resources.Copia_de_Montifer__9_;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.label_AñadirID);
            this.panel1.Controls.Add(this.pBox_DocID);
            this.panel1.Controls.Add(this.txt_CResidencia);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label_AñadirCV);
            this.panel1.Controls.Add(this.bttSalvarContinuar);
            this.panel1.Controls.Add(this.txt_PResidencia);
            this.panel1.Controls.Add(this.label_Datos);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pictureBox10);
            this.panel1.Controls.Add(this.label_IniciarSesion);
            this.panel1.Controls.Add(this.pBox_DocCV);
            this.panel1.Controls.Add(this.pictureBox8);
            this.panel1.Controls.Add(this.pictureBox7);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.panel1.Location = new System.Drawing.Point(298, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(704, 603);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Image = global::GUI.Properties.Resources.parte1;
            this.pictureBox10.Location = new System.Drawing.Point(314, 85);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(27, 26);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox10.TabIndex = 13;
            this.pictureBox10.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(65)))), ((int)(((byte)(108)))));
            this.panel2.Location = new System.Drawing.Point(350, 96);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(47, 2);
            this.panel2.TabIndex = 15;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::GUI.Properties.Resources.parte1;
            this.pictureBox3.Location = new System.Drawing.Point(403, 85);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(27, 26);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 29;
            this.pictureBox3.TabStop = false;
            // 
            // RegistrarDos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 650);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.BarraTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegistrarDos";
            this.Text = "UsuarioTipo";
            this.BarraTitulo.ResumeLayout(false);
            this.panelContenedor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnMaximizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRestaurar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_DocCV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox_DocID)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BarraTitulo;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.PictureBox btnMinimizar;
        private System.Windows.Forms.PictureBox btnCerrar;
        private System.Windows.Forms.PictureBox btnRestaurar;
        private System.Windows.Forms.PictureBox btnMaximizar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_AñadirID;
        private System.Windows.Forms.PictureBox pBox_DocID;
        private System.Windows.Forms.TextBox txt_CResidencia;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label_AñadirCV;
        private System.Windows.Forms.Button bttSalvarContinuar;
        private System.Windows.Forms.TextBox txt_PResidencia;
        private System.Windows.Forms.Label label_Datos;
        private System.Windows.Forms.Label label_IniciarSesion;
        private System.Windows.Forms.PictureBox pBox_DocCV;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox10;
    }
}