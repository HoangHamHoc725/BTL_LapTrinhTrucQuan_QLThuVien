using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLTaiLieu
{
    public partial class ucFrmQuanLyTaiLieu : UserControl
    {
        private Button currentActiveButton;

        public ucFrmQuanLyTaiLieu(string tabName = "TaiLieu")
        {
            InitializeComponent();

            // Kiểm tra tham số để mở đúng tab
            if (tabName == "DanhMuc")
            {
                // Mở tab Danh mục
                btnThongTinDanhMuc_Click(btnThongTinDanhMuc, EventArgs.Empty);
            }
            else
            {
                // Mặc định mở tab Tài liệu
                btnThongTinTaiLieu_Click(btnThongTinTaiLieu, EventArgs.Empty);
            }
        }

        private void ucFrmQuanLyTaiLieu_Load(object sender, EventArgs e)
        {
            //btnThongTinTaiLieu_Click(btnThongTinTaiLieu, EventArgs.Empty);
        }

        private void LoadSubUserControl(UserControl uc)
        {
            this.pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            this.pnlContent.Controls.Add(uc);
            uc.BringToFront();
        }
        private void btnThongTinTaiLieu_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmThongTinTaiLieu());
            SetActiveButton(btnThongTinTaiLieu);
        }

        private void btnThongTinDanhMuc_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmThongTinDanhMuc());
            SetActiveButton(btnThongTinDanhMuc);
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
