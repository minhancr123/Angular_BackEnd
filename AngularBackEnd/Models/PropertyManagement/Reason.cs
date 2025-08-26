namespace AngularBackEnd.Models.PropertyManagement
{
    public class Reason
    {
        public long IdRow { get; set; }
        public long LoaiTangGiam { get; set; }
        public string  MaTangGiam { get; set; }
        public string TenTangGiam { get; set; }
        public bool? TrangThai { get; set; }
    }
}
