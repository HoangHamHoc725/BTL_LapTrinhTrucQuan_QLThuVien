using LibraryManagerApp.DAL;
using LibraryManagerApp.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagerApp.BLL
{
    internal class TheBanDocBLL
    {
        private TheBanDocDAL _dal = new TheBanDocDAL();
        private BanDocDAL _bdDal = new BanDocDAL();

        public List<TheBanDocDTO> LayThongTinTheBanDoc()
        {
            return _dal.GetAllTheBanDocDTO();
        }

        public TheBanDocDTO LayChiTietTheBanDoc(string maTBD)
        {
            return _dal.GetTheBanDocByMaTBD(maTBD);
        }

        public List<BanDocChuaCoTheDTO> LayBanDocChuaCoThe()
        {
            return _bdDal.GetBanDocChuaCoThe();
        }
    }
}
