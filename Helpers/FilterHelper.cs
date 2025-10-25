using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.Helpers
{
    internal class FilterHelper
    {
        public static Dictionary<string, string> GetFilterAttributes(string module)
        {
            switch (module)
            {
                case "BanDoc":
                    return new Dictionary<string, string>
                    {
                        { "Mã bạn đọc", "MaBD" },
                        { "Họ và tên", "HoTen" },
                        { "Ngày sinh", "NgaySinh" },
                        { "Giới tính", "GioiTinh" },
                        { "Địa chỉ", "DiaChi" },
                        { "Số điện thoại", "SDT" },
                        { "Email", "Email" },
                        { "Mã thẻ bạn đọc", "MaTBD" },
                        { "Ngày cấp thẻ", "NgayCap" },
                        { "Ngày hết hạn thẻ", "NgayHetHan" },
                        { "Trạng thái thẻ", "TrangThai" },
                        { "Mã tài khoản cấp thẻ", "MaTK" }
                    };

                case "TaiLieu": 
                    return new Dictionary<string, string>
                    {
                        { "Mã tài liệu", "MaTL" },
                        { "Tên tài liệu", "TenTL" },
                        { "Nhà xuất bản", "TenNXB" },
                        { "Ngôn ngữ", "TenNN" },
                        { "Thể loại", "TenTheLoai" },
                        { "Định dạng", "TenDinhDang" },
                        { "Năm xuất bản", "NamXuatBan" },
                        { "Lần xuất bản", "LanXuatBan" },
                        { "Số trang", "SoTrang" },
                        { "Khổ cỡ", "KhoCo" },
                        { "Tên Tác giả", "TenTacGia" } 
                    };

                case "DanhMuc": 
                    return new Dictionary<string, string>
                    {
                        { "Mã", "Ma" },
                        { "Tên/Họ tên", "Ten" } 
                    };

                default:
                    return new Dictionary<string, string>();
            }
        }
    }
}
