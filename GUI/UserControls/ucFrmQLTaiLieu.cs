using QuanLyThuVien.BLL;
using QuanLyThuVien.DAL; // Cần thiết cho tTaiLieu, tTaiLieu_TacGia
using QuanLyThuVien.DTO; // Cần thiết cho TacGiaTaiLieuDTO
using QuanLyThuVien.GUI.Forms; // Cần thiết cho frmTimKiem, frmDanhMucTaiLieu
using QuanLyThuVien.Helpers; // Cần thiết cho FormState, FilterHelper
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
    /// <summary>
    /// User Control quản lý thông tin Tài liệu trong thư viện.
    /// </summary>
    public partial class ucFrmQLTaiLieu : UserControl
    {
        private QLTaiLieuBLL taiLieuBLL = new QLTaiLieuBLL();

        // Khai báo trạng thái của form
        private FormState currentState = FormState.View;
        private FormState previousState = FormState.View;
        private frmTimKiem _frmTimKiem;

        public ucFrmQLTaiLieu()
        {
            InitializeComponent();
        }

        // ====================================================
        // I. KHỞI TẠO & LOAD DỮ LIỆU BAN ĐẦU
        // ====================================================

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

                // 4. Đặt trạng thái ban đầu
                SetState(FormState.View);

                // 5. Gán sự kiện
                dgvQLTaiLieu.CellClick += dgvQLTaiLieu_CellClick;
                dgvQLTacGia_TaiLieu.CellClick += dgvQLTacGia_TaiLieu_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu tài liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải danh sách đầy đủ các giá trị (NXB, NN, Thể loại, Định dạng, Tác giả, Vai trò) cho các ComboBox.
        /// </summary>
        private void LoadComboBoxData()
        {
            try
            {
                // 1. Tải dữ liệu cho các ComboBox thuộc Tài liệu
                cbNhaXuatBan.DataSource = taiLieuBLL.LayDanhSachNhaXuatBan();
                cbNgonNgu.DataSource = taiLieuBLL.LayDanhSachNgonNgu();
                cbTheLoai.DataSource = taiLieuBLL.LayDanhSachTheLoai();
                cbDinhDang.DataSource = taiLieuBLL.LayDanhSachDinhDang();

                // 2. Tải dữ liệu cho các ComboBox thuộc Tác giả (để chọn khi Thêm/Sửa)
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

        // ----------------------------------------------------
        // Cấu hình DataGridView
        // ----------------------------------------------------

        private void SetupTaiLieuGridView()
        {
            if (dgvQLTaiLieu.Columns.Count == 0) return;

            dgvQLTaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvQLTaiLieu.RowHeadersVisible = false;
            dgvQLTaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTaiLieu.ReadOnly = true;
            dgvQLTaiLieu.AllowUserToResizeColumns = false;
            dgvQLTaiLieu.MultiSelect = false;

            // Cấu hình từng cột (Giữ nguyên logic đặt tên cột và kích thước)
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

            foreach (DataGridViewColumn col in dgvQLTaiLieu.Columns)
            {
                if (string.IsNullOrEmpty(col.HeaderText))
                {
                    col.Visible = false;
                }
            }
            dgvQLTaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupTacGiaGridView()
        {
            if (dgvQLTacGia_TaiLieu.Columns.Count == 0) return;

            dgvQLTacGia_TaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvQLTacGia_TaiLieu.RowHeadersVisible = false;
            dgvQLTacGia_TaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTacGia_TaiLieu.AllowUserToResizeColumns = false;

            // Tên cột: MaTG, TenTacGia, VaiTro (Dựa trên TacGiaTaiLieuDTO)
            dgvQLTacGia_TaiLieu.Columns["MaTG"].Visible = false;

            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].HeaderText = "Tên Tác giả";
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].ReadOnly = true; // Luôn chỉ đọc
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].Width = 200;
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvQLTacGia_TaiLieu.Columns["VaiTro"].HeaderText = "Vai trò";
            dgvQLTacGia_TaiLieu.Columns["VaiTro"].Width = 150;
            // Thuộc tính ReadOnly của cột VaiTro sẽ được SetState quản lý.

            dgvQLTacGia_TaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // ====================================================
        // II. XỬ LÝ SỰ KIỆN CELL CLICK & LOAD CHI TIẾT
        // ====================================================

        private void dgvQLTaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 1. Tải danh sách tác giả mới cho tài liệu vừa chọn
            LoadTacGiaBySelectedTaiLieu();

            // 2. Tải thông tin tài liệu và tác giả đang chọn lên form
            LoadTaiLieuToForm();
        }

        private void dgvQLTacGia_TaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Chỉ cần tải thông tin tác giả/vai trò của dòng được chọn lên các combobox
            LoadTacGiaToComboboxes();
        }

        /// <summary>
        /// Lấy Mã Tài liệu từ dòng đang chọn trong dgvQLTaiLieu và tải danh sách Tác giả tương ứng vào dgvQLTacGia_TaiLieu (sử dụng DTO có thể sửa).
        /// </summary>
        private void LoadTacGiaBySelectedTaiLieu()
        {
            if (dgvQLTaiLieu.CurrentRow == null || dgvQLTaiLieu.Rows.Count == 0)
            {
                dgvQLTacGia_TaiLieu.DataSource = null;
                return;
            }

            var row = dgvQLTaiLieu.CurrentRow.DataBoundItem;
            string maTL = row.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();

            if (string.IsNullOrEmpty(maTL))
            {
                dgvQLTacGia_TaiLieu.DataSource = null;
                return;
            }

            try
            {
                // 1. Lấy List<object> (Anonymous Type) từ DAL
                var rawTacGiaList = taiLieuBLL.LayTacGiaTheoMaTaiLieu(maTL);

                // 2. Chuyển đổi sang List<TacGiaTaiLieuDTO> (Mutable DTO)
                var dtoTacGiaList = rawTacGiaList.Select(tg => new TacGiaTaiLieuDTO
                {
                    MaTG = tg.GetType().GetProperty("MaTG")?.GetValue(tg)?.ToString(),
                    TenTacGia = tg.GetType().GetProperty("TenTacGia")?.GetValue(tg)?.ToString(),
                    VaiTro = tg.GetType().GetProperty("VaiTro")?.GetValue(tg)?.ToString()
                }).ToList();

                // 3. Gán nguồn dữ liệu cho dgvQLTacGia_TaiLieu
                dgvQLTacGia_TaiLieu.DataSource = dtoTacGiaList;

                // 4. Gọi Setup
                SetupTacGiaGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách tác giả: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải thông tin chi tiết tài liệu từ dòng đang chọn trong dgvQLTaiLieu lên các controls nhập liệu (TextBox, ComboBox).
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

            txtMaTaiLieu.Text = row.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();
            txtTenTaiLieu.Text = row.GetType().GetProperty("TenTL")?.GetValue(row)?.ToString();
            txtNamXuatBan.Text = row.GetType().GetProperty("NamXuatBan")?.GetValue(row)?.ToString();
            txtLanXuatBan.Text = row.GetType().GetProperty("LanXuatBan")?.GetValue(row)?.ToString();
            txtSoTrang.Text = row.GetType().GetProperty("SoTrang")?.GetValue(row)?.ToString();
            txtKhoCo.Text = row.GetType().GetProperty("KhoCo")?.GetValue(row)?.ToString();

            // ComboBoxes
            cbNhaXuatBan.SelectedItem = row.GetType().GetProperty("TenNXB")?.GetValue(row)?.ToString();
            cbNgonNgu.SelectedItem = row.GetType().GetProperty("TenNN")?.GetValue(row)?.ToString();
            cbTheLoai.SelectedItem = row.GetType().GetProperty("TenTheLoai")?.GetValue(row)?.ToString();
            cbDinhDang.SelectedItem = row.GetType().GetProperty("TenDinhDang")?.GetValue(row)?.ToString();

            // 4. Tải thông tin Tác giả đang được chọn trong DGV tác giả lên các ComboBox tác giả
            LoadTacGiaToComboboxes();
        }

        /// <summary>
        /// Tải thông tin tác giả từ dòng đang chọn trong dgvQLTacGia_TaiLieu lên cbTacGia và cbVaiTro.
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
            cbTacGia.SelectedItem = row.GetType().GetProperty("TenTacGia")?.GetValue(row)?.ToString();
            cbVaiTro.SelectedItem = row.GetType().GetProperty("VaiTro")?.GetValue(row)?.ToString();
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
        /// Lấy dữ liệu Tài liệu từ các controls nhập liệu và danh sách Tác giả từ dgvQLTacGia_TaiLieu.
        /// </summary>
        /// <returns>Tuple chứa đối tượng tTaiLieu và List<tTaiLieu_TacGia>.</returns>
        private (tTaiLieu, List<tTaiLieu_TacGia>) GetTaiLieuFromForm()
        {
            // --- Ánh xạ Tên sang Mã ---
            string maNXB = taiLieuBLL.LayMaNXB(cbNhaXuatBan.SelectedItem?.ToString());
            string maNN = taiLieuBLL.LayMaNN(cbNgonNgu.SelectedItem?.ToString());
            string maThL = taiLieuBLL.LayMaThL(cbTheLoai.SelectedItem?.ToString());
            string maDD = taiLieuBLL.LayMaDD(cbDinhDang.SelectedItem?.ToString());

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

                MaTK = "TK03" // Giả định: Người dùng đang đăng nhập (MaTK)
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
        // III. XỬ LÝ TRẠNG THÁI & GIAO DIỆN
        // ====================================================

        /// <summary>
        /// Thiết lập trạng thái hoạt động của form và cập nhật giao diện (Controls/Buttons).
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

            btnLuu.Enabled = isAdd || isEdit || isDelete;
            btnHuy.Enabled = isAdd || isEdit || isDelete || state == FormState.Search;

            // --- QUẢN LÝ CÁC NÚT TÁC GIẢ ---
            btnThemTG.Enabled = isModifying;
            btnSuaTG.Enabled = isModifying;
            btnXoaTG.Enabled = isModifying;

            // --- Quản lý DGV & Input Controls ---
            dgvQLTaiLieu.ReadOnly = !(isView || state == FormState.Search);

            // Cho phép sửa Vai trò trực tiếp trên lưới chỉ khi EDIT (nếu DGV đang hiển thị DTO)
            dgvQLTacGia_TaiLieu.ReadOnly = !(isEdit);
            dgvQLTacGia_TaiLieu.Enabled = !isDelete;

            // Quản lý input
            txtMaTaiLieu.ReadOnly = !isAdd;
            txtTenTaiLieu.ReadOnly = !(isModifying);
            txtNamXuatBan.ReadOnly = !(isModifying);
            txtLanXuatBan.ReadOnly = !(isModifying);
            txtSoTrang.ReadOnly = !(isModifying);
            txtKhoCo.ReadOnly = !(isModifying);

            // Quản lý ComboBoxes chính
            cbNhaXuatBan.Enabled = isModifying;
            cbNgonNgu.Enabled = isModifying;
            cbTheLoai.Enabled = isModifying;
            cbDinhDang.Enabled = isModifying;

            // Quản lý ComboBoxes Tác giả
            cbTacGia.Enabled = isModifying;
            cbVaiTro.Enabled = isModifying;

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
        // IV. XỬ LÝ THAO TÁC CRUD
        // ====================================================

        private void btnThemTL_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Add);
        }

        private void btnSuaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;
            // Tải lại dữ liệu chi tiết của dòng đang chọn (đảm bảo dữ liệu mới nhất được load vào form)
            LoadTaiLieuToForm();
            previousState = currentState;
            SetState(FormState.Edit);
        }

        private void btnXoaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;
            // Tải lại dữ liệu chi tiết của dòng đang chọn
            LoadTaiLieuToForm();
            previousState = currentState;
            SetState(FormState.Delete);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var (taiLieu, danhSachTacGia) = GetTaiLieuFromForm();
            string errorMessage = string.Empty;
            string maTLVuaThaoTac = taiLieu.MaTL;
            bool success = false;

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

            // --- Cập nhật DGV và Trạng thái sau khi thao tác thành công ---
            UpdateGridViewAndState(maTLVuaThaoTac);
        }

        /// <summary>
        /// Tải lại DGV chính, tái chọn dòng vừa thao tác và đặt lại trạng thái.
        /// </summary>
        /// <param name="maTLVuaThaoTac">Mã Tài liệu vừa được Thêm/Sửa/Xóa.</param>
        private void UpdateGridViewAndState(string maTLVuaThaoTac)
        {
            // 1. Tải lại toàn bộ danh sách Tài liệu
            dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
            SetupTaiLieuGridView();

            // 2. Tái chọn dòng
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
            previousState = FormState.View; // Reset
            LoadTaiLieuToForm();
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
                // Hủy thao tác Add/Edit/Delete, quay lại trạng thái trước đó (View hoặc Search)
                SetState(previousState);
            }

            // Tải lại thông tin chi tiết của dòng đang chọn (để khôi phục dữ liệu)
            LoadTaiLieuToForm();
            previousState = FormState.View;
        }

        // ====================================================
        // V. XỬ LÝ TÌM KIẾM & DANH MỤC
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
        // VI. XỬ LÝ THAO TÁC TÁC GIẢ (TẠM THỜI TRONG DGV PHỤ)
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

            // 4. Thêm tác giả mới vào danh sách DTO
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

            // Lấy đối tượng DTO đang được chọn trong DGV (vì DGV dùng List<DTO> nên sửa trực tiếp được)
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
                MessageBox.Show("Vui lòng chọn một Tác giả để xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem as TacGiaTaiLieuDTO;
            var currentTacGiaList = dgvQLTacGia_TaiLieu.DataSource as List<TacGiaTaiLieuDTO>;

            if (selectedItem != null && currentTacGiaList != null)
            {
                // Xóa khỏi danh sách DTO
                currentTacGiaList.Remove(selectedItem);

                // Cập nhật DGV
                dgvQLTacGia_TaiLieu.DataSource = null;
                dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
                SetupTacGiaGridView();

                // Tải lại combobox tác giả/vai trò (vì dòng đã bị xóa)
                LoadTacGiaToComboboxes();
            }
        }
    }
}