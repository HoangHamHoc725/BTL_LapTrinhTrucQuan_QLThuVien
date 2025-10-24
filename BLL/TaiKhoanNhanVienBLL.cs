using QuanLyThuVien.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class TaiKhoanNhanVienBLL
    {
        private TaiKhoanNhanVienDAL taiKhoanNhanVienDAL = new TaiKhoanNhanVienDAL();

        public DataTable GetAllTaiKhoanNhanVien()
        {
            return taiKhoanNhanVienDAL.GetAllTaiKhoanNhanVien();
        }
    }
}
