using QuanLyThuVien.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class DanhMucTaiLieuBLL
    {
        private DanhMucTaiLieuDAL dal = new DanhMucTaiLieuDAL();

        public DataTable GetALlDanhMuc(string table, string maCol, string tenCol)
        {
            return dal.GetAllDanhMuc(table, maCol, tenCol);
        }
    }
}
