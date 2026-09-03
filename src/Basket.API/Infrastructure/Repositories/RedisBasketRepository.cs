using Basket.API.Models;
using Basket.API.Models.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.API.Infrastructure.Repositories
{
    public class RedisBasketRepository(
        IConnectionMultiplexer multiplexer,
        IOptions<RedisOptions> options,
        ILogger<RedisBasketRepository> logger) : IBasketRepository
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();

        public async Task<BasketModel?> GetBasketAsync(string userId, CancellationToken ct = default)
        {
            var basketKey = $"basket:{userId}";
            var json = await _db.StringGetAsync(basketKey);

            if (!json.HasValue)
                return null;

            try
            {
                var basket = JsonSerializer.Deserialize<BasketModel>(json.ToString());
                return basket;
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize basket for user {UserId}", userId);
                throw;
            }
        }

        public async Task<BasketModel> SaveBasketAsync(BasketModel basket, CancellationToken ct = default)
        {
            var basketKey = $"basket:{basket.UserId}";
            await _db.StringSetAsync(
                basketKey,
                JsonSerializer.Serialize(basket),
                TimeSpan.FromDays(options.Value.TtlDays));

            return basket;
        }
    }
}
