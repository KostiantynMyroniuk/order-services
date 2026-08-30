namespace Shared.Models
{
    public sealed class JwtOptions
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SigningKey { get; set; }
        public required int ExpirationInMinutes { get; set; }
    }
}
