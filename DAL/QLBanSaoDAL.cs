using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyThuVien.DAL
{
    /// <summary>
    /// Lớp Truy cập Dữ liệu (DAL) cho việc quản lý Bản sao Tài liệu (tBanSao).
    /// </summary>
    internal class QLBanSaoDAL : DatabaseConnect
    {
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
            using (var db = new QLThuVienDataContext())
            {
                var query = from bs in db.tBanSaos
                            where bs.MaTL == maTL
                            orderby bs.MaBS
                            select new
                            {
                                bs.MaBS,
                                bs.MaTL,
                                bs.TinhTrang
                            };

                return query.ToList<object>();
            }
        }

        /// <summary>
        /// Thực hiện tìm kiếm Bản sao dựa trên Mã Bản sao và Tình trạng trong phạm vi Mã Tài liệu.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu mà bản sao trực thuộc (bắt buộc).</param>
        /// <param name="filters">Danh sách các đối tượng Filter chứa ColumnName và Value.</param>
        /// <returns>Danh sách Bản sao khớp với điều kiện lọc.</returns>
        public List<object> TimKiemBanSao(string maTL, List<Filter> filters)
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bs in db.tBanSaos
                            where bs.MaTL == maTL
                            select bs;

                foreach (var filter in filters)
                {
                    if (string.IsNullOrWhiteSpace(filter.Value)) continue;
                    string value = filter.Value.Trim().ToLower();

                    switch (filter.ColumnName)
                    {
                        case "MaBS":
                            query = query.Where(x => x.MaBS.ToLower().Contains(value));
                            break;
                        case "TinhTrang":
                            // Tình trạng thường là Chuẩn (Exact Match) hoặc Contains (Phù hợp với thiết kế search hiện tại)
                            query = query.Where(x => x.TinhTrang != null && x.TinhTrang.ToLower().Contains(value));
                            break;
                    }
                }

                // Projection ra đối tượng ẩn danh để đồng bộ với LayTatCaBanSaoTheoMaTL
                var finalResult = query.Select(bs => new
                {
                    bs.MaBS,
                    bs.MaTL,
                    bs.TinhTrang
                });

                return finalResult.ToList<object>();
            }
        }

        // ====================================================
        // II. PHƯƠNG THỨC THAO TÁC DỮ LIỆU (CRUD)
        // Lưu ý: Không có SuaBanSao vì Tình trạng được quản lý ở chức năng Mượn/Trả.
        // ====================================================

        /// <summary>
        /// Thêm mới một Bản sao vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="banSao">Đối tượng tBanSao cần thêm.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nếu thao tác thất bại.</param>
        /// <returns>True nếu thêm thành công, False nếu ngược lại.</returns>
        public bool ThemBanSao(tBanSao banSao, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(banSao.MaBS) || string.IsNullOrWhiteSpace(banSao.MaTL))
            {
                errorMessage = "Mã Bản sao và Mã Tài liệu không được để trống.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // 1. Kiểm tra trùng Mã Bản sao
                    if (db.tBanSaos.Any(x => x.MaBS == banSao.MaBS))
                    {
                        errorMessage = "Mã Bản sao đã tồn tại.";
                        return false;
                    }

                    // 2. Thiết lập Tình trạng mặc định nếu chưa có
                    if (string.IsNullOrWhiteSpace(banSao.TinhTrang))
                    {
                        // Giả định Bản sao mới luôn ở trạng thái sẵn sàng
                        banSao.TinhTrang = "Chưa được mượn";
                    }

                    // 3. Thêm Bản sao
                    db.tBanSaos.InsertOnSubmit(banSao);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Lỗi có thể do MãTL không tồn tại hoặc lỗi DB khác
                errorMessage = "Lỗi DAL khi thêm Bản sao: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa một Bản sao dựa trên Mã Bản sao.
        /// </summary>
        /// <param name="maBS">Mã Bản sao cần xóa.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nếu thao tác thất bại.</param>
        /// <returns>True nếu xóa thành công, False nếu ngược lại.</returns>
        public bool XoaBanSao(string maBS, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(maBS))
            {
                errorMessage = "Mã Bản sao không hợp lệ.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    var bs = db.tBanSaos.FirstOrDefault(x => x.MaBS == maBS);

                    if (bs == null)
                    {
                        errorMessage = "Bản sao không tồn tại.";
                        return false;
                    }

                    // Kiểm tra khóa ngoại (Nếu Bản sao đã có trong tGiaoDich_BanSao thì không thể xóa)
                    // (Lưu ý: Hành vi này có thể được điều chỉnh bởi DeleteRule trong DBML)
                    // Theo DBML, tGiaoDich_BanSao có DeleteRule="CASCADE" từ tBanSao, 
                    // nhưng ta vẫn nên bắt lỗi nếu có giao dịch liên quan để đảm bảo nghiệp vụ.

                    db.tBanSaos.DeleteOnSubmit(bs);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (SqlException sqlex) when (sqlex.Number == 547) // Lỗi khóa ngoại
            {
                // Mặc dù DBML có CASCADE, lỗi này vẫn có thể xảy ra nếu có bảng khác liên quan.
                errorMessage = "Không thể xóa Bản sao này vì nó đã được ghi nhận trong các giao dịch khác (lỗi khóa ngoại).";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi xóa Bản sao: " + ex.Message;
                return false;
            }
        }

        // ====================================================
        // III. PHƯƠNG THỨC HỖ TRỢ (COMBOBOX DATA)
        // ====================================================

        /// <summary>
        /// Lấy danh sách các Tình trạng Bản sao mặc định để hiển thị trên ComboBox.
        /// </summary>
        /// <returns>List&lt;string&gt; các tình trạng Bản sao.</returns>
        public List<string> LayDanhSachTinhTrang()
        {
            // Cung cấp danh sách cố định theo yêu cầu (Chưa được mượn / Đã được mượn)
            return new List<string> { "Chưa được mượn", "Đã được mượn", "Hỏng/Mất" };
        }
    }
}
