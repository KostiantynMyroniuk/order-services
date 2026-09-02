using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.API.Apis
{
    public static class BasketApi
    {
        public static void MapBasketApi(this IEndpointRouteBuilder app)
        {
            var basketGroup = app.MapGroup("api/basket")
                .RequireAuthorization()
                .WithTags("Basket");

            basketGroup.MapPost("/items", AddItem);
        }

        public sealed record AddItemRequest(Guid ProductId, int Quantity);
        public sealed record AddItemResponse();
        public static async Task<Results<Ok<BasketModel>, NotFound<string>>> AddItem(
            AddItemRequest request,
            IBasketService basketService,
            IIdentityProvider identityProvider, 
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            var result = await basketService.AddItemAsync(userId!, request.ProductId, request.Quantity, ct);

            return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound(result.ErrorMessage);
        }
    }
}
