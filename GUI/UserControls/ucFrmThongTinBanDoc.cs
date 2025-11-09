using LibraryManagerApp.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls
{
    public partial class ucFrmThongTinBanDoc : UserControl
    {
        private BanDocBLL _banDocBLL = new BanDocBLL();

        public ucFrmThongTinBanDoc()
        {
            InitializeComponent();
        }

        private void ucFrmThongTinBanDoc_Load(object sender, EventArgs e)
        {
            LoadThongTinBanDoc();
        }

        private void LoadThongTinBanDoc()
        {
            try
            {
                // 1. Gọi BLL để lấy dữ liệu
                var danhSachBanDoc = _banDocBLL.LayThongTinBanDoc();

                // 2. Gán dữ liệu vào DataGridView
                // Đảm bảo tên DataGridView là dgvThongTinBanDoc
                dgvThongTinBanDoc.DataSource = danhSachBanDoc;

                // 3. Tùy chỉnh hiển thị cột (nên làm)
                dgvThongTinBanDoc.Columns["MaBD"].HeaderText = "Mã Bạn Đọc";
                dgvThongTinBanDoc.Columns["HoTen"].HeaderText = "Họ Tên";
                dgvThongTinBanDoc.Columns["GioiTinhHienThi"].HeaderText = "Giới Tính";
                dgvThongTinBanDoc.Columns["NgaySinh"].HeaderText = "Ngày Sinh";
                dgvThongTinBanDoc.Columns["SDT"].HeaderText = "Số ĐT";
                dgvThongTinBanDoc.Columns["DiaChi"].HeaderText = "Địa Chỉ";
                dgvThongTinBanDoc.Columns["Email"].HeaderText = "Email";

                // Định dạng ngày tháng
                dgvThongTinBanDoc.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu bạn đọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
