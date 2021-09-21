// https://docs.microsoft.com/en-us/ef/core/saving/cascade-delete

// 1. Begynd med at undersøge relationstyperne i ER-diagrammets i forhold til om de er OPTIONAL eller REQUIRED.
// 2. Derefter kan man køre programmet og breake efter at databasen er oprettet. I log-udskriften kan DeleteBehavior kontrolleres.
// 3. Skriv de forskellige Delete constraints ind i ER-diagrammet.
// 4. Når Blog1 slettes vil dette trække de relaterede Post1, Post2 og Post3, samt de relaterede PostTag1, PostTag2, PostTag3 og PostTag4 med i faldet!.

// 5. Dernæst kan man spørge: hvor stor en del af graphen er det nødvendigt at indlæse, hvis man vil slette Blog1? Kun Blog1, fordi databasen selv laver Cascaded Delete
//      Test det ved at udkommentere alle Includes. Bemærk at EF Core kun sletter Blog1, resten ordner DB selv. Kontrollér det i SSMS.

// 6. Nu ændres følgende FK til Nullable: Post(BlogId) og PostTag(PostId). 
//      Slet Blog1 med fuld graph indlæst.
//      Bemærk at relationerne i DB nu sættes til NO ACTION og at FK sættes til NULL
//      Når Blog1 slettes, medfører det 3 * UPDATE med NULL af FK hos Posts. Bemærk at PostTag ikke berøres!

// 7. Til sidst undlades det at indlæses graphen (udkommenter Include). Dette medfører en SQL Exception (fordi Blog1 ikke må slettes uden at FK i Post bliver påvirket).

// ------------------------Repeterende forklaring fra forrige demo: ----------------------------------------------------------------------------------------------------------
// DeleteBehavior.Cascade (Default): Sæt FK Post.BlogId til int, altså Not Nullable. Relationen er Required!
//  1. Med indlæsning af relaterede Posts, altså .Include(b => b.Posts) -> Relaterede Post-objekter deletes vha. SQL fordi EF kender til dem (tracked)
//  2. Uden indlæsning af relaterede Posts, uden .Include(b => b.Posts) -> EF fjerner kun relaterede Post-objekter i memory, 
//          men DB er oprettet med ON DELETE CASCADE på relationen og derfor sørger den selv for at slette Post-objekterne. Derfor kun DELETE af Blog i SQL!

// DeleteBehavior.ClientSetNull (Default): Sæt FK Post.BlogId til int?, altså Nullable. Relationen er Optional!
//  3. Med indlæsning af relaterede Posts, altså .Include(b => b.Posts) -> Foreign key properties sættes til NULL og kun Blog-objektet slettes vha. SQL
//  4. Uden indlæsning af relaterede Posts, uden .Include(b => b.Posts) -> Exception fordi DB er oprettet med ON DELETE NO ACTION og dermed overtrædes REFERENCE CONSTRAINT

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
                    Console.WriteLine("\nSaving changes:");

                    DisplayStates(context.ChangeTracker.Entries());
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nSaveChanges threw {e.GetType().Name}: {(e is DbUpdateException ? e.InnerException.Message : e.Message)}");
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