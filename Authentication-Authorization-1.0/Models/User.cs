using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication_Authorization_1._0.Models
{
    [Table("User")]
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoleID { get; set; }
        public Role Role { get; set; }
    }
}
