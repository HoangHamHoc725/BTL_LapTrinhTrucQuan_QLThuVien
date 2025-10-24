using QuanLyThuVien.BLL;
using QuanLyThuVien.GUI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.UserControls.ucFrmDangKyDangNhap
{
    public partial class ucFrmDangNhap : UserControl
    {
        private frmDangNhapDangKy frmDangNhapDangKy;
        private AccountBLL accountBLL = new AccountBLL();

        public ucFrmDangNhap(frmDangNhapDangKy form)
        {
            InitializeComponent();

            this.frmDangNhapDangKy = form;
        }

        private void ucFrmDangNhap_Load(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text;
            string matKhau = txtMatKhau.Text;

            if (accountBLL.Login(tenDangNhap, matKhau))
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmQuanLyThuVien mainForm = new frmQuanLyThuVien();
                mainForm.Show();
                frmDangNhapDangKy.Hide();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaoTK_Click(object sender, EventArgs e)
        {
            frmDangNhapDangKy.SwitchToRegister();
        }
    }
}
