using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.DAL
{
    internal class BanDocDAL : DatabaseConnect
    {
        public List<object> LayTatCaThongTinBanDoc()
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bd in db.tBanDocs
                            join tbd in db.tTheBanDocs on bd.MaBD equals tbd.MaBD
                            select new
                            {
                                bd.MaBD,
                                bd.HoDem,
                                bd.Ten,
                                GioiTinhHienThi = bd.GioiTinh.Equals("M") ? "Nam" : "Nữ",
                                bd.NgaySinh,
                                bd.DiaChi,
                                bd.SDT,
                                bd.Email,
                                tbd.MaTBD,
                                tbd.NgayCap,
                                tbd.NgayHetHan,
                                tbd.TrangThai,
                                tbd.MaTK
                            };

                return query.ToList<object>();
            }
        }

        public List<object> TimKiemBanDoc(List<Filter> filters)
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bd in db.tBanDocs
                            join tbd in db.tTheBanDocs on bd.MaBD equals tbd.MaBD
                            select new
                            {
                                bd.MaBD,
                                bd.HoDem,
                                bd.Ten,
                                GioiTinhHienThi = bd.GioiTinh.Equals("M") ? "Nam" : "Nữ",
                                bd.NgaySinh,
                                bd.DiaChi,
                                bd.SDT,
                                bd.Email,
                                tbd.MaTBD,
                                tbd.NgayCap,
                                tbd.NgayHetHan,
                                tbd.TrangThai,
                                tbd.MaTK
                            };

                // Không có filter nào thì trả về toàn bộ
                if (filters == null || filters.Count == 0)
                    return query.ToList<object>();

                foreach (var f in filters)
                {
                    string value = f.Value?.Trim() ?? "";
                    string op = f.Operator ?? "=";

                    switch (f.ColumnName)
                    {
                        case "MaBD":
                            if (op == "Chứa")
                                query = query.Where(x => x.MaBD.Contains(value));
                            else
                                query = query.Where(x => x.MaBD == value);
                            break;

                        case "HoTen":
                            if (op == "Chứa")
                                query = query.Where(x => (x.HoDem + " " + x.Ten).Contains(value));
                            else
                                query = query.Where(x => (x.HoDem + " " + x.Ten) == value);
                            break;

                        case "GioiTinh":
                            if (value.Equals("Nam", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.GioiTinhHienThi == "Nam");
                            else if (value.Equals("Nữ", StringComparison.OrdinalIgnoreCase))
                                query = query.Where(x => x.GioiTinhHienThi == "Nữ");
                            break;

                        case "DiaChi":
                            if (op == "Chứa")
                                query = query.Where(x => x.DiaChi.Contains(value));
                            else
                                query = query.Where(x => x.DiaChi == value);
                            break;

                        case "SDT":
                            if (op == "Chứa")
                                query = query.Where(x => x.SDT.Contains(value));
                            else
                                query = query.Where(x => x.SDT == value);
                            break;

                        case "Email":
                            if (op == "Chứa")
                                query = query.Where(x => x.Email.Contains(value));
                            else
                                query = query.Where(x => x.Email == value);
                            break;

                        case "MaTBD":
                            if (op == "Chứa")
                                query = query.Where(x => x.MaTBD.Contains(value));
                            else
                                query = query.Where(x => x.MaTBD == value);
                            break;

                        case "NgayCap":
                            if (DateTime.TryParse(value, out DateTime ngayCap))
                            {
                                if (op == ">") query = query.Where(x => x.NgayCap > ngayCap);
                                else if (op == "<") query = query.Where(x => x.NgayCap < ngayCap);
                                else if (op == ">=") query = query.Where(x => x.NgayCap >= ngayCap);
                                else if (op == "<=") query = query.Where(x => x.NgayCap <= ngayCap);
                                else query = query.Where(x => x.NgayCap == ngayCap);
                            }
                            break;

                        case "NgayHetHan":
                            if (DateTime.TryParse(value, out DateTime ngayHetHan))
                            {
                                if (op == ">") query = query.Where(x => x.NgayHetHan > ngayHetHan);
                                else if (op == "<") query = query.Where(x => x.NgayHetHan < ngayHetHan);
                                else if (op == ">=") query = query.Where(x => x.NgayHetHan >= ngayHetHan);
                                else if (op == "<=") query = query.Where(x => x.NgayHetHan <= ngayHetHan);
                                else query = query.Where(x => x.NgayHetHan == ngayHetHan);
                            }
                            break;

                        case "TrangThai":
                            if (op == "Chứa")
                                query = query.Where(x => x.TrangThai.Contains(value));
                            else
                                query = query.Where(x => x.TrangThai == value);
                            break;

                        case "MaTK":
                            if (op == "Chứa")
                                query = query.Where(x => x.MaTK.Contains(value));
                            else
                                query = query.Where(x => x.MaTK == value);
                            break;

                        default:
                            break;
                    }
                }

                return query.ToList<object>();
            }
        }

        public bool ThemBanDoc(tBanDoc banDoc, tTheBanDoc theBanDoc, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(banDoc.MaBD) ||
                string.IsNullOrWhiteSpace(banDoc.HoDem) ||
                string.IsNullOrWhiteSpace(banDoc.Ten) ||
                (banDoc.GioiTinh != 'M' && banDoc.GioiTinh != 'F') ||
                string.IsNullOrWhiteSpace(banDoc.SDT) ||
                string.IsNullOrWhiteSpace(banDoc.Email) ||
                theBanDoc.NgayCap == default ||
                string.IsNullOrWhiteSpace(theBanDoc.TrangThai) ||
                string.IsNullOrWhiteSpace(theBanDoc.MaTBD))
            {
                errorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc và đúng định dạng.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // Kiểm tra trùng mã bạn đọc
                    if (db.tBanDocs.Any(x => x.MaBD == banDoc.MaBD))
                    {
                        errorMessage = "Mã bạn đọc đã tồn tại.";
                        return false;
                    }

                    // Kiểm tra trùng mã thẻ
                    if (db.tTheBanDocs.Any(x => x.MaTBD == theBanDoc.MaTBD))
                    {
                        errorMessage = "Mã thẻ đã tồn tại.";
                        return false;
                    }

                    // Thêm bạn đọc và thẻ
                    db.tBanDocs.InsertOnSubmit(banDoc);
                    db.tTheBanDocs.InsertOnSubmit(theBanDoc);

                    // SubmitChanges tự động thực hiện transaction
                    db.SubmitChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi thêm bạn đọc: " + ex.Message;
                return false;
            }
        }


        public bool SuaBanDoc(tBanDoc banDoc, tTheBanDoc theBanDoc, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(banDoc.MaBD) ||
                string.IsNullOrWhiteSpace(banDoc.HoDem) ||
                string.IsNullOrWhiteSpace(banDoc.Ten) ||
                (banDoc.GioiTinh != 'M' && banDoc.GioiTinh != 'F') ||
                string.IsNullOrWhiteSpace(banDoc.SDT) ||
                string.IsNullOrWhiteSpace(banDoc.Email) ||
                theBanDoc.NgayCap == default ||
                string.IsNullOrWhiteSpace(theBanDoc.TrangThai))
            {
                errorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc và đúng định dạng.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // LINQ to SQL tự wrap SubmitChanges() trong transaction
                    var bd = db.tBanDocs.FirstOrDefault(x => x.MaBD == banDoc.MaBD);
                    var tbd = db.tTheBanDocs.FirstOrDefault(x => x.MaBD == banDoc.MaBD);

                    if (bd == null)
                    {
                        errorMessage = "Bạn đọc không tồn tại!";
                        return false;
                    }
                    if (tbd == null)
                    {
                        errorMessage = "Thẻ bạn đọc không tồn tại!";
                        return false;
                    }

                    // Cập nhật thông tin bạn đọc
                    bd.HoDem = banDoc.HoDem;
                    bd.Ten = banDoc.Ten;
                    bd.GioiTinh = banDoc.GioiTinh;
                    bd.NgaySinh = banDoc.NgaySinh;
                    bd.DiaChi = banDoc.DiaChi;
                    bd.SDT = banDoc.SDT;
                    bd.Email = banDoc.Email;

                    // Cập nhật thông tin thẻ
                    tbd.NgayCap = theBanDoc.NgayCap;
                    tbd.NgayHetHan = theBanDoc.NgayHetHan;
                    tbd.TrangThai = theBanDoc.TrangThai;
                    tbd.MaTK = theBanDoc.MaTK;

                    db.SubmitChanges(); // Auto transaction
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi cập nhật bạn đọc: " + ex.Message;
                return false;
            }
        }

        public bool XoaBanDoc(string maBanDoc, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(maBanDoc))
            {
                errorMessage = "Mã bạn đọc không hợp lệ.";
                return false;
            }

            try
            {
                using (var db = new QLThuVienDataContext())
                {
                    // Lấy thẻ và bạn đọc
                    var tbd = db.tTheBanDocs.FirstOrDefault(x => x.MaBD == maBanDoc);
                    var bd = db.tBanDocs.FirstOrDefault(x => x.MaBD == maBanDoc);

                    if (tbd == null && bd == null)
                    {
                        errorMessage = "Bạn đọc không tồn tại!";
                        return false;
                    }

                    // Xóa thẻ trước, sau đó xóa bạn đọc
                    if (tbd != null)
                        db.tTheBanDocs.DeleteOnSubmit(tbd);

                    if (bd != null)
                        db.tBanDocs.DeleteOnSubmit(bd);

                    db.SubmitChanges(); // Auto transaction
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Lỗi khi xóa bạn đọc: " + ex.Message;
                return false;
            }
        }
    }
}
