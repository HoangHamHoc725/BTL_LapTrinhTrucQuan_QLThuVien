using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.Helpers
{
    internal class DatabaseConnect
    {
        private readonly string connectionString = @"Data Source=HOANGNGUYEN\SQLEXPRESS;Initial Catalog=QLThuVien;Integrated Security=True";

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
