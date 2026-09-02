using Basket.API.Models;
using Basket.API.Models.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.API.Infrastructure.Repositories
{
    public class RedisBasketRepository(
        IConnectionMultiplexer multiplexer,
        IOptions<RedisOptions> options) : IBasketRepository
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();

        public async Task<BasketModel?> GetBasketAsync(string userId, CancellationToken ct)
        {
            var basketKey = $"basket:{userId}";
            var json = await _db.StringGetAsync(basketKey);

            return json.HasValue 
                ? JsonSerializer.Deserialize<BasketModel>(json.ToString()) 
                : null;
        }

        public async Task<BasketModel> SaveBasketAsync(BasketModel basket, CancellationToken ct)
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
