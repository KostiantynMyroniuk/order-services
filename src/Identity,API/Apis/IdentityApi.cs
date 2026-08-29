using Identity_API.Infrastructure.Services;
using Identity_API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Apis
{
    public static class IdentityApi
    {
        public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("api/auth").WithTags("Auth");

            authGroup.MapPost("login", Login);
            authGroup.MapPost("register", Register);

            return app;
        }

        public sealed record LoginRequest(string Email, string Password);
        public sealed record LoginResponse(string AccessToken, string RefreshToken);
        public static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> Login(
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenProvider tokenProvider)
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
            var refreshToken = tokenProvider.CreateRefreshToken();

            var response = new LoginResponse(accessToken, refreshToken);

            return TypedResults.Ok(response);
        }

        public sealed record RegisterRequest(string Email, string Password);
        public sealed record RegisterResponse(string AccessToken, string RefreshToken);
        public static async Task<Results<Ok<RegisterResponse>, BadRequest<IEnumerable<IdentityError>>>> Register(
            RegisterRequest request,
            UserManager<ApplicationUser> userManager,
            ITokenProvider tokenProvider)
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
            var refreshToken = tokenProvider.CreateRefreshToken();

            var response = new RegisterResponse(accessToken, refreshToken);

            return TypedResults.Ok(response);
        }
    }
}
