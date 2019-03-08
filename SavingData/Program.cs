// https://docs.microsoft.com/en-us/ef/core/saving/basic

using SavingData.Models;
using System.Linq;

namespace SavingData
{
    class Program
    {
        static void Main(string[] args)
        {
            AddingData();
            //UpdatingData();
            //DeletingData();
            //MultipleOperationsInASingleSaveChanges();
        }

        private static void AddingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = new Blog { Url = "http://sample.com" };
                context.Blogs.Add(blog);
                context.SaveChanges();
            }
        }

        private static void UpdatingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.OrderBy(x => x.BlogId).First();
                blog.Url = "http://sample.com/blog";
                context.SaveChanges();
            }
        }

        private static void DeletingData()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs.OrderBy(x => x.BlogId).First();
                context.Blogs.Remove(blog);
                context.SaveChanges();
            }
        }

        private static void MultipleOperationsInASingleSaveChanges()
        {
            using (var context = new BloggingContext())
            {
                // add
                context.Blogs.Add(new Blog { Url = "http://sample.com/blog_one" });
                context.Blogs.Add(new Blog { Url = "http://sample.com/blog_two" });

                // update
                var firstBlog = context.Blogs.OrderBy(x => x.BlogId).First();
                firstBlog.Url = "";

                // remove
                var lastBlog = context.Blogs.OrderBy(x => x.BlogId).Last();
                context.Blogs.Remove(lastBlog);

                context.SaveChanges();
            }
        }
    }
}