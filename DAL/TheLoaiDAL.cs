using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DAL
{
    internal class TheLoaiDAL
    {
        public List<TheLoaiDTO> GetAllTheLoaiDTO()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tTheLoais.Select(t => new TheLoaiDTO { MaThL = t.MaThL, TenThL = t.TenThL }).ToList();
            }
        }

        // Hàm Read Detail
        public TheLoaiDTO GetTheLoaiByMaThL(string maThL)
        {
            using (var db = new QLThuVienDataContext())
            {
                var thl = db.tTheLoais.SingleOrDefault(t => t.MaThL == maThL);
                if (thl != null) return new TheLoaiDTO { MaThL = thl.MaThL, TenThL = thl.TenThL };
                return null;
            }
        }

        // Hàm gọi SP GenerateNewMaThL (Không có tham số đầu vào)
        public string GenerateNewMaThL()
        {
            string newMaThL = string.Empty;
            using (var db = new QLThuVienDataContext())
            {
                try { db.SP_GenerateNewMaThL(ref newMaThL); return newMaThL; }
                catch (Exception ex) { throw new Exception("Lỗi khi sinh Mã Thể loại. Chi tiết: " + ex.Message); }
            }
        }

        // Hàm Insert
        public bool InsertTheLoai(TheLoaiDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tTheLoai newThL = new tTheLoai { MaThL = model.MaThL, TenThL = model.TenThL };
                db.tTheLoais.InsertOnSubmit(newThL);
                try { db.SubmitChanges(); return true; }
                catch (Exception ex) { return false; }
            }
        }

        // Hàm Update
        public bool UpdateTheLoai(TheLoaiDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tTheLoai existingThL = db.tTheLoais.SingleOrDefault(t => t.MaThL == model.MaThL);
                if (existingThL != null)
                {
                    existingThL.TenThL = model.TenThL;
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; }
                }
                return false;
            }
        }

        // Hàm Delete
        public bool DeleteTheLoai(string maThL)
        {
            using (var db = new QLThuVienDataContext())
            {
                tTheLoai thlToDelete = db.tTheLoais.SingleOrDefault(t => t.MaThL == maThL);
                if (thlToDelete != null)
                {
                    db.tTheLoais.DeleteOnSubmit(thlToDelete);
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; } // Lỗi do ràng buộc khóa ngoại
                }
                return false;
            }
        }
    }
}
