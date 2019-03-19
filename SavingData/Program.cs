// https://docs.microsoft.com/en-us/ef/core/saving/cascade-delete

// Demo foregår nemmest ved at lade BlogId være oprettet som Nullable (int?) og styre om den er Required (.IsRequired(true)) samt DeleteBehavior via Fluent API i Contexten.
// Log-udskriften under DB-oprettelsen afslører hvilken Constraint relationen indeholder.
//  Sæt et breakpoint ved SaveChanges(). Via Locals-vinduet kan det konstateres at entiteterne bliver slettet i memory når SaveChanges() er kørt.
//  Kontroller sletningen i databasen ved at hente data fra tabellerne med SSOE. 

// Benyt følgende i context: .IsRequired(true).OnDelete(DeleteBehavior.Cascade);
// int  BlogId = required: DeleteBehavior.Cascade (Default) + .Include(b => b.Posts) -> Blog and Posts in Memory are all deleted
// int  BlogId = required: DeleteBehavior.Cascade (Default) - .Include(b => b.Posts) -> Blog in Memory and Posts in DB and are all deleted (Cascading Delete implemented)

// Benyt følgende i context: .IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
// int? BlogId = optional: DeleteBehavior.ClientSetNull (Default) + .Include(b => b.Posts) -> Foreign key properties are set to null
// int? BlogId = optional: DeleteBehavior.ClientSetNull (Default) - .Include(b => b.Posts) -> Exception because BlogId is not set to NULL in DB (NO ACTION)

//      int? BlogId = optional: DeleteBehavior.SetNull + .Include(b => b.Posts) -> Foreign key properties are set to null
//      int? BlogId = optional: DeleteBehavior.SetNull - .Include(b => b.Posts) -> Foreign key properties are set to null

//      int  BlogId = required: DeleteBehavior.Restrict -> Nothing is deleted

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
            InitializeDb();
            DeleteBlog();
        }

        private static void DeleteBlog()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Include(b => b.Posts)
                    .First();

                context.Remove(blog);

                try
                {
                    Console.WriteLine();
                    Console.WriteLine("Saving changes:");

                    DisplayStates(context.ChangeTracker.Entries());
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine($"SaveChanges threw {e.GetType().Name}: {(e is DbUpdateException ? e.InnerException.Message : e.Message)}");
                }
            }
        }

        #region INITIALIZE DATABASE
        private static void InitializeDb()
        {
            using (var context = new BloggingContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Posts = new List<Post>
                    {
                        new Post {Title = "Saving Data with EF"},
                        new Post {Title = "Cascade Delete with EF"}
                    }
                });

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
            //  DisplayStates(context.ChangeTracker.Entries());
        }
        #endregion
    }
}