using Identity.API.Infrastructure;
using Identity.API.Infrastructure.Services;
using Identity.API.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using Shared.Models;
using System.Text;

namespace Identity.API.Extensions
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
