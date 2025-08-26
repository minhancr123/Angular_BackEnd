namespace AngularBackEnd.Models.AccountManagement
{
    public class AccountRole
    {
        public long Id_Permit { get; set; }
        public string PartnerName { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Mobile { get; set; }
        public string CreatedDate { get; set; }
        public string Tenquyen { get; set; }

        public string Description { get; set; }
        public string LastLogin { get; set; }

        public bool Edit { get; set; }
        public bool Visible { get; set; }

        public bool IsLock { get; set; }
    }
}
