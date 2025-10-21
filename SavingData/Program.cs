// https://docs.microsoft.com/en-us/ef/core/saving/related-data

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SavingData.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavingData;

class Program
{
    static async Task Main()
    {
        await InitializeDatabase();

        await SavingDataDisconnected();
        //await UpdateDisconnected();
        //await UpdateGraphDisconnected();
        //await AttachGraphDisconnected();
    }


    private static async Task SavingDataDisconnected()
    {
        Blog blog;
        using (var contextFirst = new BloggingContext())  
        {
            blog = await contextFirst.Blogs.FirstAsync();
            blog.Url = "http://sample.com/disconnectedUpdate";
        }

        using (var contextSecond = new BloggingContext())
        {
            var blogOld = await contextSecond.Blogs.FirstAsync();  // Reload/track entity from Db
            blogOld.Url = blog.Url;

            DisplayStates(contextSecond.ChangeTracker.Entries());
            await contextSecond.SaveChangesAsync();
        }
    }

    private static async Task UpdateDisconnected()
    {
        Blog blog;
        using (var contextFirst = new BloggingContext())
        {
            blog = await contextFirst.Blogs.FirstAsync();
            blog.Url = "http://sample.com/disconnectedUdenUpdate";
        }
        using (var contextSecond = new BloggingContext())
        {
            contextSecond.Update(blog);                             // Alle properties for Blog Updates!
            DisplayStates(contextSecond.ChangeTracker.Entries());   
            await contextSecond.SaveChangesAsync();
        }
    }

    private static async Task UpdateGraphDisconnected()
    {
        Blog blog;
        using (var contextFirst = new BloggingContext())
        {
            blog = await contextFirst.Blogs
                .Include(p => p.Posts)
                .ThenInclude(pt => pt.Tags)
                .Include(p => p.Owner)
                .ThenInclude(pp => pp.Photo)
                .FirstAsync();
            blog.Url = "http://sample.com/disconnectedUdenUpdate";

        }
        using (var contextSecond = new BloggingContext())
        {
            contextSecond.Update(blog);
            DisplayStates(contextSecond.ChangeTracker.Entries()); // Ialt 10 komplette SQL-Updates!
            await contextSecond.SaveChangesAsync();
        }
    }

    private static async Task AttachGraphDisconnected()
    {
        // Når den komplette graph medtages, resulterer en enkelt update nu kun i én eneste Update i DB!
        Blog blog;
        using (var contextFirst = new BloggingContext())
        {
            blog = await contextFirst.Blogs
                .Include(p => p.Posts)
                .ThenInclude(pt => pt.Tags)
                .Include(p => p.Owner)
                .ThenInclude(pp => pp.Photo)
                .FirstAsync();

            blog.Url = "http://sample.com/disconnectedMedAttach";
        }
        using (var contextSecond = new BloggingContext())
        {
            //contextSecond.Add(new PostTag { PostId = 1, TagId = "Living" });   // Adder nye entiteter

            contextSecond.Attach(blog);
            contextSecond.Entry(blog).State = EntityState.Modified;                 // Nu Updates kun Blog-entiteten
            // contextSecond.Entry(blog).Property(p => p.Url).IsModified = true;    // Nu Updates kun Url- property

            DisplayStates(contextSecond.ChangeTracker.Entries());
            await contextSecond.SaveChangesAsync();
        }
    }

    #region INITIALIZE DATABASE
    private static async Task InitializeDatabase()
    {
        using (var context = new BloggingContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
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