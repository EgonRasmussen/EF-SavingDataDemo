// https://docs.microsoft.com/en-us/ef/core/saving/basic

using Microsoft.EntityFrameworkCore;
using SavingData.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SavingData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await InitializeDb();   // Køres kun første gang

            //await AddingData();
            //await UpdatingData();
            //DeletingData();
            //await MultipleOperationsInASingleSaveChanges();
        }

        private static async Task AddingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = "http://example.com" };
                context.Blogs.Add(blog);
                await context.SaveChangesAsync();
            }
        }

        private static async Task UpdatingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = await context.Blogs.SingleAsync(b => b.Url == "http://example.com");
                blog.Url = "http://example.com/blog";
                await context.SaveChangesAsync();
            }
        }

        private static async Task DeletingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = await context.Blogs.SingleAsync(b => b.Url == "http://example.com/blog");
                context.Blogs.Remove(blog);
                await context.SaveChangesAsync();
            }
        }

        private static async Task MultipleOperationsInASingleSaveChanges()
        {
            using (var context = new BloggingContext())
            {
                await InitializeDb();
                // seeding database
                context.Blogs.Add(new Blog { Url = "http://example.com/blog" });
                context.Blogs.Add(new Blog { Url = "http://example.com/another_blog" });
                await context.SaveChangesAsync();
            }

            using (var context = new BloggingContext())
            {
                // add
                context.Blogs.Add(new Blog { Url = "http://example.com/blog_one" });
                context.Blogs.Add(new Blog { Url = "http://example.com/blog_two" });

                // update
                var firstBlog = await context.Blogs.FirstAsync();
                firstBlog.Url = "";

                // remove
                var lastBlog = await context.Blogs.OrderBy(e => e.BlogId).LastAsync();
                context.Blogs.Remove(lastBlog);

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