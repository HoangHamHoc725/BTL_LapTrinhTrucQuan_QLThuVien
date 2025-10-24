using QuanLyThuVien.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class QLTaiLieuBLL
    {
        private QLTaiLieuDAL dal = new QLTaiLieuDAL();

        public DataTable GetAllTaiLieu()
        {
            return dal.GetAllTaiLieu();
        }
    }
}
