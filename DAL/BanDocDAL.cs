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
        public List<BanDocDTO> GetAllBanDocDTO() 
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bd in db.tBanDocs
                            select new BanDocDTO
                            {
                                MaBD = bd.MaBD,
                                HoDem = bd.HoDem, 
                                Ten = bd.Ten,     
                                GioiTinh = bd.GioiTinh.ToString(), 
                                NgaySinh = bd.NgaySinh,
                                DiaChi = bd.DiaChi,
                                SDT = bd.SDT,
                                Email = bd.Email
                            };

                return query.ToList();
            }
        }

        public BanDocDTO GetBanDocByMaBD(string maBD) 
        {
            using (var db = new QLThuVienDataContext())
            {
                var bd = db.tBanDocs.SingleOrDefault(p => p.MaBD == maBD);
                if (bd != null)
                {
                    return new BanDocDTO
                    {
                        MaBD = bd.MaBD,
                        HoDem = bd.HoDem,
                        Ten = bd.Ten,
                        GioiTinh = bd.GioiTinh.ToString(),
                        NgaySinh = bd.NgaySinh,
                        DiaChi = bd.DiaChi,
                        SDT = bd.SDT,
                        Email = bd.Email
                    };
                }
                return null;
            }
        }
    }
}
