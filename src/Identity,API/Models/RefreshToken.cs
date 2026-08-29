namespace Identity_API.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
