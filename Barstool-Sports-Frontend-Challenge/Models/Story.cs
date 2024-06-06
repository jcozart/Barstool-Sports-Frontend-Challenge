namespace Barstool_Sports_Frontend_Challenge.Models
{
    public class Story
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string Title { get; set; }
         public Thumbnail Thumbnail { get; set; }
        public bool NSFW { get; set; }
        public Author Author { get; set; }
    }
}
