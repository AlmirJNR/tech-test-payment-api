using ECommerce.Contracts.Enums;

namespace ECommerce.Contracts.Dtos.Purchase;

public readonly record struct PurchaseDto()
{
    public Guid Id { get; init; } = Guid.Empty;
    public Guid SellerId { get; init; } = Guid.Empty;
    public PurchaseStatusEnum? PurchaseStatusId { get; init; } = null;
    public DateTime? CreatedAt { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;

    public static PurchaseDto? FromModel(Data.Models.Purchase? purchaseModel)
    {
        if (purchaseModel is null)
            return null;

        return new PurchaseDto
        {
            Id = purchaseModel.Id,
            SellerId = purchaseModel.SellerId,
            PurchaseStatusId = (PurchaseStatusEnum?)purchaseModel.PurchaseStatusId,
            CreatedAt = purchaseModel.CreatedAt,
            UpdatedAt = purchaseModel.UpdatedAt
        };
    }
}