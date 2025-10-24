using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.DTO
{
    internal class AccountDTO
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }

        public AccountDTO() { }

        public AccountDTO(string tenDangNhap, string matKhau)
        {
            this.TenDangNhap = tenDangNhap;
            this.MatKhau = matKhau;
        }
    }
}
