using Identity_API.Infrastructure;
using Identity_API.Infrastructure.Services;
using Identity_API.Models;
using Identity_API.Models.Options;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using System.Text;

namespace Identity_API.Extensions
{
    public static class Extension
    {
        public static void AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddTransient<ITokenProvider, TokenProvider>();
        }

        public static void AddIdentity(this IHostApplicationBuilder builder)
        {
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UsersDbContext>();

            builder.AddDefaultAuthentication();
        }

        public static void AddPersistence(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb"));
            });
        }
    }
}
