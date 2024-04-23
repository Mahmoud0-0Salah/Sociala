using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sociala.Models
{
    public class Request
    {
        [Key]

        public int Id { get; set; }
        [StringLength(450)]
        public string  RequestingUserId { get; set; }
      

        [StringLength(450)]
        public string RequestedUserId { get; set; }
    

        public DateTime CreatedAt
        {
            get; set;
        }
        virtual public User User { get; set; }
    }
}
