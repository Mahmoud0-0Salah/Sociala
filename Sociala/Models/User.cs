using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Sociala.Models
{
    public class User
    {
             [Key]
        [StringLength(450)]
        public  string Id { get; set; }
            [Required]
            public string First_ame { get; set; }
            [Required]  
            public string LastName { get; set; }
          public string UesrName {  get; set; }
            [Required]
            public string Email { get; set; }
            [Required] 
            public string PhoneNumber { get; set; }
            [Required]
            public string Password { get; set; }
            public string UrlPhoto{ get; set; }
            public string bio {  get; set; }
            public bool IsActive { get; set; }
            public string ActiveKey { get; set; }
             public  DateTime CreateAt { get; set; }

            public int RoleId { get; set; }
             [ForeignKey("RoleId")]
            public Role Roles { get; set; }
    }
}
