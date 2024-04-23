using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public string Status { get; set; } = "Pending";
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }

        [StringLength(450)]
        [ForeignKey("User")]
        public string  UserId { get; set; }
        virtual public User User { get; set; }
    }
}
