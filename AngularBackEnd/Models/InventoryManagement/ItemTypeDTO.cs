namespace AngularBackEnd.Models.InventoryManagement
{
    public class ItemTypeDTO
    {
        public int IdLMH { get; set; }
        public string? MaLMH { get; set; }
        public string TenLMH { get; set; } = string.Empty;
        public int? IdCustomer { get; set; }
        public int? IdLMHParent { get; set; }
        public string? Mota { get; set; }
        public string? HinhAnh { get; set; }
        public int? DoUuTien { get; set; }
        public bool isDel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? IdKho { get; set; }
    }
}
