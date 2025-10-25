using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.DTO
{
    internal class TacGiaTaiLieuDTO
    {
        public string MaTG { get; set; }
        // TenTacGia hiển thị, chỉ đọc
        public string TenTacGia { get; set; }
        // VaiTro có thể sửa đổi (trực tiếp trên lưới)
        public string VaiTro { get; set; }
    }
}
