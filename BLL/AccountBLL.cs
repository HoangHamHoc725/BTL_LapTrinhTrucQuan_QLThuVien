using QuanLyThuVien.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.BLL
{
    internal class AccountBLL
    {
        private AccountDAL accountDAL = new AccountDAL();

        public bool Login(string tenDangNhap, string matKhau)
        {
            return accountDAL.CheckLogin(tenDangNhap, matKhau);
        }

        public bool Register(string tenDangNhap, string matKhau)
        {
            return accountDAL.Register(tenDangNhap, matKhau);
        }
    }
}
