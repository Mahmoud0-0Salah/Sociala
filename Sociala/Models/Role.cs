using System.ComponentModel.DataAnnotations;

namespace Sociala.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

    }
}
