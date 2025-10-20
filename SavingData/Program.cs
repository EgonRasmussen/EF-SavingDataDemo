// https://docs.microsoft.com/en-us/ef/core/saving/related-data

// Demonstreres i debug og med Blogs og Posts tabellerne åbne i SSOE.

using Microsoft.EntityFrameworkCore;
using SavingData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SavingData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await InitializeDb(); // Kør kun én gang for at genskabe databasen

            //await AddingGraphNewEntities();
            //await AddingRelatedEntity();
            //await ChangingRelationships();
            //await RemovingRelationships();
        }

        // https://docs.microsoft.com/da-dk/ef/core/saving/related-data#adding-a-graph-of-new-entities
        private static async Task AddingGraphNewEntities()
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
                await context.SaveChangesAsync();
            }
        }

        // https://docs.microsoft.com/da-dk/ef/core/saving/related-data#adding-a-related-entity
        private static async Task AddingRelatedEntity()
        {
            using (var context = new BloggingContext())
            {
                var blog = await context.Blogs
                    .Include(b => b.Posts)            // Mangler denne linje, fejler den når vi når til Add(post) fordi der ikke findes en Posts collection. 
                    .FirstAsync();
                var post = new Post { Title = "Intro to EF Core" };

                blog.Posts.Add(post);
                //blog.Posts = new List<Post> { post };   // Alternativ til at indlæse relaterede Blogs. Eller lav initialiseringa af Blog.Posts i modellen
                await context.SaveChangesAsync();
            }
        }

        // https://docs.microsoft.com/da-dk/ef/core/saving/related-data#changing-relationships
        private static async Task ChangingRelationships()   
        {    
            using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = "http://blogs.msdn.com/visualstudio" };
                var post = await context.Posts.FirstAsync();       // Den fremsøgte Post 1 relaterer til en tidligere oprettet Blog 1

                post.Blog = blog;                     // Nu ændres tilhørsforholdet for Post 1, som nu relaterer til den nye Blog 2. FK i Post 1 ændres via navigation property
                await context.SaveChangesAsync();
            }
        }

        // https://docs.microsoft.com/da-dk/ef/core/saving/related-data#removing-relationships
        private static async Task RemovingRelationships()
        {
            using (var context = new BloggingContext())
            {
                var blog = await context.Blogs
                    .OrderBy(b => b.BlogId)
                    .Include(b => b.Posts).FirstAsync();
                var post = blog.Posts.First();

                blog.Posts.Remove(post);
                await context.SaveChangesAsync();
            }
        }

        private static async Task InitializeDb()
        {
            using (var context = new BloggingContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Database recreated");
            }
        }
    }
}