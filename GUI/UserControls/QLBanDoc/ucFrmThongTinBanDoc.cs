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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LibraryManagerApp.GUI.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLBanDoc
{
    public partial class ucFrmThongTinBanDoc : UserControl
    {
        private BLL.BanDocBLL _bll = new BLL.BanDocBLL();

        private State _currentState;
        private string _selectedMaBD = string.Empty;
        private FrmTimKiem _searchForm;

        #region KHỞI TẠO VÀ CẤU HÌNH
        public ucFrmThongTinBanDoc()
        {
            InitializeComponent();

            cboGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ" });

            ConfigureDGV();
        }

        private void ucFrmThongTinBanDoc_Load(object sender, EventArgs e)
        {
            SetState(State.READ);

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

        #region QUẢN LÝ TRẠNG THÁI (STATE)
        private void SetState (State state)
        {
            _currentState = state;

            bool isEditing = (state == State.CREATE || state == State.UPDATE);

            // Inputs
            txtMaBD.Enabled = (state == State.CREATE); // Chỉ cho phép nhập Mã BD khi tạo mới
            txtHoDem.Enabled = isEditing;
            txtTen.Enabled = isEditing;
            dtpNgaySinh.Enabled = isEditing;
            cboGioiTinh.Enabled = isEditing;
            txtDiaChi.Enabled = isEditing;
            txtSDT.Enabled = isEditing;
            txtEmail.Enabled = isEditing;

            // Buttons
            btnThem.Enabled = (state == State.READ);
            btnSua.Enabled = (state == State.READ && !string.IsNullOrEmpty(txtMaBD.Text));
            btnXoa.Enabled = (state == State.READ && !string.IsNullOrEmpty(txtMaBD.Text));

            btnLuu.Enabled = isEditing;
            btnHuy.Enabled = isEditing;

            btnTimKiem.Enabled = (state == State.READ);

            if (isEditing)
            {
                // TO DO: Ẩn DGV khi thêm/sửa nếu cần
            }
        }
        #endregion

        #region CHỨC NĂNG READ
        private void LoadData()
        {
            try
            {
                // Lấy dữ liệu từ BLL
                dgvDuLieu.DataSource = null;
                List<BanDocDTO> danhSach = _bll.LayThongTinBanDoc();
                dgvDuLieu.DataSource = danhSach;

                // Tự động điều chỉnh kích thước cột
                dgvDuLieu.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadModelToInputs(BanDocDTO model)
        {
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

        private void dgvDuLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra hàng hợp lệ
            if (e.RowIndex < 0 || e.RowIndex >= dgvDuLieu.RowCount)
                return;

            // Lấy MaBD từ hàng được chọn
            string maBD = dgvDuLieu.Rows[e.RowIndex].Cells["MaBD"].Value.ToString();

            _selectedMaBD = maBD;

            // Gọi BLL để lấy thông tin chi tiết đầy đủ (BanDocModel)
            BanDocDTO model = _bll.LayChiTietBanDoc(maBD);

            if (model != null)
            {
                LoadModelToInputs(model); 
            }

            // Cập nhật trạng thái nút sửa/xóa
            if (_currentState == State.READ)
            {
                // Kiểm tra xem có bản ghi nào được chọn chưa (để bật Sửa/Xóa)
                bool isRowSelected = !string.IsNullOrEmpty(txtMaBD.Text);
                btnSua.Enabled = isRowSelected;
                btnXoa.Enabled = isRowSelected;

            }
        }
        #endregion

        #region CHỨC NĂNG CREATE
        private void btnThem_Click(object sender, EventArgs e)
        {
            ClearInputs();
            SetState(State.CREATE);
            txtMaBD.Focus();
        }
        #endregion

        #region CHỨC NĂNG UPDATE
        private void btnSua_Click(object sender, EventArgs e)
        {
            // Đảm bảo có bản ghi được chọn và đang ở trạng thái READ
            if (_currentState == State.READ && !string.IsNullOrEmpty(txtMaBD.Text))
            {
                SetState(State.UPDATE);
                txtHoDem.Focus(); // Bắt đầu chỉnh sửa từ trường Họ Đệm
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bản ghi để chỉnh sửa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region CHỨC NĂNG DELETE
        private void btnXoa_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có bản ghi nào được chọn chưa
            string maBD = txtMaBD.Text.Trim();
            string hoTen = txtHoDem.Text.Trim() + " " + txtTen.Text.Trim();

            if (string.IsNullOrEmpty(maBD))
            {
                MessageBox.Show("Vui lòng chọn một Bạn Đọc để xóa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa Bạn Đọc:\n[ {maBD} - {hoTen} ] không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // 3. Xử lý kết quả xác nhận
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_bll.XoaBanDoc(maBD))
                    {
                        MessageBox.Show("Xóa Bạn Đọc thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Tải lại danh sách
                        ClearInputs(); // Xóa dữ liệu trên Inputs
                                       // Không cần SetState vì đang ở State.READ
                    }
                    else
                    {
                        // Thông báo lỗi chung (có thể do khóa ngoại)
                        MessageBox.Show("Xóa Bạn Đọc thất bại. Có thể Bạn Đọc này có dữ liệu liên quan (ví dụ: đang mượn sách).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Đảm bảo không tạo nhiều instance của Form tìm kiếm VÀ CHỈ ĐĂNG KÝ EVENT MỘT LẦN
            if (_searchForm == null || _searchForm.IsDisposed)
            {
                _searchForm = new LibraryManagerApp.GUI.Forms.FrmTimKiem();

                // 1. Đăng ký Event (CHỈ 1 LẦN)
                _searchForm.OnSearchApplied += HandleSearchApplied;

                // 2. Xử lý khi Form tìm kiếm bị đóng
                //_searchForm.FormClosed += SearchForm_FormClosed;
            }

            // 3. Hiển thị Form non-modal
            _searchForm.Show();
            _searchForm.BringToFront(); // Đưa lên trên
        }

        // Hàm xử lý Event khi người dùng nhấn nút "Tìm" trong frmTimKiem
        private void HandleSearchApplied(List<SearchFilter> filters)
        {
            try
            {
                dgvDuLieu.DataSource = null;

                // Load lại data gốc nếu không có bộ lọc
                if (filters == null || filters.Count == 0)
                {
                    // Nếu không có bộ lọc, tải lại toàn bộ dữ liệu (trạng thái READ mặc định)
                    LoadData();
                }
                else
                {
                    // Gọi BLL để thực hiện tìm kiếm với các bộ lọc
                    List<BanDocDTO> danhSachTimKiem = _bll.TimKiemBanDoc(filters);
                    dgvDuLieu.DataSource = danhSachTimKiem;
                    //Kích hoạt nút để hủy bộ lọc tìm kiếm
                    btnHuy.Enabled = true;

                    MessageBox.Show($"Tìm thấy {danhSachTimKiem.Count} kết quả.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Điều chỉnh kích thước cột
                dgvDuLieu.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                // Xóa Inputs của Form cha để tập trung vào kết quả tìm kiếm
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực hiện tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm xử lý khi Form tìm kiếm bị đóng
        //private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    // Hủy đăng ký Event để tránh rò rỉ bộ nhớ
        //    if (_searchForm != null)
        //    {
        //        _searchForm.OnSearchApplied -= HandleSearchApplied;
        //    }

        //    // Khôi phục DGV về trạng thái mặc định (Load lại toàn bộ dữ liệu)
        //    LoadData();
        //    _searchForm = null;
        //}
        

        #endregion

        #region XỬ LÝ SỰ KIỆN CÁC NÚT - LƯU - HỦY
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            BanDocDTO model = GetModelFromInputs();

            // 1. Xử lý logic theo State CREATE (Đã có từ Bước 2)
            if (_currentState == Helpers.State.CREATE)
            {
                try
                {
                    if (_bll.ThemBanDoc(model))
                    {
                        MessageBox.Show("Thêm Bạn Đọc thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        SetState(Helpers.State.READ);
                    }
                    else
                    {
                        MessageBox.Show("Thêm Bạn Đọc thất bại. (Mã BD đã tồn tại)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống khi thêm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 2. Xử lý logic theo State UPDATE
            else if (_currentState == Helpers.State.UPDATE)
            {
                // Không cần kiểm tra Mã BD tồn tại vì đang cập nhật bản ghi đã có
                try
                {
                    if (_bll.CapNhatBanDoc(model))
                    {
                        MessageBox.Show("Cập nhật Bạn Đọc thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Tải lại danh sách
                        SetState(Helpers.State.READ); // Chuyển về trạng thái READ
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật Bạn Đọc thất bại. (Không tìm thấy Mã BD)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                if (!string.IsNullOrEmpty(_selectedMaBD))
                {
                    // Tải lại dữ liệu chi tiết của bản ghi đang sửa
                    BanDocDTO model = _bll.LayChiTietBanDoc(_selectedMaBD);
                    if (model != null)
                    {
                        LoadModelToInputs(model); // Gọi hàm mới để tải dữ liệu (sẽ được tạo ở bước 3)
                    }
                    else
                    {
                        // Trường hợp ngoại lệ: Bản ghi gốc đã bị xóa bởi người dùng khác, ta chỉ cần Clear
                        ClearInputs();
                    }
                }
            }

            // Luôn luôn chuyển về trạng thái READ sau khi Hủy
            SetState(State.READ);
            LoadData(); // Tải lại dữ liệu để đồng bộ DGV
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

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private BanDocDTO GetModelFromInputs()
        {
            // Chuyển "Nam"/"Nữ" sang 'M'/'F' để lưu vào DB
            string gioiTinhCode = cboGioiTinh.SelectedItem.ToString().Equals("Nam") ? "M" : "F";

            return new BanDocDTO
            {
                MaBD = txtMaBD.Text.Trim(),
                HoDem = txtHoDem.Text.Trim(),
                Ten = txtTen.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                GioiTinh = gioiTinhCode,
                DiaChi = txtDiaChi.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };
        }

        private bool ValidateInputs()
        {
            // Lấy dữ liệu đã Trim() để kiểm tra
            string maBD = txtMaBD.Text.Trim();
            string hoDem = txtHoDem.Text.Trim();
            string ten = txtTen.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string sdt = txtSDT.Text.Trim();
            string email = txtEmail.Text.Trim();

            // 1. Kiểm tra trường BẮT BUỘC (theo NOT NULL của DB)
            if (string.IsNullOrEmpty(maBD) || string.IsNullOrEmpty(hoDem) || string.IsNullOrEmpty(ten) ||
                string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(email) || cboGioiTinh.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã BD, Họ Đệm, Tên, Giới Tính, Số Điện Thoại và Email.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Kiểm tra định dạng Mã Bạn Đọc (MaBD CHAR(9))
            if (maBD.Length != 9)
            {
                MessageBox.Show("Mã Bạn Đọc phải có chính xác 9 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaBD.Focus();
                return false;
            }

            // (ĐÃ THÊM Ở BLL - Chú ý: Nếu đây là trạng thái CREATE, bạn cần kiểm tra MaBD đã tồn tại chưa ở lớp BLL.)

            // 3. Kiểm tra Ngày Sinh hợp lệ (DATE NOT NULL)
            // Ngày sinh không được sau ngày hiện tại
            if (dtpNgaySinh.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Ngày Sinh không hợp lệ (Không được sau ngày hiện tại).", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpNgaySinh.Focus();
                return false;
            }

            // 4. Kiểm tra Định dạng Số Điện Thoại (Ưu tiên SDT Việt Nam: 10 chữ số, bắt đầu bằng 0)
            // Regex cơ bản: Bắt đầu bằng 0, theo sau là 9 chữ số (tổng cộng 10 số)
            // Ví dụ: 0901234567
            string vnPhonePattern = @"^(?:\+84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-6|8|9]|9[0-4|6-9])\d{7}$";
            if (!Regex.IsMatch(sdt, vnPhonePattern))
            {
                MessageBox.Show("Số Điện Thoại không hợp lệ. Vui lòng nhập số điện thoại 10 chữ số (bắt đầu bằng 0).", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSDT.Focus();
                return false;
            }

            // 5. Kiểm tra Định dạng Email (Email NVARCHAR(200) NOT NULL)
            // Regex cơ bản: [một hoặc nhiều ký tự]@[một hoặc nhiều ký tự].[hai hoặc nhiều ký tự]
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Địa chỉ Email không đúng định dạng.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // 6. Kiểm tra giới hạn ký tự (theo độ dài NVARCHAR của DB)
            if (hoDem.Length > 50)
            {
                MessageBox.Show("Họ Đệm không được vượt quá 50 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoDem.Focus();
                return false;
            }
            if (ten.Length > 30)
            {
                MessageBox.Show("Tên không được vượt quá 30 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTen.Focus();
                return false;
            }
            if (diaChi.Length > 200)
            {
                MessageBox.Show("Địa Chỉ không được vượt quá 200 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiaChi.Focus();
                return false;
            }
            if (email.Length > 200)
            {
                MessageBox.Show("Email không được vượt quá 200 ký tự.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }
        #endregion

       
    }
}
