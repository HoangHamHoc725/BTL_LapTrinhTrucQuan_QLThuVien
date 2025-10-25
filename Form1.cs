using form1.GUI;
using QuanLyThuVien.GUI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien
{
    public partial class frmQuanLyThuVien : Form
    {
        private ucFrmQLBanDoc ucFrmQLBanDoc;
        private ucFrmQLTaiKhoanNhanVien ucFrmQLTaiKhoanNhanVien;
        private ucFrmQLTaiLieu ucFrmQLTaiLieu;

        public frmQuanLyThuVien()
        {
            InitializeComponent();

            ucFrmQLBanDoc = new ucFrmQLBanDoc();
            ucFrmQLTaiKhoanNhanVien = new ucFrmQLTaiKhoanNhanVien();
            ucFrmQLTaiLieu = new ucFrmQLTaiLieu();

            // Tùy chỉnh bố cục Form
            this.StartPosition = FormStartPosition.CenterScreen;
            
            pnlContent.Dock = DockStyle.Fill;
        }

        private void frmQuanLyThuVien_Load(object sender, EventArgs e)
        {

        }

        private void ShowUserControl(UserControl uc)
        {
            pnlContent.Controls.Clear();
            
            pnlContent.Controls.Add(uc);
            CenterUC(uc);
        }

        private void CenterUC(UserControl uc)
        {
            uc.Left = (pnlContent.ClientSize.Width - uc.Width) / 2;
        }

        private void tsmQLBanDoc_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucFrmQLBanDoc);
        }

        private void tsmQLTaiKhoan_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucFrmQLTaiKhoanNhanVien);
        }

        private void tsmQLTaiLieu_Click(object sender, EventArgs e)
        {
            ShowUserControl(ucFrmQLTaiLieu);
        }
    }
}
