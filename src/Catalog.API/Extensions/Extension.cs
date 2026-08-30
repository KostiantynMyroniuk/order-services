using Shared.Extensions;

namespace Catalog.API.Extensions
{
    public static class Extension
    {
        public static void AddTokenValidation(this IHostApplicationBuilder builder)
        {
            builder.AddDefaultAuthentication();
        }

        public static void AddPersistence(this IHostApplicationBuilder builder)
        {

        }
    }
}
