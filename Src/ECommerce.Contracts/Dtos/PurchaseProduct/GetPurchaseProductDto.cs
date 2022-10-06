namespace ECommerce.Contracts.Dtos.PurchaseProduct;

public readonly record struct GetPurchaseProductDto()
{
    public Guid PurchaseId { get; init; } = Guid.Empty;
    public Guid ProductId { get; init; } = Guid.Empty;
}