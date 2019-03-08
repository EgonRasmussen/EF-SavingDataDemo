// https://docs.microsoft.com/en-us/ef/core/saving/related-data

using Microsoft.EntityFrameworkCore;
using SavingData.Models;
using System.Collections.Generic;
using System.Linq;

namespace SavingData
{
    class Program
    {
        static void Main(string[] args)
        {
            AddingGraphNewEntities();
            //AddingRelatedEntity();
            //ChangingRelationships();
            //RemovingRelationships();
        }


        private static void AddingGraphNewEntities()
        {
            using (var context = new BloggingContext())
            {
                var blog = new Blog
                {
                    Url = "http://blogs.msdn.com/dotnet",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Intro to C#" },
                        new Post { Title = "Intro to VB.NET" },
                        new Post { Title = "Intro to F#" }
                    }
                };

                context.Blogs.Add(blog);
                context.SaveChanges();
            }
        }

        private static void AddingRelatedEntity()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Include(b => b.Posts)            // Mangler denne linje, kastes en "Object reference not set to an instance of an object" exception. 
                    .First();
                var post = new Post { Title = "Intro to EF Core" };

                blog.Posts.Add(post);
                //blog.Posts = new List<Post> { post };   // Alternativ til at indlæse relaterede Blogs
                context.SaveChanges();
            }
        }

        private static void ChangingRelationships()
        {
            using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = "http://blogs.msdn.com/visualstudio" };
                var post = context.Posts.First();

                post.Blog = blog;
                context.SaveChanges();
            }
        }

        private static void RemovingRelationships()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.Include(b => b.Posts).First();
                var post = blog.Posts.First();

                blog.Posts.Remove(post);
                context.SaveChanges();
            }
        }
    }
}