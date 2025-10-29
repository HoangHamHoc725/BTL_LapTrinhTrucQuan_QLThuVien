using QuanLyThuVien.DAL;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyThuVien.DAL; // Đảm bảo tTaiLieu được tham chiếu từ DAL

namespace QuanLyThuVien.BLL
{
    /// <summary>
    /// Lớp Logic Nghiệp vụ (BLL) xử lý các thao tác liên quan đến Tài liệu.
    /// Lớp này chịu trách nhiệm kiểm tra logic nghiệp vụ và gọi DAL.
    /// </summary>
    internal class QLTaiLieuBLL
    {
        private readonly QLTaiLieuDAL dal = new QLTaiLieuDAL();

        // ====================================================
        // I. PHƯƠNG THỨC TRUY VẤN & TÌM KIẾM (GET/READ/SEARCH)
        // ====================================================

        /// <summary>
        /// Lấy tất cả thông tin tài liệu đã được join với các bảng liên quan (NXB, Ngôn ngữ, Thể loại, Định dạng).
        /// </summary>
        /// <returns>Danh sách các đối tượng anonymous chứa thông tin chi tiết tài liệu.</returns>
        public List<object> LayTatCaThongTinTaiLieu()
        {
            // Chỉ cần gọi DAL
            return dal.LayTatCaThongTinTaiLieu();
        }

        /// <summary>
        /// Lấy danh sách Tác giả và Vai trò của một Tài liệu cụ thể.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu cần truy vấn.</param>
        /// <returns>Danh sách các đối tượng anonymous chứa Mã tác giả, Tên tác giả và Vai trò.</returns>
        public List<object> LayTacGiaTheoMaTaiLieu(string maTL)
        {
            if (string.IsNullOrEmpty(maTL))
            {
                // Trả về danh sách rỗng nếu Mã TL không hợp lệ
                return new List<object>();
            }

            return dal.LayTacGiaTheoMaTaiLieu(maTL);
        }

        /// <summary>
        /// Thực hiện tìm kiếm Tài liệu dựa trên danh sách bộ lọc.
        /// </summary>
        /// <param name="filters">Danh sách các đối tượng Filter chứa thông tin tìm kiếm.</param>
        /// <returns>Danh sách các đối tượng Tài liệu khớp với bộ lọc.</returns>
        public List<object> TimKiemTaiLieu(List<Filter> filters)
        {
            // Nếu bộ lọc rỗng, trả về tất cả tài liệu.
            if (filters == null || filters.Count == 0 || filters.All(f => string.IsNullOrWhiteSpace(f.Value)))
            {
                return dal.LayTatCaThongTinTaiLieu();
            }

            // Gọi phương thức tìm kiếm trong DAL
            return dal.TimKiemTaiLieu(filters);
        }

        // ====================================================
        // II. PHƯƠNG THỨC THAO TÁC DỮ LIỆU (CRUD)
        // ====================================================

        /// <summary>
        /// Thêm mới một Tài liệu và các Tác giả liên quan vào hệ thống.
        /// </summary>
        /// <param name="taiLieu">Đối tượng tTaiLieu cần thêm.</param>
        /// <param name="danhSachTacGia">Danh sách tTaiLieu_TacGia liên kết.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nghiệp vụ hoặc lỗi DAL.</param>
        /// <returns>True nếu thêm thành công, False nếu ngược lại.</returns>
        public bool ThemTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGia, out string errorMessage)
        {
            // ************ Kiểm tra Logic Nghiệp vụ (Business Logic) ************
            // Kiểm tra Năm xuất bản
            if (taiLieu.NamXuatBan.HasValue && taiLieu.NamXuatBan.Value > DateTime.Now.Year)
            {
                errorMessage = "Năm xuất bản không thể lớn hơn năm hiện tại.";
                return false;
            }

            // Kiểm tra số trang
            if (taiLieu.SoTrang.HasValue && taiLieu.SoTrang.Value <= 0)
            {
                errorMessage = "Số trang phải là một số dương.";
                return false;
            }

            // ... (Thêm các kiểm tra nghiệp vụ khác tại đây) ...

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.ThemTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
        }

