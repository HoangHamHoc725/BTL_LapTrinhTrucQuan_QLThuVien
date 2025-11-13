using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLPhanQuyen
{
    public partial class ucFrmQuanLyPhanQuyen : UserControl
    {
        private Button currentActiveButton;

        public ucFrmQuanLyPhanQuyen(string tabName = "NhanVien")
        {
            InitializeComponent();

            // Xử lý điều hướng ngay khi khởi tạo
            if (tabName == "TaiKhoan")
            {
                btnThongTinTaiKhoan_Click(btnThongTinTaiKhoan, EventArgs.Empty);
            }
            else
            {
                // Mặc định (hoặc nếu tabName == "NhanVien")
                btnThongTinNhanVien_Click(btnThongTinNhanVien, EventArgs.Empty);
            }
        }

        private void ucFrmQuanLyPhanQuyen_Load(object sender, EventArgs e)
        {
            //btnThongTinNhanVien_Click(btnThongTinNhanVien, EventArgs.Empty);
        }

        private void LoadSubUserControl(UserControl uc)
        {
            this.pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            this.pnlContent.Controls.Add(uc);
            uc.BringToFront();
        }
        private void btnThongTinNhanVien_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmThongTinNhanVien());
            SetActiveButton(btnThongTinNhanVien);
        }

        private void btnThongTinTaiKhoan_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmThongTinTaiKhoan());
            SetActiveButton(btnThongTinTaiKhoan);
        }

        private void SetActiveButton(Button activeButton)
        {
            string newButtonTag = activeButton.Tag as string;

            if (currentActiveButton != null)
            {
                // Reset trạng thái nút cũ
                currentActiveButton.BackColor = Color.FromArgb(48, 52, 129);
                currentActiveButton.ForeColor = Color.FromArgb(245, 245, 245);
            }

            currentActiveButton = activeButton;

            currentActiveButton.BackColor = Color.FromArgb(37, 40, 106);
            currentActiveButton.ForeColor = Color.FromArgb(255, 242, 0);
        }
    }
}
