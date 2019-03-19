// https://docs.microsoft.com/en-us/ef/core/saving/cascade-delete

// Begynd med at identificere hvilken DeleteBehavior der bliver sat på de forskellige relationer.
// Derefter kan man køre programmet og breake efter at databasen er oprettet. I log-udskriften kan DeleteBehavior kontrolleres.
// I dette tilfælde slettes Blog1, de relaterede Post1, Post2 og Post3, samt de relaterede PostTag1, PostTag2, PostTag3 og PostTag4.

// Dernæst kan man spørge: hvor stor en del af graphen er det nødvendigt at indlæse, hvis man vil slette Blog1? Kun Blog1, fordi databasen selv laver Cascaded Delete
//      Test det ved at udkommentere alle Includes. Bemærk at EF Core kun sletter Blog1, resten ordner DB selv. Kontrollér det i SSMS.


// Nu ændres FK til Nullable: Post(BlogId) og PostTag(PostId). 
// Slet Blog1 med fuld graph indlæst.
// Bemærk at relationerne i DB nu sættes til NO ACTION og at FK sættes til NULL
// Når Blog1 slettes, medfører det 3 * UPDATE med NULL af FK hos Posts. Bemærk at PostTag ikke berøres!

// Nu indlæses graphen ikke (udkommenter Include). Dette medfører en SQL Exception (fordi Blog1 ikke må slettes uden at FK i Post bliver påvirket).

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
// int  BlogId = required: DeleteBehavior.Cascade (Default) + .Include(b => b.Posts) -> Blog and Posts in Memory are all deleted
// int  BlogId = required: DeleteBehavior.Cascade (Default) - .Include(b => b.Posts) -> Blog in Memory and Posts in DB and are all deleted (Cascading Delete implemented)

// int? BlogId = optional: DeleteBehavior.ClientSetNull (Default) + .Include(b => b.Posts) -> Foreign key properties are set to null
// int? BlogId = optional: DeleteBehavior.ClientSetNull (Default) - .Include(b => b.Posts) -> Exception because BlogId is not set to NULL in DB (NO ACTION)

// int? BlogId = optional: DeleteBehavior.SetNull + .Include(b => b.Posts) -> Foreign key properties are set to null
// int? BlogId = optional: DeleteBehavior.SetNull - .Include(b => b.Posts) -> Foreign key properties are set to null

// int  BlogId = required: DeleteBehavior.Restrict -> Nothing is deleted

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            DeleteBlog();
        }

        private static void DeleteBlog()
        {
            using (var context = new BloggingContext())
            {
                var blog = context.Blogs
                    .Include(b => b.Posts)
                        .ThenInclude(p => p.Tags)
                    .Include(p => p.Owner)
                        .ThenInclude(p => p.Photo)
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
            //  DisplayStates(context.ChangeTracker.Entries());
        }
        #endregion
    }
}