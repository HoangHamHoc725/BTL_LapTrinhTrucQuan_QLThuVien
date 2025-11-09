using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.DTO
{
    internal class BanDocDTO
    {
        public string MaBD { get; set; }
        public string HoTen { get; set; } // (HoDem + Ten)
        public string GioiTinhHienThi { get; set; } // Nam/Nữ
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
    }
}
