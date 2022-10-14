using System.Net;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Enums;

namespace ECommerce.Contracts.Interfaces.Services;

public interface IPurchaseService
{
    public Task<(PurchaseDto?, HttpStatusCode)> CreatePurchase(Guid sellerId);
    public Task<HttpStatusCode> DeletePurchase(Guid purchaseId);
    public Task<(PurchaseDto?, HttpStatusCode)> GetPurchaseById(Guid purchaseId);
    public Task<HttpStatusCode> UpdatePurchase(Guid purchaseId, PurchaseStatusEnum? purchaseStatus);
}