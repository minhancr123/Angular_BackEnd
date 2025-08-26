namespace AngularBackEnd.Models.InventoryManagement
{
    public class  ItemDTO
    {
        public int? IdMH { get; set; }
        public string MaHang { get; set; }
        public string TenMatHang { get; set; }
        public int? IdLMH { get; set; }
        public int? IdDVT { get; set; }
        public decimal? GiaMua { get; set; }
        public decimal? GiaBan { get; set; }
        public double? VAT { get; set; }
        public string Barcode { get; set; }
        public bool? NgungKinhDoanh { get; set; }
        public string TenOnSite { get; set; }
        public string HinhAnh { get; set; }
        public string Mota { get; set; }
        public string ChiTietMoTa { get; set; }
        public string MaPhu { get; set; }
        public string KichThuoc { get; set; }
        public string ThongSo { get; set; }
        public bool? TheoDoiTonKho { get; set; }
        public bool? TheodoiLo { get; set; }
        public string MaLuuKho { get; set; }
        public string MaViTriKho { get; set; }
        public int? SoKyTinhKhauHaoToiThieu { get; set; }
        public int? SoKyTinhKhauHaoToiDa { get; set; }
        public bool? IsTaiSan { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
