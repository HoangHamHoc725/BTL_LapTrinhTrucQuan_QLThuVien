using QuanLyThuVien.BLL;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.Forms
{
    /// <summary>
    /// Form quản lý chi tiết các Bản sao (tBanSao) của một Tài liệu cụ thể.
    /// </summary>
    public partial class frmBanSao : Form
    {
        private readonly QLBanSaoBLL banSaoBLL = new QLBanSaoBLL();
        private readonly string maTaiLieuGoc; // Mã Tài liệu (MaTL) được truyền từ form cha

        // Khai báo trạng thái hiện tại và trạng thái trước đó của form
        private FormState currentState = FormState.View;
        private FormState previousState = FormState.View;
        private frmTimKiem _frmTimKiem;

        /// <summary>
        /// Constructor của form Quản lý Bản sao.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu mà các Bản sao này thuộc về.</param>
        public frmBanSao(string maTL)
        {
            InitializeComponent();
            this.maTaiLieuGoc = maTL;
            // Hiển thị MaTL lên textbox ngay lập tức
            txtMaTaiLieu.Text = maTL;
        }

        // ====================================================
        // I. KHỞI TẠO VÀ TẢI DỮ LIỆU BAN ĐẦU
        // ====================================================

        private void frmBanSao_Load(object sender, EventArgs e)
        {
            this.Text = $"Quản lý Bản sao Tài liệu: {maTaiLieuGoc}";
            try
            {
                // 1. Tải dữ liệu cho ComboBox Tình trạng
                LoadComboBoxData();

                // 2. Tải và setup DataGridView chính (dgvBanSaoTaiLieu)
                dgvBanSaoTaiLieu.DataSource = banSaoBLL.LayTatCaBanSaoTheoMaTL(maTaiLieuGoc);
                SetupBanSaoGridView();

                // 3. Tải thông tin chi tiết Bản sao đầu tiên lên form
                LoadBanSaoToForm();

                // 4. Đặt trạng thái ban đầu và gán sự kiện
                SetState(FormState.View);
                dgvBanSaoTaiLieu.CellClick += dgvBanSaoTaiLieu_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu Bản sao: " + ex.Message, "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải danh sách các giá trị (Tình trạng) cho ComboBox.
        /// </summary>
        private void LoadComboBoxData()
        {
            cbTinhTrang.DataSource = banSaoBLL.LayDanhSachTinhTrang();
            cbTinhTrang.SelectedIndex = -1;
        }

        // ====================================================
        // II. CẤU HÌNH DATAGRIDVIEW
        // ====================================================

        /// <summary>
        /// Cấu hình hiển thị và thuộc tính cho DataGridView Bản sao.
        /// </summary>
        private void SetupBanSaoGridView()
        {
            if (dgvBanSaoTaiLieu.Columns.Count == 0) return;

            // Cấu hình chung
            dgvBanSaoTaiLieu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBanSaoTaiLieu.RowHeadersVisible = false;
            dgvBanSaoTaiLieu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBanSaoTaiLieu.ReadOnly = true;
            dgvBanSaoTaiLieu.AllowUserToResizeColumns = false;
            dgvBanSaoTaiLieu.MultiSelect = false;
            dgvBanSaoTaiLieu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cấu hình tên cột và kích thước
            dgvBanSaoTaiLieu.Columns["MaBS"].HeaderText = "Mã Bản sao"; dgvBanSaoTaiLieu.Columns["MaBS"].Width = 120;
            dgvBanSaoTaiLieu.Columns["MaTL"].Visible = false; // MaTL đã hiển thị ở textbox ngoài
            dgvBanSaoTaiLieu.Columns["TinhTrang"].HeaderText = "Tình trạng"; dgvBanSaoTaiLieu.Columns["TinhTrang"].Width = 200;

            // Ẩn các cột không có HeaderText
            foreach (DataGridViewColumn col in dgvBanSaoTaiLieu.Columns)
            {
                if (string.IsNullOrEmpty(col.HeaderText) && col.Name != "MaTL")
                {
                    col.Visible = false;
                }
            }
        }

        // ====================================================
        // III. XỬ LÝ SỰ KIỆN CELL CLICK & TẢI DỮ LIỆU CHI TIẾT
        // ====================================================

        /// <summary>
        /// Xử lý khi click vào một dòng trên DataGridView Bản sao.
        /// </summary>
        private void dgvBanSaoTaiLieu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            LoadBanSaoToForm();
        }

        /// <summary>
        /// Tải thông tin chi tiết Bản sao từ dòng đang chọn trong DGV lên các controls nhập liệu.
        /// </summary>
        private void LoadBanSaoToForm()
        {
            // 1. Đảm bảo có dòng được chọn. Nếu không, cố gắng chọn dòng đầu tiên.
            if (dgvBanSaoTaiLieu.Rows.Count > 0 && dgvBanSaoTaiLieu.CurrentRow == null)
            {
                dgvBanSaoTaiLieu.Rows[0].Selected = true;
                dgvBanSaoTaiLieu.CurrentCell = dgvBanSaoTaiLieu.Rows[0].Cells[0];
            }

            if (dgvBanSaoTaiLieu.CurrentRow == null || dgvBanSaoTaiLieu.CurrentRow.DataBoundItem == null)
            {
                ClearForm(); // Xóa trắng khi không có dữ liệu
                return;
            }

            // 2. Tải dữ liệu Bản sao lên các controls
            var row = dgvBanSaoTaiLieu.CurrentRow.DataBoundItem;

            // Dùng Reflection để lấy giá trị (vì DataSource là Anonymous Type)
            txtMaBanSao.Text = row?.GetType().GetProperty("MaBS")?.GetValue(row)?.ToString();
            // MaTL không cần load vì đã set ở Constructor (readonly)
            cbTinhTrang.SelectedItem = row?.GetType().GetProperty("TinhTrang")?.GetValue(row)?.ToString();
        }

        /// <summary>
        /// Xóa trắng nội dung tất cả các control nhập liệu trên form (trừ MaTL).
        /// </summary>
        private void ClearForm()
        {
            txtMaBanSao.Clear();
            cbTinhTrang.SelectedIndex = -1;
            // Giữ lại MaTaiLieu (txtMaTaiLieu)
        }

        /// <summary>
        /// Lấy dữ liệu Bản sao từ các controls nhập liệu để chuẩn bị cho thao tác CRUD.
        /// </summary>
        /// <returns>Đối tượng tBanSao.</returns>
        private tBanSao GetBanSaoFromForm()
        {
            return new tBanSao
            {
                MaBS = txtMaBanSao.Text.Trim(),
                MaTL = maTaiLieuGoc, // Luôn lấy MaTL từ biến readonly
                TinhTrang = cbTinhTrang.SelectedItem?.ToString()
            };
        }

        // ====================================================
        // IV. XỬ LÝ TRẠNG THÁI & GIAO DIỆN
        // ====================================================

        /// <summary>
        /// Thiết lập trạng thái hoạt động của form và cập nhật giao diện (Controls/Buttons) tương ứng.
        /// </summary>
        /// <param name="state">Trạng thái mới của form (View, Add, Delete, Search).</param>
        private void SetState(FormState state)
        {
            currentState = state;

            bool isView = (state == FormState.View);
            bool isAdd = (state == FormState.Add);
            bool isDelete = (state == FormState.Delete);
            bool isModifying = isAdd; // Chỉ có thêm mới

            // --- Quản lý các nút ---
            btnThem.Enabled = isView || state == FormState.Search;
            btnXoa.Enabled = isView || state == FormState.Search;
            btnTimKiem.Enabled = isView;

            btnLuu.Enabled = isAdd || isDelete;
            btnHuy.Enabled = isAdd || isDelete || state == FormState.Search;

            // --- Quản lý Input Controls ---
            txtMaTaiLieu.ReadOnly = true; // Luôn chỉ đọc
            txtMaBanSao.ReadOnly = !isAdd;

            // Tình trạng chỉ được xem (View) và tìm kiếm. Không cho phép Thêm/Sửa/Xóa Tình trạng tại đây.
            cbTinhTrang.Enabled = isView || state == FormState.Search;
            cbTinhTrang.DropDownStyle = (isView || state == FormState.Search) ? ComboBoxStyle.DropDownList : ComboBoxStyle.Simple;

            dgvBanSaoTaiLieu.ReadOnly = !isView;

            // --- Màu nền theo trạng thái ---
            switch (state)
            {
                case FormState.View:
                    this.BackColor = Color.White;
                    break;
                case FormState.Add:
                    this.BackColor = Color.LightGreen;
                    ClearForm();
                    txtMaBanSao.Focus();
                    // Đặt lại Tình trạng mặc định cho chế độ Thêm
                    cbTinhTrang.SelectedItem = "Chưa được mượn";
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
        // V. XỬ LÝ THAO TÁC CRUD
        // ====================================================

        private void btnThem_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Add);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvBanSaoTaiLieu.CurrentRow == null) return;
            LoadBanSaoToForm(); // Tải dữ liệu dòng đang chọn vào form
            previousState = currentState;
            SetState(FormState.Delete);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var banSao = GetBanSaoFromForm();
            string errorMessage = string.Empty;
            string maBSVuaThaoTac = banSao.MaBS;
            bool success = false;

            // Xử lý logic theo trạng thái
            switch (currentState)
            {
                case FormState.Add:
                    success = banSaoBLL.ThemBanSao(banSao, out errorMessage);
                    if (success) MessageBox.Show("Thêm Bản sao thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case FormState.Delete:
                    if (MessageBox.Show($"Bạn có chắc chắn muốn xóa Bản sao '{banSao.MaBS}' này không? Thao tác này có thể ảnh hưởng đến các giao dịch liên quan.", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        success = banSaoBLL.XoaBanSao(banSao.MaBS, out errorMessage);
                        if (success) MessageBox.Show("Xóa Bản sao thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else return; // Người dùng hủy xóa
                    break;
            }

            if (!success)
            {
                MessageBox.Show($"{currentState} Bản sao thất bại: {errorMessage}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật DGV và Trạng thái sau khi thao tác thành công
            UpdateGridViewAndState(maBSVuaThaoTac);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            if (currentState == FormState.Search)
            {
                // Thoát khỏi Search → trở về View, tải lại toàn bộ dữ liệu
                dgvBanSaoTaiLieu.DataSource = banSaoBLL.LayTatCaBanSaoTheoMaTL(maTaiLieuGoc);
                SetupBanSaoGridView();
                SetState(FormState.View);
            }
            else
            {
                // Hủy thao tác Add/Delete, quay lại trạng thái trước đó
                SetState(previousState);
            }

            // Tải lại thông tin chi tiết của dòng đang chọn (để khôi phục dữ liệu)
            LoadBanSaoToForm();
            previousState = FormState.View; // Reset trạng thái trước đó
        }

        // ====================================================
        // VI. XỬ LÝ TÌM KIẾM & HỖ TRỢ
        // ====================================================

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Search);

            // Lấy thuộc tính lọc cho module Bản sao (MaBS, TinhTrang)
            var ds = FilterHelper.GetFilterAttributes("BanSao");

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

                    // Gọi BLL để tìm kiếm, truyền kèm MaTL
                    var result = banSaoBLL.TimKiemBanSao(maTaiLieuGoc, filters);
                    dgvBanSaoTaiLieu.DataSource = result;
                    SetupBanSaoGridView();

                    // Tải thông tin của bản sao đầu tiên trong kết quả tìm kiếm
                    LoadBanSaoToForm();
                };
            }

            _frmTimKiem.Show();
            _frmTimKiem.BringToFront();
        }

        /// <summary>
        /// Tải lại DGV Bản sao, tái chọn dòng vừa thao tác và đặt lại trạng thái.
        /// </summary>
        /// <param name="maBSVuaThaoTac">Mã Bản sao vừa được Thêm/Xóa.</param>
        private void UpdateGridViewAndState(string maBSVuaThaoTac)
        {
            // 1. Tải lại toàn bộ danh sách Bản sao
            dgvBanSaoTaiLieu.DataSource = banSaoBLL.LayTatCaBanSaoTheoMaTL(maTaiLieuGoc);
            SetupBanSaoGridView();

            // 2. Tái chọn dòng (Tìm và chọn lại bản sao vừa thao tác)
            bool found = false;
            if (!string.IsNullOrEmpty(maBSVuaThaoTac))
            {
                foreach (DataGridViewRow row in dgvBanSaoTaiLieu.Rows)
                {
                    var rowData = row.DataBoundItem;
                    string maBSTrongDGV = rowData?.GetType().GetProperty("MaBS")?.GetValue(rowData)?.ToString();

                    if (maBSTrongDGV == maBSVuaThaoTac)
                    {
                        dgvBanSaoTaiLieu.ClearSelection();
                        row.Selected = true;
                        dgvBanSaoTaiLieu.CurrentCell = row.Cells[0];
                        found = true;
                        break;
                    }
                }
            }

            // Nếu không tìm thấy (hoặc vừa xóa) và còn dữ liệu, chọn dòng đầu tiên
            if (!found && dgvBanSaoTaiLieu.Rows.Count > 0)
            {
                dgvBanSaoTaiLieu.ClearSelection();
                dgvBanSaoTaiLieu.Rows[0].Selected = true;
                dgvBanSaoTaiLieu.CurrentCell = dgvBanSaoTaiLieu.Rows[0].Cells[0];
            }

            // 3. Đặt trạng thái về View/Search và tải lại form chi tiết
            SetState(previousState == FormState.Search ? FormState.Search : FormState.View);
            LoadBanSaoToForm();
        }
    }
}
