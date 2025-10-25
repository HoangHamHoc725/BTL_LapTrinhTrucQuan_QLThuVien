using QuanLyThuVien.BLL;
using QuanLyThuVien.DTO;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace QuanLyThuVien.GUI.Forms
{
    public partial class frmDanhMucTaiLieu : Form
    {
        public event Action DanhMucUpdated;

        private DanhMucTaiLieuBLL DanhMucTaiLieuBLL = new DanhMucTaiLieuBLL();

        private FormState currentState = FormState.View;
        private FormState previousState = FormState.View;
        private string tableHienTai = "tTacGia";

        // Giả định các controls: txtMa, txtTen, dgvDanhMuc, cbDanhMuc, 
        // btnThem, btnSua, btnXoa, btnLuu, btnHuy.

        public frmDanhMucTaiLieu()
        {
            InitializeComponent();
        }

        private void frmDanhMucTaiLieu_Load(object sender, EventArgs e)
        {
            cbDanhMuc.Items.Clear();
            cbDanhMuc.Items.AddRange(new object[] { "Tác giả", "Nhà xuất bản", "Ngôn ngữ", "Thể loại", "Định dạng" });

            cbDanhMuc.SelectedIndex = 0;
            tableHienTai = "tTacGia";

            LoadDanhMucHienTai();
            SetState(FormState.View);
        }

        // ----------------------------------------------------
        // LOGIC TẢI DỮ LIỆU VÀ QUẢN LÝ TRẠNG THÁI
        // ----------------------------------------------------

        // PHƯƠNG THỨC HỖ TRỢ: Xóa trắng input
        private void ClearInputControls()
        {
            txtMa.Clear();
            txtTen.Clear();
        }

        private void LoadDanhMucHienTai()
        {
            try
            {
                // Nếu đang ở trạng thái Search, sử dụng bộ lọc
                if (currentState == FormState.Search && currentFilters.Any())
                {
                    dgvDanhMuc.DataSource = DanhMucTaiLieuBLL.TimKiemDanhMuc(tableHienTai, currentFilters);
                }
                else
                {
                    // Tải toàn bộ danh mục
                    dgvDanhMuc.DataSource = DanhMucTaiLieuBLL.GetALlDanhMuc(tableHienTai);
                    currentFilters.Clear(); // Xóa bộ lọc nếu không phải Search
                }

                SetupDanhMucGridView();
                ClearInputControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh mục: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDanhMucGridView()
        {
            if (dgvDanhMuc.Columns.Count == 0) return;

            bool isTacGia = (tableHienTai == "tTacGia");

            dgvDanhMuc.Columns["Ma"].HeaderText = "Mã";
            dgvDanhMuc.Columns["Ma"].Width = 100;
            dgvDanhMuc.Columns["Ma"].DisplayIndex = 0;

            if (dgvDanhMuc.Columns.Contains("HoDem")) dgvDanhMuc.Columns["HoDem"].Visible = false;
            if (dgvDanhMuc.Columns.Contains("Ten")) dgvDanhMuc.Columns["Ten"].Visible = !isTacGia;
            if (dgvDanhMuc.Columns.Contains("TenDayDu")) dgvDanhMuc.Columns["TenDayDu"].Visible = isTacGia;

            if (isTacGia)
            {
                dgvDanhMuc.Columns["TenDayDu"].HeaderText = "Họ và Tên";
                dgvDanhMuc.Columns["TenDayDu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvDanhMuc.Columns["TenDayDu"].DisplayIndex = 1;
            }
            else
            {
                dgvDanhMuc.Columns["Ten"].HeaderText = "Tên Danh mục";
                dgvDanhMuc.Columns["Ten"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvDanhMuc.Columns["Ten"].DisplayIndex = 1;
            }

            dgvDanhMuc.Columns["Ma"].ReadOnly = true;
            dgvDanhMuc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDanhMuc.ReadOnly = true;
        }

        private void SetState(FormState state)
        {
            currentState = state;
            bool isView = (state == FormState.View);
            bool isAdd = (state == FormState.Add);
            bool isEdit = (state == FormState.Edit);
            bool isSearch = (state == FormState.Search);

            bool isModifying = (isAdd || isEdit || state == FormState.Delete);

            // --- Quản lý các nút chính ---

            // Nút Thêm: Chỉ bật ở VIEW
            btnThem.Enabled = isView;

            // Nút Sửa/Xóa/Tìm kiếm: Bật ở VIEW hoặc SEARCH
            btnSua.Enabled = isView || isSearch;
            btnXoa.Enabled = isView || isSearch;
            btnTimKiem.Enabled = isView || isSearch;

            // Nút Lưu: Chỉ bật khi đang thực hiện thao tác (Add, Edit)
            btnLuu.Enabled = (isAdd || isEdit);

            // Nút Hủy: Bật khi KHÔNG PHẢI VIEW (Bao gồm Add, Edit, Delete, Search)
            btnHuy.Enabled = !isView;

            // --- Controls và DGV ---
            txtMa.ReadOnly = !isAdd;
            txtTen.ReadOnly = !isModifying;

            dgvDanhMuc.Enabled = !isAdd;
            dgvDanhMuc.ReadOnly = true;

            // Màu nền
            this.BackColor = (isAdd || isEdit) ? Color.LightYellow : Color.White;
            if (isSearch) this.BackColor = Color.LightCyan;

            if (isView || isSearch)
            {
                LoadDanhMucHienTai();
            }

            if (isAdd)
            {
                ClearInputControls();
            }
        }

        // ----------------------------------------------------
        // XỬ LÝ SỰ KIỆN DGV & CHUYỂN DANH MỤC
        // ----------------------------------------------------

        private void cbDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isModifying = (currentState == FormState.Add || currentState == FormState.Edit || currentState == FormState.Delete);

            // 1. Cập nhật tableHienTai
            switch (cbDanhMuc.SelectedItem.ToString())
            {
                case "Tác giả": tableHienTai = "tTacGia"; break;
                case "Nhà xuất bản": tableHienTai = "tNhaXuatBan"; break;
                case "Ngôn ngữ": tableHienTai = "tNgonNgu"; break;
                case "Thể loại": tableHienTai = "tTheLoai"; break;
                case "Định dạng": tableHienTai = "tDinhDang"; break;
            }

            // 2. Tải dữ liệu danh mục mới
            LoadDanhMucHienTai();

            // 3. Quản lý trạng thái:
            if (!isModifying)
            {
                // Chỉ chuyển về View (reset) khi trạng thái ban đầu là View hoặc Search
                SetState(FormState.View);
            }
            else if (currentState == FormState.Add)
            {
                // Giữ trạng thái Add, nhưng xóa input
                txtMa.Clear();
                txtTen.Clear();
            }
            // Nếu đang Edit/Delete, trạng thái và dữ liệu input sẽ được giữ, cần click chọn dòng mới.
        }

        private void dgvDanhMuc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || (currentState == FormState.Add)) return; // Không load khi Add

            var row = dgvDanhMuc.CurrentRow.DataBoundItem as DanhMucTaiLieuDTO;
            if (row == null) return;

            txtMa.Text = row.Ma;

            if (tableHienTai == "tTacGia")
            {
                txtTen.Text = row.TenDayDu;
            }
            else
            {
                txtTen.Text = row.Ten;
            }
        }

        // ----------------------------------------------------
        // XỬ LÝ THAO TÁC CRUD
        // ----------------------------------------------------

        private DanhMucTaiLieuDTO GetDtoFromForm()
        {
            var dto = new DanhMucTaiLieuDTO
            {
                Ma = txtMa.Text.Trim(),
                Ten = txtTen.Text.Trim()
            };

            if (tableHienTai == "tTacGia")
            {
                string hoTenDayDu = txtTen.Text.Trim();
                string[] parts = hoTenDayDu.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 1)
                {
                    dto.Ten = parts.Last();
                    dto.HoDem = string.Join(" ", parts.Take(parts.Length - 1));
                }
                else
                {
                    dto.Ten = hoTenDayDu;
                    dto.HoDem = string.Empty;
                }
            }
            return dto;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            previousState = currentState; // Lưu trạng thái hiện tại (View/Search)
            SetState(FormState.Add);
            txtMa.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.CurrentRow == null) return;
            previousState = currentState; // Lưu trạng thái hiện tại (View/Search)
            SetState(FormState.Edit);
            txtMa.ReadOnly = true;
            txtTen.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhMuc.CurrentRow == null) return;

            // Lưu trạng thái hiện tại (View hoặc Search)
            FormState stateBeforeDelete = currentState;

            var dto = dgvDanhMuc.CurrentRow.DataBoundItem as DanhMucTaiLieuDTO;

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa danh mục '{dto.TenDayDu}' có Mã là {dto.Ma}?",
                                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string errorMessage;
                if (DanhMucTaiLieuBLL.XoaDanhMuc(tableHienTai, dto.Ma, out errorMessage))
                {
                    MessageBox.Show("Xóa danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Quay lại trạng thái trước đó (View hoặc Search) sau khi thành công
                    SetState(stateBeforeDelete);
                    DanhMucUpdated?.Invoke();
                }
                else
                {
                    MessageBox.Show("Xóa danh mục thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Giữ nguyên trạng thái để người dùng có thể thử lại/chọn lại
                    // Tải lại DGV để chắc chắn (nếu không SetState, phải gọi thủ công)
                    LoadDanhMucHienTai();
                }
            }
            // Nếu người dùng chọn KHÔNG, ta không làm gì cả, giữ nguyên View/Search
        }

        // ----------------------------------------------------
        // BỔ SUNG: XỬ LÝ SỰ KIỆN TÌM KIẾM
        // ----------------------------------------------------
        private frmTimKiem _frmTimKiem; // Đảm bảo khai báo biến này ở đầu class
        private List<Filter> currentFilters = new List<Filter>(); // Lưu trữ bộ lọc hiện tại

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Bắt đầu quá trình tìm kiếm chỉ khi ở View
            if (currentState == FormState.View || currentState == FormState.Search)
            {
                previousState = currentState; // LƯU TRẠNG THÁI HIỆN TẠI (View HOẶC Search)
                SetState(FormState.Search); // Chuyển sang Search (tạm thời vô hiệu hóa các nút)

                // Lấy thuộc tính lọc cho module Danh mục
                var ds = FilterHelper.GetFilterAttributes("DanhMuc");

                if (_frmTimKiem == null || _frmTimKiem.IsDisposed)
                {
                    _frmTimKiem = new frmTimKiem(ds);

                    _frmTimKiem.OnSearch += (s, filters) =>
                    {
                        currentFilters = filters; // Lưu bộ lọc

                        if (currentFilters == null || !currentFilters.Any())
                        {
                            MessageBox.Show("Không có bộ lọc nào được chọn!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Quay lại View nếu không có bộ lọc
                            LoadDanhMucHienTai();
                            SetState(FormState.View);
                            return;
                        }

                        // Thực hiện tìm kiếm
                        var result = DanhMucTaiLieuBLL.TimKiemDanhMuc(tableHienTai, currentFilters);
                        dgvDanhMuc.DataSource = result;
                        SetupDanhMucGridView();

                        // Sau khi tìm kiếm, xóa input
                        ClearInputControls();
                    };
                }

                _frmTimKiem.Show();
                _frmTimKiem.BringToFront();
            }
            // Nếu đang ở trạng thái Search, có thể cho phép mở lại form tìm kiếm
            else if (currentState == FormState.Search)
            {
                _frmTimKiem.Show();
                _frmTimKiem.BringToFront();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var dto = GetDtoFromForm();
            string errorMessage;

            bool success = false;
            // Ghi nhận trạng thái đang thực hiện
            FormState actionState = currentState;

            // 1. Thực hiện Thêm/Sửa
            if (actionState == FormState.Add)
            {
                success = DanhMucTaiLieuBLL.ThemDanhMuc(tableHienTai, dto, out errorMessage);
                if (success)
                    MessageBox.Show("Thêm danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Thêm danh mục thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (actionState == FormState.Edit)
            {
                success = DanhMucTaiLieuBLL.SuaDanhMuc(tableHienTai, dto, out errorMessage);
                if (success)
                    MessageBox.Show("Sửa danh mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Sửa danh mục thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 2. Xử lý sau khi thành công
            if (success)
            {
                // Thông báo Form cha cập nhật ComboBox
                DanhMucUpdated?.Invoke();

                // Quay lại trạng thái trước đó (View hoặc Search)
                if (previousState == FormState.Search)
                {
                    SetState(FormState.Search); // Giữ trạng thái Search và hiển thị kết quả lọc mới
                }
                else
                {
                    SetState(FormState.View); // Quay lại View
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Thoát khỏi Search
            if (currentState == FormState.Search)
            {
                // Thoát khỏi Search -> trở về View
                currentFilters.Clear();
                SetState(FormState.View);
            }
            // Hủy thao tác CRUD -> quay lại trạng thái trước đó (View/Search)
            else if (currentState == FormState.Add || currentState == FormState.Edit || currentState == FormState.Delete)
            {
                // Khi hủy thao tác con, ta quay lại View hoặc Search
                if (previousState == FormState.Search)
                {
                    // Quay lại trạng thái Search, tải lại kết quả lọc
                    SetState(FormState.Search);
                }
                else
                {
                    // Quay lại trạng thái View
                    SetState(FormState.View);
                }
            }
        }
    }
}