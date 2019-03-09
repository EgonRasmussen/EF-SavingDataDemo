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
            InitializeDatabase();

            SavingDataDisconnected();
            //UpdateDisconnected();
            //UpdateGraphDisconnected();
            //AttachGraphDisconnected();
        }



        private static void SavingDataDisconnected()
        {
            Blog blog;
            using (var contextFirst = new BloggingContext())
            {
                blog = contextFirst.Blogs.First();
                blog.Url = "http://sample.com/disconnectedUpdate";       
            }
            using (var contextSecond = new BloggingContext())
            {
                var blogOld = contextSecond.Blogs.First();  // Reload entity
                blogOld.Url = blog.Url;

                DisplayStates(contextSecond.ChangeTracker.Entries());
                contextSecond.SaveChanges();
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
                contextSecond.Entry(blog).State = EntityState.Modified;                 // Nu Updates kun Blog-entiteten
                // contextSecond.Entry(blog).Property(p => p.Url).IsModified = true;    // Nu Updates kun Url- property

                DisplayStates(contextSecond.ChangeTracker.Entries());
                contextSecond.SaveChanges();
            }
        }

        #region INITIALIZE DATABASE
        private static void InitializeDatabase()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
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