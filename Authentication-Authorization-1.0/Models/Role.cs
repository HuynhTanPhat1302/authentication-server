using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication_Authorization_1._0.Models
{
    [Table("Role")]
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
