using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DAL
{
    internal class DinhDangDAL
    {
        public List<DinhDangDTO> GetAllDinhDangDTO()
        {
            using (var db = new QLThuVienDataContext())
            {
                return db.tDinhDangs.Select(d => new DinhDangDTO { MaDD = d.MaDD, TenDD = d.TenDD }).ToList();
            }
        }

        // Hàm Read Detail
        public DinhDangDTO GetDinhDangByMaDD(string maDD)
        {
            using (var db = new QLThuVienDataContext())
            {
                var dd = db.tDinhDangs.SingleOrDefault(d => d.MaDD == maDD);
                if (dd != null) return new DinhDangDTO { MaDD = dd.MaDD, TenDD = dd.TenDD };
                return null;
            }
        }

        // Hàm gọi SP GenerateNewMaDD
        public string GenerateNewMaDD()
        {
            string newMaDD = string.Empty;
            using (var db = new QLThuVienDataContext())
            {
                try { db.SP_GenerateNewMaDD(ref newMaDD); return newMaDD; }
                catch (Exception ex) { throw new Exception("Lỗi khi sinh Mã Định dạng. Chi tiết: " + ex.Message); }
            }
        }

        // Hàm Insert
        public bool InsertDinhDang(DinhDangDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tDinhDang newDD = new tDinhDang { MaDD = model.MaDD, TenDD = model.TenDD };
                db.tDinhDangs.InsertOnSubmit(newDD);
                try { db.SubmitChanges(); return true; }
                catch (Exception ex) { return false; }
            }
        }

        // Hàm Update
        public bool UpdateDinhDang(DinhDangDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tDinhDang existingDD = db.tDinhDangs.SingleOrDefault(d => d.MaDD == model.MaDD);
                if (existingDD != null)
                {
                    existingDD.TenDD = model.TenDD;
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; }
                }
                return false;
            }
        }

        // Hàm Delete
        public bool DeleteDinhDang(string maDD)
        {
            using (var db = new QLThuVienDataContext())
            {
                tDinhDang ddToDelete = db.tDinhDangs.SingleOrDefault(d => d.MaDD == maDD);
                if (ddToDelete != null)
                {
                    db.tDinhDangs.DeleteOnSubmit(ddToDelete);
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; } // Lỗi do ràng buộc khóa ngoại
                }
                return false;
            }
        }
    }
}
