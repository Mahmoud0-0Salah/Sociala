namespace Sociala.Models
{
    public class Like
    {
        public string UserId { get; set; }
        public  int PostId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
