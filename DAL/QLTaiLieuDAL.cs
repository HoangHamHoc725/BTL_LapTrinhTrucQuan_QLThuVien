using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace QuanLyThuVien.DAL
{
    /// <summary>
    /// Lớp truy cập dữ liệu (DAL) cho các bảng liên quan đến Tài liệu (tTaiLieu, tNhaXuatBan, tTacGia,...)
    /// </summary>
    internal class QLTaiLieuDAL : DatabaseConnect
    {
        // ====================================================
        // I. PHƯƠNG THỨC TRUY VẤN DỮ LIỆU (GET/READ)
        // ====================================================

        /// <summary>
        /// Lấy tất cả thông tin chi tiết của Tài liệu, bao gồm cả tên của các khóa ngoại.
        /// </summary>
        /// <returns>Danh sách các đối tượng ẩn danh chứa thông tin Tài liệu đã được JOIN.</returns>
        public List<object> LayTatCaThongTinTaiLieu()
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from tl in db.tTaiLieus
                            join nxb in db.tNhaXuatBans on tl.MaNXB equals nxb.MaNXB
                            join nn in db.tNgonNgus on tl.MaNN equals nn.MaNN
                            join thl in db.tTheLoais on tl.MaThL equals thl.MaThL
                            join dd in db.tDinhDangs on tl.MaDD equals dd.MaDD
                            select new
                            {
                                tl.MaTL,
                                tl.TenTL,
                                tl.LanXuatBan,
                                tl.NamXuatBan,
                                tl.SoTrang,
                                tl.KhoCo,
                                TenNXB = nxb.TenNXB,
                                TenNN = nn.TenNN,
                                TenTheLoai = thl.TenThL,
                                TenDinhDang = dd.TenDD
                            };

                return query.ToList<object>();
            }
        }

        /// <summary>
        /// Lấy danh sách Tác giả và Vai trò của họ theo Mã Tài liệu cụ thể.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu cần truy vấn.</param>
        /// <returns>Danh sách các đối tượng ẩn danh chứa Mã Tác giả, Tên Tác giả và Vai trò.</returns>
        public List<object> LayTacGiaTheoMaTaiLieu(string maTL)
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from tltg in db.tTaiLieu_TacGias
                            join tg in db.tTacGias on tltg.MaTG equals tg.MaTG
                            where tltg.MaTL == maTL
                            select new
                            {
                                tltg.MaTG,
                                TenTacGia = (tg.HoDem + " " + tg.Ten).Trim(),
                                tltg.VaiTro
                            };

                return query.ToList<object>();
            }
        }

        /// <summary>
        /// Thực hiện tìm kiếm Tài liệu dựa trên danh sách các bộ lọc được cung cấp.
        /// </summary>
        /// <param name="filters">Danh sách các đối tượng Filter chứa ColumnName và Value.</param>
        /// <returns>Danh sách Tài liệu khớp với điều kiện lọc (dùng cho DataGridView).</returns>
        public List<object> TimKiemTaiLieu(List<Filter> filters)
        {
            using (var db = new QLThuVienDataContext())
            {
                // 1. Khởi tạo truy vấn ban đầu (JOIN tất cả các bảng liên quan)
                var query = from tl in db.tTaiLieus
                            join nxb in db.tNhaXuatBans on tl.MaNXB equals nxb.MaNXB
                            join nn in db.tNgonNgus on tl.MaNN equals nn.MaNN
                            join thl in db.tTheLoais on tl.MaThL equals thl.MaThL
                            join dd in db.tDinhDangs on tl.MaDD equals dd.MaDD
                            select new
                            {
                                tl,
                                nxb,
                                nn,
                                thl,
                                dd,
                                // Sub-query để lấy tên Tác giả chính (hoặc Tác giả đầu tiên)
                                TenTacGia = (from tltg in db.tTaiLieu_TacGias
                                             join tg in db.tTacGias on tltg.MaTG equals tg.MaTG
                                             where tltg.MaTL == tl.MaTL
                                             select (tg.HoDem + " " + tg.Ten).Trim()).FirstOrDefault()
                            };

                // 2. Áp dụng từng bộ lọc
                foreach (var filter in filters)
                {
                    if (string.IsNullOrWhiteSpace(filter.Value)) continue;
                    string value = filter.Value.Trim().ToLower();

                    switch (filter.ColumnName)
                    {
                        case "MaTL":
                            query = query.Where(x => x.tl.MaTL.ToLower().Contains(value));
                            break;
                        case "TenTL":
                            query = query.Where(x => x.tl.TenTL.ToLower().Contains(value));
                            break;
                        case "TenNXB":
                            query = query.Where(x => x.nxb.TenNXB.ToLower().Contains(value));
                            break;
                        case "TenNN":
                            query = query.Where(x => x.nn.TenNN.ToLower().Contains(value));
                            break;
                        case "TenTheLoai":
                            query = query.Where(x => x.thl.TenThL.ToLower().Contains(value));
                            break;
                        case "TenDinhDang":
                            query = query.Where(x => x.dd.TenDD.ToLower().Contains(value));
                            break;
                        case "NamXuatBan":
                            query = query.Where(x => x.tl.NamXuatBan.ToString().Contains(value));
                            break;
                        case "LanXuatBan":
                            query = query.Where(x => x.tl.LanXuatBan.ToString().Contains(value));
                            break;
                        case "SoTrang":
                            query = query.Where(x => x.tl.SoTrang.ToString().Contains(value));
                            break;
                        case "KhoCo":
                            query = query.Where(x => x.tl.KhoCo.ToLower().Contains(value));
                            break;
                        case "TenTacGia":
                            query = query.Where(x => x.TenTacGia != null && x.TenTacGia.ToLower().Contains(value));
                            break;
                    }
                }

                // 3. Kết quả cuối cùng (Projection)
                var finalResult = query.Select(x => new
                {
                    x.tl.MaTL,
                    x.tl.TenTL,
                    x.tl.LanXuatBan,
                    x.tl.NamXuatBan,
                    x.tl.SoTrang,
                    x.tl.KhoCo,
                    TenNXB = x.nxb.TenNXB,
                    TenNN = x.nn.TenNN,
                    TenTheLoai = x.thl.TenThL,
                    TenDinhDang = x.dd.TenDD
                });

                return finalResult.ToList<object>();
            }
        }

        // ====================================================
        // II. PHƯƠNG THỨC THAO TÁC DỮ LIỆU (CRUD)
        // ====================================================

        /// <summary>
        /// Thêm mới một Tài liệu và danh sách các Tác giả liên quan vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="taiLieu">Đối tượng tTaiLieu cần thêm.</param>
        /// <param name="danhSachTacGia">Danh sách tTaiLieu_TacGia liên kết.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nếu thao tác thất bại.</param>
        /// <returns>True nếu thêm thành công, False nếu ngược lại.</returns>
        public bool ThemTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGia, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(taiLieu.MaTL) || string.IsNullOrWhiteSpace(taiLieu.TenTL))
            {
                errorMessage = "Mã và Tên tài liệu không được để trống.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // 1. Kiểm tra trùng mã
                    if (db.tTaiLieus.Any(x => x.MaTL == taiLieu.MaTL))
                    {
                        errorMessage = "Mã tài liệu đã tồn tại.";
                        return false;
                    }

                    // 2. Thêm Tài liệu
                    db.tTaiLieus.InsertOnSubmit(taiLieu);

                    // 3. Thêm Tác giả liên quan
                    if (danhSachTacGia != null && danhSachTacGia.Count > 0)
                    {
                        db.tTaiLieu_TacGias.InsertAllOnSubmit(danhSachTacGia);
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi thêm tài liệu: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một Tài liệu và danh sách Tác giả liên quan.
        /// </summary>
        /// <param name="taiLieu">Đối tượng tTaiLieu chứa thông tin cập nhật.</param>
        /// <param name="danhSachTacGiaMoi">Danh sách tTaiLieu_TacGia mới (sẽ thay thế cái cũ).</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nếu thao tác thất bại.</param>
        /// <returns>True nếu sửa thành công, False nếu ngược lại.</returns>
        public bool SuaTaiLieu(tTaiLieu taiLieu, List<tTaiLieu_TacGia> danhSachTacGiaMoi, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(taiLieu.MaTL))
            {
                errorMessage = "Mã tài liệu không hợp lệ.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    var tlHienTai = db.tTaiLieus.FirstOrDefault(x => x.MaTL == taiLieu.MaTL);

                    if (tlHienTai == null)
                    {
                        errorMessage = "Tài liệu không tồn tại.";
                        return false;
                    }

                    // 1. Cập nhật thông tin Tài liệu chính
                    tlHienTai.TenTL = taiLieu.TenTL;
                    tlHienTai.MaNXB = taiLieu.MaNXB;
                    tlHienTai.MaNN = taiLieu.MaNN;
                    tlHienTai.MaThL = taiLieu.MaThL;
                    tlHienTai.MaDD = taiLieu.MaDD;
                    tlHienTai.NamXuatBan = taiLieu.NamXuatBan;
                    tlHienTai.LanXuatBan = taiLieu.LanXuatBan;
                    tlHienTai.SoTrang = taiLieu.SoTrang;
                    tlHienTai.KhoCo = taiLieu.KhoCo;

                    // 2. Cập nhật Tác giả (Xóa cũ, Thêm mới)
                    var dsTacGiaCu = db.tTaiLieu_TacGias.Where(x => x.MaTL == taiLieu.MaTL);
                    db.tTaiLieu_TacGias.DeleteAllOnSubmit(dsTacGiaCu);

                    if (danhSachTacGiaMoi != null && danhSachTacGiaMoi.Count > 0)
                    {
                        db.tTaiLieu_TacGias.InsertAllOnSubmit(danhSachTacGiaMoi);
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi sửa tài liệu: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Xóa một Tài liệu và tất cả các liên kết Tác giả tương ứng.
        /// </summary>
        /// <param name="maTL">Mã Tài liệu cần xóa.</param>
        /// <param name="errorMessage">Tham số đầu ra chứa thông báo lỗi nếu thao tác thất bại.</param>
        /// <returns>True nếu xóa thành công, False nếu ngược lại.</returns>
        public bool XoaTaiLieu(string maTL, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(maTL))
            {
                errorMessage = "Mã tài liệu không hợp lệ.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    var tl = db.tTaiLieus.FirstOrDefault(x => x.MaTL == maTL);

                    if (tl == null)
                    {
                        errorMessage = "Tài liệu không tồn tại.";
                        return false;
                    }

                    // Xóa Tác giả liên quan trước
                    var dsTacGia = db.tTaiLieu_TacGias.Where(x => x.MaTL == maTL);
                    db.tTaiLieu_TacGias.DeleteAllOnSubmit(dsTacGia);

                    // Xóa Tài liệu
                    db.tTaiLieus.DeleteOnSubmit(tl);

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (SqlException sqlex) when (sqlex.Number == 547) // Lỗi khóa ngoại
            {
                errorMessage = "Không thể xóa tài liệu này vì nó đang được sử dụng trong các giao dịch hoặc bảng khác.";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi xóa tài liệu: " + ex.Message;
                return false;
            }
        }

        // ====================================================
        // III. PHƯƠNG THỨC HỖ TRỢ (COMBOBOX & MAPPING)
        // ====================================================

        /// <summary>
        /// Lấy danh sách tên Nhà xuất bản (dùng cho ComboBox/Filter).
        /// </summary>
        /// <returns>List<string> tên Nhà xuất bản.</returns>
        public List<string> LayDanhSachNhaXuatBan()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNhaXuatBans
                           .Select(nxb => nxb.TenNXB)
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách tên Ngôn ngữ (dùng cho ComboBox/Filter).
        /// </summary>
        /// <returns>List<string> tên Ngôn ngữ.</returns>
        public List<string> LayDanhSachNgonNgu()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNgonNgus
                           .Select(nn => nn.TenNN)
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách tên Thể loại (dùng cho ComboBox/Filter).
        /// </summary>
        /// <returns>List<string> tên Thể loại.</returns>
        public List<string> LayDanhSachTheLoai()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTheLoais
                           .Select(thl => thl.TenThL)
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách tên Định dạng (dùng cho ComboBox/Filter).
        /// </summary>
        /// <returns>List<string> tên Định dạng.</returns>
        public List<string> LayDanhSachDinhDang()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tDinhDangs
                           .Select(dd => dd.TenDD)
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách Tên đầy đủ Tác giả (dùng cho ComboBox/Filter).
        /// </summary>
        /// <returns>List<string> tên đầy đủ Tác giả.</returns>
        public List<string> LayDanhSachTacGia()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTacGias
                           .Select(tg => (tg.HoDem + " " + tg.Ten).Trim())
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách các Vai trò duy nhất của Tác giả (dùng cho ComboBox).
        /// </summary>
        /// <returns>List<string> các Vai trò duy nhất.</returns>
        public List<string> LayDanhSachVaiTro()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTaiLieu_TacGias
                           .Select(tltg => tltg.VaiTro)
                           .Distinct()
                           .ToList();
            }
        }

        /// <summary>
        /// Lấy Mã Nhà xuất bản từ Tên Nhà xuất bản.
        /// </summary>
        /// <param name="tenNXB">Tên Nhà xuất bản.</param>
        /// <returns>Mã NXB hoặc null.</returns>
        public string LayMaNXB(string tenNXB)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNhaXuatBans.FirstOrDefault(n => n.TenNXB == tenNXB)?.MaNXB;
            }
        }

        /// <summary>
        /// Lấy Mã Ngôn ngữ từ Tên Ngôn ngữ.
        /// </summary>
        /// <param name="tenNN">Tên Ngôn ngữ.</param>
        /// <returns>Mã NN hoặc null.</returns>
        public string LayMaNN(string tenNN)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNgonNgus.FirstOrDefault(n => n.TenNN == tenNN)?.MaNN;
            }
        }

        /// <summary>
        /// Lấy Mã Thể loại từ Tên Thể loại.
        /// </summary>
        /// <param name="tenThL">Tên Thể loại.</param>
        /// <returns>Mã ThL hoặc null.</returns>
        public string LayMaThL(string tenThL)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTheLoais.FirstOrDefault(t => t.TenThL == tenThL)?.MaThL;
            }
        }

        /// <summary>
        /// Lấy Mã Định dạng từ Tên Định dạng.
        /// </summary>
        /// <param name="tenDD">Tên Định dạng.</param>
        /// <returns>Mã DD hoặc null.</returns>
        public string LayMaDD(string tenDD)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tDinhDangs.FirstOrDefault(d => d.TenDD == tenDD)?.MaDD;
            }
        }

        /// <summary>
        /// Lấy Mã Tác giả từ Tên đầy đủ Tác giả.
        /// </summary>
        /// <param name="tenTacGia">Tên đầy đủ của Tác giả (VD: Nguyễn Văn A).</param>
        /// <returns>Mã TG hoặc null.</returns>
        public string LayMaTacGia(string tenTacGia)
        {
            using (var db = new QLThuVienDataContext())
            {
                // Tách Họ đệm và Tên để tìm kiếm
                string trimmedName = tenTacGia.Trim();
                int lastSpaceIndex = trimmedName.LastIndexOf(' ');

                string ten = (lastSpaceIndex > -1) ? trimmedName.Substring(lastSpaceIndex + 1) : trimmedName;
                string hoDem = (lastSpaceIndex > -1) ? trimmedName.Substring(0, lastSpaceIndex) : string.Empty;

                return db.tTacGias.FirstOrDefault(tg => tg.Ten == ten && tg.HoDem == hoDem)?.MaTG;
            }
        }
    }
}