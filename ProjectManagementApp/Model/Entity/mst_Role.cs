using System.ComponentModel.DataAnnotations;

namespace ProjectManagementApp.Model.Entity
{
    public class mst_Role
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public ICollection<mst_User> Users { get; set; }
    }
}
