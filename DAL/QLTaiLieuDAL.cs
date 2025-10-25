using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace QuanLyThuVien.DAL
{
    internal class QLTaiLieuDAL : DatabaseConnect
    {
        // ----------------------------------------------------
        // 1. Lấy tất cả Tài liệu (cho dgvTaiLieu)
        // ----------------------------------------------------
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
                                // Hiển thị tên thay vì mã
                                TenNXB = nxb.TenNXB,
                                TenNN = nn.TenNN,
                                TenTheLoai = thl.TenThL,
                                TenDinhDang = dd.TenDD
                                // Bạn có thể thêm tl.MaTK nếu cần
                            };

                return query.ToList<object>();
            }
        }

        // ----------------------------------------------------
        // 2. Lấy Tác giả theo Mã Tài liệu (cho dgvTacGia)
        // ----------------------------------------------------
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

        // ----------------------------------------------------
        // 3. Lấy danh sách Nhà xuất bản (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachNhaXuatBan()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNhaXuatBans
                         .Select(nxb => nxb.TenNXB)
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 4. Lấy danh sách Ngôn ngữ (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachNgonNgu()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNgonNgus
                         .Select(nn => nn.TenNN)
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 5. Lấy danh sách Thể loại (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachTheLoai()
        {
            using (var db = new QLThuVienDataContext())
            {
                // Giả định trường tên là TenThL
                return db.tTheLoais
                         .Select(thl => thl.TenThL)
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 6. Lấy danh sách Định dạng (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachDinhDang()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tDinhDangs
                         .Select(dd => dd.TenDD)
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 7. Lấy danh sách Tác giả (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachTacGia()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTacGias
                         .Select(tg => (tg.HoDem + " " + tg.Ten).Trim())
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 8. Lấy danh sách Vai trò (cho ComboBox)
        // ----------------------------------------------------
        public List<string> LayDanhSachVaiTro()
        {
            using (var db = new QLThuVienDataContext())
            {
                // Giả định Vai trò được lưu trong bảng tTaiLieu_TacGia và cần lấy các giá trị distinct
                return db.tTaiLieu_TacGias
                         .Select(tltg => tltg.VaiTro)
                         .Distinct() // Chỉ lấy các giá trị VaiTro duy nhất
                         .ToList();
            }
        }

        // ----------------------------------------------------
        // 9. Tìm kiếm Tài liệu theo Bộ lọc
        // ----------------------------------------------------
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

        // ----------------------------------------------------
        // 10. Thêm Tài liệu (và Tác giả liên quan)
        // ----------------------------------------------------
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

        // ----------------------------------------------------
        // 11. Sửa Tài liệu (và Tác giả liên quan)
        // ----------------------------------------------------
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
                    // tlHienTai.MaTK = taiLieu.MaTK; // Giả định MaTK có thể thay đổi

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

        // ----------------------------------------------------
        // 12. Xóa Tài liệu (và Tác giả liên quan)
        // ----------------------------------------------------
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

                    // Lưu ý: Cần kiểm tra khóa ngoại (ví dụ: đang có Sách nào mượn Tài liệu này không?)
                    // Giả định hệ thống Database đã có Foreign Key CASCADE DELETE hoặc đang dùng ON DELETE RESTRICT
                    // Nếu là RESTRICT và có Sách đang liên kết, SubmitChanges sẽ báo lỗi.

                    // Xóa Tác giả liên quan trước (do tTaiLieu_TacGia tham chiếu tTaiLieu)
                    var dsTacGia = db.tTaiLieu_TacGias.Where(x => x.MaTL == maTL);
                    db.tTaiLieu_TacGias.DeleteAllOnSubmit(dsTacGia);

                    // Xóa Tài liệu
                    db.tTaiLieus.DeleteOnSubmit(tl);

                    db.SubmitChanges();
                    return true;
                }
            }
            catch (SqlException sqlex)
            {
                // Xử lý lỗi khóa ngoại cụ thể nếu cần
                if (sqlex.Number == 547)
                {
                    errorMessage = "Không thể xóa tài liệu này vì nó đang được sử dụng trong các giao dịch hoặc bảng khác.";
                }
                else
                {
                    errorMessage = "Lỗi DAL khi xóa tài liệu: " + sqlex.Message;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi xóa tài liệu: " + ex.Message;
                return false;
            }
        }

        // ----------------------------------------------------
        // 13. Mapping Tên sang Mã
        // ----------------------------------------------------
        public string LayMaNXB(string tenNXB)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNhaXuatBans.FirstOrDefault(n => n.TenNXB == tenNXB)?.MaNXB;
            }
        }
        public string LayMaNN(string tenNN)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tNgonNgus.FirstOrDefault(n => n.TenNN == tenNN)?.MaNN;
            }
        }
        public string LayMaThL(string tenThL)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTheLoais.FirstOrDefault(t => t.TenThL == tenThL)?.MaThL;
            }
        }
        public string LayMaDD(string tenDD)
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tDinhDangs.FirstOrDefault(d => d.TenDD == tenDD)?.MaDD;
            }
        }
        public string LayMaTacGia(string tenTacGia)
        {
            using (var db = new QLThuVienDataContext())
            {
                // Tách Họ đệm và Tên để tìm kiếm
                string[] parts = tenTacGia.Trim().Split(' ');
                string ten = parts.Last();
                string hoDem = string.Join(" ", parts.Take(parts.Length - 1));

                return db.tTacGias.FirstOrDefault(tg => tg.Ten == ten && tg.HoDem == hoDem)?.MaTG;
            }
        }
    }
}
