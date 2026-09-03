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
            basketGroup.MapGet("/", GetBasket);
            basketGroup.MapDelete("items/{productId:guid}", DeleteItem);
        }

        public sealed record AddItemRequest(Guid ProductId, int Quantity);
        public sealed record AddItemResponse();
        public static async Task<Results<Ok<BasketModel>, NotFound<string>, BadRequest<string>>> AddItem(
            AddItemRequest request,
            IBasketService basketService,
            IIdentityProvider identityProvider, 
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            var result = await basketService.AddItemAsync(userId!, request.ProductId, request.Quantity, ct);

            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);

            return result.StatusCode switch
            {
                StatusCodes.Status404NotFound => TypedResults.NotFound(result.ErrorMessage),
                _ => TypedResults.BadRequest(result.ErrorMessage)
            };
        }

        public sealed record GetBasketResponse(List<BasketItem> Items);
        public static async Task<Results<Ok<GetBasketResponse>, BadRequest>> GetBasket(
            IBasketService basketService,
            IIdentityProvider identityProvider,
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            var result = await basketService.GetUserBasketAsync(userId!, ct);

            return TypedResults.Ok(new GetBasketResponse(result.Value!.Items));
        }

        public static async Task<Results<NoContent, BadRequest>> DeleteItem(
            Guid productId,
            IBasketService basketService,
            IIdentityProvider identityProvider,
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            var result = await basketService.DeleteItemAsync(userId!, productId, ct);

            return TypedResults.NoContent();
        }
    }
}
