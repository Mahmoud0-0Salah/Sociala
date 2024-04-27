using System.ComponentModel.DataAnnotations;

namespace Sociala.Models
{
    public class Block
    {

        [StringLength(450)]
        public string Blocking { get; set; }

        [StringLength(450)]
        public string Blocked{ get; set; }
        virtual public User User { get; set; }
    }
}
