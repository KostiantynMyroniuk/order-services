namespace Basket.API.Models
{
    public sealed record ProductSnapshot(
        Guid Id,
        string Name,
        string Description,
        decimal Price);
}
