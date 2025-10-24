using QuanLyThuVien.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien.GUI.UserControls
{
    public partial class ucFrmQLTaiLieu : UserControl
    {
        private QLTaiLieuBLL QLTaiLieuBLL = new QLTaiLieuBLL(); 

        public ucFrmQLTaiLieu()
        {
            InitializeComponent();
        }

        private void ucFrmQLTaiLieu_Load(object sender, EventArgs e)
        {
            dgvQLTaiLieu.DataSource = QLTaiLieuBLL.GetAllTaiLieu();
        }


    }
}
