using System.Collections.Generic;

namespace SavingData.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }


        public int? PhotoId { get; set; }       // FK
        public List<Post> AuthoredPosts { get; set; }
        public List<Blog> OwnedBlogs { get; set; }
        public PersonPhoto Photo { get; set; }
    }
}
