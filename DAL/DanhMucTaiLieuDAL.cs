// File: QuanLyThuVien.DAL/DanhMucTaiLieuDAL.cs

using QuanLyThuVien.DTO;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyThuVien.DAL
{
    internal class DanhMucTaiLieuDAL : DatabaseConnect
    {
        // ----------------------------------------------------
        // Lấy tất cả Danh mục (Sử dụng LINQ và DTO)
        // ----------------------------------------------------
        public List<DanhMucTaiLieuDTO> GetAllDanhMuc(string table)
        {
            using (var db = new QLThuVienDataContext())
            {
                IQueryable<DanhMucTaiLieuDTO> query;

                switch (table)
                {
                    case "tTacGia":
                        query = db.tTacGias
                            .Select(tg => new DanhMucTaiLieuDTO
                            {
                                Ma = tg.MaTG,
                                HoDem = tg.HoDem,
                                Ten = tg.Ten
                            });
                        break;
                    case "tNhaXuatBan":
                        query = db.tNhaXuatBans
                            .Select(nxb => new DanhMucTaiLieuDTO { Ma = nxb.MaNXB, Ten = nxb.TenNXB });
                        break;
                    case "tNgonNgu":
                        query = db.tNgonNgus
                            .Select(nn => new DanhMucTaiLieuDTO { Ma = nn.MaNN, Ten = nn.TenNN });
                        break;
                    case "tTheLoai":
                        query = db.tTheLoais
                            .Select(thl => new DanhMucTaiLieuDTO { Ma = thl.MaThL, Ten = thl.TenThL });
                        break;
                    case "tDinhDang":
                        query = db.tDinhDangs
                            .Select(dd => new DanhMucTaiLieuDTO { Ma = dd.MaDD, Ten = dd.TenDD });
                        break;
                    default:
                        return new List<DanhMucTaiLieuDTO>();
                }

                return query.ToList();
            }
        }

        // ----------------------------------------------------
        // Thêm Danh mục (CRUD)
        // ----------------------------------------------------
        public bool ThemDanhMuc(string table, DanhMucTaiLieuDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // Kiểm tra trùng mã (Đơn giản hóa: Cần phải check theo từng Entity để tránh lỗi Reflection)
                    if (table == "tTacGia" && db.tTacGias.Any(x => x.MaTG == dto.Ma)) { errorMessage = "Mã tác giả đã tồn tại."; return false; }
                    if (table == "tNhaXuatBan" && db.tNhaXuatBans.Any(x => x.MaNXB == dto.Ma)) { errorMessage = "Mã NXB đã tồn tại."; return false; }
                    // ... (Tương tự cho các bảng khác)

                    switch (table)
                    {
                        case "tTacGia":
                            db.tTacGias.InsertOnSubmit(new tTacGia { MaTG = dto.Ma, HoDem = dto.HoDem, Ten = dto.Ten });
                            break;
                        case "tNhaXuatBan":
                            db.tNhaXuatBans.InsertOnSubmit(new tNhaXuatBan { MaNXB = dto.Ma, TenNXB = dto.Ten });
                            break;
                        case "tNgonNgu":
                            db.tNgonNgus.InsertOnSubmit(new tNgonNgu { MaNN = dto.Ma, TenNN = dto.Ten });
                            break;
                        case "tTheLoai":
                            db.tTheLoais.InsertOnSubmit(new tTheLoai { MaThL = dto.Ma, TenThL = dto.Ten });
                            break;
                        case "tDinhDang":
                            db.tDinhDangs.InsertOnSubmit(new tDinhDang { MaDD = dto.Ma, TenDD = dto.Ten });
                            break;
                        default: return false;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi thêm danh mục: " + ex.Message;
                return false;
            }
        }

        // ----------------------------------------------------
        // Sửa Danh mục (CRUD)
        // ----------------------------------------------------
        public bool SuaDanhMuc(string table, DanhMucTaiLieuDTO dto, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    switch (table)
                    {
                        case "tTacGia":
                            var tg = db.tTacGias.FirstOrDefault(x => x.MaTG == dto.Ma);
                            if (tg == null) { errorMessage = "Tác giả không tồn tại."; return false; }
                            tg.HoDem = dto.HoDem;
                            tg.Ten = dto.Ten;
                            break;
                        case "tNhaXuatBan":
                            var nxb = db.tNhaXuatBans.FirstOrDefault(x => x.MaNXB == dto.Ma);
                            if (nxb == null) { errorMessage = "Nhà xuất bản không tồn tại."; return false; }
                            nxb.TenNXB = dto.Ten;
                            break;
                        case "tNgonNgu":
                            var nn = db.tNgonNgus.FirstOrDefault(x => x.MaNN == dto.Ma);
                            if (nn == null) { errorMessage = "Ngôn ngữ không tồn tại."; return false; }
                            nn.TenNN = dto.Ten;
                            break;
                        case "tTheLoai":
                            var thl = db.tTheLoais.FirstOrDefault(x => x.MaThL == dto.Ma);
                            if (thl == null) { errorMessage = "Thể loại không tồn tại."; return false; }
                            thl.TenThL = dto.Ten;
                            break;
                        case "tDinhDang":
                            var dd = db.tDinhDangs.FirstOrDefault(x => x.MaDD == dto.Ma);
                            if (dd == null) { errorMessage = "Định dạng không tồn tại."; return false; }
                            dd.TenDD = dto.Ten;
                            break;
                        default: return false;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi sửa danh mục: " + ex.Message;
                return false;
            }
        }

        // ----------------------------------------------------
        // Xóa Danh mục (CRUD)
        // ----------------------------------------------------
        public bool XoaDanhMuc(string table, string ma, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    object itemToDelete = null;
                    switch (table)
                    {
                        case "tTacGia": itemToDelete = db.tTacGias.FirstOrDefault(x => x.MaTG == ma); break;
                        case "tNhaXuatBan": itemToDelete = db.tNhaXuatBans.FirstOrDefault(x => x.MaNXB == ma); break;
                        case "tNgonNgu": itemToDelete = db.tNgonNgus.FirstOrDefault(x => x.MaNN == ma); break;
                        case "tTheLoai": itemToDelete = db.tTheLoais.FirstOrDefault(x => x.MaThL == ma); break;
                        case "tDinhDang": itemToDelete = db.tDinhDangs.FirstOrDefault(x => x.MaDD == ma); break;
                        default: return false;
                    }

                    if (itemToDelete == null) { errorMessage = "Danh mục không tồn tại."; return false; }

                    db.GetTable(itemToDelete.GetType()).DeleteOnSubmit(itemToDelete);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 547)
                {
                    errorMessage = "Không thể xóa danh mục này vì đang có Tài liệu sử dụng.";
                }
                else
                {
                    errorMessage = "Lỗi DAL khi xóa danh mục: " + sqlex.Message;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi DAL khi xóa danh mục: " + ex.Message;
                return false;
            }
        }

        // ----------------------------------------------------
        // Tìm kiếm Danh mục theo Bộ lọc
        // ----------------------------------------------------
        public List<DanhMucTaiLieuDTO> TimKiemDanhMuc(string table, List<Filter> filters)
        {
            // Bắt đầu bằng việc lấy toàn bộ danh mục của bảng hiện tại
            var query = GetAllDanhMuc(table).AsQueryable();

            if (filters == null || !filters.Any())
            {
                return query.ToList();
            }

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Value)) continue;
                string value = filter.Value.Trim().ToLower();

                switch (filter.ColumnName)
                {
                    case "Ma":
                        query = query.Where(x => x.Ma.ToLower().Contains(value));
                        break;

                    case "Ten":
                        // Tìm kiếm theo TenDayDu (đã bao gồm HoDem và Ten)
                        if (table == "tTacGia")
                        {
                            query = query.Where(x => x.TenDayDu.ToLower().Contains(value));
                        }
                        else
                        {
                            // Đối với các bảng khác (NXB, NN, Thể loại, Định dạng), chỉ tìm kiếm theo trường Ten
                            query = query.Where(x => x.Ten.ToLower().Contains(value));
                        }
                        break;
                }
            }

            return query.ToList();
        }
    }
}