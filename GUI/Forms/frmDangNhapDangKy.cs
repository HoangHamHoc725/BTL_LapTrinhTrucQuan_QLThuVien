using form1.GUI;
using QuanLyThuVien.GUI.UserControls.ucFrmDangKyDangNhap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.Forms
{
    public partial class frmDangNhapDangKy : Form
    {
        private ucFrmDangNhap ucFrmDangNhap;
        private ucFrmDangKy ucFrmDangKy;

        public frmDangNhapDangKy()
        {
            InitializeComponent();

            ucFrmDangNhap = new ucFrmDangNhap(this);
            ucFrmDangKy = new ucFrmDangKy(this);

            pnlContent.Controls.Add(ucFrmDangNhap);
            CenterUC(ucFrmDangNhap);
        }

        private void frmDangNhapDangKy_Load(object sender, EventArgs e)
        {

        }
        private void CenterUC(UserControl uc)
        {
            uc.Left = (pnlContent.ClientSize.Width - uc.Width) / 2;
        }

        public void SwitchToRegister()
        {
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(ucFrmDangKy);
            CenterUC(ucFrmDangKy);
            this.Height = 800;
        }

        public void SwitchToLogin()
        {
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(ucFrmDangNhap);
            CenterUC(ucFrmDangNhap);
            this.Height = 550;
        }
    }
}
