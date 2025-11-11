// File: LibraryManagerApp.GUI.UserControls.QLBanDoc/ucFrmTheBanDoc.cs

using LibraryManagerApp.BLL;
using LibraryManagerApp.DAL; // Cần thiết cho BanDocChuaCoTheDTO
using LibraryManagerApp.DTO;
using LibraryManagerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLBanDoc
{
    public partial class ucFrmTheBanDoc : UserControl
    {
        private TheBanDocBLL _bll = new BLL.TheBanDocBLL();
        private State _currentState;
        private string _selectedMaTBD = string.Empty;

        #region KHỞI TẠO VÀ CẤU HÌNH
        public ucFrmTheBanDoc()
        {
            InitializeComponent();

            cboTrangThai.Items.AddRange(new string[] { "Hoạt động", "Khóa" });

            ConfigureDGV();
        }
        
        private void ucFrmTheBanDoc_Load(object sender, EventArgs e)
        {
            SetState(State.READ);

            LoadData();
        }

        private void ConfigureDGV()
        {
            dgvDuLieu.AutoGenerateColumns = false;
            dgvDuLieu.ReadOnly = true;
            dgvDuLieu.AllowUserToAddRows = false;
            dgvDuLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDuLieu.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvDuLieu.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDuLieu.Font, FontStyle.Bold);
            dgvDuLieu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            if (dgvDuLieu.Columns.Count == 0)
            {
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã TBD", DataPropertyName = "MaTBD", Name = "MaTBD"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã BD", DataPropertyName = "MaBD", Name = "MaBD"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ Tên BD", DataPropertyName = "HoTenBD", Name = "HoTenBD"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã TK", DataPropertyName = "MaTK", Name = "MaTK"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Họ Tên NV", DataPropertyName = "HoTenNV", Name = "HoTenNV"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Cấp", DataPropertyName = "NgayCap", Name = "NgayCap", DefaultCellStyle = { Format = "dd/MM/yyyy" }});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Hết Hạn", DataPropertyName = "NgayHetHanHienThi", Name = "NgayHetHanHienThi"});
                dgvDuLieu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng Thái", DataPropertyName = "TrangThai", Name = "TrangThai"});
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
            txtMaTBD.Enabled = false;

            cboBanDoc.Enabled = isCreating; // Chỉ Enable khi CREATE

            txtMaTK.Enabled = false; // Luôn disable (Lấy từ Session)
            txtHoTenNV.Enabled = false; // Luôn disable (Dữ liệu JOIN)

            // Tạm thời gán MaTK và HoTenNV mặc định
            if (isCreating)
            {
                if (SessionManager.IsLoggedIn)
                {
                    txtMaTK.Text = SessionManager.GetMaTaiKhoan();
                    // HoTenNV được lưu trong CurrentUser (Đã JOIN tNhanVien)
                    txtHoTenNV.Text = SessionManager.CurrentUser.HoTenNV;
                }
                else
                {
                    // Trường hợp ngoại lệ: Chưa đăng nhập (Chỉ xảy ra nếu bỏ qua màn hình đăng nhập)
                    MessageBox.Show("Vui lòng đăng nhập để thực hiện cấp thẻ.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaTK.Text = string.Empty;
                    txtHoTenNV.Text = string.Empty;
                }
            }
            else if (state == State.READ)
            {
                // Xóa input phụ khi chuyển về READ
                cboBanDoc.DataSource = null;
                cboBanDoc.Text = string.Empty;

                // Khi quay về READ, cần xóa inputs TK/NV để khi chọn hàng mới được LoadModelToInputs cập nhật
                txtMaTK.Text = string.Empty;
                txtHoTenNV.Text = string.Empty;
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
                // Sử dụng DTO đã định nghĩa trong BanDocDAL
                List<BanDocChuaCoTheDTO> danhSach = _bll.LayBanDocChuaCoThe();

                // Tạo danh sách Anonymous/DTO mới để hiển thị Mã BD
                var danhSachCombo = danhSach.Select(bd => new
                {
                    MaBD = bd.MaBD,
                    HoTenVaMa = $"{bd.MaBD} - {bd.HoTen}" // Trường hiển thị mới
                }).ToList();

                cboBanDoc.DataSource = danhSachCombo;

                // Cập nhật DisplayMember và ValueMember
                cboBanDoc.DisplayMember = "HoTenVaMa"; // <<< Hiển thị: MaBD - HoTen
                cboBanDoc.ValueMember = "MaBD";        // <<< Giá trị lấy: MaBD

                cboBanDoc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách Bạn Đọc: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadModelToInputs(TheBanDocDTO model)
        {
            txtMaTBD.Text = model.MaTBD;

            // Hiển thị MaBD và HoTenBD của thẻ đã tồn tại
            cboBanDoc.Enabled = false;
            cboBanDoc.Text = $"{model.MaBD} - {model.HoTenBD}";

            txtMaTK.Text = model.MaTK;
            txtHoTenNV.Text = model.HoTenNV;
            dtpNgayCap.Value = model.NgayCap;
            dtpNgayHetHan.Value = model.NgayHetHan ?? model.NgayCap.AddYears(4); // Nếu DB trả về NULL (rất hiếm), tính lại
            cboTrangThai.SelectedItem = model.TrangThai;
        }

        private void dgvDuLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvDuLieu.RowCount)
                return;

            string maTBD = dgvDuLieu.Rows[e.RowIndex].Cells["MaTBD"].Value.ToString();

            _selectedMaTBD = maTBD;

            TheBanDocDTO model = _bll.LayChiTietTheBanDoc(maTBD);

            if (model != null)
            {
                LoadModelToInputs(model);
            }

            if (_currentState == State.READ)
            {
                bool isRowSelected = !string.IsNullOrEmpty(txtMaTBD.Text);
                btnSua.Enabled = isRowSelected;
                btnXoa.Enabled = isRowSelected;
            }
        }
        #endregion

        #region CHỨC NĂNG CREATE
        private void btnThem_Click(object sender, EventArgs e)
        {
            ClearInputs();

            dtpNgayCap.Value = DateTime.Now.Date;
            dtpNgayHetHan.Value = DateTime.Now.Date.AddYears(4);

            SetState(State.CREATE);
            cboBanDoc.Focus();
        }
        #endregion

        #region CHỨC NĂNG UPDATE
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_currentState == State.READ && !string.IsNullOrEmpty(txtMaTBD.Text))
            {
                SetState(State.UPDATE);
                dtpNgayCap.Focus();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một Thẻ Bạn Đọc để chỉnh sửa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region CHỨC NĂNG DELETE
        private void btnXoa_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có bản ghi nào được chọn chưa
            string maTBD = txtMaTBD.Text.Trim();
            string hoTenBD = cboBanDoc.Text.Trim(); // Lấy tên hiển thị khi ở trạng thái READ

            if (string.IsNullOrEmpty(maTBD))
            {
                MessageBox.Show("Vui lòng chọn một Thẻ Bạn Đọc để xóa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa Thẻ Bạn Đọc:\n[ {maTBD} - {hoTenBD} ] không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // 3. Xử lý kết quả xác nhận
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_bll.XoaTheBanDoc(maTBD))
                    {
                        MessageBox.Show("Xóa Thẻ Bạn Đọc thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Tải lại danh sách
                        ClearInputs(); // Xóa dữ liệu trên Inputs
                        // SetState(State.READ) đã được thực hiện bởi ClearInputs
                    }
                    else
                    {
                        // Thông báo lỗi chung (có thể do khóa ngoại, thẻ đang hoạt động,...)
                        MessageBox.Show("Xóa Thẻ Bạn Đọc thất bại. Có thể Thẻ đang được sử dụng hoặc có ràng buộc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region CHỨC NĂNG TÌM KIẾM
        // Sẽ triển khai sau
        #endregion

        #region XỬ LÝ SỰ KIỆN CÁC NÚT - LƯU - HỦY
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            TheBanDocDTO model = GetModelFromInputs();
            int errorStatus;

            // Bắt đầu khối try-catch bao quanh toàn bộ thao tác DB
            try
            {
                if (_currentState == State.CREATE)
                {
                    // --- LOGIC CREATE ---
                    string newMaTBD = _bll.ThemTheBanDoc(model, out errorStatus);

                    if (errorStatus == 0)
                    {
                        MessageBox.Show("Tạo Thẻ Bạn Đọc thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        SetState(State.READ);
                        _selectedMaTBD = newMaTBD;
                    }
                    else
                    {
                        // Xử lý lỗi nghiệp vụ từ BLL/SP
                        string errorMessage = GetErrorMessage(errorStatus);
                        MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (_currentState == State.UPDATE)
                {
                    // --- LOGIC UPDATE ---
                    model.MaTBD = _selectedMaTBD;

                    if (_bll.CapNhatTheBanDoc(model))
                    {
                        MessageBox.Show("Cập nhật Thẻ Bạn Đọc thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        SetState(State.READ);
                    }
                    else
                    {
                        // Lỗi nghiệp vụ (ví dụ: MaTK không tồn tại, ràng buộc NgayHetHan)
                        MessageBox.Show("Cập nhật Thẻ Bạn Đọc thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi hệ thống/lỗi ngoại lệ không mong muốn
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (_currentState == State.CREATE)
            {
                // 1. Nếu đang ở trạng thái TẠO MỚI: Chỉ cần xóa hết dữ liệu trên Inputs
                ClearInputs();
            }
            else if (_currentState == State.UPDATE)
            {
                // 2. Nếu đang ở trạng thái CẬP NHẬT: Khôi phục lại dữ liệu gốc của bản ghi đã chọn
                if (!string.IsNullOrEmpty(_selectedMaTBD))
                {
                    try
                    {
                        // Tải lại dữ liệu chi tiết của bản ghi đang sửa
                        TheBanDocDTO model = _bll.LayChiTietTheBanDoc(_selectedMaTBD);
                        if (model != null)
                        {
                            LoadModelToInputs(model);
                        }
                        else
                        {
                            // Trường hợp ngoại lệ: Bản ghi gốc đã bị xóa bởi người dùng khác
                            ClearInputs();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi hệ thống khi tải lại dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearInputs();
                    }
                }
            }

            // Luôn luôn chuyển về trạng thái READ sau khi Hủy
            SetState(State.READ);
        }
        #endregion

        #region HÀM BỔ TRỢ
        private void ClearInputs()
        {
            txtMaTBD.Text = string.Empty;
            cboBanDoc.DataSource = null;
            cboBanDoc.Text = string.Empty;

            txtMaTK.Text = string.Empty;
            txtHoTenNV.Text = string.Empty;

            dtpNgayCap.Value = DateTime.Now.Date;
            dtpNgayHetHan.Value = DateTime.Now.Date.AddYears(4);
            cboTrangThai.SelectedIndex = cboTrangThai.Items.IndexOf("Hoạt động"); // Đặt về "Hoạt động"

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private TheBanDocDTO GetModelFromInputs()
        {
            // Lấy Mã BD cho CREATE/UPDATE
            string maBDHienTai;
            if (_currentState == State.CREATE)
            {
                // Lấy MaBD từ SelectedValue của ComboBox
                maBDHienTai = cboBanDoc.SelectedValue?.ToString() ?? string.Empty;
            }
            else // READ hoặc UPDATE
            {
                // Lấy MaBD bằng cách cắt chuỗi MaTBD (vì MaTBD = TBD + MaBD)
                // Cần đảm bảo txtMaTBD.Text có đủ 12 ký tự và bắt đầu bằng "TBD"
                if (txtMaTBD.Text.Length == 12 && txtMaTBD.Text.StartsWith("TBD"))
                {
                    maBDHienTai = txtMaTBD.Text.Substring(3);
                }
                else
                {
                    maBDHienTai = "UNKNOWN"; // Giá trị mặc định nếu không hợp lệ
                }
            }

            return new TheBanDocDTO
            {
                MaTBD = txtMaTBD.Text.Trim(),
                MaBD = maBDHienTai,
                MaTK = txtMaTK.Text.Trim(),
                NgayCap = dtpNgayCap.Value.Date,
                NgayHetHan = dtpNgayHetHan.Value.Date,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Hoạt động"
            };
        }

        private bool ValidateInputs()
        {
            // Lấy dữ liệu đã Trim() để kiểm tra
            string maTK = txtMaTK.Text.Trim();

            // 1. Kiểm tra trường BẮT BUỘC (theo NOT NULL của DB)
            // Các trường NOT NULL: MaBD (kiểm tra qua cbo), MaTK, NgayCap, TrangThai (kiểm tra qua cbo)

            // Kiểm tra cboBanDoc chỉ cần thiết khi CREATE
            if (_currentState == State.CREATE && (cboBanDoc.SelectedIndex == -1 || string.IsNullOrEmpty(cboBanDoc.SelectedValue?.ToString())))
            {
                MessageBox.Show("Vui lòng chọn Mã/Tên Bạn Đọc chưa có thẻ.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboBanDoc.Focus();
                return false;
            }

            // Kiểm tra MaTK và cboTrangThai (NOT NULL)
            if (string.IsNullOrEmpty(maTK) || cboTrangThai.SelectedIndex == -1)
            {
                MessageBox.Show("Mã Tài Khoản và Trạng Thái không được rỗng.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra định dạng Mã Tài Khoản (MaTK CHAR(7))
            if (maTK.Length != 7)
            {
                MessageBox.Show("Mã Tài Khoản phải có chính xác 7 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaTK.Focus();
                return false;
            }

            // 3. Kiểm tra Ngày Cấp hợp lệ (DATE NOT NULL)
            // Ngày cấp không được sau ngày hiện tại
            if (dtpNgayCap.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Ngày Cấp Thẻ không hợp lệ (Không được sau ngày hiện tại).", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayCap.Focus();
                return false;
            }

            // 4. Kiểm tra Ngày Hết Hạn >= Ngày Cấp (Theo ràng buộc CHK)
            if (dtpNgayHetHan.Value.Date < dtpNgayCap.Value.Date)
            {
                MessageBox.Show("Ngày Hết Hạn phải lớn hơn hoặc bằng Ngày Cấp.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgayHetHan.Focus();
                return false;
            }

            // 5. Kiểm tra trạng thái (nếu là CREATE, chỉ cho phép "Hoạt động")
            if (_currentState == State.CREATE && cboTrangThai.SelectedItem.ToString() != "Hoạt động")
            {
                MessageBox.Show("Khi tạo mới, trạng thái mặc định phải là 'Hoạt động'.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Không cần kiểm tra giới hạn ký tự cho các trường khác vì chúng là kết quả JOIN hoặc DatePicker đã kiểm soát.

            return true;
        }

        private string GetErrorMessage(int status)
        {
            switch (status)
            {
                case 1: return "Lỗi: Mã Bạn Đọc không tồn tại trong hệ thống.";
                case 2: return "Lỗi: Bạn Đọc này đã được cấp Thẻ.";
                case 3: return "Lỗi: Chiều dài Mã Thẻ không hợp lệ (Liên hệ quản trị).";
                case 4: return "Lỗi nghiệp vụ: Ngày Cấp Thẻ không hợp lệ.";
                case 99: return "Lỗi hệ thống: Không thể lưu Thẻ Bạn Đọc vào CSDL.";
                default: return "Lỗi không xác định.";
            }
        }
        #endregion
    }
}