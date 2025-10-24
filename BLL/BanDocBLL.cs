using QuanLyThuVien.DAL;
using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class BanDocBLL
    {
        private BanDocDAL banDocDAL = new BanDocDAL();

        public List<object> LayTatCaThongTinBanDoc()
        {
            return banDocDAL.LayTatCaThongTinBanDoc();
        }

        public List<object> TimKiemBanDoc(List<Filter> filters)
        {
            return banDocDAL.TimKiemBanDoc(filters);
        }

        public bool ThemBanDoc(tBanDoc banDoc, tTheBanDoc theBanDoc, out string errorMessage)
        {
            return banDocDAL.ThemBanDoc(banDoc, theBanDoc, out errorMessage);
        }

        public bool SuaBanDoc(tBanDoc banDoc, tTheBanDoc theBanDoc, out string errorMessage)
        {
            return banDocDAL.SuaBanDoc(banDoc, theBanDoc, out errorMessage);
        }

        public bool XoaBanDoc(string maBanDoc, out string errorMessage)
        {
            return banDocDAL.XoaBanDoc(maBanDoc, out errorMessage);
        }
    }
}
