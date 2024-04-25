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
  
        public bool IsHidden { get; set; } = false;
       
        [ForeignKey("User")]
        public string  UserId {  get; set; }
        virtual public User User { get; set; }


    }
}
