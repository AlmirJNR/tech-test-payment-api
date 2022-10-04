using ECommerce.Contracts.Enums;

namespace ECommerce.Contracts.Dtos.Purchase;

public record struct CreatePurchaseDto()
{
    public Guid SellerId { get; init; } = Guid.Empty;
    public PurchaseStatusEnum? PurchaseStatusId { get; set; } = null;

    public Data.Models.Purchase ToModel() => new()
    {
        SellerId = SellerId,
        PurchaseStatusId = (short?) PurchaseStatusId
    };
}