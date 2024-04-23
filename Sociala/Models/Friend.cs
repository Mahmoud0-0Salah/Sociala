using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sociala.Models
{
    public class Friend
    {
        [Key]

        public int Id { get; set; }
        [StringLength(450)]
        public string RequestingUserId { get; set; }

        [StringLength(450)]
        public string RequestedUserId { get; set; }
        virtual public User User { get; set; }
    }
}
