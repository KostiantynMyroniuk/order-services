using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Shared.Services
{
    public interface IIdentityProvider
    {
        string? GetUserId();
        string? GetUserEmail();
    }

    public class IdentityProvider(IHttpContextAccessor httpContext) : IIdentityProvider
    {
        public string? GetUserEmail()
        {
            return httpContext.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public string? GetUserId()
        {
            return httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
