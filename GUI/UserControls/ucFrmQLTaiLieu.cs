using QuanLyThuVien.BLL;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;
using QuanLyThuVien.GUI.Forms;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.UserControls
{
    /// <summary>
    /// User Control quản lý thông tin Tài liệu trong thư viện.
    /// Bao gồm các chức năng xem, thêm, sửa, xóa, tìm kiếm Tài liệu và quản lý Tác giả liên quan.
    /// </summary>
    public partial class ucFrmQLTaiLieu : UserControl
    {
        // Business Logic Layer (BLL) để thao tác dữ liệu
        private readonly QLTaiLieuBLL taiLieuBLL = new QLTaiLieuBLL();

        // Khai báo trạng thái hiện tại và trạng thái trước đó của form
        private FormState currentState = FormState.View;
        private FormState previousState = FormState.View;

        // Form tìm kiếm dùng chung
        private frmTimKiem _frmTimKiem;

        public ucFrmQLTaiLieu()
        {
            InitializeComponent();
        }

        // ====================================================
        // I. KHỞI TẠO VÀ TẢI DỮ LIỆU BAN ĐẦU
        // ====================================================

        /// <summary>
        /// Xử lý sự kiện Load của UserControl. Tải dữ liệu ban đầu và thiết lập trạng thái.
        /// </summary>
        private void ucFrmQLTaiLieu_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Tải dữ liệu cho các ComboBox (NXB, NN, Thể loại, Tác giả, Vai trò,...)
                LoadComboBoxData();

                // 2. Tải và setup DataGridView chính (dgvQLTaiLieu)
                dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
                SetupTaiLieuGridView();

                // 3. Tải thông tin chi tiết tài liệu đầu tiên lên form (và dgv tác giả)
                LoadTaiLieuToForm();

                // 4. Đặt trạng thái ban đầu và gán sự kiện
                SetState(FormState.View);
                dgvQLTaiLieu.CellClick += dgvQLTaiLieu_CellClick;
                dgvQLTacGia_TaiLieu.CellClick += dgvQLTacGia_TaiLieu_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu tài liệu: " + ex.Message, "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải danh sách đầy đủ các giá trị (NXB, NN, Thể loại, Định dạng, Tác giả, Vai trò) cho các ComboBox.
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // Tải dữ liệu cho các ComboBox thuộc Tài liệu
                cbNhaXuatBan.DataSource = taiLieuBLL.LayDanhSachNhaXuatBan();
                cbNgonNgu.DataSource = taiLieuBLL.LayDanhSachNgonNgu();
                cbTheLoai.DataSource = taiLieuBLL.LayDanhSachTheLoai();
                cbDinhDang.DataSource = taiLieuBLL.LayDanhSachDinhDang();

                // Tải dữ liệu cho các ComboBox thuộc Tác giả (để chọn khi Thêm/Sửa Tác giả)
                cbTacGia.DataSource = taiLieuBLL.LayDanhSachTacGia();
                cbVaiTro.DataSource = taiLieuBLL.LayDanhSachVaiTro();

                // Đặt mặc định không chọn gì sau khi load
                cbNhaXuatBan.SelectedIndex = -1;
                cbNgonNgu.SelectedIndex = -1;
                cbTheLoai.SelectedIndex = -1;
                cbDinhDang.SelectedIndex = -1;
                cbTacGia.SelectedIndex = -1;
                cbVaiTro.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu cho các ComboBox: " + ex.Message, "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================
        // II. CẤU HÌNH DATAGRIDVIEW
        // ====================================================

        /// <summary>
        /// Cấu hình hiển thị và thuộc tính cho DataGridView chính (Danh sách Tài liệu).
        /// </summary>
        private void SetupTaiLieuGridView()
        {
            if (dgvQLTaiLieu.Columns.Count == 0) return;

            // Cấu hình chung
            dgvQLTaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQLTaiLieu.RowHeadersVisible = false;
            dgvQLTaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTaiLieu.ReadOnly = true;
            dgvQLTaiLieu.AllowUserToResizeColumns = false;
            dgvQLTaiLieu.MultiSelect = false;
            dgvQLTaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cấu hình tên cột và kích thước
            dgvQLTaiLieu.Columns["MaTL"].HeaderText = "Mã tài liệu"; dgvQLTaiLieu.Columns["MaTL"].Width = 100;
            dgvQLTaiLieu.Columns["TenTL"].HeaderText = "Tên tài liệu"; dgvQLTaiLieu.Columns["TenTL"].Width = 300;
            dgvQLTaiLieu.Columns["LanXuatBan"].HeaderText = "Lần XB"; dgvQLTaiLieu.Columns["LanXuatBan"].Width = 60;
            dgvQLTaiLieu.Columns["NamXuatBan"].HeaderText = "Năm XB"; dgvQLTaiLieu.Columns["NamXuatBan"].Width = 80;
            dgvQLTaiLieu.Columns["SoTrang"].HeaderText = "Số trang"; dgvQLTaiLieu.Columns["SoTrang"].Width = 80;
            dgvQLTaiLieu.Columns["KhoCo"].HeaderText = "Khổ cỡ"; dgvQLTaiLieu.Columns["KhoCo"].Width = 100;
            dgvQLTaiLieu.Columns["TenNXB"].HeaderText = "Nhà xuất bản"; dgvQLTaiLieu.Columns["TenNXB"].Width = 150;
            dgvQLTaiLieu.Columns["TenNN"].HeaderText = "Ngôn ngữ"; dgvQLTaiLieu.Columns["TenNN"].Width = 100;
            dgvQLTaiLieu.Columns["TenTheLoai"].HeaderText = "Thể loại"; dgvQLTaiLieu.Columns["TenTheLoai"].Width = 120;
            dgvQLTaiLieu.Columns["TenDinhDang"].HeaderText = "Định dạng"; dgvQLTaiLieu.Columns["TenDinhDang"].Width = 100;

            // Ẩn các cột không có HeaderText (các trường ID hoặc không cần hiển thị)
            foreach (DataGridViewColumn col in dgvQLTaiLieu.Columns)
            {
                if (string.IsNullOrEmpty(col.HeaderText))
                {
                    col.Visible = false;
                }
            }
        }

        /// <summary>
        /// Cấu hình hiển thị và thuộc tính cho DataGridView phụ (Danh sách Tác giả của Tài liệu đang chọn).
        /// </summary>
        private void SetupTacGiaGridView()
        {
            if (dgvQLTacGia_TaiLieu.Columns.Count == 0) return;

            // Cấu hình chung
            dgvQLTacGia_TaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvQLTacGia_TaiLieu.RowHeadersVisible = false;
            dgvQLTacGia_TaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTacGia_TaiLieu.AllowUserToResizeColumns = false;
            dgvQLTacGia_TaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Tên cột: MaTG (ẩn), TenTacGia (Fill), VaiTro (150)
            dgvQLTacGia_TaiLieu.Columns["MaTG"].Visible = false;

            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].HeaderText = "Tên Tác giả";
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].ReadOnly = true; // Luôn chỉ đọc
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].Width = 200;
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvQLTacGia_TaiLieu.Columns["VaiTro"].HeaderText = "Vai trò";
            dgvQLTacGia_TaiLieu.Columns["VaiTro"].Width = 150;
            // Thuộc tính ReadOnly của cột VaiTro sẽ được SetState quản lý.
        }

        // ====================================================
        // III. XỬ LÝ SỰ KIỆN CELL CLICK & TẢI DỮ LIỆU CHI TIẾT
        // ====================================================

        /// <summary>
        /// Xử lý khi click vào một dòng trên DataGridView Tài liệu.
        /// </summary>
        private void dgvQLTaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Tải thông tin tài liệu và tác giả đang chọn lên form
            LoadTaiLieuToForm();
        }

        /// <summary>
        /// Xử lý khi click vào một dòng trên DataGridView Tác giả.
        /// </summary>
        private void dgvQLTacGia_TaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Chỉ cần tải thông tin tác giả/vai trò của dòng được chọn lên các combobox
            LoadTacGiaToComboboxes();
        }

        /// <summary>
        /// Lấy Mã Tài liệu từ dòng đang chọn trong dgvQLTaiLieu và tải danh sách Tác giả tương ứng
        /// vào dgvQLTacGia_TaiLieu (sử dụng DTO để có thể sửa đổi tạm thời).
        /// </summary>
        private void LoadTacGiaBySelectedTaiLieu()
        {
            if (dgvQLTaiLieu.CurrentRow == null || dgvQLTaiLieu.Rows.Count == 0)
            {
                dgvQLTacGia_TaiLieu.DataSource = null;
                return;
            }

            var row = dgvQLTaiLieu.CurrentRow.DataBoundItem;
            // Dùng Reflection để lấy giá trị (vì DataSource là Anonymous Type)
            string maTL = row?.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();

            if (string.IsNullOrEmpty(maTL))
            {
                dgvQLTacGia_TaiLieu.DataSource = null;
                return;
            }

            try
            {
                // 1. Lấy List<object> (Anonymous Type) từ BLL
                var rawTacGiaList = taiLieuBLL.LayTacGiaTheoMaTaiLieu(maTL);

                // 2. Chuyển đổi sang List<TacGiaTaiLieuDTO> (Mutable DTO)
                var dtoTacGiaList = rawTacGiaList.Select(tg => new TacGiaTaiLieuDTO
                {
                    MaTG = tg?.GetType().GetProperty("MaTG")?.GetValue(tg)?.ToString(),
                    TenTacGia = tg?.GetType().GetProperty("TenTacGia")?.GetValue(tg)?.ToString(),
                    VaiTro = tg?.GetType().GetProperty("VaiTro")?.GetValue(tg)?.ToString()
                }).ToList();

                // 3. Gán nguồn dữ liệu cho dgvQLTacGia_TaiLieu
                dgvQLTacGia_TaiLieu.DataSource = dtoTacGiaList;

                // 4. Gọi Setup và chọn dòng đầu tiên
                SetupTacGiaGridView();
                if (dgvQLTacGia_TaiLieu.Rows.Count > 0)
                {
                    dgvQLTacGia_TaiLieu.Rows[0].Selected = true;
                    dgvQLTacGia_TaiLieu.CurrentCell = dgvQLTacGia_TaiLieu.Rows[0].Cells[1];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải thông tin chi tiết tài liệu từ dòng đang chọn trong dgvQLTaiLieu lên các controls nhập liệu.
        /// </summary>
        private void LoadTaiLieuToForm()
        {
            // 1. Đảm bảo có dòng được chọn. Nếu không, cố gắng chọn dòng đầu tiên.
            if (dgvQLTaiLieu.Rows.Count > 0 && dgvQLTaiLieu.CurrentRow == null)
            {
                dgvQLTaiLieu.Rows[0].Selected = true;
                dgvQLTaiLieu.CurrentCell = dgvQLTaiLieu.Rows[0].Cells[0];
            }

            if (dgvQLTaiLieu.CurrentRow == null || dgvQLTaiLieu.CurrentRow.DataBoundItem == null)
            {
                ClearForm(); // Xóa trắng khi không có dữ liệu
                return;
            }

            // 2. Tải Tác giả cho DGV phụ trước (đảm bảo đồng bộ)
            LoadTacGiaBySelectedTaiLieu();

            // 3. Tải dữ liệu Tài liệu lên các controls
            var row = dgvQLTaiLieu.CurrentRow.DataBoundItem;

            txtMaTaiLieu.Text = row?.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();
            txtTenTaiLieu.Text = row?.GetType().GetProperty("TenTL")?.GetValue(row)?.ToString();
            txtNamXuatBan.Text = row?.GetType().GetProperty("NamXuatBan")?.GetValue(row)?.ToString();
            txtLanXuatBan.Text = row?.GetType().GetProperty("LanXuatBan")?.GetValue(row)?.ToString();
            txtSoTrang.Text = row?.GetType().GetProperty("SoTrang")?.GetValue(row)?.ToString();
            txtKhoCo.Text = row?.GetType().GetProperty("KhoCo")?.GetValue(row)?.ToString();

            // ComboBoxes (Chọn item dựa trên giá trị hiển thị)
            cbNhaXuatBan.SelectedItem = row?.GetType().GetProperty("TenNXB")?.GetValue(row)?.ToString();
            cbNgonNgu.SelectedItem = row?.GetType().GetProperty("TenNN")?.GetValue(row)?.ToString();
            cbTheLoai.SelectedItem = row?.GetType().GetProperty("TenTheLoai")?.GetValue(row)?.ToString();
            cbDinhDang.SelectedItem = row?.GetType().GetProperty("TenDinhDang")?.GetValue(row)?.ToString();

            // 4. Tải thông tin Tác giả đang được chọn trong DGV tác giả lên các ComboBox tác giả
            LoadTacGiaToComboboxes();
        }

        /// <summary>
        /// Tải thông tin tác giả (Tên, Vai trò) từ dòng đang chọn trong dgvQLTacGia_TaiLieu lên cbTacGia và cbVaiTro.
        /// </summary>
        private void LoadTacGiaToComboboxes()
        {
            if (dgvQLTacGia_TaiLieu.CurrentRow == null || dgvQLTacGia_TaiLieu.Rows.Count == 0)
            {
                cbTacGia.SelectedIndex = -1;
                cbVaiTro.SelectedIndex = -1;
                return;
            }

            var row = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem;
            cbTacGia.SelectedItem = row?.GetType().GetProperty("TenTacGia")?.GetValue(row)?.ToString();
            cbVaiTro.SelectedItem = row?.GetType().GetProperty("VaiTro")?.GetValue(row)?.ToString();
        }

        /// <summary>
        /// Xóa trắng nội dung tất cả các control nhập liệu trên form.
        /// </summary>
        private void ClearForm()
        {
            txtMaTaiLieu.Clear();
            txtTenTaiLieu.Clear();
            txtNamXuatBan.Clear();
            txtLanXuatBan.Clear();
            txtSoTrang.Clear();
            txtKhoCo.Clear();

            cbNhaXuatBan.SelectedIndex = -1;
            cbNgonNgu.SelectedIndex = -1;
            cbTheLoai.SelectedIndex = -1;
            cbDinhDang.SelectedIndex = -1;
            cbTacGia.SelectedIndex = -1;
            cbVaiTro.SelectedIndex = -1;

            dgvQLTacGia_TaiLieu.DataSource = null; // Xóa dữ liệu tác giả
        }

        /// <summary>
        /// Lấy dữ liệu Tài liệu từ các controls nhập liệu và danh sách Tác giả từ dgvQLTacGia_TaiLieu 
        /// để chuẩn bị cho thao tác CRUD.
        /// </summary>
        /// <returns>Tuple chứa đối tượng tTaiLieu và List&lt;tTaiLieu_TacGia&gt;.</returns>
        private (tTaiLieu, List<tTaiLieu_TacGia>) GetTaiLieuFromForm()
        {
            // --- Ánh xạ Tên sang Mã (Sử dụng BLL) ---
            string maNXB = taiLieuBLL.LayMaNXB(cbNhaXuatBan.SelectedItem?.ToString());
            string maNN = taiLieuBLL.LayMaNN(cbNgonNgu.SelectedItem?.ToString());
            string maThL = taiLieuBLL.LayMaThL(cbTheLoai.SelectedItem?.ToString());
            string maDD = taiLieuBLL.LayMaDD(cbDinhDang.SelectedItem?.ToString());

            // Tạo đối tượng Tài liệu
            var taiLieu = new tTaiLieu
            {
                MaTL = txtMaTaiLieu.Text.Trim(),
                TenTL = txtTenTaiLieu.Text.Trim(),
                NamXuatBan = int.TryParse(txtNamXuatBan.Text, out int nam) ? (int?)nam : null,
                LanXuatBan = int.TryParse(txtLanXuatBan.Text, out int lan) ? (int?)lan : null,
                SoTrang = int.TryParse(txtSoTrang.Text, out int trang) ? (int?)trang : null,
                KhoCo = txtKhoCo.Text.Trim(),

                MaNXB = maNXB,
                MaNN = maNN,
                MaThL = maThL,
                MaDD = maDD,

                MaTK = "TK03" // Giả định: Mã tài khoản người dùng đang thao tác
            };

            // --- Lấy Danh sách Tác giả từ dgvQLTacGia_TaiLieu ---
            var danhSachTacGia = new List<tTaiLieu_TacGia>();
            if (dgvQLTacGia_TaiLieu.DataSource is List<TacGiaTaiLieuDTO> currentTacGiaList)
            {
                foreach (var tg in currentTacGiaList)
                {
                    if (!string.IsNullOrWhiteSpace(tg.MaTG))
                    {
                        danhSachTacGia.Add(new tTaiLieu_TacGia
                        {
                            MaTL = taiLieu.MaTL,
                            MaTG = tg.MaTG,
                            VaiTro = tg.VaiTro
                        });
                    }
                }
            }

            return (taiLieu, danhSachTacGia);
        }

        // ====================================================
        // IV. XỬ LÝ TRẠNG THÁI & GIAO DIỆN
        // ====================================================

        /// <summary>
        /// Thiết lập trạng thái hoạt động của form và cập nhật giao diện (Controls/Buttons) tương ứng.
        /// </summary>
        /// <param name="state">Trạng thái mới của form (View, Add, Edit, Delete, Search).</param>
        private void SetState(FormState state)
        {
            currentState = state;

            bool isView = (state == FormState.View);
            bool isAdd = (state == FormState.Add);
            bool isEdit = (state == FormState.Edit);
            bool isDelete = (state == FormState.Delete);
            bool isModifying = isAdd || isEdit;

            // --- Quản lý các nút chính ---
            btnThemTL.Enabled = isView;
            btnSuaTL.Enabled = isView || state == FormState.Search;
            btnXoaTL.Enabled = isView || state == FormState.Search;
            btnTimKiem.Enabled = isView || state == FormState.Search;
            btnDanhMucTaiLieu.Enabled = isView;
            btnXemBanSao.Enabled = isView || state == FormState.Search; // Nút Xem Bản sao

            btnLuu.Enabled = isAdd || isEdit || isDelete;
            btnHuy.Enabled = isAdd || isEdit || isDelete || state == FormState.Search;

            // --- QUẢN LÝ CÁC NÚT TÁC GIẢ (Chỉ cho phép khi ở chế độ Thêm/Sửa) ---
            btnThemTG.Enabled = isModifying;
            btnSuaTG.Enabled = isModifying;
            btnXoaTG.Enabled = isModifying;
            cbTacGia.Enabled = isModifying;
            cbVaiTro.Enabled = isModifying;


            // --- Quản lý DGV & Input Controls ---
            dgvQLTaiLieu.ReadOnly = !isView; // DGV chính chỉ cho phép tương tác (CellClick) khi View
            dgvQLTacGia_TaiLieu.ReadOnly = !isEdit; // DGV tác giả cho phép sửa Vai trò trực tiếp khi Edit
            dgvQLTacGia_TaiLieu.Enabled = !isDelete; // Vẫn cho phép xem khi Delete

            // Quản lý input (ReadOnly)
            txtMaTaiLieu.ReadOnly = !isAdd;
            txtTenTaiLieu.ReadOnly = !isModifying;
            txtNamXuatBan.ReadOnly = !isModifying;
            txtLanXuatBan.ReadOnly = !isModifying;
            txtSoTrang.ReadOnly = !isModifying;
            txtKhoCo.ReadOnly = !isModifying;

            // Quản lý ComboBoxes chính (Enabled)
            cbNhaXuatBan.Enabled = isModifying;
            cbNgonNgu.Enabled = isModifying;
            cbTheLoai.Enabled = isModifying;
            cbDinhDang.Enabled = isModifying;


            // --- Màu nền theo trạng thái ---
            switch (state)
            {
                case FormState.View:
                    this.BackColor = Color.White;
                    break;
                case FormState.Add:
                    this.BackColor = Color.LightGreen;
                    ClearForm();
                    break;
                case FormState.Edit:
                    this.BackColor = Color.LightBlue;
                    break;
                case FormState.Delete:
                    this.BackColor = Color.LightCoral;
                    break;
                case FormState.Search:
                    this.BackColor = Color.LightYellow;
                    break;
            }
        }

        // ====================================================
        // V. XỬ LÝ THAO TÁC CRUD TRÊN TÀI LIỆU
        // ====================================================

        private void btnThemTL_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Add);
        }

        private void btnSuaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;
            LoadTaiLieuToForm(); // Đảm bảo dữ liệu chi tiết mới nhất được load vào form
            previousState = currentState;
            SetState(FormState.Edit);
        }

        private void btnXoaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;
            LoadTaiLieuToForm(); // Đảm bảo dữ liệu chi tiết mới nhất được load vào form
            previousState = currentState;
            SetState(FormState.Delete);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var (taiLieu, danhSachTacGia) = GetTaiLieuFromForm();
            string errorMessage = string.Empty;
            string maTLVuaThaoTac = taiLieu.MaTL;
            bool success = false;

            // Xử lý logic theo trạng thái
            switch (currentState)
            {
                case FormState.Add:
                    success = taiLieuBLL.ThemTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
                    if (success) MessageBox.Show("Thêm tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case FormState.Edit:
                    success = taiLieuBLL.SuaTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
                    if (success) MessageBox.Show("Cập nhật tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case FormState.Delete:
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        success = taiLieuBLL.XoaTaiLieu(taiLieu.MaTL, out errorMessage);
                        if (success) MessageBox.Show("Xóa tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else return; // Người dùng hủy xóa
                    break;
            }

            if (!success)
            {
                MessageBox.Show($"{currentState} tài liệu thất bại: {errorMessage}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật DGV và Trạng thái sau khi thao tác thành công
            UpdateGridViewAndState(maTLVuaThaoTac);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (currentState == FormState.Search)
            {
                // Thoát khỏi Search → trở về View, tải lại toàn bộ dữ liệu
                dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
                SetupTaiLieuGridView();
                SetState(FormState.View);
            }
            else
            {
                // Hủy thao tác Add/Edit/Delete, quay lại trạng thái trước đó
                SetState(previousState);
            }

            // Tải lại thông tin chi tiết của dòng đang chọn (để khôi phục dữ liệu)
            LoadTaiLieuToForm();
            previousState = FormState.View; // Reset
        }

        /// <summary>
        /// Xử lý sự kiện click nút "Xem Bản sao". Mở form quản lý bản sao của tài liệu đang chọn.
        /// </summary>
        private void btnXemBanSao_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null || dgvQLTaiLieu.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một Tài liệu để xem Bản sao.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy Mã Tài liệu của dòng đang chọn (cần dùng reflection vì DataSource là anonymous type)
            var row = dgvQLTaiLieu.CurrentRow.DataBoundItem;
            string maTL = row?.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();

            if (!string.IsNullOrEmpty(maTL))
            {
                // Khởi tạo và hiển thị form quản lý bản sao
                frmBanSao frm = new frmBanSao(maTL);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Không tìm thấy Mã Tài liệu hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================
        // VI. XỬ LÝ TÌM KIẾM & DANH MỤC
        // ====================================================

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Search);

            // Lấy thuộc tính lọc cho module Tài liệu
            var ds = FilterHelper.GetFilterAttributes("TaiLieu");

            if (_frmTimKiem == null || _frmTimKiem.IsDisposed)
            {
                _frmTimKiem = new frmTimKiem(ds);

                // Đăng ký sự kiện khi form tìm kiếm trả về kết quả lọc
                _frmTimKiem.OnSearch += (s, filters) =>
                {
                    if (filters == null || filters.Count == 0 || filters.All(f => string.IsNullOrWhiteSpace(f.Value)))
                    {
                        MessageBox.Show("Vui lòng nhập điều kiện tìm kiếm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Gọi BLL để tìm kiếm
                    var result = taiLieuBLL.TimKiemTaiLieu(filters);
                    dgvQLTaiLieu.DataSource = result;
                    SetupTaiLieuGridView();

                    // Tải thông tin của tài liệu đầu tiên trong kết quả tìm kiếm
                    LoadTaiLieuToForm();
                };
            }

            _frmTimKiem.Show();
            _frmTimKiem.BringToFront();
        }

        private void btnDanhMucTaiLieu_Click(object sender, EventArgs e)
        {
            frmDanhMucTaiLieu frm = new frmDanhMucTaiLieu();

            // Đăng ký sự kiện để tải lại ComboBox sau khi form Danh mục cập nhật
            frm.DanhMucUpdated += LoadComboBoxData;

            frm.ShowDialog();
        }

        // ====================================================
        // VII. XỬ LÝ THAO TÁC TÁC GIẢ (TRONG DGV PHỤ)
        // ====================================================

        private void btnThemTG_Click(object sender, EventArgs e)
        {
            string tenTacGia = cbTacGia.SelectedItem?.ToString();
            string vaiTro = cbVaiTro.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(tenTacGia))
            {
                MessageBox.Show("Vui lòng chọn Tên Tác giả.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Map tên tác giả sang Mã tác giả
            string maTG = taiLieuBLL.LayMaTacGia(tenTacGia);
            if (string.IsNullOrEmpty(maTG))
            {
                MessageBox.Show("Không tìm thấy Mã Tác giả tương ứng.", "Lỗi Map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Lấy danh sách hiện tại từ DGV
            var currentTacGiaList = dgvQLTacGia_TaiLieu.DataSource as List<TacGiaTaiLieuDTO>;
            if (currentTacGiaList == null)
            {
                currentTacGiaList = new List<TacGiaTaiLieuDTO>();
            }

            // 3. Kiểm tra trùng lặp
            if (currentTacGiaList.Any(tg => tg.MaTG == maTG))
            {
                MessageBox.Show("Tác giả này đã được thêm vào tài liệu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Thêm tác giả mới vào danh sách DTO (sẽ được lưu vào DB khi nhấn btnLuu)
            currentTacGiaList.Add(new TacGiaTaiLieuDTO
            {
                MaTG = maTG,
                TenTacGia = tenTacGia,
                VaiTro = vaiTro
            });

            // 5. Cập nhật DGV
            dgvQLTacGia_TaiLieu.DataSource = null;
            dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
            SetupTacGiaGridView();
            dgvQLTacGia_TaiLieu.CurrentCell = dgvQLTacGia_TaiLieu.Rows[dgvQLTacGia_TaiLieu.Rows.Count - 1].Cells[1]; // Chọn dòng cuối
        }

        private void btnSuaTG_Click(object sender, EventArgs e)
        {
            if (dgvQLTacGia_TaiLieu.CurrentRow == null || dgvQLTacGia_TaiLieu.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn Tác giả cần sửa Vai trò.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string vaiTroMoi = cbVaiTro.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(vaiTroMoi))
            {
                MessageBox.Show("Vui lòng chọn Vai trò mới.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy đối tượng DTO đang được chọn và danh sách hiện tại
            var selectedItem = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem as TacGiaTaiLieuDTO;
            var currentTacGiaList = dgvQLTacGia_TaiLieu.DataSource as List<TacGiaTaiLieuDTO>;
            int rowIndex = dgvQLTacGia_TaiLieu.CurrentRow.Index;

            if (selectedItem != null && currentTacGiaList != null)
            {
                selectedItem.VaiTro = vaiTroMoi; // 1. Cập nhật DTO

                // 2. Kích hoạt lại Binding để DGV hiển thị thay đổi
                dgvQLTacGia_TaiLieu.DataSource = null;
                dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
                SetupTacGiaGridView();

                // 3. Chọn lại dòng vừa sửa
                dgvQLTacGia_TaiLieu.ClearSelection();
                dgvQLTacGia_TaiLieu.Rows[rowIndex].Selected = true;
                dgvQLTacGia_TaiLieu.CurrentCell = dgvQLTacGia_TaiLieu.Rows[rowIndex].Cells[1];

                MessageBox.Show($"Đã cập nhật Vai trò của Tác giả {selectedItem.TenTacGia} thành '{vaiTroMoi}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXoaTG_Click(object sender, EventArgs e)
        {
            if (dgvQLTacGia_TaiLieu.CurrentRow == null || dgvQLTacGia_TaiLieu.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một Tác giả để xóa khỏi danh sách Tài liệu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem as TacGiaTaiLieuDTO;
            var currentTacGiaList = dgvQLTacGia_TaiLieu.DataSource as List<TacGiaTaiLieuDTO>;

            if (selectedItem != null && currentTacGiaList != null)
            {
                // Xóa khỏi danh sách DTO (sẽ được cập nhật vào DB khi nhấn btnLuu)
                currentTacGiaList.Remove(selectedItem);

                // Cập nhật DGV
                dgvQLTacGia_TaiLieu.DataSource = null;
                dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
                SetupTacGiaGridView();

                // Tải lại combobox tác giả/vai trò (vì dòng đã bị xóa, dòng chọn có thể thay đổi)
                LoadTacGiaToComboboxes();
            }
        }

        // ====================================================
        // VIII. PHƯƠNG THỨC HỖ TRỢ
        // ====================================================

        /// <summary>
        /// Tải lại DGV chính, tái chọn dòng vừa thao tác và đặt lại trạng thái.
        /// </summary>
        /// <param name="maTLVuaThaoTac">Mã Tài liệu vừa được Thêm/Sửa/Xóa.</param>
        private void UpdateGridViewAndState(string maTLVuaThaoTac)
        {
            // 1. Tải lại toàn bộ danh sách Tài liệu
            dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
            SetupTaiLieuGridView();

            // 2. Tái chọn dòng (Tìm và chọn lại tài liệu vừa thao tác)
            bool found = false;
            if (!string.IsNullOrEmpty(maTLVuaThaoTac))
            {
                foreach (DataGridViewRow row in dgvQLTaiLieu.Rows)
                {
                    var rowData = row.DataBoundItem;
                    string maTLTrongDGV = rowData?.GetType().GetProperty("MaTL")?.GetValue(rowData)?.ToString();

                    if (maTLTrongDGV == maTLVuaThaoTac)
                    {
                        dgvQLTaiLieu.ClearSelection();
                        row.Selected = true;
                        dgvQLTaiLieu.CurrentCell = row.Cells[0];
                        found = true;
                        break;
                    }
                }
            }

            // Nếu không tìm thấy (hoặc vừa xóa) và còn dữ liệu, chọn dòng đầu tiên
            if (!found && dgvQLTaiLieu.Rows.Count > 0)
            {
                dgvQLTaiLieu.ClearSelection();
                dgvQLTaiLieu.Rows[0].Selected = true;
                dgvQLTaiLieu.CurrentCell = dgvQLTaiLieu.Rows[0].Cells[0];
            }

            // 3. Đặt trạng thái về View/Search và tải lại form chi tiết
            SetState(previousState == FormState.Search ? FormState.Search : FormState.View);
            LoadTaiLieuToForm();
            previousState = FormState.View; // Reset trạng thái trước đó
        }
    }
}