using QuanLyThuVien.BLL;
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
    public partial class ucFrmQLDanhMucTaiLieu : UserControl
    {
        private string tableHienTai, maCol, tenCol;
        private DanhMucTaiLieuBLL DanhMucTaiLieuBLL = new DanhMucTaiLieuBLL();

        public ucFrmQLDanhMucTaiLieu()
        {
            InitializeComponent();
        }

        private void ucFrmQLDanhMucTaiLieu_Load(object sender, EventArgs e)
        {
            // Thêm item cho combobox
            cbDanhMuc.Items.Clear();
            cbDanhMuc.Items.Add("Tác giả");
            cbDanhMuc.Items.Add("Nhà xuất bản");
            cbDanhMuc.Items.Add("Ngôn ngữ");
            cbDanhMuc.Items.Add("Thể loại");
            cbDanhMuc.Items.Add("Định dạng");

            // Mặc định chọn tác giả
            cbDanhMuc.SelectedIndex = 0;

            // set biến
            tableHienTai = "tTacGia";
            maCol = "MaTG";
            tenCol = "ISNULL(HoDem,'') + ' ' + ISNULL(Ten,'')";

            LoadDanhMucHienTai();
        }

        private void cbDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbDanhMuc.SelectedItem.ToString())
            {
                case "Tác giả":
                    tableHienTai = "tTacGia";
                    maCol = "MaTG";
                    tenCol = "HoDem + ' ' + Ten";
                    break;
                case "Nhà xuất bản":
                    tableHienTai = "tNhaXuatBan";
                    maCol = "MaNXB";
                    tenCol = "TenNXB";
                    break;
                case "Ngôn ngữ":
                    tableHienTai = "tNgonNgu";
                    maCol = "MaNN";
                    tenCol = "TenNN";
                    break;
                case "Thể loại":
                    tableHienTai = "tTheLoai";
                    maCol = "MaThL";
                    tenCol = "TenThL";
                    break;
                case "Định dạng":
                    tableHienTai = "tDinhDang";
                    maCol = "MaDD";
                    tenCol = "TenDD";
                    break;
            }

            LoadDanhMucHienTai();
        }

        private void LoadDanhMucHienTai()
        {
            dgvDanhMuc.DataSource = DanhMucTaiLieuBLL.GetALlDanhMuc(tableHienTai, maCol, tenCol);

            txtMa.Clear();
            txtTen.Clear();
        }
    }
}
