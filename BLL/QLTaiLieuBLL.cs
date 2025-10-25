using QuanLyThuVien.DAL;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class QLTaiLieuBLL
    {
        private QLTaiLieuDAL dal = new QLTaiLieuDAL();

        /// <summary>
        /// Lấy tất cả thông tin tài liệu đã được join với các bảng liên quan (NXB, Ngôn ngữ, Thể loại, Định dạng).
        /// </summary>
        /// <returns>Danh sách các đối tượng anonymous chứa thông tin chi tiết tài liệu.</returns>
        public List<object> LayTatCaThongTinTaiLieu()
        {
            // Gọi phương thức tương ứng từ DAL
            return dal.LayTatCaThongTinTaiLieu();
        }

        /// <summary>
        /// Lấy danh sách Tác giả và Vai trò của một Tài liệu cụ thể.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu cần truy vấn.</param>
        /// <returns>Danh sách các đối tượng anonymous chứa Mã tác giả, Tên tác giả và Vai trò.</returns>
        public List<object> LayTacGiaTheoMaTaiLieu(string maTL)
        {
            // Kiểm tra đầu vào cơ bản (tùy chọn)
            if (string.IsNullOrEmpty(maTL))
            {
                // Trả về danh sách rỗng nếu Mã TL không hợp lệ
                return new List<object>();
            }

            // Gọi phương thức tương ứng từ DAL
            return dal.LayTacGiaTheoMaTaiLieu(maTL);
        }

        /// <summary>
        /// Lấy danh sách tên Nhà xuất bản để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachNhaXuatBan()
        {
            return dal.LayDanhSachNhaXuatBan();
        }

        /// <summary>
        /// Lấy danh sách tên Ngôn ngữ để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachNgonNgu()
        {
            return dal.LayDanhSachNgonNgu();
        }

        /// <summary>
        /// Lấy danh sách tên Thể loại để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachTheLoai()
        {
            return dal.LayDanhSachTheLoai();
        }

        /// <summary>
        /// Lấy danh sách tên Định dạng để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachDinhDang()
        {
            return dal.LayDanhSachDinhDang();
        }

        /// <summary>
        /// Lấy danh sách Tên đầy đủ của Tác giả để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachTacGia()
        {
            return dal.LayDanhSachTacGia();
        }

        /// <summary>
        /// Lấy danh sách các Vai trò (chức danh) của tác giả để điền vào ComboBox.
        /// </summary>
        public List<string> LayDanhSachVaiTro()
        {
            return dal.LayDanhSachVaiTro();
        }

        public string LayMaNXB(string tenNXB) => dal.LayMaNXB(tenNXB);
        public string LayMaNN(string tenNN) => dal.LayMaNN(tenNN);
        public string LayMaThL(string tenThL) => dal.LayMaThL(tenThL);
        public string LayMaDD(string tenDD) => dal.LayMaDD(tenDD);
        public string LayMaTacGia(string tenTacGia) => dal.LayMaTacGia(tenTacGia);

        /// <summary>
        /// Thực hiện tìm kiếm Tài liệu dựa trên danh sách bộ lọc.
        /// </summary>
        /// <param name="filters">Danh sách các đối tượng Filter chứa thông tin tìm kiếm.</param>
        /// <returns>Danh sách các đối tượng Tài liệu khớp với bộ lọc.</returns>
        public List<object> TimKiemTaiLieu(List<Filter> filters)
        {
            // Kiểm tra bộ lọc rỗng
            if (filters == null || filters.Count == 0)
            {
                // Nếu không có bộ lọc, trả về tất cả tài liệu
                return dal.LayTatCaThongTinTaiLieu();
            }

            // Gọi phương thức tìm kiếm trong DAL
            return dal.TimKiemTaiLieu(filters);
        }

        /// <summary>
        /// Thêm mới một Tài liệu vào hệ thống.
        /// </summary>
        public bool ThemTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGia, out string errorMessage)
        {
            // Kiểm tra logic nghiệp vụ phức tạp nếu cần
            // Ví dụ: NamXuatBan không được lớn hơn năm hiện tại
            if (taiLieu.NamXuatBan > DateTime.Now.Year)
            {
                errorMessage = "Năm xuất bản không thể lớn hơn năm hiện tại.";
                return false;
            }

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.ThemTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
        }

        /// <summary>
        /// Cập nhật thông tin của một Tài liệu đã tồn tại.
        /// </summary>
        public bool SuaTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGia, out string errorMessage)
        {
            // Kiểm tra logic nghiệp vụ phức tạp nếu cần
            if (taiLieu.NamXuatBan > DateTime.Now.Year)
            {
                errorMessage = "Năm xuất bản không thể lớn hơn năm hiện tại.";
                return false;
            }

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.SuaTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
        }

        /// <summary>
        /// Xóa Tài liệu dựa trên Mã Tài liệu.
        /// </summary>
        public bool XoaTaiLieu(string maTL, out string errorMessage)
        {
            // Kiểm tra logic nghiệp vụ: Tài liệu có đang được mượn không?
            // (Giả định QLTaiLieuDAL sẽ xử lý kiểm tra khóa ngoại)

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.XoaTaiLieu(maTL, out errorMessage);
        }


    }
}
