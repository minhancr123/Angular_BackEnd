namespace AngularBackEnd.Models.AccountManagement
{
    public class RoleDTO
    {
        public string Id_Permit { get; set; }
        public string Description { get; set; }
        public bool Edit { get; set; } // đổi sang bool
        public bool viewOnly { get; set; }
    }
}
