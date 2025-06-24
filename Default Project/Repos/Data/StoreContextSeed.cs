using Default_Project.Cores.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Default_Project.Repos.Data
{
    public static class StoreContextSeed
    {
       public static async Task SeedAsync(StoreContext dbContext)
       {
           try
           {
               if (!await dbContext.Tags.AnyAsync())
               {
                   var tagData = await ReadFileAsync($"../Default Project/Repos/Data/DataSeed/tags.json");
                   if (tagData != null)
                   {
                       var tags = JsonSerializer.Deserialize<List<Tag>>(tagData);
                       if (tags != null && tags.Any())
                       {
                           await dbContext.Tags.AddRangeAsync(tags);
                           await dbContext.SaveChangesAsync();
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error during seeding: {ex.Message}");
           }
       }

       // Helper method to handle file reading asynchronously
       private static async Task<string?> ReadFileAsync(string filePath)
       {
           try
           {
               return await File.ReadAllTextAsync(filePath);
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error reading file at {filePath}: {ex.Message}");
               return null; // Return null if the file couldn't be read
           }
       }
    }
}
