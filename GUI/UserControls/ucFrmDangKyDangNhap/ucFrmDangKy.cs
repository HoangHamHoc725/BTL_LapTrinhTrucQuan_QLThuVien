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
    public partial class ucFrmDangKy : UserControl
    {
        private frmDangNhapDangKy frmDangNhapDangKy;
        private AccountBLL accountBLL = new AccountBLL();

        public ucFrmDangKy()
        {
            InitializeComponent();
        }

        public ucFrmDangKy(frmDangNhapDangKy form)
        {
            InitializeComponent();
            this.frmDangNhapDangKy = form;
        }

        private void ucFrmDangKy_Load(object sender, EventArgs e)
        {

        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            string tenDangNhap = txtTenDangNhap.Text;
            string matKhau = txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (accountBLL.Register(tenDangNhap, matKhau))
            {
                MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmDangNhapDangKy.SwitchToLogin();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            frmDangNhapDangKy.SwitchToLogin();
        }
    }
}
