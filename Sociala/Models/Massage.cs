using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Massage
    {


        [ForeignKey("User")]
        public string SenderId { get; set; }

        public string ResverId { get; set; }

        public string Content { get; set; }
        virtual public User User { get; set; }

    }
}
