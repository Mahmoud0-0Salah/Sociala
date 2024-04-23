using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Seen { get; set; }
        [StringLength(450)]

        [ForeignKey("User")]
        public string  UserId { get; set; }
        virtual public User User { get; set; }

    }
}
