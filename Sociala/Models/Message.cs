using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Message
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string SenderId { get; set; }

        public string ResverId { get; set; }

        public string Content { get; set; }
        virtual public User User { get; set; }

    }
}
