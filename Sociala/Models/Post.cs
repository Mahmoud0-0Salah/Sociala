using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Post
    {
        public int Id { get; set; }
        [StringLength(1000)]
        public string? content { get; set; }
        public string? Imj { get; set; }
        public DateTime CreateAt { get; set; }
        [StringLength(450)]

        public bool IsHidden { get; set; } = false;
        public string  UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }


    }
}
