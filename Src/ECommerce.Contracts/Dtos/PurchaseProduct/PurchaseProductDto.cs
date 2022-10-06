namespace ECommerce.Contracts.Dtos.PurchaseProduct;

public readonly record struct PurchaseProductDto()
{
    public Guid PurchaseId { get; init; } = Guid.Empty;
    public Guid ProductId { get; init; } = Guid.Empty;
    public short ProductAmount { get; init; } = 1;
    public DateTime? CreatedAt { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;

    public static PurchaseProductDto? FromModel(Data.Models.PurchaseProduct? purchaseProductModel)
    {
        if (purchaseProductModel is null)
            return null;

        return new PurchaseProductDto
        {
            PurchaseId = purchaseProductModel.PurchaseId,
            ProductId = purchaseProductModel.ProductId,
            ProductAmount = purchaseProductModel.ProductAmount ?? 0,
            CreatedAt = purchaseProductModel.CreatedAt,
            UpdatedAt = purchaseProductModel.UpdatedAt
        };
    }
}