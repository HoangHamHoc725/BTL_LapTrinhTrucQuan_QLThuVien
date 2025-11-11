using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.Helpers
{
    // Lớp mô hình hóa Metadata của một trường tìm kiếm
    public class FieldMetadata
    {
        public string FieldName { get; set; }     // MaBD, HoDem, Ten, NgaySinh, v.v.
        public string DisplayName { get; set; }   // Mã Bạn Đọc, Họ Đệm, Tên, v.v.
        public TypeCode DataType { get; set; }    // String, DateTime, etc.
        public List<string> SupportedOperators { get; set; } // =, LIKE, >, <, v.v.
    }

    public static class SearchMetadata
    {
        // Khởi tạo các cấu hình tìm kiếm cho bảng Bạn Đọc
        public static List<FieldMetadata> GetBanDocFields()
        {
            return new List<FieldMetadata>
            {
                new FieldMetadata
                {
                    FieldName = "MaBD",
                    DisplayName = "Mã Bạn Đọc",
                    DataType = TypeCode.String,
                    SupportedOperators = new List<string> { "=", "LIKE", "Bắt đầu bằng" }
                },
                new FieldMetadata
                {
                    FieldName = "HoTen", // Kết hợp HoDem và Ten cho tìm kiếm tiện lợi
                    DisplayName = "Họ Tên",
                    DataType = TypeCode.String,
                    SupportedOperators = new List<string> { "LIKE", "Bắt đầu bằng" }
                },
                new FieldMetadata
                {
                    FieldName = "Email",
                    DisplayName = "Email",
                    DataType = TypeCode.String,
                    SupportedOperators = new List<string> { "=", "LIKE" }
                },
                new FieldMetadata
                {
                    FieldName = "DiaChi",
                    DisplayName = "Địa Chỉ",
                    DataType = TypeCode.String,
                    SupportedOperators = new List<string> { "=", "LIKE" }
                },
                new FieldMetadata
                {
                    FieldName = "SDT",
                    DisplayName = "Số Điện Thoại",
                    DataType = TypeCode.String,
                    SupportedOperators = new List<string> { "=" }
                },
                new FieldMetadata
                {
                    FieldName = "NgaySinh",
                    DisplayName = "Ngày Sinh",
                    DataType = TypeCode.DateTime,
                    SupportedOperators = new List<string> { "=", ">", "<", ">=", "<=", "Khoảng", "Đoạn" }
                }
            };
        }
    }
}
