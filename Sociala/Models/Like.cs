using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Like
    {

        [ForeignKey("User")]

        public string UserId { get; set; }
        public  int PostId { get; set; }
        virtual public User User { get; set; }
        virtual public Post Post { get; set; }
    }
}
