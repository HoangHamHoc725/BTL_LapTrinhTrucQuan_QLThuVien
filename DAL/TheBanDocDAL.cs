using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DAL
{
    internal class TheBanDocDAL
    {
        #region: LẤY THÔNG TIN
        // Hàm READ chính: Lấy tất cả Thẻ Bạn Đọc kèm thông tin liên quan
        public List<TheBanDocDTO> GetAllTheBanDocDTO()
        {
            using (var db = new QLThuVienDataContext())
            {
                // Truy vấn phức tạp: JOIN tTheBanDoc, tBanDoc, tTaiKhoan, tNhanVien
                var query = from tbd in db.tTheBanDocs
                            join bd in db.tBanDocs on tbd.MaBD equals bd.MaBD
                            join tk in db.tTaiKhoans on tbd.MaTK equals tk.MaTK
                            join nv in db.tNhanViens on tk.MaNV equals nv.MaNV
                            select new TheBanDocDTO
                            {
                                MaTBD = tbd.MaTBD,
                                MaBD = tbd.MaBD,
                                MaTK = tbd.MaTK,
                                NgayCap = tbd.NgayCap,
                                NgayHetHan = tbd.NgayHetHan,
                                TrangThai = tbd.TrangThai,

                                HoTenBD = bd.HoDem + " " + bd.Ten,
                                HoTenNV = nv.HoDem + " " + nv.Ten
                            };

                return query.ToList();
            }
        }

        // Hàm READ Chi tiết (dùng cho CellClick)
        public TheBanDocDTO GetTheBanDocByMaTBD(string maTBD)
        {
            // Tối ưu: Chỉ truy vấn bản ghi cụ thể nếu có nhiều dữ liệu. 
            // Hiện tại, ta dùng GetAllTheBanDocDTO().SingleOrDefault() để đơn giản
            return GetAllTheBanDocDTO().SingleOrDefault(p => p.MaTBD == maTBD);
        }

        // 1. Hàm gọi Stored Procedure SP_GenerateNewMaTBD
        public string GenerateNewMaTBD(string maBD, out int errorStatus)
        {
            using (var db = new QLThuVienDataContext())
            {
                string newMaTBD = string.Empty;

                // >>> KHẮC PHỤC LỖI: Sử dụng int? (Nullable int)
                int? nullableErrorStatus = -1;

                try
                {
                    // Giả định SP_GenerateNewMaTBD có tham số OUTPUT thứ 3 là ref string và ref int?
                    db.SP_GenerateNewMaTBD(maBD, ref newMaTBD, ref nullableErrorStatus);

                    // Chuyển kết quả từ int? sang int cho BLL
                    errorStatus = nullableErrorStatus ?? 99; // Nếu NULL, gán lỗi 99

                    return newMaTBD;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi gọi SP: " + ex.Message);
                    errorStatus = 99; // Lỗi hệ thống/SP
                    return string.Empty;
                }
            }
        }

        // Hàm Kiểm tra Thẻ (Cho chức năng Mượn)
        public TheBanDocDTO GetTheBanDocForMuon(string maTBD)
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from tbd in db.tTheBanDocs
                            join bd in db.tBanDocs on tbd.MaBD equals bd.MaBD
                            where tbd.MaTBD == maTBD
                            select new TheBanDocDTO
                            {
                                MaTBD = tbd.MaTBD,
                                HoTenBD = bd.HoDem + " " + bd.Ten,
                                TrangThai = tbd.TrangThai
                            };

                return query.SingleOrDefault(); // Trả về DTO hoặc null
            }
        }
        #endregion

        #region NHẬP SỬA XÓA
        // 2. Hàm Insert (CREATE)
        public bool InsertTheBanDoc(TheBanDocDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                // Tạo một đối tượng tTheBanDoc từ DTO (chỉ lấy các trường thô)
                tTheBanDoc newThe = new tTheBanDoc
                {
                    MaTBD = model.MaTBD,
                    MaBD = model.MaBD,
                    MaTK = model.MaTK,
                    NgayCap = model.NgayCap,
                    NgayHetHan = model.NgayHetHan,
                    TrangThai = model.TrangThai
                };

                db.tTheBanDocs.InsertOnSubmit(newThe);

                try
                {
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    // Lỗi DB (trùng Mã TBD, sai FK,...)
                    Console.WriteLine("Lỗi khi thêm Thẻ Bạn Đọc: " + ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateTheBanDoc(TheBanDocDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                // 1. Tìm bản ghi cần cập nhật
                tTheBanDoc existingThe = db.tTheBanDocs.SingleOrDefault(tbd => tbd.MaTBD == model.MaTBD);

                if (existingThe != null)
                {
                    // 2. Cập nhật các thuộc tính (Không cập nhật MaTBD và MaBD)
                    existingThe.MaTK = model.MaTK;
                    existingThe.NgayCap = model.NgayCap;
                    existingThe.NgayHetHan = model.NgayHetHan; // Ngày hết hạn có thể được sửa đổi thủ công
                    existingThe.TrangThai = model.TrangThai;

                    try
                    {
                        // 3. Thực hiện lưu thay đổi vào DB
                        db.SubmitChanges();
                        return true; // Cập nhật thành công
                    }
                    catch (Exception ex)
                    {
                        // Lỗi DB (Ví dụ: MaTK không hợp lệ, Ngày Hết Hạn < Ngày Cấp)
                        Console.WriteLine("Lỗi khi cập nhật Thẻ Bạn Đọc: " + ex.Message);
                        return false;
                    }
                }
                return false; // Không tìm thấy Mã TBD
            }
        }

        public bool DeleteTheBanDoc(string maTBD)
        {
            using (var db = new QLThuVienDataContext())
            {
                // 1. Tìm bản ghi cần xóa
                tTheBanDoc theToDelete = db.tTheBanDocs.SingleOrDefault(tbd => tbd.MaTBD == maTBD);

                if (theToDelete != null)
                {
                    // 2. Thực hiện xóa khỏi bảng
                    db.tTheBanDocs.DeleteOnSubmit(theToDelete);

                    try
                    {
                        // 3. Thực hiện lưu thay đổi vào DB
                        db.SubmitChanges();
                        return true; // Xóa thành công
                    }
                    catch (Exception ex)
                    {
                        // Lỗi ràng buộc khóa ngoại (ví dụ: Thẻ đang có phiếu mượn)
                        Console.WriteLine("Lỗi khi xóa Thẻ Bạn Đọc: " + ex.Message);
                        return false;
                    }
                }
                return false; // Không tìm thấy Mã TBD
            }
        }
        #endregion

        #region TÌM KIẾM
        // Hàm Tìm kiếm Thẻ Bạn Đọc
        public List<TheBanDocDTO> SearchTheBanDoc(List<SearchFilter> filters)
        {
            using (var db = new QLThuVienDataContext()) // Giả sử QLThuVienDataContext
            {
                IQueryable<tTheBanDoc> query = db.tTheBanDocs.AsQueryable();

                foreach (var filter in filters)
                {
                    string fieldName = filter.FieldName;
                    string op = filter.Operator;
                    string value = filter.Value;
                    string valueTo = filter.ValueTo;

                    // 1. Xử lý các trường kiểu chuỗi
                    if (fieldName == "MaTBD")
                    {
                        if (op == "=") query = query.Where(tbd => tbd.MaTBD == value);
                        else if (op == "LIKE") query = query.Where(tbd => tbd.MaTBD.Contains(value));
                        else if (op == "Bắt đầu bằng") query = query.Where(tbd => tbd.MaTBD.StartsWith(value));
                    }
                    else if (fieldName == "MaBD")
                    {
                        if (op == "=") query = query.Where(tbd => tbd.MaBD == value);
                        else if (op == "LIKE") query = query.Where(tbd => tbd.MaBD.Contains(value));
                        else if (op == "Bắt đầu bằng") query = query.Where(tbd => tbd.MaBD.StartsWith(value));
                    }
                    else if (fieldName == "MaTK" && op == "=")
                    {
                        query = query.Where(tbd => tbd.MaTK == value);
                    }
                    else if (fieldName == "TrangThai" && op == "=")
                    {
                        query = query.Where(tbd => tbd.TrangThai == value);
                    }

                    // 2. Xử lý các trường Ngày tháng
                    else if (fieldName == "NgayCap" && DateTime.TryParse(value, out DateTime dtValue))
                    {
                        DateTime dtEnd;
                        if (op == "=")
                            query = query.Where(tbd => tbd.NgayCap.Date == dtValue.Date);
                        else if (op == ">")
                            query = query.Where(tbd => tbd.NgayCap > dtValue);
                        // ... (Thêm các toán tử <, >=, <=)
                        else if (DateTime.TryParse(valueTo, out dtEnd))
                        {
                            DateTime dtStart = dtValue;
                            if (op == "Khoảng")
                                query = query.Where(tbd => tbd.NgayCap > dtStart && tbd.NgayCap < dtEnd);
                            else if (op == "Đoạn")
                                query = query.Where(tbd => tbd.NgayCap >= dtStart && tbd.NgayCap <= dtEnd);
                        }
                    }
                    else if (fieldName == "NgayHetHan" && DateTime.TryParse(value, out DateTime dtValueHetHan))
                    {
                        DateTime dtEnd;
                        if (op == "=")
                            query = query.Where(tbd => tbd.NgayHetHan.Value.Date == dtValueHetHan.Date);
                        else if (op == ">")
                            query = query.Where(tbd => tbd.NgayHetHan > dtValueHetHan);
                        // ... (Thêm các toán tử <, >=, <=)
                        else if (DateTime.TryParse(valueTo, out dtEnd))
                        {
                            DateTime dtStart = dtValueHetHan;
                            if (op == "Khoảng")
                                query = query.Where(tbd => tbd.NgayHetHan > dtStart && tbd.NgayHetHan < dtEnd);
                            else if (op == "Đoạn")
                                query = query.Where(tbd => tbd.NgayHetHan >= dtStart && tbd.NgayHetHan <= dtEnd);
                        }
                    }
                }

                // 3. Thực hiện JOIN và MAP ra DTO đầy đủ (HoTenBD, TenDangNhapTK, v.v.)
                var finalQuery = from tbd in query
                                 join bd in db.tBanDocs on tbd.MaBD equals bd.MaBD
                                 join tk in db.tTaiKhoans on tbd.MaTK equals tk.MaTK
                                 select new TheBanDocDTO
                                 {
                                     MaTBD = tbd.MaTBD,
                                     MaBD = tbd.MaBD,
                                     MaTK = tbd.MaTK,
                                     NgayCap = tbd.NgayCap,
                                     NgayHetHan = tbd.NgayHetHan,
                                     TrangThai = tbd.TrangThai,

                                     // Thông tin JOIN
                                     HoTenBD = bd.HoDem + " " + bd.Ten
                                   
                                 };

                return finalQuery.ToList();
            }
        }
        #endregion
    }
}
