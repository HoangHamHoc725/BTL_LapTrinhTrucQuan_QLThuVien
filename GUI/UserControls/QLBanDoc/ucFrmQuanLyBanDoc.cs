using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLBanDoc
{
    public partial class ucFrmQuanLyBanDoc : UserControl
    {
        private Button currentActiveButton;

        public ucFrmQuanLyBanDoc()
        {
            InitializeComponent();
        }

        private void LoadSubUserControl(UserControl uc)
        {
            this.pnlContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            this.pnlContent.Controls.Add(uc);
            uc.BringToFront();
        }

        private void btnThongTinBanDoc_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmThongTinBanDoc());
            SetActiveButton(btnThongTinBanDoc);
        }

        private void btnTheBanDoc_Click(object sender, EventArgs e)
        {
            LoadSubUserControl(new ucFrmTheBanDoc());
            SetActiveButton(btnTheBanDoc);
        }

        private void ucFrmQuanLyBanDoc_Load(object sender, EventArgs e)
        {
            btnThongTinBanDoc_Click(btnThongTinBanDoc, EventArgs.Empty);
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
