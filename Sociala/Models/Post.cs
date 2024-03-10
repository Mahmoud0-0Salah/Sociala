using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string content { get; set; }
        public DateTime CreateAt { get; set; }
        [StringLength(450)]
        public string  UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }


    }
}
