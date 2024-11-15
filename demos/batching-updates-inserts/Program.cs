using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace batching_updates_inserts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new AppDbContext();

            // Ensure database is created
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create a large batch of data
            var newUsers = GenerateUsers(10000);

            Console.WriteLine("Inserting data...");
            var stopwatch = Stopwatch.StartNew();

            // Insert using EF Core
            await context.Users.AddRangeAsync(newUsers);
            await context.SaveChangesAsync();

            stopwatch.Stop();
            Console.WriteLine($"Inserted {newUsers.Count} users in {stopwatch.ElapsedMilliseconds} ms.");

            // Verify the inserted data
            var userCount = await context.Users.CountAsync();
            Console.WriteLine($"Total users in the database: {userCount}");
        }

        private static List<User> GenerateUsers(int count)
        {
            var users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                users.Add(new User { Name = $"User {i + 1}", Email = $"user{i + 1}@example.com" });
            }
            return users;
        }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFBatchInsertDemo;Trusted_Connection=True;")
                          .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
