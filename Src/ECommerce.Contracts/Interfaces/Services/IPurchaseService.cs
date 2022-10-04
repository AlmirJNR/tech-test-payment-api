using System.Net;
using ECommerce.Contracts.Dtos.Purchase;

namespace ECommerce.Contracts.Interfaces.Services;

public interface IPurchaseService
{
    public Task<(PurchaseDto?, HttpStatusCode)> CreatePurchase(CreatePurchaseDto purchaseDto);
    public Task<HttpStatusCode> DeletePurchase(Guid purchaseId);
    public Task<(PurchaseDto?, HttpStatusCode)> GetPurchaseById(Guid purchaseId);
    public Task<HttpStatusCode> UpdatePurchase(Guid purchaseId, UpdatePurchaseDto purchaseDto);
}