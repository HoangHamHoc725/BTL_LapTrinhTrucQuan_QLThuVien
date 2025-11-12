using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerApp.GUI.UserControls.QLMuonTra
{
    public partial class ucFrmQuanLyMuonTra : UserControl
    {
        private Button currentActiveButton;

        public ucFrmQuanLyMuonTra()
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
    }
}
