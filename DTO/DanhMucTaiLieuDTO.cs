namespace QuanLyThuVien.DTO
{
    public class DanhMucTaiLieuDTO
    {
        public string Ma { get; set; }
        public string HoDem { get; set; } // Giữ lại cho DAL/Database
        public string Ten { get; set; }   // Dùng để chứa Tên hoặc Họ và Tên khi nhập liệu

        public string TenDayDu
        {
            get
            {
                // Logic hiển thị: Nếu có HoDem, kết hợp lại. Nếu không, dùng Ten
                if (!string.IsNullOrEmpty(HoDem) && !string.IsNullOrEmpty(Ten))
                    return $"{HoDem.Trim()} {Ten.Trim()}";
                return Ten;
            }
        }
    }
}