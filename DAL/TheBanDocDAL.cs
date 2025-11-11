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
    }
}
