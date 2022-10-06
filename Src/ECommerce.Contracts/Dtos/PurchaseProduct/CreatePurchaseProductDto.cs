namespace ECommerce.Contracts.Dtos.PurchaseProduct;

public readonly record struct CreatePurchaseProductDto()
{
    public Guid PurchaseId { get; init; } = Guid.Empty;
    public Guid ProductId { get; init; } = Guid.Empty;
    public short ProductAmount { get; init; } = 1;

    public Data.Models.PurchaseProduct ToModel() => new()
    {
        PurchaseId = PurchaseId,
        ProductId = ProductId,
        ProductAmount = ProductAmount
    };
}