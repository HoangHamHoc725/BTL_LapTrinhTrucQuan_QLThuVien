namespace LibraryManagerApp.GUI.UserControls
{
    partial class ucFrmThongTinBanDoc
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvThongTinBanDoc = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongTinBanDoc)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvThongTinBanDoc
            // 
            this.dgvThongTinBanDoc.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvThongTinBanDoc.Location = new System.Drawing.Point(84, 421);
            this.dgvThongTinBanDoc.Name = "dgvThongTinBanDoc";
            this.dgvThongTinBanDoc.Size = new System.Drawing.Size(616, 203);
            this.dgvThongTinBanDoc.TabIndex = 0;
            // 
            // ucFrmThongTinBanDoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvThongTinBanDoc);
            this.Name = "ucFrmThongTinBanDoc";
            this.Size = new System.Drawing.Size(864, 681);
            this.Load += new System.EventHandler(this.ucFrmThongTinBanDoc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongTinBanDoc)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvThongTinBanDoc;
    }
}
