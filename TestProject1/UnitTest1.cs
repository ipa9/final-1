using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using Reddit.Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Reddit.Tests.Repositories
{
    public class PostsRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            // Create a new service provider and a new options instance with a new database
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"RedditTestDb_{Guid.NewGuid()}")
                .Options;
        }

        private async Task SeedDatabase(ApplicationDbContext context)
        {
            context.Posts.AddRange(
                new Post { Id = 1, Title = "First Post", Content = "Content of the first post", Upvotes = 10, Downvotes = 2 },
                new Post { Id = 2, Title = "Second Post", Content = "Content of the second post", Upvotes = 5, Downvotes = 1 },
                new Post { Id = 3, Title = "Third Post", Content = "Content of the third post", Upvotes = 2, Downvotes = 3 }
            );

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetPosts_ShouldReturnPagedPosts()
        {
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            await SeedDatabase(context);
            var repository = new PostsRepository(context);

            var result = await repository.GetPosts(1, 2, null, null);

            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetPosts_ShouldFilterBySearchTerm()
        {
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            await SeedDatabase(context);
            var repository = new PostsRepository(context);

            var result = await repository.GetPosts(1, 2, "First", null);

            Assert.Single(result.Items);
            Assert.Equal("First Post", result.Items[0].Title);
        }

        [Fact]
        public async Task GetPosts_ShouldSortByUpvotes()
        {
            var options = CreateNewContextOptions();
            using var context = new ApplicationDbContext(options);
            await SeedDatabase(context);
            var repository = new PostsRepository(context);

            var result = await repository.GetPosts(1, 2, null, "positivity", false);

            Assert.Equal("First Post", result.Items[0].Title);
        }
    }
}
