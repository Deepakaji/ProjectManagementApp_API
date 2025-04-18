using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementApp.Model.Entity
{
    public class mst_User
    {
        [Key]
        public int UserID { get; set; }

        public string? UserName { get; set; }
                     
        public string? Email { get; set; }
                     
        public string? Password { get; set; }

        public int? RoleID { get; set; }

        [ForeignKey("RoleID")]
        public mst_Role? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