        /// <summary>
        /// Cập nhật thông tin của một Tài liệu đã tồn tại và danh sách Tác giả liên quan.
        /// </summary>
        /// <param name="taiLieu">Đối tượng tTaiLieu chứa thông tin cập nhật.</param>
        /// <param name="danhSachTacGia">Danh sách tTaiLieu_TacGia mới (sẽ thay thế cái cũ).</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nghiệp vụ hoặc lỗi DAL.</param>
        /// <returns>True nếu sửa thành công, False nếu ngược lại.</returns>
        public bool SuaTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGia, out string errorMessage)
        {
            // ************ Kiểm tra Logic Nghiệp vụ (Business Logic) ************
            // Kiểm tra Năm xuất bản
            if (taiLieu.NamXuatBan.HasValue && taiLieu.NamXuatBan.Value > DateTime.Now.Year)
            {
                errorMessage = "Năm xuất bản không thể lớn hơn năm hiện tại.";
                return false;
            }

            // Kiểm tra số trang
            if (taiLieu.SoTrang.HasValue && taiLieu.SoTrang.Value <= 0)
            {
                errorMessage = "Số trang phải là một số dương.";
                return false;
            }

            // ... (Thêm các kiểm tra nghiệp vụ khác tại đây) ...

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.SuaTaiLieu(taiLieu, danhSachTacGia, out errorMessage);
        }

        /// <summary>
        /// Xóa Tài liệu dựa trên Mã Tài liệu.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu cần xóa.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi.</param>
        /// <returns>True nếu xóa thành công, False nếu ngược lại.</returns>
        public bool XoaTaiLieu(string maTL, out string errorMessage)
        {
            // ************ Kiểm tra Logic Nghiệp vụ (Business Logic) ************
            // (Tại đây có thể thêm logic kiểm tra xem tài liệu có đang được mượn hay không)

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.XoaTaiLieu(maTL, out errorMessage);
        }

        // ====================================================
        // III. PHƯƠNG THỨC HỖ TRỢ & MAPPING (COMBOBOX DATA & ID LOOKUP)
        // ====================================================

        // Các phương thức chỉ gọi DAL, sử dụng expression body (=>)

        /// <summary>
        /// Lấy danh sách tên Nhà xuất bản (dùng cho ComboBox/Filter).
        /// </summary>
        public List<string> LayDanhSachNhaXuatBan() => dal.LayDanhSachNhaXuatBan();

        /// <summary>
        /// Lấy danh sách tên Ngôn ngữ (dùng cho ComboBox/Filter).
        /// </summary>
        public List<string> LayDanhSachNgonNgu() => dal.LayDanhSachNgonNgu();

        /// <summary>
        /// Lấy danh sách tên Thể loại (dùng cho ComboBox/Filter).
        /// </summary>
        public List<string> LayDanhSachTheLoai() => dal.LayDanhSachTheLoai();

        /// <summary>
        /// Lấy danh sách tên Định dạng (dùng cho ComboBox/Filter).
        /// </summary>
        public List<string> LayDanhSachDinhDang() => dal.LayDanhSachDinhDang();

        /// <summary>
        /// Lấy danh sách Tên đầy đủ của Tác giả (dùng cho ComboBox/Filter).
        /// </summary>
        public List<string> LayDanhSachTacGia() => dal.LayDanhSachTacGia();

        /// <summary>
        /// Lấy danh sách các Vai trò (chức danh) của tác giả (dùng cho ComboBox).
        /// </summary>
        public List<string> LayDanhSachVaiTro() => dal.LayDanhSachVaiTro();

        // ----------------------------------------------------
        // Mapping Tên sang Mã (Hỗ trợ khi người dùng chọn từ ComboBox)
        // ----------------------------------------------------

        /// <summary>
        /// Lấy Mã Nhà xuất bản từ Tên Nhà xuất bản.
        /// </summary>
        public string LayMaNXB(string tenNXB) => dal.LayMaNXB(tenNXB);

        /// <summary>
        /// Lấy Mã Ngôn ngữ từ Tên Ngôn ngữ.
        /// </summary>
        public string LayMaNN(string tenNN) => dal.LayMaNN(tenNN);

        /// <summary>
        /// Lấy Mã Thể loại từ Tên Thể loại.
        /// </summary>
        public string LayMaThL(string tenThL) => dal.LayMaThL(tenThL);

        /// <summary>
        /// Lấy Mã Định dạng từ Tên Định dạng.
        /// </summary>
        public string LayMaDD(string tenDD) => dal.LayMaDD(tenDD);

        /// <summary>
        /// Lấy Mã Tác giả từ Tên đầy đủ Tác giả.
        /// </summary>
        public string LayMaTacGia(string tenTacGia) => dal.LayMaTacGia(tenTacGia);
    }
}
