using Sociala.Models;
namespace Sociala.ViewModel
{
    public class PostInfo
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string PostContent { get; set; }
        public string? OriginalPostContent { get; set; }
        public string? OriginalUserPhoto { get; set; }
        public string? OriginalUserName { get; set; }
        public User? User { get; set; }
        public string PostImj { get; set; }
        public string UserPhoto { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string OriginalUserId { get; set; }
        public bool Isliked { get; set; }
        public bool IsHidden { get; set; }
        public bool IsBanned { get; set; }
    }
}
