using LibraryManagerApp.BLL;
using LibraryManagerApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.TrangChu
{
    public partial class ucFrmTrangChu : UserControl
    {
        private readonly TaiLieuBLL _taiLieuBLL = new TaiLieuBLL();
        private readonly BanDocBLL _banDocBLL = new BanDocBLL();
        private readonly GiaoDichBLL _giaoDichBLL = new GiaoDichBLL();

        private Timer _timer;
        public ucFrmTrangChu()
        {
            InitializeComponent();
            this.Load += UcFrmTrangChu_Load;
        }
        private void UcFrmTrangChu_Load(object sender, EventArgs e)
        {
            SetupTimer();
            UpdateUI(); // Cập nhật ngay khi load
            LoadThongKe();
        }
        private void LoadThongKe()
        {
            try
            {
                // 1. Tổng số sách (Đầu sách)
                int tongSach = _taiLieuBLL.DemTongDauSach();
                lblSoLuongSach.Text = tongSach.ToString("N0");

                // 2. Bạn đọc (Mới hoặc Tổng)
                int banDoc = _banDocBLL.DemBanDocMoiTrongThang();
                lblSoLuongBanDoc.Text = banDoc.ToString("N0");

                // 3. Mượn trả & Quá hạn
                int dangMuon = 0;
                int quaHan = 0;

                // Gọi hàm out từ GiaoDichBLL
                _giaoDichBLL.LayThongKeMuonTra(out dangMuon, out quaHan);

                lblSoLuongDangMuon.Text = dangMuon.ToString("N0");
                lblSoLuongQuaHan.Text = quaHan.ToString("N0");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                Console.WriteLine(ex.Message);
            }
        }
        private void SetupTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            // 1. Cập nhật đồng hồ
            lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            // 2. Lấy tên người dùng trực tiếp từ SessionManager
            string tenHienThi = "Bạn"; // Mặc định

            // Kiểm tra xem user đã đăng nhập chưa
            if (SessionManager.CurrentUser != null)
            {
                // Giả sử trong SessionManager có lưu property CurrentUser là DTO
                tenHienThi = SessionManager.CurrentUser.HoTenNV;
            }

            // 3. Cập nhật lời chào
            int hour = DateTime.Now.Hour;
            string greeting = "";

            if (hour >= 5 && hour < 11) greeting = "Chào buổi sáng";
            else if (hour >= 11 && hour < 13) greeting = "Chào buổi trưa";
            else if (hour >= 13 && hour < 18) greeting = "Chào buổi chiều";
            else greeting = "Chào buổi tối";

            lblGreeting.Text = $"{greeting}, {tenHienThi}!";
        }
        private void lblGreeting_Click(object sender, EventArgs e)
        {

        }

        private frmMain GetFrmMain()
        {
            return this.ParentForm as frmMain;
        }
        private void btnBanDoc_Click(object sender, EventArgs e)
        {
            var main = GetFrmMain();
            if (main != null) main.MoTabQuanLyBanDoc();
        }
        private void btnTheBanDoc_Click(object sender, EventArgs e)
        {
            var main = this.ParentForm as frmMain;
            if (main != null) main.MoTabTheBanDoc();
        }
        private void btnTaiLieu_Click(object sender, EventArgs e)
        {
            var main = GetFrmMain();
            if (main != null) main.MoTabQuanLyTaiLieu();
        }
        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            var main = this.ParentForm as frmMain;
            if (main != null) main.MoTabDanhMuc();
        }
        private void btnMuonTra_Click(object sender, EventArgs e)
        {
            var main = GetFrmMain();
            if (main != null) main.MoTabQuanLyMuonTra();
        }
        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            var main = this.ParentForm as frmMain;
            if (main != null) main.MoTabQuanLyNhanVien();
        }
        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {
            var main = this.ParentForm as frmMain;
            if (main != null) main.MoThongTinTaiKhoan();
        }
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            var main = GetFrmMain();
            if (main != null) main.MoTabThongKeBaoCao();
        }
    }
}

