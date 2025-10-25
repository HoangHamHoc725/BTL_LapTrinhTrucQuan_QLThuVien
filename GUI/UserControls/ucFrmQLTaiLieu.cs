using QuanLyThuVien.BLL;
using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;
using QuanLyThuVien.GUI.Forms;
using QuanLyThuVien.Helpers;
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

        private void ucFrmQLTaiLieu_Load(object sender, EventArgs e)
        {
            try
            {
                // ***** Bổ sung: Tải dữ liệu cho các ComboBox trước tiên *****
                LoadComboBoxData();

                // 1. Tải và setup dgvQLTaiLieu
                dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
                SetupTaiLieuGridView();

                // 2. Tải tác giả của tài liệu đầu tiên
                LoadTacGiaBySelectedTaiLieu();

                // 3. Tải thông tin chi tiết tài liệu đầu tiên lên form
                LoadTaiLieuToForm();

                // 4. ĐẶT TRẠNG THÁI BAN ĐẦU LÀ VIEW (QUAN TRỌNG)
                SetState(FormState.View);

                // 5. Gán sự kiện
                dgvQLTaiLieu.CellClick += dgvQLTaiLieu_CellClick;
                dgvQLTacGia_TaiLieu.CellClick += dgvQLTacGia_TaiLieu_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu tài liệu: " + ex.Message);
            }
        }

        // Lấy dữ liệu tài liệu và danh sách tác giả từ form
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

                MaTK = "TK03" // Mặc định Mã Tài khoản là TK03 (cho người dùng đang đăng nhập)
            };

            // --- Lấy Danh sách Tác giả từ dgvQLTacGia_TaiLieu (SỬA ĐỔI) ---
            var danhSachTacGia = new List<tTaiLieu_TacGia>();

            // Đọc DGV bằng cách kiểm tra DTO Class mới
            if (dgvQLTacGia_TaiLieu.DataSource is List<TacGiaTaiLieuDTO> currentTacGiaList)
            {
                foreach (var tg in currentTacGiaList)
                {
                    // Đọc trực tiếp từ DTO (vì DTO đã được cập nhật khi người dùng sửa VaiTro trên DGV)
                    string maTG = tg.MaTG;
                    string vaiTro = tg.VaiTro;

                    if (!string.IsNullOrWhiteSpace(maTG))
                    {
                        danhSachTacGia.Add(new tTaiLieu_TacGia
                        {
                            MaTL = taiLieu.MaTL,
                            MaTG = maTG,
                            VaiTro = vaiTro
                        });
                    }
                }
            }
            else if (dgvQLTacGia_TaiLieu.DataSource != null)
            {
                // Xử lý trường hợp DataSource là List<object> (Anonymous)
                // Đây là fallback (dữ liệu cũ, không sửa được) nhưng cần thiết để tránh mất dữ liệu nếu chưa dùng DTO
                // Tuy nhiên, nếu bạn chỉ cho phép sửa trong chế độ Edit và dùng DTO, phần này có thể bỏ qua.
            }

            return (taiLieu, danhSachTacGia);
        }

        private void dgvQLTaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // 1. Tải danh sách tác giả mới cho tài liệu vừa chọn
            LoadTacGiaBySelectedTaiLieu();

            // 2. Tải thông tin tài liệu và tác giả đầu tiên lên form
            LoadTaiLieuToForm(); // LoadTaiLieuToForm sẽ gọi LoadTacGiaToComboboxes()
        }

        // Xử lý khi click vào dgvQLTacGia_TaiLieu để cập nhật cbTacGia và cbVaiTro
        private void dgvQLTacGia_TaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Chỉ cần tải thông tin tác giả/vai trò của dòng được chọn lên các combobox
            LoadTacGiaToComboboxes();
        }

        private void SetupTaiLieuGridView()
        {
            if (dgvQLTaiLieu.Columns.Count == 0)
                return;

            // Cấu hình chung theo mẫu
            dgvQLTaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Dùng None để tự đặt Width
            dgvQLTaiLieu.RowHeadersVisible = false;
            dgvQLTaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTaiLieu.ReadOnly = true;
            dgvQLTaiLieu.AllowUserToResizeColumns = false;
            dgvQLTaiLieu.MultiSelect = false; // Đảm bảo chỉ chọn một tài liệu chính

            // Cấu hình từng cột (Dựa trên kết quả trả về từ LayTatCaThongTinTaiLieu)
            dgvQLTaiLieu.Columns["MaTL"].HeaderText = "Mã tài liệu";
            dgvQLTaiLieu.Columns["MaTL"].Width = 100;

            dgvQLTaiLieu.Columns["TenTL"].HeaderText = "Tên tài liệu";
            dgvQLTaiLieu.Columns["TenTL"].Width = 300;

            dgvQLTaiLieu.Columns["LanXuatBan"].HeaderText = "Lần XB";
            dgvQLTaiLieu.Columns["LanXuatBan"].Width = 60;

            dgvQLTaiLieu.Columns["NamXuatBan"].HeaderText = "Năm XB";
            dgvQLTaiLieu.Columns["NamXuatBan"].Width = 80;

            dgvQLTaiLieu.Columns["SoTrang"].HeaderText = "Số trang";
            dgvQLTaiLieu.Columns["SoTrang"].Width = 80;

            dgvQLTaiLieu.Columns["KhoCo"].HeaderText = "Khổ cỡ";
            dgvQLTaiLieu.Columns["KhoCo"].Width = 100;

            dgvQLTaiLieu.Columns["TenNXB"].HeaderText = "Nhà xuất bản";
            dgvQLTaiLieu.Columns["TenNXB"].Width = 150;

            dgvQLTaiLieu.Columns["TenNN"].HeaderText = "Ngôn ngữ";
            dgvQLTaiLieu.Columns["TenNN"].Width = 100;

            dgvQLTaiLieu.Columns["TenTheLoai"].HeaderText = "Thể loại";
            dgvQLTaiLieu.Columns["TenTheLoai"].Width = 120;

            dgvQLTaiLieu.Columns["TenDinhDang"].HeaderText = "Định dạng";
            dgvQLTaiLieu.Columns["TenDinhDang"].Width = 100;

            // Đảm bảo các cột khác không hiển thị (nếu có)
            foreach (DataGridViewColumn col in dgvQLTaiLieu.Columns)
            {
                if (col.HeaderText == null)
                {
                    col.Visible = false;
                }
            }

            dgvQLTaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupTacGiaGridView()
        {
            if (dgvQLTacGia_TaiLieu.Columns.Count == 0)
                return;

            // Cấu hình chung theo mẫu
            dgvQLTacGia_TaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvQLTacGia_TaiLieu.RowHeadersVisible = false;
            dgvQLTacGia_TaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvQLTacGia_TaiLieu.ReadOnly = true;
            dgvQLTacGia_TaiLieu.AllowUserToResizeColumns = false;

            // Cấu hình từng cột (Dựa trên kết quả trả về từ LayTacGiaTheoMaTaiLieu)
            // Tên cột: MaTG, TenTacGia, VaiTro

            // Ẩn cột Mã Tác giả
            dgvQLTacGia_TaiLieu.Columns["MaTG"].Visible = false;

            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].HeaderText = "Tên Tác giả";
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].Width = 200;
            dgvQLTacGia_TaiLieu.Columns["TenTacGia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Tự động lấp đầy

            dgvQLTacGia_TaiLieu.Columns["VaiTro"].HeaderText = "Vai trò";
            dgvQLTacGia_TaiLieu.Columns["VaiTro"].Width = 150;

            dgvQLTacGia_TaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

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
                var dtoTacGiaList = rawTacGiaList.Select(tg => new TacGiaTaiLieuDTO // ĐÃ SỬA LỖI: Dùng class DTO đã được định nghĩa
                {
                    MaTG = tg.GetType().GetProperty("MaTG")?.GetValue(tg)?.ToString(),
                    TenTacGia = tg.GetType().GetProperty("TenTacGia")?.GetValue(tg)?.ToString(),
                    VaiTro = tg.GetType().GetProperty("VaiTro")?.GetValue(tg)?.ToString()
                }).ToList();

                // 3. Gán nguồn dữ liệu cho dgvQLTacGia_TaiLieu
                dgvQLTacGia_TaiLieu.DataSource = dtoTacGiaList;

                // 4. Gọi Setup
                SetupTacGiaGridView();

                // Đảm bảo cột TenTacGia chỉ đọc (chỉ VaiTro được phép sửa trên lưới)
                if (dgvQLTacGia_TaiLieu.Columns.Contains("TenTacGia"))
                {
                    dgvQLTacGia_TaiLieu.Columns["TenTacGia"].ReadOnly = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách tác giả: " + ex.Message);
            }
        }

        // Tải thông tin chi tiết tài liệu từ dgvQLTaiLieu lên các control nhập liệu
        private void LoadTaiLieuToForm()
        {
            // 1. Đảm bảo có dòng được chọn. Nếu không, chọn dòng đầu tiên.
            if (dgvQLTaiLieu.Rows.Count > 0 && dgvQLTaiLieu.CurrentRow == null)
            {
                // Chọn dòng đầu tiên nếu DGV có dữ liệu nhưng không có dòng nào được focus
                dgvQLTaiLieu.Rows[0].Selected = true;
                dgvQLTaiLieu.CurrentCell = dgvQLTaiLieu.Rows[0].Cells[0];
            }

            // Kiểm tra và thoát nếu không có dòng nào đang được chọn
            if (dgvQLTaiLieu.CurrentRow == null || dgvQLTaiLieu.CurrentRow.DataBoundItem == null)
            {
                ClearForm(); // Xóa trắng khi không có dữ liệu
                return;
            }

            // 2. Tải Tác giả cho DGV phụ trước (để đảm bảo DGV tác giả có dữ liệu liên quan)
            LoadTacGiaBySelectedTaiLieu();

            // 3. Tải dữ liệu Tài liệu lên các controls
            var row = dgvQLTaiLieu.CurrentRow.DataBoundItem;

            txtMaTaiLieu.Text = row.GetType().GetProperty("MaTL")?.GetValue(row)?.ToString();
            txtTenTaiLieu.Text = row.GetType().GetProperty("TenTL")?.GetValue(row)?.ToString();

            // TextBoxs
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

        // Tải danh sách đầy đủ các giá trị (NXB, NN, Thể loại, Định dạng, Tác giả, Vai trò) cho các ComboBox
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

                // Đặt mặc định không chọn gì sau khi load (trừ khi cần mặc định)
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

        // Tải thông tin tác giả từ dgvQLTacGia_TaiLieu lên cbTacGia và cbVaiTro
        private void LoadTacGiaToComboboxes()
        {
            if (dgvQLTacGia_TaiLieu.CurrentRow == null || dgvQLTacGia_TaiLieu.Rows.Count == 0)
            {
                // Xóa trắng ComboBox tác giả nếu không có tác giả
                cbTacGia.SelectedIndex = -1;
                cbVaiTro.SelectedIndex = -1;
                return;
            }

            var row = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem;

            // Gán dữ liệu cho combobox Tác giả và Vai trò
            cbTacGia.SelectedItem = row.GetType().GetProperty("TenTacGia")?.GetValue(row)?.ToString();
            cbVaiTro.SelectedItem = row.GetType().GetProperty("VaiTro")?.GetValue(row)?.ToString();
        }

        // Thiết lập trạng thái hoạt động của form (View, Add, Edit, Delete, Search)
        private void SetState(FormState state)
        {
            currentState = state;

            bool isView = (state == FormState.View);
            bool isAdd = (state == FormState.Add);
            bool isEdit = (state == FormState.Edit);
            bool isDelete = (state == FormState.Delete);
            bool isSearch = (state == FormState.Search);

            // Biến kiểm tra chế độ cho phép thay đổi tác giả
            bool isModifying = isAdd || isEdit; 

            // --- Quản lý các nút chính (Giữ nguyên) ---
            btnThemTL.Enabled = isView;
            btnSuaTL.Enabled = isView || isSearch;
            btnXoaTL.Enabled = isView || isSearch;
            btnTimKiem.Enabled = isView || isSearch;

            btnLuu.Enabled = isAdd || isEdit || isDelete;
            btnHuy.Enabled = isAdd || isEdit || isDelete || isSearch;

            // --- QUẢN LÝ CÁC NÚT TÁC GIẢ (THEO YÊU CẦU MỚI) ---
            // Chỉ ENABLE khi ở trạng thái ADD hoặc EDIT
            btnThemTG.Enabled = isModifying;
            btnSuaTG.Enabled = isModifying;
            btnXoaTG.Enabled = isModifying;


            // --- Quản lý DGV ---
            dgvQLTaiLieu.ReadOnly = !(isView || isSearch);
            dgvQLTaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Đặt ReadOnly cho DGV tác giả
            // Cho phép sửa Vai trò trực tiếp trên lưới chỉ khi EDIT
            dgvQLTacGia_TaiLieu.ReadOnly = !(isEdit);

            if (isDelete)
            {
                dgvQLTacGia_TaiLieu.Enabled = false;
            }
            else
            {
                dgvQLTacGia_TaiLieu.Enabled = true;
            }

            // --- Quản lý input ---
            // Giả định: Mã TL chỉ nhập khi thêm
            txtMaTaiLieu.ReadOnly = !isAdd;
            txtTenTaiLieu.ReadOnly = !(isModifying);
            txtNamXuatBan.ReadOnly = !(isModifying);
            txtLanXuatBan.ReadOnly = !(isModifying);
            txtSoTrang.ReadOnly = !(isModifying);
            txtKhoCo.ReadOnly = !(isModifying);

            cbNhaXuatBan.Enabled = isModifying;
            cbNgonNgu.Enabled = isModifying;
            cbTheLoai.Enabled = isModifying;
            cbDinhDang.Enabled = isModifying;

            // Bật ComboBox Tác giả/Vai trò (Controls dùng để chọn TG thêm/sửa)
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

        // Xóa trắng nội dung tất cả các control nhập liệu trên form
        private void ClearForm()
        {
            txtMaTaiLieu.Clear();
            txtTenTaiLieu.Clear();
            txtNamXuatBan.Clear();
            txtLanXuatBan.Clear();
            txtSoTrang.Clear();
            txtKhoCo.Clear();

            // Đặt ComboBoxes về trạng thái chưa chọn
            cbNhaXuatBan.SelectedIndex = -1;
            cbNgonNgu.SelectedIndex = -1;
            cbTheLoai.SelectedIndex = -1;
            cbDinhDang.SelectedIndex = -1;
            cbTacGia.SelectedIndex = -1;
            cbVaiTro.SelectedIndex = -1;

            dgvQLTacGia_TaiLieu.DataSource = null; // Xóa dữ liệu tác giả
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            previousState = currentState; // Lưu trạng thái trước đó
            SetState(FormState.Search);

            // Lấy thuộc tính lọc cho module Tài liệu
            var ds = FilterHelper.GetFilterAttributes("TaiLieu"); // CẦN THÊM LOGIC CHO "TaiLieu"

            if (_frmTimKiem == null || _frmTimKiem.IsDisposed)
            {
                // Giả định frmTimKiem đã được tạo
                _frmTimKiem = new frmTimKiem(ds);

                _frmTimKiem.OnSearch += (s, filters) =>
                {
                    if (filters == null || filters.Count == 0)
                    {
                        MessageBox.Show("Không có bộ lọc nào được chọn!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Gọi BLL để tìm kiếm
                    var result = taiLieuBLL.TimKiemTaiLieu(filters); // CẦN THÊM TimKiemTaiLieu trong BLL
                    dgvQLTaiLieu.DataSource = result;
                    SetupTaiLieuGridView();

                    // Tải thông tin của tài liệu đầu tiên trong kết quả tìm kiếm
                    LoadTaiLieuToForm();
                };
            }

            _frmTimKiem.Show();
            _frmTimKiem.BringToFront();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu đang ở Search
            if (currentState == FormState.Search)
            {
                // Thoát khỏi Search → trở về View, tải lại toàn bộ dữ liệu
                dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
                SetupTaiLieuGridView();
                LoadTaiLieuToForm(); // Load lại thông tin của tài liệu đầu tiên (nếu có)
                SetState(FormState.View);
                previousState = FormState.View; // reset
                return;
            }

            // Nếu đang trong Add/Edit/Delete và trước đó là Search, quay lại Search
            if (previousState == FormState.Search)
            {
                // Quay lại trạng thái Search, giữ nguyên kết quả tìm kiếm
                SetState(FormState.Search);
                previousState = FormState.View; // reset để lần sau Hủy thoát được
            }
            else
            {
                // Quay lại trạng thái View, tải lại toàn bộ dữ liệu
                dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
                SetupTaiLieuGridView();
                LoadTaiLieuToForm(); // Load lại thông tin của tài liệu đầu tiên (nếu có)
                SetState(FormState.View);
                previousState = FormState.View;
            }
            // Load lại thông tin hiện tại của dòng đang chọn (nếu có)
            // dgvQLTaiLieu_CellClick(null, new DataGridViewCellEventArgs(0, dgvQLTaiLieu.CurrentRow?.Index ?? 0));
        }

        private void btnThemTL_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Add);
        }

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
                // Khởi tạo danh sách nếu đang ở chế độ Add (dgvQLTacGia_TaiLieu.DataSource = null)
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
            dgvQLTacGia_TaiLieu.DataSource = null; // Reset DataSource
            dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
            SetupTacGiaGridView(); // Đảm bảo DGV hiển thị đúng
        }

        private void btnSuaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;

            // Tải lại dữ liệu chi tiết của dòng đang chọn
            LoadTacGiaBySelectedTaiLieu(); // Load data cho dgvQLTacGia_TaiLieu
            LoadTaiLieuToForm();         // Load data cho các controls

            previousState = currentState;
            SetState(FormState.Edit);
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

            // Lấy đối tượng DTO đang được chọn trong DGV
            var selectedItem = dgvQLTacGia_TaiLieu.CurrentRow.DataBoundItem as TacGiaTaiLieuDTO;
            var currentTacGiaList = dgvQLTacGia_TaiLieu.DataSource as List<TacGiaTaiLieuDTO>;

            if (selectedItem != null && currentTacGiaList != null)
            {
                // 1. Cập nhật thuộc tính VaiTro trên đối tượng DTO
                selectedItem.VaiTro = vaiTroMoi;

                // 2. Kích hoạt lại Binding để DGV hiển thị thay đổi
                dgvQLTacGia_TaiLieu.DataSource = null;
                dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
                SetupTacGiaGridView();

                // Optional: Chọn lại dòng vừa sửa (để giữ focus)
                // Đây là cách đơn giản nhất để refresh DGV khi sửa trực tiếp trên DataSource.
                dgvQLTacGia_TaiLieu.CurrentRow.Selected = true;

                MessageBox.Show($"Đã cập nhật Vai trò của Tác giả {selectedItem.TenTacGia} thành '{vaiTroMoi}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnXoaTL_Click(object sender, EventArgs e)
        {
            if (dgvQLTaiLieu.CurrentRow == null) return;
            previousState = currentState;
            SetState(FormState.Delete);
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
                dgvQLTacGia_TaiLieu.DataSource = null; // Reset DataSource
                dgvQLTacGia_TaiLieu.DataSource = currentTacGiaList;
                SetupTacGiaGridView();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ form
            var (taiLieu, danhSachTacGia) = GetTaiLieuFromForm();
            string errorMessage;
            string maTLVuaThaoTac = taiLieu.MaTL;

            switch (currentState)
            {
                case FormState.Add:
                    if (taiLieuBLL.ThemTaiLieu(taiLieu, danhSachTacGia, out errorMessage))
                    {
                        MessageBox.Show("Thêm tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Thêm tài liệu thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;

                case FormState.Edit:
                    if (taiLieuBLL.SuaTaiLieu(taiLieu, danhSachTacGia, out errorMessage))
                    {
                        MessageBox.Show("Cập nhật tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật tài liệu thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;

                case FormState.Delete:
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài liệu này không?",
                                         "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        if (taiLieuBLL.XoaTaiLieu(taiLieu.MaTL, out errorMessage))
                        {
                            MessageBox.Show("Xóa tài liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xóa tài liệu thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        return; // Người dùng chọn "Không"
                    }
                    break;
            }

            // --- Cập nhật DGV và Trạng thái sau khi thao tác thành công ---

            // 1. Tải lại toàn bộ danh sách Tài liệu
            dgvQLTaiLieu.DataSource = taiLieuBLL.LayTatCaThongTinTaiLieu();
            SetupTaiLieuGridView();

            // 2. Tái chọn dòng (cực kỳ quan trọng)
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
            if (previousState == FormState.Search)
            {
                SetState(FormState.Search);
            }
            else
            {
                SetState(FormState.View);
            }

            // Tải lại thông tin chi tiết (sẽ kích hoạt tải DGV tác giả)
            LoadTaiLieuToForm();
        }

        private void btnDanhMucTaiLieu_Click(object sender, EventArgs e)
        {
            // Tạo form mới
            frmDanhMucTaiLieu frm = new frmDanhMucTaiLieu();

            // Đăng ký sự kiện để tải lại ComboBox sau khi form Danh mục cập nhật
            frm.DanhMucUpdated += LoadComboBoxData;

            // Mở form dưới dạng dialog
            frm.ShowDialog();
        }
    }
}
