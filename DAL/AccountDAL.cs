using QuanLyThuVien.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.DAL
{
    internal class AccountDAL
    {
        private static List<AccountDTO> mockAccounts = new List<AccountDTO>
        {
            new AccountDTO("admin", "admin123"),
            new AccountDTO("user", "user123")
        };

        public bool CheckLogin(string tenDangNhap, string matKhau)
        {
            return mockAccounts.Any(acc => acc.TenDangNhap == tenDangNhap && acc.MatKhau == matKhau);
        }

        public bool Register(string tenDangNhap, string matKhau)
        {
            if (mockAccounts.Any(acc => acc.TenDangNhap == tenDangNhap))
            {
                return false; // Username already exists
            }
            mockAccounts.Add(new AccountDTO(tenDangNhap, matKhau));
            return true;
        }
    }
}
