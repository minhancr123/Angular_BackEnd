namespace AngularBackEnd.Models.InventoryManagement
{
    public class BrandModel
    {
        public int IdNhanHieu { get; set; }
        public string TenNhanHieu { get; set; }

        public int? CreatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public bool? IsDel { get; set; }

        public int? IdCustomer { get; set; }
    }
}
