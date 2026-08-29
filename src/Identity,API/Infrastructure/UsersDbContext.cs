using Identity_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.Infrastructure
{
    public class UsersDbContext : IdentityDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("identity");
        }
    }
}
