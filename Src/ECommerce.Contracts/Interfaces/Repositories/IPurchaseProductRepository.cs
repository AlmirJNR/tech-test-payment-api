using System.Net;
using ECommerce.Data.Models;

namespace ECommerce.Contracts.Interfaces.Repositories;

public interface IPurchaseProductRepository
{
    public Task<(PurchaseProduct?, HttpStatusCode)> CreatePurchaseProduct(PurchaseProduct purchaseProductModel);
    public Task<HttpStatusCode> DeletePurchaseProduct(Guid purchaseId, Guid productId);
    public Task<(PurchaseProduct?, HttpStatusCode)> GetPurchaseProductById(Guid purchaseId, Guid productId);
    public Task<HttpStatusCode> UpdatePurchaseProduct(PurchaseProduct purchaseProductModel);
}