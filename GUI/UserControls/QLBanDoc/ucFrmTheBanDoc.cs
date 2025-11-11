using LibraryManagerApp.BLL;
using LibraryManagerApp.DAL;
using LibraryManagerApp.DTO;
using LibraryManagerApp.Helpers;
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
    public partial class ucFrmTheBanDoc : UserControl
    {
        private TheBanDocBLL _bll = new BLL.TheBanDocBLL();
        private State _currentState;
        private string _selectedMaTBD = string.Empty;

        public ucFrmTheBanDoc()
        {
            InitializeComponent();
            ConfigureDGV();
        }

        #region KHỞI TẠO VÀ CẤU HÌNH
        private void ucFrmTheBanDoc_Load(object sender, EventArgs e)
        {
            // Khởi tạo các giá trị mặc định cho ComboBox
            cboTrangThai.Items.AddRange(new string[] { "Hoạt động", "Khóa" });

            SetState(State.READ); // Thiết lập trạng thái ban đầu
            LoadData();
        }

        private void ConfigureDGV()
        {
            dgvDuLieu.AutoGenerateColumns = false;
            dgvDuLieu.ReadOnly = true;
            dgvDuLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDuLieu.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold);

            // Định nghĩa các cột hiển thị (giúp kiểm soát thứ tự và tên hiển thị)
            if (dgvDuLieu.Columns.Count == 0)
            {
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã TBD", DataPropertyName = "MaTBD", Name = "MaTBD", Width = 100 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã BD", DataPropertyName = "MaBD", Name = "MaBD", Width = 80 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ Tên BD", DataPropertyName = "HoTenBD", Name = "HoTenBD", Width = 150 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã TK", DataPropertyName = "MaTK", Name = "MaTK", Width = 80 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ Tên NV", DataPropertyName = "HoTenNV", Name = "HoTenNV", Width = 150 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Cấp", DataPropertyName = "NgayCap", Name = "NgayCap", DefaultCellStyle = { Format = "dd/MM/yyyy" }, Width = 100 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hết Hạn", DataPropertyName = "NgayHetHanHienThi", Name = "NgayHetHanHienThi", Width = 100 });
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng Thái", DataPropertyName = "TrangThai", Name = "TrangThai", Width = 100 });
            }
        }
        #endregion

        #region QUẢN LÝ TRẠNG THÁI (STATE)
        private void SetState(State state)
        {
            _currentState = state;

            bool isEditing = (state == State.CREATE || state == State.UPDATE);
            bool isCreating = (state == State.CREATE);

            // Inputs
            txtMaTBD.Enabled = false; // Mã TBD luôn disable (sinh từ SP)

            // Cbo Ban Doc: Chỉ Enable khi CREATE
            cboBanDoc.Enabled = isCreating;

            // Tài Khoản: Mặc định cố định (Giả định Mã TK đang đăng nhập là "TK00001")
            txtMaTK.Enabled = false;
            txtHoTenNV.Enabled = false;
            if (isCreating)
            {
                txtMaTK.Text = "TK25-07"; // Ghi chú: Cập nhật sau khi có login session
                txtHoTenNV.Text = "Phạm Ngọc Thanh Quang";
            }

            dtpNgayCap.Enabled = isEditing;
            dtpNgayHetHan.Enabled = isEditing;
            cboTrangThai.Enabled = isEditing;

            // Buttons
            btnThem.Enabled = (state == State.READ);
            btnSua.Enabled = (state == State.READ && !string.IsNullOrEmpty(txtMaTBD.Text));
            btnXoa.Enabled = (state == State.READ && !string.IsNullOrEmpty(txtMaTBD.Text));

            btnLuu.Enabled = isEditing;
            btnHuy.Enabled = isEditing;

            btnTimKiem.Enabled = (state == State.READ);

            // Logic đặc biệt: Tải Combo chỉ khi chuyển sang CREATE
            if (isCreating)
            {
                LoadBanDocVaoCombo();
            }
        }
        #endregion

        #region CHỨC NĂNG READ
        private void LoadData()
        {
            try
            {
                dgvDuLieu.DataSource = null;
                List<TheBanDocDTO> danhSach = _bll.LayThongTinTheBanDoc();
                dgvDuLieu.DataSource = danhSach;

                dgvDuLieu.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu Thẻ Bạn Đọc: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBanDocVaoCombo()
        {
            try
            {
                List<BanDocChuaCoTheDTO> danhSach = _bll.LayBanDocChuaCoThe();

                cboBanDoc.DataSource = danhSach;
                cboBanDoc.DisplayMember = "HoTen";
                cboBanDoc.ValueMember = "MaBD";
                cboBanDoc.SelectedIndex = -1; // Chọn không có gì mặc định
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách Bạn Đọc: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDuLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra hàng hợp lệ
            if (e.RowIndex < 0 || e.RowIndex >= dgvDuLieu.RowCount)
                return;

            // Lấy MaTBD từ hàng được chọn
            string maTBD = dgvDuLieu.Rows[e.RowIndex].Cells["MaTBD"].Value.ToString();

            _selectedMaTBD = maTBD;

            TheBanDocDTO model = _bll.LayChiTietTheBanDoc(maTBD);

            if (model != null)
            {
                LoadModelToInputs(model);
            }

            // Cập nhật trạng thái nút sửa/xóa
            if (_currentState == State.READ)
            {
                bool isRowSelected = !string.IsNullOrEmpty(txtMaTBD.Text);
                btnSua.Enabled = isRowSelected;
                btnXoa.Enabled = isRowSelected;
            }
        }

        private void LoadModelToInputs(TheBanDocDTO model)
        {
            txtMaTBD.Text = model.MaTBD;

            // KHÔNG GÁN VÀO cboBanDoc khi READ/UPDATE
            // (Vì cboBanDoc chỉ chứa danh sách *chưa có thẻ*)
            // Thay vào đó, hiển thị dữ liệu đã JOIN:
            cboBanDoc.Enabled = false; // Luôn disable khi xem/sửa
            cboBanDoc.Text = $"{model.MaBD} - {model.HoTenBD}";

            txtMaTK.Text = model.MaTK;
            txtHoTenNV.Text = model.HoTenNV;
            dtpNgayCap.Value = model.NgayCap;
            dtpNgayHetHan.Value = model.NgayHetHan ?? DateTime.Now.AddYears(4);
            cboTrangThai.SelectedItem = model.TrangThai;
        }

        private void ClearInputs()
        {
            txtMaTBD.Text = string.Empty;
            cboBanDoc.DataSource = null; // Xóa DataSource khi Clear
            cboBanDoc.Text = string.Empty;

            // Tài khoản: Đặt về mặc định (sẽ được SetState xử lý)
            txtMaTK.Text = string.Empty;
            txtHoTenNV.Text = string.Empty;

            dtpNgayCap.Value = DateTime.Now;
            dtpNgayHetHan.Value = DateTime.Now.AddYears(4);
            cboTrangThai.SelectedIndex = 0;

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }
        #endregion

        #region CHỨC NĂNG CREATE

        #endregion

        #region CHỨC NĂNG UPDATE
        #endregion

        #region CHỨC NĂNG DELETE
        #endregion

        #region CHỨC NĂNG TÌM KIẾM
        #endregion

        #region XỬ LÝ SỰ KIỆN CÁC NÚT - LƯU - HỦY
        #endregion

        #region HÀM BỔ TRỢ
        #endregion
    }
}
