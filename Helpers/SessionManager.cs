using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.Helpers
{
    internal class SessionManager
    {
        // Thuộc tính lưu trữ thông tin người dùng đã đăng nhập
        public static LoginSessionDTO CurrentUser { get; private set; }

        // Kiểm tra trạng thái đăng nhập
        public static bool IsLoggedIn
        {
            get { return CurrentUser != null; }
        }

        // Hàm thiết lập thông tin người dùng sau khi đăng nhập thành công
        public static void Login(LoginSessionDTO userSession)
        {
            if (userSession == null)
            {
                throw new ArgumentNullException(nameof(userSession), "Phiên người dùng không được rỗng.");
            }
            CurrentUser = userSession;
        }

        // Hàm xóa phiên làm việc khi người dùng Đăng xuất
        public static void Logout()
        {
            CurrentUser = null;
        }

        // Hàm lấy Mã Vai trò để kiểm tra phân quyền
        public static string GetMaVaiTro()
        {
            return CurrentUser?.MaVT;
        }

        // Hàm lấy Mã Tài khoản (rất quan trọng cho các thao tác CRUD)
        public static string GetMaTaiKhoan()
        {
            return CurrentUser?.MaTK;
        }
    }
}
