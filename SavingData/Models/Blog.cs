using System.Collections.Generic;

namespace SavingData.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }; // = new List<Post>();
    }
}
