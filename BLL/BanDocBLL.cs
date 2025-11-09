using LibraryManagerApp.DAL;
using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.BLL
{
    internal class BanDocBLL
    {
        private BanDocDAL _dal = new BanDocDAL();

        public List<BanDocDTO> LayThongTinBanDoc() // Đổi tên phương thức cho rõ ràng
        {
            return _dal.GetAllBanDocDTO();
        }
    }
}
