using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DAL
{
    internal class BanDocDAL
    {
        public List<BanDocDTO> GetAllBanDocDTO() // Đổi tên phương thức cho rõ ràng hơn
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bd in db.tBanDocs
                            select new BanDocDTO // Chiếu dữ liệu sang BanDocDTO
                            {
                                MaBD = bd.MaBD,
                                HoTen = bd.HoDem + " " + bd.Ten,
                                GioiTinhHienThi = bd.GioiTinh.Equals("M") ? "Nam" : "Nữ",
                                NgaySinh = bd.NgaySinh,
                                DiaChi = bd.DiaChi,
                                SDT = bd.SDT,
                                Email = bd.Email
                            };

                return query.ToList();
            }
        }
    }
}
