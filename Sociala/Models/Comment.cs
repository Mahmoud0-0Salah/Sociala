using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }
      public DateTime CreatedAt { get; set; }
        [StringLength(450)]
        
        [ForeignKey("User")]
        public string  UserId { get; set; }
        
        [ForeignKey("Post")]
        public int PostId { get; set; }
        virtual public Post Post { get; set; }
        virtual public User User { get; set; }

    }
}
