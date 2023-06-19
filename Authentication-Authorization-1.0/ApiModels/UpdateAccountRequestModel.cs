
using System.ComponentModel.DataAnnotations;

namespace Authentication_Authorization_1._0.ApiModels
{
    public class UpdateAccountRequestModel
    {
       
       
        public string? Email { get; set; }

       
        public string? Password { get; set; }

       

      
        public int? RoleID { get; set; }
        // Add any other registration fields as needed
    }
}



