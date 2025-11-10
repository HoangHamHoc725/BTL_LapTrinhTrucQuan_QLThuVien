using LibraryManagerApp.GUI.UserControls;
using LibraryManagerApp.GUI.UserControls.QLBanDoc;
using LibraryManagerApp.GUI.UserControls.QLMuonTra;
using LibraryManagerApp.GUI.UserControls.QLPhanQuyen;
using LibraryManagerApp.GUI.UserControls.QLTaiLieu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp
{
    public partial class frmMain : Form
    {
        private Button currentActiveButton;

        public frmMain()
        {
            InitializeComponent();

            btnTrangChu.Tag = "trangchu";
            btnQLBanDoc.Tag = "bandoc";
            btnQLTaiLieu.Tag = "tailieu";
            btnQLMuonTra.Tag = "muontra";
            btnQLPhanQuyen.Tag = "phanquyen";
            btnThongKeBaoCao.Tag = "thongke";
        }

        private void SetActiveButton(Button activeButton)
        {
            string newButtonTag = activeButton.Tag as string;

            if (currentActiveButton != null)
            {
                string oldButtonTag = currentActiveButton.Tag as string;

                // Reset trạng thái nút cũ
                currentActiveButton.BackColor = Color.FromArgb(48, 52, 129);
                currentActiveButton.ForeColor = Color.FromArgb(255, 242, 0);

                if (!string.IsNullOrEmpty(oldButtonTag))
                {
                    // Tên hình ảnh cũ: "icon_{Tag}_default"
                    currentActiveButton.Image = (Image)Properties.Resources.ResourceManager.GetObject($"icon_{oldButtonTag}");
                }
            }

            currentActiveButton = activeButton;

            currentActiveButton.BackColor = Color.FromArgb(214, 230, 242); // Màu nền Active (ví dụ màu nổi bật)
            currentActiveButton.ForeColor = Color.FromArgb(48, 52, 129); // Màu chữ Active

            if (!string.IsNullOrEmpty(newButtonTag))
            {
                // Tên hình ảnh mới: "icon_{Tag}_active"
                currentActiveButton.Image = (Image)Properties.Resources.ResourceManager.GetObject($"icon_{newButtonTag}_active");
            }
        }

        private void LoadUserControl(UserControl uc)
        {
            pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(uc);
            uc.BringToFront();
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnTrangChu);
        }

        private void btnQLBanDoc_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnQLBanDoc);
            LoadUserControl(new ucFrmQuanLyBanDoc());
        }

        private void btnQLTaiLieu_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnQLTaiLieu);
            LoadUserControl(new ucFrmQuanLyTaiLieu());
        }

        private void btnQLMuonTra_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnQLMuonTra);
            LoadUserControl(new ucFrmQuanLyMuonTra());
        }

        private void btnQLPhanQuyen_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnQLPhanQuyen);
            LoadUserControl(new ucFrmQuanLyPhanQuyen());
        }

        private void btnThongKeBaoCao_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnThongKeBaoCao);
        }
    }
}
