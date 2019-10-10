namespace SavingData.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }    // FK, som styrer Delete behavior
        public Blog Blog { get; set; }
    }
}
