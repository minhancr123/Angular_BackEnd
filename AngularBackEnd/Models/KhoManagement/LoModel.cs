namespace JeeBeginner.Models.KhoManagement
{
    public class LoModel
    {
        public int IdLo { get; set; }
        public string MaLo { get; set; }
        public string TenLo { get; set; }
        public string NgayNhap { get; set; }
        public string NgaySanXuat { get; set; }
        public string HanSuDung { get; set; }

        public string NgayBaoHanh { get; set; }
        public bool TrangThai { get; set; } 
        //public int SoLuong { get; set; }
        //public string GhiChu { get; set; }
        //public long HangHoaID { get; set; }
        //public string HangHoaName { get; set; }
        //public string HangHoaCode { get; set; }
        //public bool IsActive { get; set; } = true;
    }
}
