// File: QuanLyThuVien.BLL/DanhMucTaiLieuBLL.cs

using QuanLyThuVien.DAL;
using QuanLyThuVien.DTO;
using QuanLyThuVien.Helpers;
using System.Collections.Generic;

namespace QuanLyThuVien.BLL
{
    internal class DanhMucTaiLieuBLL
    {
        private DanhMucTaiLieuDAL dal = new DanhMucTaiLieuDAL();

        public List<DanhMucTaiLieuDTO> GetALlDanhMuc(string table)
        {
            return dal.GetAllDanhMuc(table);
        }

        public bool ThemDanhMuc(string table, DanhMucTaiLieuDTO dto, out string errorMessage)
        {
            return dal.ThemDanhMuc(table, dto, out errorMessage);
        }

        public bool SuaDanhMuc(string table, DanhMucTaiLieuDTO dto, out string errorMessage)
        {
            return dal.SuaDanhMuc(table, dto, out errorMessage);
        }

        public bool XoaDanhMuc(string table, string ma, out string errorMessage)
        {
            return dal.XoaDanhMuc(table, ma, out errorMessage);
        }

        public List<DanhMucTaiLieuDTO> TimKiemDanhMuc(string table, List<Filter> filters)
        {
            return dal.TimKiemDanhMuc(table, filters);
        }
    }
}