using QuanLyThuVien.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.UserControls
{
    public partial class ucFrmQLTaiKhoanNhanVien : UserControl
    {
        private TaiKhoanNhanVienBLL TaiKhoanNhanVienBLL = new TaiKhoanNhanVienBLL();

        public ucFrmQLTaiKhoanNhanVien()
        {
            InitializeComponent();
        }

        private void ucFrmQLTaiKhoanNhanVien_Load(object sender, EventArgs e)
        {
            dgvTaiKhoanNhanVien.DataSource = TaiKhoanNhanVienBLL.GetAllTaiKhoanNhanVien();

            // Tùy chỉnh hiển thị
            dgvTaiKhoanNhanVien.Columns["MaNV"].HeaderText = "Mã nhân viên";
        }
    }
}
