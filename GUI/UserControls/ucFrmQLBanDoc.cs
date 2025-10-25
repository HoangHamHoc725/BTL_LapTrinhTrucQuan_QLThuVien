using QuanLyThuVien.BLL;
using QuanLyThuVien.DAL;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace form1.GUI
{
    public partial class ucFrmQLBanDoc : UserControl
    {
        private BanDocBLL BanDocBLL = new BanDocBLL();

        // Khai báo trạng thái của form
        private FormState currentState = FormState.View;
        private FormState previousState = FormState.View;

        // Khởi tạo UserControl
        public ucFrmQLBanDoc()
        {
            InitializeComponent();
        }

        // Xử lý sự kiện khi UserControl được tải (Load)
        private void ucFrmQLBanDoc_Load(object sender, EventArgs e)
        {
            dgvBanDoc.DataSource = BanDocBLL.LayTatCaThongTinBanDoc();
            SetupBanDocGridView();
            SetState(FormState.View);
        }

        // Lấy dữ liệu bạn đọc và thẻ bạn đọc từ các control nhập liệu trên form
        private (tBanDoc, tTheBanDoc) GetBanDocFromForm()
        {
            // Lấy giá trị từ ComboBox, lấy ký tự đầu tiên
            char gioiTinh = 'M'; // mặc định
            if (cbGioiTinh.SelectedItem != null)
            {
                var str = cbGioiTinh.SelectedItem.ToString().ToUpper();
                if (str == "M" || str == "F")
                    gioiTinh = str[0]; // lấy ký tự đầu tiên
            }

            var banDoc = new tBanDoc
            {
                MaBD = txtMaBanDoc.Text.Trim(),
                HoDem = txtHoDem.Text.Trim(),
                Ten = txtTen.Text.Trim(),
                GioiTinh = gioiTinh,    // chỉ 'M' hoặc 'F'
                NgaySinh = dtpNgaySinh.Value.Date,
                DiaChi = txtDiaChi.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            var theBanDoc = new tTheBanDoc
            {
                MaTBD = txtMaTheBanDoc.Text.Trim(),
                MaBD = banDoc.MaBD,
                MaTK = string.IsNullOrWhiteSpace(txtMaTKNhanVien.Text) ? "TK11" : txtMaTKNhanVien.Text.Trim(),
                NgayCap = dtpNgayLamThe.Value.Date,
                NgayHetHan = string.IsNullOrWhiteSpace(dtpNgayHetHan.Text) ? (DateTime?)null : dtpNgayHetHan.Value.Date,
                TrangThai = cbTrangThai.SelectedItem?.ToString()
            };

            return (banDoc, theBanDoc);
        }

        // Thiết lập trạng thái hoạt động của form (View, Add, Edit, Delete, Search)
        private void SetState(FormState state)
        {
            // Logic điều chỉnh enabled/readonly của control và nút bấm
            currentState = state;

            bool isView = (state == FormState.View);
            bool isAdd = (state == FormState.Add);
            bool isEdit = (state == FormState.Edit);
            bool isDelete = (state == FormState.Delete);
            bool isSearch = (state == FormState.Search);

            // --- Quản lý các nút ---
            btnThem.Enabled = isView;
            btnSua.Enabled = isView || isSearch;
            btnXoa.Enabled = isView || isSearch;
            btnTimKiem.Enabled = isView || isSearch;

            btnLuu.Enabled = isAdd || isEdit || isDelete;
            btnHuy.Enabled = isAdd || isEdit || isDelete || isSearch;

            // --- Quản lý DGV ---
            if (isAdd)
            {
                // Add state: không cho chọn dòng nhưng vẫn scroll được
                dgvBanDoc.ClearSelection();
                dgvBanDoc.CurrentCell = null; // bỏ focus
                dgvBanDoc.EnableHeadersVisualStyles = false;
                dgvBanDoc.ReadOnly = true;     // không thể edit
                dgvBanDoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            else
            {
                dgvBanDoc.ReadOnly = !(isView || isSearch); // view/search: đọc, edit/delete tùy readonly
                dgvBanDoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            // --- Quản lý input ---
            txtMaBanDoc.ReadOnly = !isAdd;               // Chỉ Add mới nhập mã
            txtHoDem.ReadOnly = !(isAdd || isEdit);
            txtTen.ReadOnly = !(isAdd || isEdit);
            cbGioiTinh.Enabled = isAdd || isEdit;
            dtpNgaySinh.Enabled = isAdd || isEdit;
            txtDiaChi.ReadOnly = !(isAdd || isEdit);
            txtSDT.ReadOnly = !(isAdd || isEdit);
            txtEmail.ReadOnly = !(isAdd || isEdit);

            txtMaTheBanDoc.ReadOnly = !isAdd;            // Mã thẻ chỉ Add
            dtpNgayLamThe.Enabled = isAdd || isEdit;
            dtpNgayHetHan.Enabled = isAdd || isEdit;
            cbTrangThai.Enabled = isAdd || isEdit;
            txtMaTKNhanVien.Enabled = isAdd || isEdit;

            if (isAdd)
            {
                ClearForm();
            }

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

        private void ClearForm()
        {
            txtMaBanDoc.Clear();
            txtHoDem.Clear();
            txtTen.Clear();
            cbGioiTinh.SelectedIndex = -1;
            dtpNgaySinh.Value = DateTime.Now;
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtMaTheBanDoc.Clear();
            dtpNgayLamThe.Value = DateTime.Now;
            dtpNgayHetHan.Value = DateTime.Now;
            cbTrangThai.SelectedIndex = -1;
            txtMaTKNhanVien.Clear();
        }

        private frmTimKiem _frmTimKiem;

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            SetState(FormState.Search);

            var ds = FilterHelper.GetFilterAttributes("BanDoc");

            // Kiểm tra nếu form tìm kiếm chưa tồn tại hoặc đã bị dispose
            if (_frmTimKiem == null || _frmTimKiem.IsDisposed)
            {
                _frmTimKiem = new frmTimKiem(ds);

                // Đăng ký sự kiện khi người dùng nhấn "Tìm kiếm"
                _frmTimKiem.OnSearch += (s, filters) =>
                {
                    if (filters == null || filters.Count == 0)
                    {
                        MessageBox.Show("Không có bộ lọc nào được chọn!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var result = BanDocBLL.TimKiemBanDoc(filters);
                    dgvBanDoc.DataSource = result;
                    SetupBanDocGridView();
                };
            }

            _frmTimKiem.Show();          // Mở modeless
            _frmTimKiem.BringToFront();  // Đưa lên trước
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            previousState = currentState;
            SetState(FormState.Add);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvBanDoc.CurrentRow == null) return;
            previousState = currentState;
            SetState(FormState.Edit);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvBanDoc.CurrentRow == null) return;
            previousState = currentState;
            SetState(FormState.Delete);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            var (banDoc, theBanDoc) = GetBanDocFromForm();
            string errorMessage;

            switch (currentState)
            {
                case FormState.Add:
                    if (BanDocBLL.ThemBanDoc(banDoc, theBanDoc, out errorMessage))
                    {
                        MessageBox.Show("Thêm bạn đọc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Thêm bạn đọc thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // dừng xử lý nếu thất bại
                    }
                    break;
                case FormState.Edit:
                    if (BanDocBLL.SuaBanDoc(banDoc, theBanDoc, out errorMessage))
                    {
                        MessageBox.Show("Cập nhật bạn đọc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật bạn đọc thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // dừng xử lý nếu thất bại
                    }
                    break;
                case FormState.Delete:
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa bạn đọc này không?",
                                        "Xác nhận xóa",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        if (BanDocBLL.XoaBanDoc(banDoc.MaBD, out errorMessage))
                        {
                            MessageBox.Show("Xóa bạn đọc thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xóa bạn đọc thất bại: " + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        return; // người dùng chọn "Không"
                    }
                    break;
            }

            // Nếu trước đó là Search → load lại kết quả tìm kiếm
            if (previousState == FormState.Search)
            {
                // (tùy chọn: bạn có thể lưu lại từ khóa + cột tìm để gọi lại hàm TimKiemBanDoc)
                SetState(FormState.Search);
            }
            else
            {
                dgvBanDoc.DataSource = BanDocBLL.LayTatCaThongTinBanDoc();
                SetState(FormState.View);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Nếu đang ở Search và không phải đang hủy trong thao tác con (Add/Edit/Delete)
            if (currentState == FormState.Search)
            {
                // Thoát khỏi Search → trở về View
                dgvBanDoc.DataSource = BanDocBLL.LayTatCaThongTinBanDoc();
                SetState(FormState.View);
                previousState = FormState.View; // reset
                return;
            }

            // Nếu đang trong Add/Edit/Delete và trước đó là Search
            if (previousState == FormState.Search)
            {
                SetState(FormState.Search);
                previousState = FormState.View; // reset để lần sau Hủy thoát được
            }
            else
            {
                dgvBanDoc.DataSource = BanDocBLL.LayTatCaThongTinBanDoc();
                SetState(FormState.View);
                previousState = FormState.View;
            }
        }

        private void SetupBanDocGridView()
        {
            if (dgvBanDoc.Columns.Count == 0)
                return;

            dgvBanDoc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvBanDoc.RowHeadersVisible = false;
            dgvBanDoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBanDoc.ReadOnly = true;
            dgvBanDoc.AllowUserToResizeColumns = false;

            dgvBanDoc.Columns["MaBD"].HeaderText = "Mã bạn đọc";
            dgvBanDoc.Columns["MaBD"].Width = 150;

            dgvBanDoc.Columns["HoDem"].HeaderText = "Họ đệm";
            dgvBanDoc.Columns["HoDem"].Width = 150;

            dgvBanDoc.Columns["Ten"].HeaderText = "Tên";
            dgvBanDoc.Columns["Ten"].Width = 50;

            dgvBanDoc.Columns["GioiTinhHienThi"].HeaderText = "Giới tính";
            dgvBanDoc.Columns["GioiTinhHienThi"].Width = 80;

            dgvBanDoc.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            dgvBanDoc.Columns["NgaySinh"].Width = 120;

            dgvBanDoc.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvBanDoc.Columns["DiaChi"].Width = 450;

            dgvBanDoc.Columns["SDT"].HeaderText = "SĐT";
            dgvBanDoc.Columns["SDT"].Width = 100;

            dgvBanDoc.Columns["Email"].HeaderText = "Email";
            dgvBanDoc.Columns["Email"].Width = 200;

            dgvBanDoc.Columns["MaTBD"].HeaderText = "Mã thẻ bạn đọc";
            dgvBanDoc.Columns["MaTBD"].Width = 100;

            dgvBanDoc.Columns["NgayCap"].HeaderText = "Ngày cấp thẻ";
            dgvBanDoc.Columns["NgayCap"].Width = 100;

            dgvBanDoc.Columns["NgayHetHan"].HeaderText = "Ngày hết hạn thẻ";
            dgvBanDoc.Columns["NgayHetHan"].Width = 100;

            dgvBanDoc.Columns["TrangThai"].HeaderText = "Trạng thái thẻ";
            dgvBanDoc.Columns["TrangThai"].Width = 100;

            dgvBanDoc.Columns["MaTK"].HeaderText = "Mã tài khoản cấp thẻ";
            dgvBanDoc.Columns["MaTK"].Width = 150;

            dgvBanDoc.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dgvBanDoc_SelectionChanged(object sender, EventArgs e)
        {
            if (currentState == FormState.Add) return; // Add state không load dữ liệu lên input

            if (dgvBanDoc.CurrentRow == null || dgvBanDoc.CurrentRow.DataBoundItem == null)
                return;

            // Lấy object đang bind
            var row = dgvBanDoc.CurrentRow.DataBoundItem;
            var prop = row.GetType().GetProperty("MaBD"); // kiểm tra tồn tại
            if (prop == null) return;

            // Gán dữ liệu từ row lên các input
            txtMaBanDoc.Text = row.GetType().GetProperty("MaBD").GetValue(row)?.ToString();
            txtHoDem.Text = row.GetType().GetProperty("HoDem").GetValue(row)?.ToString();
            txtTen.Text = row.GetType().GetProperty("Ten").GetValue(row)?.ToString();

            string gioiTinh = row.GetType().GetProperty("GioiTinhHienThi").GetValue(row)?.ToString();

            if (!string.IsNullOrEmpty(gioiTinh))
            {
                if (gioiTinh == "Nam")
                    cbGioiTinh.SelectedItem = "M";
                else if (gioiTinh == "Nữ")
                    cbGioiTinh.SelectedItem = "F";
                else
                    cbGioiTinh.SelectedIndex = -1;
            }
            else
            {
                cbGioiTinh.SelectedIndex = -1;
            }

            dtpNgaySinh.Value = row.GetType().GetProperty("NgaySinh").GetValue(row) != null
                ? (DateTime)row.GetType().GetProperty("NgaySinh").GetValue(row)
                : DateTime.Now;

            txtDiaChi.Text = row.GetType().GetProperty("DiaChi").GetValue(row)?.ToString();
            txtSDT.Text = row.GetType().GetProperty("SDT").GetValue(row)?.ToString();
            txtEmail.Text = row.GetType().GetProperty("Email").GetValue(row)?.ToString();

            txtMaTheBanDoc.Text = row.GetType().GetProperty("MaTBD").GetValue(row)?.ToString();
            dtpNgayLamThe.Value = row.GetType().GetProperty("NgayCap").GetValue(row) != null
                ? (DateTime)row.GetType().GetProperty("NgayCap").GetValue(row)
                : DateTime.Now;

            dtpNgayHetHan.Value = row.GetType().GetProperty("NgayHetHan").GetValue(row) != null
                ? (DateTime)row.GetType().GetProperty("NgayHetHan").GetValue(row)
                : DateTime.Now;

            cbTrangThai.SelectedItem = row.GetType().GetProperty("TrangThai").GetValue(row)?.ToString();
            txtMaTKNhanVien.Text = row.GetType().GetProperty("MaTK").GetValue(row)?.ToString();
        }
    }
}
