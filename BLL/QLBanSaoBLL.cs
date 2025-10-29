using QuanLyThuVien.DAL;
using QuanLyThuVien.Helpers;
using System.Collections.Generic;
using System.Linq;

// Lưu ý: tBanSao được giả định đã được khai báo (thường thông qua LINQ to SQL DataContext)

namespace QuanLyThuVien.BLL
{
    /// <summary>
    /// Lớp Logic Nghiệp vụ (BLL) xử lý các thao tác liên quan đến Bản sao Tài liệu (tBanSao).
    /// </summary>
    internal class QLBanSaoBLL
    {
        private readonly QLBanSaoDAL dal = new QLBanSaoDAL();

        // ====================================================
        // I. PHƯƠNG THỨC TRUY VẤN & TÌM KIẾM (GET/READ/SEARCH)
        // ====================================================

        /// <summary>
        /// Lấy tất cả Bản sao của một Tài liệu cụ thể.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu (MaTL) để lọc Bản sao.</param>
        /// <returns>Danh sách các đối tượng ẩn danh chứa thông tin Bản sao.</returns>
        public List<object> LayTatCaBanSaoTheoMaTL(string maTL)
        {
            if (string.IsNullOrEmpty(maTL))
            {
                return new List<object>();
            }
            // Chỉ cần gọi DAL
            return dal.LayTatCaBanSaoTheoMaTL(maTL);
        }

        /// <summary>
        /// Thực hiện tìm kiếm Bản sao dựa trên Mã Bản sao và Tình trạng.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu mà bản sao trực thuộc.</param>
        /// <param name="filters">Danh sách các đối tượng Filter chứa thông tin tìm kiếm.</param>
        /// <returns>Danh sách các đối tượng Bản sao khớp với bộ lọc.</returns>
        public List<object> TimKiemBanSao(string maTL, List<Filter> filters)
        {
            // Kiểm tra bộ lọc rỗng. Nếu rỗng, trả về tất cả bản sao của MaTL đó.
            if (filters == null || filters.Count == 0 || filters.All(f => string.IsNullOrWhiteSpace(f.Value)))
            {
                return dal.LayTatCaBanSaoTheoMaTL(maTL);
            }

            // Gọi phương thức tìm kiếm trong DAL
            return dal.TimKiemBanSao(maTL, filters);
        }

        // ====================================================
        // II. PHƯƠNG THỨC THAO TÁC DỮ LIỆU (CUD)
        // ====================================================

        /// <summary>
        /// Thêm mới một Bản sao vào hệ thống.
        /// </summary>
        /// <param name="banSao">Đối tượng tBanSao cần thêm.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nghiệp vụ hoặc lỗi DAL.</param>
        /// <returns>True nếu thêm thành công, False nếu ngược lại.</returns>
        public bool ThemBanSao(tBanSao banSao, out string errorMessage)
        {
            // ************ Kiểm tra Logic Nghiệp vụ (Business Logic) ************
            if (string.IsNullOrWhiteSpace(banSao.MaBS))
            {
                errorMessage = "Mã Bản sao không được để trống.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(banSao.MaTL))
            {
                errorMessage = "Mã Tài liệu liên kết không được để trống.";
                return false;
            }

            // Gán giá trị mặc định cho TinhTrang nếu cần
            if (string.IsNullOrWhiteSpace(banSao.TinhTrang))
            {
                banSao.TinhTrang = "Chưa được mượn";
            }

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.ThemBanSao(banSao, out errorMessage);
        }

        /// <summary>
        /// Xóa Bản sao dựa trên Mã Bản sao.
        /// </summary>
        /// <param name="maBS">Mã Bản sao cần xóa.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi.</param>
        /// <returns>True nếu xóa thành công, False nếu ngược lại.</returns>
        public bool XoaBanSao(string maBS, out string errorMessage)
        {
            // ************ Kiểm tra Logic Nghiệp vụ (Business Logic) ************
            if (string.IsNullOrWhiteSpace(maBS))
            {
                errorMessage = "Mã Bản sao cần xóa không được để trống.";
                return false;
            }

            // Note: Kiểm tra xem Bản sao có đang ở trạng thái 'Đã được mượn' không 
            // có thể được thực hiện ở đây nếu BLL có quyền truy cập thông tin chi tiết. 
            // Hiện tại, ta dựa vào việc bắt lỗi khóa ngoại ở tầng DAL.

            // Gọi DAL để thực hiện thao tác cơ sở dữ liệu
            return dal.XoaBanSao(maBS, out errorMessage);
        }

        // ====================================================
        // III. PHƯƠNG THỨC HỖ TRỢ (COMBOBOX DATA)
        // ====================================================

        /// <summary>
        /// Lấy danh sách các Tình trạng Bản sao mặc định để hiển thị trên ComboBox.
        /// </summary>
        public List<string> LayDanhSachTinhTrang() => dal.LayDanhSachTinhTrang();
    }
}
