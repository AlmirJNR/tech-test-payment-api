using ECommerce.Contracts.Enums;

namespace ECommerce.Contracts.Dtos.Purchase;

public readonly record struct UpdatePurchaseDto()
{
    public Guid? SellerId { get; init; } = null;
    public PurchaseStatusEnum? PurchaseStatusId { get; init; } = null;

    public Data.Models.Purchase ToModel() => new()
    {
        SellerId = SellerId ?? Guid.Empty,
        PurchaseStatusId = (short?) PurchaseStatusId
    };
}