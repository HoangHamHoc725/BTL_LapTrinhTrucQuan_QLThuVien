using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLBanDoc
{
    public partial class ucFrmThongTinBanDoc : UserControl
    {
        private BLL.BanDocBLL _bll = new BLL.BanDocBLL();

        #region KHỞI TẠO VÀ CẤU HÌNH
        public ucFrmThongTinBanDoc()
        {
            InitializeComponent();

            cboGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ" });
            ConfigureDGV();
        }

        private void ucFrmThongTinBanDoc_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ConfigureDGV()
        {
            // Cấu hình cho dgvDuLieu
            dgvDuLieu.AutoGenerateColumns = false; // Tắt tự động sinh cột
            dgvDuLieu.ReadOnly = true;            // Chỉ cho phép xem
            dgvDuLieu.AllowUserToAddRows = false; // Không cho phép thêm hàng mới qua DGV
            dgvDuLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Chọn cả hàng
            dgvDuLieu.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Tùy chỉnh Header Style (ví dụ: chữ in đậm)
            dgvDuLieu.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold);
            dgvDuLieu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Định nghĩa các cột hiển thị (giúp kiểm soát thứ tự và tên hiển thị)

            if (dgvDuLieu.Columns.Count == 0) // Kiểm tra để không thêm lại
            {
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã BD", DataPropertyName = "MaBD", Name = "MaBD"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ Đệm", DataPropertyName = "HoDem", Name = "HoDem" });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên", DataPropertyName = "Ten", Name = "Ten" });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Sinh", DataPropertyName = "NgaySinh", Name = "NgaySinh", DefaultCellStyle = { Format = "dd/MM/yyyy" } });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giới Tính", DataPropertyName = "GioiTinhHienThi", Name = "GioiTinhHienThi" });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Địa Chỉ", DataPropertyName = "DiaChi", Name = "DiaChi" });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "SĐT", DataPropertyName = "SDT", Name = "SDT" });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email", Name = "Email" });
            }
        }
        #endregion

        #region CHỨC NĂNG READ (CRUD)
        private void LoadData()
        {
            try
            {
                dgvDuLieu.DataSource = null;
                List<BanDocDTO> danhSach = _bll.LayThongTinBanDoc();
                dgvDuLieu.DataSource = danhSach;

                // Tự động điều chỉnh kích thước cột
                dgvDuLieu.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDuLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra hàng hợp lệ
            if (e.RowIndex < 0 || e.RowIndex >= dgvDuLieu.RowCount)
                return;

            // Lấy MaBD từ hàng được chọn
            string maBD = dgvDuLieu.Rows[e.RowIndex].Cells["MaBD"].Value.ToString();

            // Gọi BLL để lấy thông tin chi tiết đầy đủ (BanDocModel)
            BanDocDTO model = _bll.LayChiTietBanDoc(maBD);

            if (model != null)
            {
                // Load dữ liệu lên Inputs
                txtMaBD.Text = model.MaBD;
                txtHoDem.Text = model.HoDem;
                txtTen.Text = model.Ten;
                dtpNgaySinh.Value = model.NgaySinh;
                txtDiaChi.Text = model.DiaChi;
                txtSDT.Text = model.SDT;
                txtEmail.Text = model.Email;

                // Xử lý Giới Tính: Chuyển 'M'/'F' sang "Nam"/"Nữ"
                cboGioiTinh.SelectedItem = model.GioiTinh.Equals("M") ? "Nam" : "Nữ";
            }
        }
        #endregion

        #region HÀM BỔ TRỢ
        private void ClearInputs()
        {
            txtMaBD.Text = string.Empty;
            txtHoDem.Text = string.Empty;
            txtTen.Text = string.Empty;
            dtpNgaySinh.Value = DateTime.Now;
            cboGioiTinh.SelectedIndex = -1;
            txtDiaChi.Text = string.Empty;
            txtSDT.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }
        #endregion
    }
}
