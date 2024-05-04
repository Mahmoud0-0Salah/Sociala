using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sociala.Models
{
    public class SharePost
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int PostId { get; set; }
        public bool IsHidden { get; set; } = false;
        public Post Post { get; set; }

     
        public string UserId { get; set; }
        virtual public User User { get; set; }
    }
}
