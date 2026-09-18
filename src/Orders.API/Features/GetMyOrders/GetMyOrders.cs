using MediatR;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Shared.Models;

namespace Orders.API.Features.GetMyOrders
{
    public record GetMyOrdersQuery(
        string UserId,
        int PageNumber,
        int PageSize) : IRequest<Result<PaginatedList<GetMyOrdersResponse>>>;

    public class GetMyOrders(
        OrdersDbContext context) : IRequestHandler<GetMyOrdersQuery, Result<PaginatedList<GetMyOrdersResponse>>>
    {
        public async Task<Result<PaginatedList<GetMyOrdersResponse>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == request.UserId);

            var totalCount = await query.CountAsync(cancellationToken);

            var ordersPaginated = await query
                .OrderByDescending(o => o.CreatedAtUtc)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new GetMyOrdersResponse(
                    o.Id, 
                    o.CreatedAtUtc,
                    o.Status,
                    o.Address))
                .ToListAsync(cancellationToken);

            return Result<PaginatedList<GetMyOrdersResponse>>
                .Success(new PaginatedList<GetMyOrdersResponse>(
                    ordersPaginated,
                    totalCount,
                    request.PageNumber,
                    request.PageSize));
        }
    }
}
