namespace ECommerce.Contracts.Dtos.PurchaseProduct;

public readonly record struct UpdatePurchaseProductDto()
{
    public Guid? PurchaseId { get; init; } = null;
    public Guid? ProductId { get; init; } = null;
    public short? ProductAmount { get; init; } = null;

    public Data.Models.PurchaseProduct ToModel() => new()
    {
        PurchaseId = PurchaseId ?? Guid.Empty,
        ProductId = ProductId ?? Guid.Empty,
        ProductAmount = ProductAmount
    };
}