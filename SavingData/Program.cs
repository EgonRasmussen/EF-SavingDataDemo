// https://docs.microsoft.com/en-us/ef/core/saving/related-data

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SavingData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SavingData
{
    class Program
    {
        static void Main(string[] args)
        {
            //AddingGraphNewEntities();
            AddingRelatedEntity();
            //ChangingRelationships();
            //RemovingRelationships();

            //SavingDataConnected();
            //SavingDataDisconnected();

            //UpdateDisconnected();
            //UpdateGraphDisconnected();
            //AttachGraphDisconnected();

            //DeletePost();
        }





        #region SAVING RELATED DATA
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
        #endregion

        #region UPDATE CONNECTED/DISCONNECTED
        private static void SavingDataConnected()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.First();
                blog.Url = "http://sample.com/connected";
                context.SaveChanges();
            }
        }

        private static void SavingDataDisconnected()
        {
            using (var contextFirst = new BloggingContext())
            {
                var blogNew = contextFirst.Blogs.First();
                blogNew.Url = "http://sample.com/disconnectedUdenUpdate";
                using (var contextSecond = new BloggingContext())
                {
                    var blogOld = contextSecond.Blogs.First();
                    blogOld.Url = blogNew.Url;
                    contextSecond.SaveChanges();
                }
            }
        }

        private static void UpdateDisconnected()
        {
            Blog blog;
            using (var contextFirst = new BloggingContext())
            {
                blog = contextFirst.Blogs.First();
                blog.Url = "http://sample.com/disconnectedUdenUpdate";
            }
            using (var contextSecond = new BloggingContext())
            {
                contextSecond.Update(blog);
                DisplayStates(contextSecond.ChangeTracker.Entries());   // Alle properties for Blog Updates!
                contextSecond.SaveChanges();
            }
        }

        private static void UpdateGraphDisconnected()
        {
            Blog blog;
            using (var contextFirst = new BloggingContext())
            {
                blog = contextFirst.Blogs.Include(p => p.Posts).ThenInclude(pt => pt.Tags).Include(p => p.Owner).ThenInclude(pp => pp.Photo).First();
                blog.Url = "http://sample.com/disconnectedUdenUpdate";

            }
            using (var contextSecond = new BloggingContext())
            {
                contextSecond.Update(blog);
                DisplayStates(contextSecond.ChangeTracker.Entries()); // Ialt 10 komplette SQL-Updates!
                contextSecond.SaveChanges();
            }
        }

        private static void AttachGraphDisconnected()
        {
            Blog blog;
            // Når den komplette graph medtages, resulterer en enkelt update nu kun i en eneste Update i DB!
            using (var contextFirst = new BloggingContext())
            {
                blog = contextFirst.Blogs.Include(p => p.Posts).ThenInclude(pt => pt.Tags).Include(p => p.Owner).ThenInclude(pp => pp.Photo).First();

                blog.Url = "http://sample.com/disconnectedUdenUpdate";
            }
            using (var contextSecond = new BloggingContext())
            {
                //contextSecond.Add(new PostTag { PostId = 1, TagId = "Living" });   // Adder nye entiteter

                contextSecond.Attach(blog);
                contextSecond.Entry(blog).State = EntityState.Modified;  // Nu Updates kun Blog-entiteten
                // contextSecond.Entry(blog).Property(p => p.Url).IsModified = true;

                DisplayStates(contextSecond.ChangeTracker.Entries());
                contextSecond.SaveChanges();
            }
        }
        #endregion

        #region DELETE
        private static void DeletePost()
        {
            using (var context = new BloggingContext())
            {
                var post = context.Posts
                    .Include(p => p.Tags)
                    .Include(pt => pt.Tags);
                context.Posts.Remove(post.First());
                context.SaveChanges();
            }
        }
        #endregion

        #region DISPLAY STATES
        private static void DisplayStates(IEnumerable<EntityEntry> entries)
        {
            Console.WriteLine("\n-------------- EntityStates ----------------");
            foreach (var entry in entries)
            {
                Console.WriteLine("Entity: {0, -15} State: {1}", entry.Entity.GetType().Name, entry.State.ToString());
                if (entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Members)
                    {
                        Console.WriteLine("\tProperty: {0, -15} IsModified: {1}", prop.Metadata.Name, prop.IsModified);
                    }
                }
            }
            Console.WriteLine("--------------------------------------------\n");
        }
        #endregion
    }
}