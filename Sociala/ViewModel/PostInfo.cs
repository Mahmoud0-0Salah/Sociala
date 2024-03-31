namespace Sociala.ViewModel
{
    public class PostInfo
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string PostContent { get; set; }
        public string PostImj { get; set; }
        public string UserPhoto { get; set; }
        public string UserName { get; set; }
        public bool Isliked { get; set; }
        public bool IsHidden { get; set; }
    }
}
