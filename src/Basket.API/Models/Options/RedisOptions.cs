namespace Basket.API.Models.Options
{
    public class RedisOptions
    {
        public required int TtlDays { get; set; }
        public required int CatalogCacheTtlMinutes { get; set; }
    }
}
