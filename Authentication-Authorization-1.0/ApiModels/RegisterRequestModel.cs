
using System.ComponentModel.DataAnnotations;

namespace Authentication_Authorization_1._0.ApiModels
{
    public class RegisterRequestModel
    {
        public RegisterRequestModel()
        {
            CreatedAt = DateTime.Now;
        }
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int RoleID { get; set; }
        // Add any other registration fields as needed
    }
}



