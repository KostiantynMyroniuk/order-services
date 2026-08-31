using Identity.API.Infrastructure;
using Identity.API.Infrastructure.Services;
using Identity.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Apis
{
    public static class IdentityApi
    {
        public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("api/auth").WithTags("Auth");

            authGroup.MapPost("login", Login);
            authGroup.MapPost("register", Register);
            authGroup.MapPost("refresh", Refresh);

            return app;
        }

        public sealed record LoginRequest(string Email, string Password);
        public sealed record LoginResponse(string AccessToken, string RefreshToken);
        public static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> Login(
            LoginRequest request,
            UsersDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenProvider tokenProvider,
            CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return TypedResults.Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(
                user, 
                request.Password, 
                lockoutOnFailure: true);

            if (!result.Succeeded)
                return TypedResults.Unauthorized();

            var userRoles = await userManager.GetRolesAsync(user);

            var accessToken = tokenProvider.CreateToken(user, userRoles);
            var refreshTokenValue = await IssueRefreshTokenAsync(context, tokenProvider, user.Id, ct);

            var response = new LoginResponse(accessToken, refreshTokenValue);

            return TypedResults.Ok(response);
        }

        public sealed record RegisterRequest(string Email, string Password);
        public sealed record RegisterResponse(string AccessToken, string RefreshToken);
        public static async Task<Results<Ok<RegisterResponse>, BadRequest<IEnumerable<IdentityError>>>> Register(
            RegisterRequest request,
            UsersDbContext context,
            UserManager<ApplicationUser> userManager,
            ITokenProvider tokenProvider,
            CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return TypedResults.BadRequest(result.Errors);
            }

            var userRoles = await userManager.GetRolesAsync(user);

            var accessToken = tokenProvider.CreateToken(user, userRoles);
            var refreshTokenValue = await IssueRefreshTokenAsync(context, tokenProvider, user.Id, ct);

            var response = new RegisterResponse(accessToken, refreshTokenValue);

            return TypedResults.Ok(response);
        }

        public sealed record RefreshResponse(string AccessToken, string RefreshToken);

        public static async Task<Results<Ok<RefreshResponse>, UnauthorizedHttpResult>> Refresh(
            [FromHeader(Name = "X-Refresh-Token")] string refreshTokenValue,
            UsersDbContext context,
            UserManager<ApplicationUser> userManager,
            ITokenProvider tokenProvider,
            CancellationToken ct)
        {
            var tokenHash = tokenProvider.HashToken(refreshTokenValue);

            var refreshToken = await context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, ct);

            if (refreshToken is null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
                return TypedResults.Unauthorized();

            var roles = await userManager.GetRolesAsync(refreshToken.User);
            var accessToken = tokenProvider.CreateToken(refreshToken.User, roles);

            var newRefreshToken = tokenProvider.CreateRefreshToken();
            refreshToken.TokenHash = tokenProvider.HashToken(newRefreshToken);
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(7);

            await context.SaveChangesAsync(ct);

            return TypedResults.Ok(new RefreshResponse(accessToken, newRefreshToken));
        }

        private static async Task<string> IssueRefreshTokenAsync(
            UsersDbContext context, ITokenProvider tokenProvider, string userId, CancellationToken ct)
        {
            var raw = tokenProvider.CreateRefreshToken();
            context.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenProvider.HashToken(raw),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
            });
            await context.SaveChangesAsync(ct);
            return raw;
        }
    }
}
