// https://docs.microsoft.com/en-us/ef/core/saving/cascade-delete

// Log-udskriften under DB-oprettelsen afslører hvilken Constraint relationen indeholder.
//  Sæt et breakpoint ved SaveChanges(). Via Locals-vinduet kan det konstateres at entiteterne bliver slettet i memory når SaveChanges() er kørt.
//  Kontrollér sletningen i databasen ved at hente data fra tabellerne med SSOE. 

// Demo: DeleteBehavior.Cascade (Default): Sæt FK Post.BlogId til int, altså Not Nullable. Relationen er Required!
//  1. Med indlæsning af relaterede Posts, altså .Include(b => b.Posts) -> Relaterede Post-objekter deletes vha. SQL fordi EF kender til dem (tracked)
//  2. Uden indlæsning af relaterede Posts, uden .Include(b => b.Posts) -> EF fjerner kun relaterede Post-objekter i memory, 
//          men DB er oprettet med ON DELETE CASCADE på relationen og derfor sørger den selv for at slette Post-objekterne. Derfor kun DELETE af Blog i SQL!

// Demo: DeleteBehavior.ClientSetNull (Default): Sæt FK Post.BlogId til int?, altså Nullable. Relationen er Optional!
//  3. Med indlæsning af relaterede Posts, altså .Include(b => b.Posts) -> Foreign key properties sættes til NULL og kun Blog-objektet slettes vha. SQL
//  4. Uden indlæsning af relaterede Posts, uden .Include(b => b.Posts) -> Exception fordi DB er oprettet med ON DELETE NO ACTION og dermed overtrædes REFERENCE CONSTRAINT

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