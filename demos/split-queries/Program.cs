using Microsoft.EntityFrameworkCore;

namespace split_queries
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new BloggingContext();

            // Ensure the database is created
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Seed some data
            if (!context.Blogs.Any())
            {
                await SeedData(context);
            }

            // Query data using split queries
            Console.WriteLine("Fetching Blogs and their Posts with Split Queries:");
            var blogs = await context.Blogs
                .Include(b => b.Posts)
                .AsSplitQuery()
                .ToListAsync();

            foreach (var blog in blogs)
            {
                Console.WriteLine($"Blog: {blog.Name}");
                foreach (var post in blog.Posts)
                {
                    Console.WriteLine($"  Post: {post.Title}");
                }
            }
        }

        private static async Task SeedData(BloggingContext context)
        {
            var blogs = new List<Blog>
            {
                new Blog
                {
                    Name = "Tech Blog",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Understanding EF Core" },
                        new Post { Title = "Mastering LINQ" }
                    }
                },
                new Blog
                {
                    Name = "Food Blog",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Top 10 Recipes" },
                        new Post { Title = "Easy Cooking Tips" }
                    }
                }
            };

            await context.AddRangeAsync(blogs);
            await context.SaveChangesAsync();
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFCoreSplitQueriesDemo;Trusted_Connection=True;")
                          .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
