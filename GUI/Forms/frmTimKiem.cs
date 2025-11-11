// File: LibraryManagerApp.GUI.Forms/frmTimKiem.cs

using LibraryManagerApp.DTO;
using LibraryManagerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace LibraryManagerApp.GUI.Forms
{
    // Đảm bảo SearchFilter là public để Delegate hoạt động
    public delegate void SearchAppliedHandler(List<SearchFilter> filters);

    public partial class frmTimKiem : Form
    {
        private List<FieldMetadata> _metadata;
        private List<SearchFilter> _currentFilters = new List<SearchFilter>();
        public event SearchAppliedHandler OnSearchApplied;

        // Quản lý State
        private State _currentState;
        private SearchFilter _selectedFilter = null; // Dùng cho trạng thái UPDATE

        public frmTimKiem()
        {
            InitializeComponent();

            // Khởi tạo
            _metadata = SearchMetadata.GetBanDocFields();
            ConfigureListView();
        }

        #region KHỞI TẠO VÀ CẤU HÌNH GIAO DIỆN

        private void ConfigureListView()
        {
            lsvBoLoc.View = View.Details;
            lsvBoLoc.FullRowSelect = true;
            lsvBoLoc.Columns.Add("Điều kiện Tìm kiếm", 270);
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            LoadTimTheoComboBox();
            SetState(State.READ); // Bắt đầu ở trạng thái READ
        }

        #endregion

        // ---

        #region QUẢN LÝ STATE (CHO BỘ LỌC)

        private void SetState(State state)
        {
            _currentState = state;
            bool isEditing = (state == State.CREATE || state == State.UPDATE);

            // INPUTS
            cboTimTheo.Enabled = isEditing;
            cboToanTu.Enabled = isEditing;
            txtGiaTri.Enabled = isEditing && txtGiaTri.Visible;
            txtTu.Enabled = isEditing && txtTu.Visible;
            txtDen.Enabled = isEditing && txtDen.Visible;

            // THAO TÁC TRỰC TIẾP
            btnThemBoLoc.Enabled = (state == State.READ);
            btnSuaBoLoc.Enabled = (state == State.READ && lsvBoLoc.SelectedItems.Count > 0);
            btnXoaBoLoc.Enabled = (state == State.READ && lsvBoLoc.SelectedItems.Count > 0);
            btnTim.Enabled = (state == State.READ); // Chỉ tìm khi không đang sửa bộ lọc

            // THAO TÁC CRUD STATE
            btnLuu.Enabled = isEditing;
            btnHuy.Enabled = isEditing;

            lsvBoLoc.Enabled = (state == State.READ);

            // Đảm bảo khi chuyển trạng thái, các trường INPUT được reset/cập nhật
            if (state == State.READ)
            {
                ClearFilterInputs();
            }
            else if (state == State.CREATE)
            {
                // Đặt focus vào ComboBox tìm kiếm
                cboTimTheo.Focus();
            }
        }

        // Sự kiện ListView click để bật nút Sửa/Xóa khi ở trạng thái READ
        private void lsvBoLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentState == State.READ)
            {
                bool isSelected = lsvBoLoc.SelectedItems.Count > 0;
                btnSuaBoLoc.Enabled = isSelected;
                btnXoaBoLoc.Enabled = isSelected;
            }
        }

        #endregion

        // ---

        #region QUẢN LÝ INPUTS (COMBOBOX VÀ INPUT FIELD)

        private void LoadTimTheoComboBox()
        {
            cboTimTheo.DataSource = _metadata;
            cboTimTheo.DisplayMember = "DisplayName";
            cboTimTheo.ValueMember = "FieldName";

            if (cboTimTheo.Items.Count > 0)
            {
                cboTimTheo.SelectedIndex = 0;
            }
        }

        private void cboTimTheo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldMetadata selectedField = cboTimTheo.SelectedItem as FieldMetadata;

            if (selectedField != null)
            {
                cboToanTu.DataSource = selectedField.SupportedOperators;
                if (cboToanTu.Items.Count > 0) cboToanTu.SelectedIndex = 0;

                // Quản lý hiển thị Inputs
                bool isDateTime = selectedField.DataType == TypeCode.DateTime;

                txtGiaTri.Visible = !isDateTime;
                label2.Text = !isDateTime ? "Giá trị:" : "";

                label3.Visible = isDateTime;
                txtTu.Visible = isDateTime;

                // Gọi hàm quản lý toán tử để xử lý trường 'Đến'
                cboToanTu_SelectedIndexChanged(null, null);
            }
        }

        private void cboToanTu_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldMetadata selectedField = cboTimTheo.SelectedItem as FieldMetadata;
            string selectedOperator = cboToanTu.SelectedItem?.ToString();

            if (selectedField != null && selectedField.DataType == TypeCode.DateTime)
            {
                bool isRange = selectedOperator == "Khoảng" || selectedOperator == "Đoạn";

                label4.Visible = isRange;
                txtDen.Visible = isRange;

                // Cập nhật nhãn "Từ"
                label3.Text = isRange ? "Từ:" : "Ngày:";
            }
            else
            {
                label4.Visible = false;
                txtDen.Visible = false;
            }
        }

        #endregion

        // ---

        #region XỬ LÝ NÚT CRUD STATE (LƯU - HỦY)

        // Nút Lưu (Thực hiện CREATE hoặc UPDATE Bộ lọc)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            FieldMetadata selectedField = cboTimTheo.SelectedItem as FieldMetadata;
            string selectedOperator = cboToanTu.SelectedItem?.ToString();

            // 1. Validation và Thu thập giá trị
            if (!TryCollectAndValidateValue(selectedField, selectedOperator, out string value, out string valueTo))
            {
                return;
            }

            // 2. Tạo hoặc Cập nhật Filter Object
            SearchFilter filterToSave;

            if (_currentState == State.CREATE)
            {
                filterToSave = new SearchFilter();
                _currentFilters.Add(filterToSave);
            }
            else // UPDATE
            {
                filterToSave = _selectedFilter;
            }

            // 3. Gán giá trị mới
            filterToSave.FieldName = selectedField.FieldName;
            filterToSave.DisplayName = selectedField.DisplayName;
            filterToSave.DataType = selectedField.DataType;
            filterToSave.Operator = selectedOperator;
            filterToSave.Value = value;
            filterToSave.ValueTo = valueTo;

            // 4. Hoàn tất
            UpdateBoLocListView();
            SetState(State.READ);
        }

        // Nút Hủy (Thoát khỏi CREATE/UPDATE)
        private void btnHuy_Click(object sender, EventArgs e)
        {
            SetState(State.READ);
        }

        #endregion

        // ---

        #region XỬ LÝ NÚT HÀNH ĐỘNG (THÊM, SỬA, XÓA, TÌM)

        // Nút này chuyển sang trạng thái CREATE
        private void btnThemBoLoc_Click(object sender, EventArgs e)
        {
            ClearFilterInputs();
            _selectedFilter = null;
            SetState(State.CREATE);
        }

        // Nút này chuyển sang trạng thái UPDATE
        private void btnSuaBoLoc_Click(object sender, EventArgs e)
        {
            if (lsvBoLoc.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một Bộ lọc để sửa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedFilter = lsvBoLoc.SelectedItems[0].Tag as SearchFilter;
            LoadFilterToInputs(_selectedFilter);

            SetState(State.UPDATE);
        }

        // Nút này thực hiện XÓA và chuyển về READ
        private void btnXoaBoLoc_Click(object sender, EventArgs e)
        {
            // Cần kiểm tra lại trước khi hiển thị MessageBox xác nhận
            if (lsvBoLoc.SelectedItems.Count == 0)
            {
                // Giữ lại cảnh báo này vì nó kiểm tra trước khi xác nhận.
                MessageBox.Show("Vui lòng chọn một Bộ lọc để xóa.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa bộ lọc này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SearchFilter filterToDelete = lsvBoLoc.SelectedItems[0].Tag as SearchFilter;
                _currentFilters.Remove(filterToDelete);
                UpdateBoLocListView();

                // Sau khi xóa, nếu không còn bộ lọc nào được chọn (đã xóa item),
                // lsvBoLoc_SelectedIndexChanged sẽ được gọi và tắt nút Sửa/Xóa.
                // Chỉ cần gọi SetState(State.READ) để dọn dẹp inputs và đặt lại trạng thái.
                SetState(State.READ);

                // ***QUAN TRỌNG: Gọi Event Find ngay lập tức sau khi xóa***
                // Nếu List rỗng, Form cha sẽ tự động LoadData() gốc.
                btnTim_Click(sender, e);
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            if (OnSearchApplied != null)
            {
                // Truyền _currentFilters (danh sách các bộ lọc) về Form cha
                OnSearchApplied.Invoke(_currentFilters ?? new List<SearchFilter>());
            }
        }

        #endregion

        // ---

        #region HÀM BỔ TRỢ VÀ VALIDATION

        private bool TryCollectAndValidateValue(FieldMetadata selectedField, string selectedOperator, out string value, out string valueTo)
        {
            value = "";
            valueTo = "";

            if (selectedField.DataType == TypeCode.DateTime)
            {
                value = txtTu.Text.Trim();

                if (string.IsNullOrEmpty(value) && selectedOperator != "Khoảng" && selectedOperator != "Đoạn")
                {
                    MessageBox.Show("Vui lòng nhập Ngày tháng cho tìm kiếm.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Kiểm tra định dạng ngày tháng
                if (!string.IsNullOrEmpty(value) && !DateTime.TryParse(value, out DateTime dtStart))
                {
                    MessageBox.Show("Ngày nhập vào không hợp lệ (DD/MM/YYYY).", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (selectedOperator == "Khoảng" || selectedOperator == "Đoạn")
                {
                    valueTo = txtDen.Text.Trim();

                    if (string.IsNullOrEmpty(valueTo))
                    {
                        MessageBox.Show("Vui lòng nhập Ngày kết thúc.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (!DateTime.TryParse(valueTo, out DateTime dtEnd))
                    {
                        MessageBox.Show("Ngày kết thúc không hợp lệ (DD/MM/YYYY).", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (DateTime.TryParse(value, out dtStart) && dtStart > dtEnd)
                    {
                        MessageBox.Show("Ngày bắt đầu không được lớn hơn Ngày kết thúc.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            else
            {
                value = txtGiaTri.Text.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    MessageBox.Show("Vui lòng nhập Giá trị tìm kiếm.", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void ClearFilterInputs()
        {
            txtGiaTri.Text = string.Empty;
            txtTu.Text = string.Empty;
            txtDen.Text = string.Empty;
            cboTimTheo.SelectedIndex = 0;

            // Xóa lựa chọn trong ListView
            lsvBoLoc.SelectedItems.Clear();

            // Đảm bảo nút Sửa/Xóa bị tắt khi không có gì được chọn
            btnSuaBoLoc.Enabled = false;
            btnXoaBoLoc.Enabled = false;
        }

        // Hàm mới để tải filter object lên Inputs khi sửa
        private void LoadFilterToInputs(SearchFilter filter)
        {
            cboTimTheo.SelectedValue = filter.FieldName;
            cboToanTu.SelectedItem = filter.Operator;

            if (filter.DataType == TypeCode.DateTime)
            {
                txtTu.Text = filter.Value;
                txtDen.Text = filter.ValueTo;
                txtGiaTri.Text = string.Empty;
            }
            else
            {
                txtGiaTri.Text = filter.Value;
                txtTu.Text = string.Empty;
                txtDen.Text = string.Empty;
            }
        }

        private void UpdateBoLocListView()
        {
            lsvBoLoc.Items.Clear();
            foreach (var filter in _currentFilters)
            {
                ListViewItem item = new ListViewItem(filter.ToString());
                item.Tag = filter;
                lsvBoLoc.Items.Add(item);
            }
        }
        #endregion
    }
}