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
    internal class QLTaiLieuDAL : DatabaseConnect
    {
        public DataTable GetAllTaiLieu()
        {
            string sql = "SELECT * FROM tTaiLieu";
            SqlDataAdapter da = new SqlDataAdapter(sql, GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
