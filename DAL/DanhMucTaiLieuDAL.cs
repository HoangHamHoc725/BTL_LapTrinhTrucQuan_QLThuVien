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
    internal class DanhMucTaiLieuDAL : DatabaseConnect
    {
        public DataTable GetAllDanhMuc(string table, string maCol, string tenCol) 
        {
            string sql = $"SELECT {maCol} AS Ma, {tenCol} AS Ten FROM {table}";

            SqlDataAdapter da = new SqlDataAdapter(sql, GetConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
