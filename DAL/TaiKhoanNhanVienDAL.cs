using QuanLyThuVien.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.DAL
{
    internal class TaiKhoanNhanVienDAL : DatabaseConnect
    {
        public DataTable GetAllTaiKhoanNhanVien()
        {
            string query = "SELECT * FROM vThongTinTaiKhoan";
            SqlDataAdapter da = new SqlDataAdapter(query, GetConnection());

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
