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

namespace QuanLyThuVien.GUI.Forms
{
    public partial class frmTimKiem : Form
    {
        private Dictionary<string, string> _dsThuocTinh;

        private List<Filter> _filters = new List<Filter>();

        public List<Filter> KetQuaBoLoc => _filters;

        public event EventHandler<List<Filter>> OnSearch;

        public frmTimKiem(Dictionary<string, string> ds)
        {
            InitializeComponent();
            _dsThuocTinh = ds;
        }

        private void frmTimKiem_Load(object sender, EventArgs e)
        {
            // Load combobox thuộc tính
            if (_dsThuocTinh != null && _dsThuocTinh.Count > 0)
            {
                cbTimKiem.DataSource = new BindingSource(_dsThuocTinh, null);
                cbTimKiem.DisplayMember = "Key";
                cbTimKiem.ValueMember = "Value";
            }

            // Load combobox toán tử
            cbToanTu.Items.AddRange(new string[] { "=", "Chứa", ">", "<", ">=", "<=" });
            cbToanTu.SelectedIndex = 1;

            // Cấu hình ListView
            lvBoLoc.View = View.Details;
            lvBoLoc.FullRowSelect = true;
            lvBoLoc.GridLines = true;
            lvBoLoc.Columns.Add("Thuộc tính", 150);
            lvBoLoc.Columns.Add("Toán tử", 80);
            lvBoLoc.Columns.Add("Giá trị", 180);
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            if (_filters.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một điều kiện lọc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Phát sự kiện gửi danh sách bộ lọc ra ngoài
            OnSearch?.Invoke(this, _filters);
        }

        private void btnThemBoLoc_Click(object sender, EventArgs e)
        {
            if (cbTimKiem.SelectedItem == null || cbToanTu.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn thuộc tính và toán tử!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTuKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập giá trị lọc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = (KeyValuePair<string, string>)cbTimKiem.SelectedItem;
            string displayName = selectedItem.Key;
            string columnName = selectedItem.Value;
            string op = cbToanTu.SelectedItem.ToString();
            string value = txtTuKhoa.Text.Trim();

            // Kiểm tra trùng lặp thuộc tính
            if (_filters.Any(f => f.ColumnName == columnName))
            {
                MessageBox.Show($"Thuộc tính '{displayName}' đã tồn tại trong danh sách bộ lọc!\nVui lòng xóa trước khi thêm lại.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Thêm bộ lọc mới
            var filter = new Filter(displayName, columnName, value)
            {
                Operator = op
            };
            _filters.Add(filter);

            // Hiển thị trong ListView
            var item = new ListViewItem(new[] { displayName, op, value });
            lvBoLoc.Items.Add(item);

            // Reset ô nhập
            txtTuKhoa.Clear();
            cbTimKiem.SelectedIndex = 0;
            cbToanTu.SelectedIndex = 1;
        }

        private void btnXoaBoLoc_Click(object sender, EventArgs e)
        {
            if (lvBoLoc.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một điều kiện để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = lvBoLoc.SelectedItems[0];
            string displayName = selected.SubItems[0].Text;
            string columnName = _dsThuocTinh[displayName];

            // Xoá trong danh sách logic
            _filters.RemoveAll(f => f.ColumnName == columnName);

            // Xoá trong giao diện
            lvBoLoc.Items.Remove(selected);
        }
    }
}
