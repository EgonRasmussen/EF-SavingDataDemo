﻿namespace SavingData.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int? BlogId { get; set; }    // Default Nullable
        public Blog Blog { get; set; }
    }
}
