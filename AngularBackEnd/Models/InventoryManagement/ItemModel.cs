namespace AngularBackEnd.Models.InventoryManagement
{
    public class ItemModel
    {
        public int? IdMH { get; set; }
        public string MaHang { get; set; }
        public string TenMatHang { get; set; }
        public int? IdLMH { get; set; }
        public int? IdDVT { get; set; }
        public string? Mota { get; set; }
        public decimal? GiaMua { get; set; }
        public decimal? GiaBan { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDel { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public double? VAT { get; set; }
        public string? Barcode { get; set; }
        public bool? NgungKinhDoanh { get; set; }
        public int? IdDVTCap2 { get; set; }
        public decimal? QuyDoiDVTCap2 { get; set; }
        public int? IdDVTCap3 { get; set; }
        public decimal? QuyDoiDVTCap3 { get; set; }
        public string TenOnSite { get; set; }
        public int? IdNhanHieu { get; set; }
        public int? IdXuatXu { get; set; }
        public string? HinhAnh { get; set; }
        public string? ChiTietMoTa { get; set; }
        public string? MaPhu { get; set; }
        public string? KichThuoc { get; set; }
        public string? ThongSo { get; set; }
        public bool? TheoDoiTonKho { get; set; }
        public bool? TheodoiLo { get; set; }
        public string? MaLuuKho { get; set; }
        public string? MaViTriKho { get; set; }
        public int? UpperLimit { get; set; }
        public int? LowerLimit { get; set; }
        public bool? IsTaiSan { get; set; }
        public int? SoKyTinhKhauHaoToiThieu { get; set; }
        public int? SoKyTinhKhauHaoToiDa { get; set; }
        public int? SoNamDeNghi { get; set; }
        public double? TiLeHaoMon { get; set; }
    }
}
