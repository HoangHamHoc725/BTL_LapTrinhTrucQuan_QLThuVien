namespace QuanLyThuVien
{
    partial class frmQuanLyThuVien
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
            this.components = new System.ComponentModel.Container();
            this.mnsMain = new System.Windows.Forms.MenuStrip();
            this.hệThốngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmQLBanDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmQLTaiLieu = new System.Windows.Forms.ToolStripMenuItem();
            this.quảnLýTácGiảToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmQLTaiKhoan = new System.Windows.Forms.ToolStripMenuItem();
            this.qLThuVienDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.qLThuVienDataSet = new QuanLyThuVien.QLThuVienDataSet();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.mnsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qLThuVienDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLThuVienDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // mnsMain
            // 
            this.mnsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hệThốngToolStripMenuItem,
            this.tsmQLBanDoc,
            this.tsmQLTaiLieu,
            this.quảnLýTácGiảToolStripMenuItem,
            this.tsmQLTaiKhoan});
            this.mnsMain.Location = new System.Drawing.Point(0, 0);
            this.mnsMain.Name = "mnsMain";
            this.mnsMain.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.mnsMain.Size = new System.Drawing.Size(1008, 25);
            this.mnsMain.TabIndex = 0;
            this.mnsMain.Text = "menuStrip1";
            // 
            // hệThốngToolStripMenuItem
            // 
            this.hệThốngToolStripMenuItem.Name = "hệThốngToolStripMenuItem";
            this.hệThốngToolStripMenuItem.Size = new System.Drawing.Size(72, 19);
            this.hệThốngToolStripMenuItem.Text = "Hệ Thống";
            // 
            // tsmQLBanDoc
            // 
            this.tsmQLBanDoc.Name = "tsmQLBanDoc";
            this.tsmQLBanDoc.Size = new System.Drawing.Size(106, 19);
            this.tsmQLBanDoc.Text = "Quản lý Bạn đọc";
            this.tsmQLBanDoc.Click += new System.EventHandler(this.tsmQLBanDoc_Click);
            // 
            // tsmQLTaiLieu
            // 
            this.tsmQLTaiLieu.Name = "tsmQLTaiLieu";
            this.tsmQLTaiLieu.Size = new System.Drawing.Size(100, 19);
            this.tsmQLTaiLieu.Text = "Quản lý Tài liệu";
            this.tsmQLTaiLieu.Click += new System.EventHandler(this.tsmQLTaiLieu_Click);
            // 
            // quảnLýTácGiảToolStripMenuItem
            // 
            this.quảnLýTácGiảToolStripMenuItem.Name = "quảnLýTácGiảToolStripMenuItem";
            this.quảnLýTácGiảToolStripMenuItem.Size = new System.Drawing.Size(122, 19);
            this.quảnLýTácGiảToolStripMenuItem.Text = "Quản lý Mượn - Trả";
            // 
            // tsmQLTaiKhoan
            // 
            this.tsmQLTaiKhoan.Name = "tsmQLTaiKhoan";
            this.tsmQLTaiKhoan.Size = new System.Drawing.Size(114, 19);
            this.tsmQLTaiKhoan.Text = "Quản lý Tài khoản";
            this.tsmQLTaiKhoan.Click += new System.EventHandler(this.tsmQLTaiKhoan_Click);
            // 
            // qLThuVienDataSetBindingSource
            // 
            this.qLThuVienDataSetBindingSource.DataSource = this.qLThuVienDataSet;
            this.qLThuVienDataSetBindingSource.Position = 0;
            // 
            // qLThuVienDataSet
            // 
            this.qLThuVienDataSet.DataSetName = "QLThuVienDataSet";
            this.qLThuVienDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // pnlContent
            // 
            this.pnlContent.Location = new System.Drawing.Point(0, 28);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(1008, 561);
            this.pnlContent.TabIndex = 2;
            // 
            // frmQuanLyThuVien
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1008, 601);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.mnsMain);
            this.Font = new System.Drawing.Font("Cascadia Code", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mnsMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmQuanLyThuVien";
            this.Text = "Phần mềm quản lý thư viện";
            this.Load += new System.EventHandler(this.frmQuanLyThuVien_Load);
            this.mnsMain.ResumeLayout(false);
            this.mnsMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qLThuVienDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qLThuVienDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnsMain;
        private System.Windows.Forms.ToolStripMenuItem hệThốngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmQLBanDoc;
        private System.Windows.Forms.ToolStripMenuItem tsmQLTaiLieu;
        private System.Windows.Forms.ToolStripMenuItem quảnLýTácGiảToolStripMenuItem;
        private System.Windows.Forms.BindingSource qLThuVienDataSetBindingSource;
        private QLThuVienDataSet qLThuVienDataSet;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.ToolStripMenuItem tsmQLTaiKhoan;
    }
}

