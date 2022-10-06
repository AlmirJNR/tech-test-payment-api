using System.Net;
using ECommerce.Contracts.Dtos.PurchaseProduct;

namespace ECommerce.Contracts.Interfaces.Services;

public interface IPurchaseProductService
{
    public Task<(PurchaseProductDto?, HttpStatusCode)> CreatePurchaseProduct(CreatePurchaseProductDto purchaseProductDto);
    public Task<HttpStatusCode> DeletePurchaseProduct(Guid purchaseId, Guid productId);
    public Task<(PurchaseProductDto?, HttpStatusCode)> GetPurchaseProductById(Guid purchaseId, Guid productId);
    public Task<HttpStatusCode> UpdatePurchaseProduct(UpdatePurchaseProductDto purchaseProductDto);
}