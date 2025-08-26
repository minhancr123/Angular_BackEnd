namespace AngularBackEnd.Models.InventoryManagement
{
    public class DVTModel
    {
        public long IdDVT { get; set; }
        public string TenDVT { get; set; }
        public long? IdCustomer { get; set; }
        public bool? IsDel { get; set; }
        public long? CreatedBy { get; set; }
        public long? DeleteBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
