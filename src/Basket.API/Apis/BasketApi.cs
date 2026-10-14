using Asp.Versioning;
using Basket.API.Features.AddItem;
using Basket.API.Features.DeleteItem;
using Basket.API.Features.GetBasket;
using Basket.API.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json;

namespace Basket.API.Apis
{
    public static class BasketApi
    {
        public static void MapBasketApi(this IEndpointRouteBuilder app)
        {
            var api = app.NewVersionedApi("Basket")
                .RequireAuthorization()
                .WithTags("Basket");

            var v1 = api.MapGroup("api/v{version:apiVersion}/basket").HasApiVersion(new ApiVersion(1, 0));
            var v2 = api.MapGroup("api/v{version:apiVersion}/basket").HasApiVersion(new ApiVersion(2, 0));

            v1.MapPost("items", AddItem)
                .WithName("AddItem")
                .WithSummary("Add item to the basket")
                .WithDescription("Endpoint for adding item to the basket")
                .WithTags("UserBasket");

            v1.MapGet("/", GetBasket)
                .WithName("GetBasket")
                .WithSummary("Get user basket")
                .WithDescription("Endpoint for getting user basket")
                .WithTags("UserBasket");

            v1.MapDelete("items/{productId:guid}", DeleteItem)
                .WithName("DeleteItem")
                .WithSummary("Delete item from the basket")
                .WithDescription("Endpoint for deleting item from the basket")
                .WithTags("UserBasket");
        }

        public sealed record AddItemRequest(Guid ProductId, int Quantity);
        public sealed record AddItemResponse(IEnumerable<BasketItem> Items);
        public static async Task<Results<Ok<AddItemResponse>, NotFound<string>, BadRequest<string>>> AddItem(
            [FromBody] AddItemRequest request,
            ISender sender,
            IIdentityProvider identityProvider)
        {
            var userId = identityProvider.GetUserId();

            var result = await sender.Send(new AddItemCommand(userId!, request.ProductId, request.Quantity));

            if (result.IsSuccess)
                return TypedResults.Ok(new AddItemResponse(result.Value!.Items));

            return result.StatusCode switch
            {
                StatusCodes.Status404NotFound => TypedResults.NotFound(result.ErrorMessage),
                _ => TypedResults.BadRequest(result.ErrorMessage)
            };
        }

        public sealed record GetBasketResponse(IEnumerable<BasketItem> Items);
        public static async Task<Results<Ok<GetBasketResponse>, BadRequest>> GetBasket(
            ISender sender,
            IIdentityProvider identityProvider)
        {
            var userId = identityProvider.GetUserId();

            var result = await sender.Send(new GetBasketQuery(userId!));

            return TypedResults.Ok(new GetBasketResponse(result.Value!.Items));
        }

        public static async Task<Results<NoContent, BadRequest>> DeleteItem(
            [FromRoute] Guid productId,
            ISender sender,
            IIdentityProvider identityProvider)
        {
            var userId = identityProvider.GetUserId();

            var result = await sender.Send(new DeleteItemCommand(userId!, productId));

            return TypedResults.NoContent();
        }
    }
}
