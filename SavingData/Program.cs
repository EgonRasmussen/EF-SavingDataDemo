// Julie Lerman: Mapping the Entities (Pluralsight video)

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

            //InsertInManyToMany();
            //InsertInManyToManyByTrackedPost();
            InsertInManyToManyByNonTrackedPost();

        }



        private static void InsertInManyToMany()
        {
            using (var context = new BloggingContext())
            {
                PostTag postTag = new PostTag { PostId = 1, TagId = "Living" };
                // context.PostTags.Add(postTag);    Der er ikke defineret en DbSet<PostTag>, så der indsættes direkte i DbContexten:
                context.Add(postTag);

                DisplayStates(context.ChangeTracker.Entries());
                context.SaveChanges();
            }
        }

        private static void InsertInManyToManyByTrackedPost()
        {
            // Her indlæses Post1 først, derefter addes et nyt PostTag objekt med tilhørende nyt Tag.
            using (var context = new BloggingContext())
            {
                Post post = context.Posts.Include(pt => pt.Tags).Where(p => p.PostId == 1).SingleOrDefault();
                post.Tags.Add( new PostTag { TagId = "Living" } );

                DisplayStates(context.ChangeTracker.Entries());  
                context.SaveChanges();
            }
        }

        private static void InsertInManyToManyByNonTrackedPost()
        {
            // Fordi post-objektet er totalt ukendt for Contexten, skal det attaches - ellers vil der ikke ske en tilføjelse af PostTag-objektet!
            Post post;
            using (var contextFirst = new BloggingContext())
            {
                post = contextFirst.Posts.Find(1);

            }
            using (var contextSecond = new BloggingContext())
            {
                post.Tags = new List<PostTag> { new PostTag { TagId = "Living" } };

                contextSecond.Posts.Attach(post);   // Prøv at udkommentere denne linje!
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