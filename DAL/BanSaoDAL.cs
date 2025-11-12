using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DAL
{
    internal class BanSaoDAL
    {
        // 1. READ (List): Lấy tất cả bản sao của một Tài liệu
        public List<BanSaoDTO> GetBanSaoByMaTL(string maTL)
        {
            using (var db = new QLThuVienDataContext())
            {
                var query = from bs in db.tBanSaos
                            where bs.MaTL == maTL
                            select new BanSaoDTO
                            {
                                MaBS = bs.MaBS,
                                MaTL = bs.MaTL,
                                TrangThai = bs.TrangThai
                            };
                return query.ToList();
            }
        }

        // 2. READ (Detail): Lấy chi tiết một bản sao
        public BanSaoDTO GetBanSaoByMaBS(string maBS)
        {
            using (var db = new QLThuVienDataContext())
            {
                var bs = db.tBanSaos.SingleOrDefault(b => b.MaBS == maBS);
                if (bs != null)
                {
                    return new BanSaoDTO { MaBS = bs.MaBS, MaTL = bs.MaTL, TrangThai = bs.TrangThai };
                }
                return null;
            }
        }

        // 3. Generate (Gọi SP_GenerateNewMaBS)
        public string GenerateNewMaBS(string maTL)
        {
            using (var db = new QLThuVienDataContext())
            {
                string newMaBS = string.Empty;
                try
                {
                    // Giả định LINQ to SQL đã ánh xạ SP
                    db.SP_GenerateNewMaBS(maTL, ref newMaBS);
                    return newMaBS;
                }
                catch (Exception ex)
                {
                    // Bắt lỗi RAISERROR từ SP (ví dụ: Vượt quá giới hạn 999)
                    throw new Exception("Lỗi SP khi sinh mã Bản sao: " + ex.Message);
                }
            }
        }

        // 4. CREATE (Insert)
        public bool InsertBanSao(BanSaoDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tBanSao newBS = new tBanSao
                {
                    MaBS = model.MaBS, // Mã đã được sinh
                    MaTL = model.MaTL,
                    TrangThai = model.TrangThai
                };
                db.tBanSaos.InsertOnSubmit(newBS);
                try { db.SubmitChanges(); return true; }
                catch (Exception ex) { return false; }
            }
        }

        // 5. UPDATE
        public bool UpdateBanSao(BanSaoDTO model)
        {
            using (var db = new QLThuVienDataContext())
            {
                tBanSao existingBS = db.tBanSaos.SingleOrDefault(b => b.MaBS == model.MaBS);
                if (existingBS != null)
                {
                    existingBS.TrangThai = model.TrangThai;
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; }
                }
                return false;
            }
        }

        // 6. DELETE
        public bool DeleteBanSao(string maBS)
        {
            using (var db = new QLThuVienDataContext())
            {
                tBanSao bsToDelete = db.tBanSaos.SingleOrDefault(b => b.MaBS == maBS);
                if (bsToDelete != null)
                {
                    db.tBanSaos.DeleteOnSubmit(bsToDelete);
                    try { db.SubmitChanges(); return true; }
                    catch (Exception ex) { return false; } // Lỗi do ràng buộc (đang được mượn)
                }
                return false;
            }
        }
    }
}
