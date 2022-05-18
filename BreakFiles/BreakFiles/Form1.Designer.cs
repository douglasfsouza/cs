
namespace ReplaceCV
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtPasta = new System.Windows.Forms.TextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grdFiles = new System.Windows.Forms.DataGridView();
            this.txtLinhasPorArquivo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Folder:";
            // 
            // txtPasta
            // 
            this.txtPasta.Location = new System.Drawing.Point(43, 0);
            this.txtPasta.Name = "txtPasta";
            this.txtPasta.Size = new System.Drawing.Size(823, 20);
            this.txtPasta.TabIndex = 1;
            this.txtPasta.Text = "e:\\cnab";
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(872, 198);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 27);
            this.btnReplace.TabIndex = 2;
            this.btnReplace.Text = "Quebrar";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.Location = new System.Drawing.Point(872, 0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 53);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.button1_Click);
            // 
            // grdFiles
            // 
            this.grdFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdFiles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grdFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdFiles.ColumnHeadersVisible = false;
            this.grdFiles.Location = new System.Drawing.Point(43, 27);
            this.grdFiles.Name = "grdFiles";
            this.grdFiles.RowHeadersVisible = false;
            this.grdFiles.Size = new System.Drawing.Size(823, 198);
            this.grdFiles.TabIndex = 4;
            // 
            // txtLinhasPorArquivo
            // 
            this.txtLinhasPorArquivo.Location = new System.Drawing.Point(872, 172);
            this.txtLinhasPorArquivo.Name = "txtLinhasPorArquivo";
            this.txtLinhasPorArquivo.Size = new System.Drawing.Size(75, 20);
            this.txtLinhasPorArquivo.TabIndex = 5;
            this.txtLinhasPorArquivo.Text = "500";
            this.txtLinhasPorArquivo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(872, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Qtde Linhas";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 237);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLinhasPorArquivo);
            this.Controls.Add(this.grdFiles);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txtPasta);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Quebrar arquivos de Extrato bancario";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdFiles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPasta;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.DataGridView grdFiles;
        private System.Windows.Forms.TextBox txtLinhasPorArquivo;
        private System.Windows.Forms.Label label2;
    }
}

