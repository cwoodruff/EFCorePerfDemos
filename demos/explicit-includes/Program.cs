using Microsoft.EntityFrameworkCore;

namespace explicit_includes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new BloggingContext();

            // Ensure database is created
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Seed the database
            if (!context.Blogs.Any())
            {
                await SeedData(context);
            }

            // Explicitly load related data
            Console.WriteLine("Fetching Blogs:");
            var blogs = await context.Blogs.ToListAsync();
            foreach (var blog in blogs)
            {
                Console.WriteLine($"Blog: {blog.Name}");

                // Explicitly load related Posts
                await context.Entry(blog)
                    .Collection(b => b.Posts)
                    .LoadAsync();

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
                        new Post { Title = "Advanced C# Programming" }
                    }
                },
                new Blog
                {
                    Name = "Food Blog",
                    Posts = new List<Post>
                    {
                        new Post { Title = "Easy Recipes" },
                        new Post { Title = "Cooking Tips for Beginners" }
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
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFCoreExplicitIncludesDemo;Trusted_Connection=True;")
                          .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; } = new();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
