using System.Net;
using ECommerce.Contracts.Dtos.PurchaseProduct;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;

namespace ECommerce.Api.Services;

public class PurchaseProductService : IPurchaseProductService
{
    private readonly IPurchaseProductRepository _purchaseProductRepository;

    public PurchaseProductService(IPurchaseProductRepository purchaseProductRepository)
    {
        _purchaseProductRepository = purchaseProductRepository;
    }

    public async Task<(PurchaseProductDto?, HttpStatusCode)> CreatePurchaseProduct(CreatePurchaseProductDto purchaseProductDto)
    {
        var (purchaseProductModel, statusCode) = await _purchaseProductRepository.CreatePurchaseProduct(purchaseProductDto.ToModel());
        return (PurchaseProductDto.FromModel(purchaseProductModel), statusCode);
    }

    public Task<HttpStatusCode> DeletePurchaseProduct(Guid purchaseId, Guid productId)
        => _purchaseProductRepository.DeletePurchaseProduct(purchaseId, productId);

    public async Task<(PurchaseProductDto?, HttpStatusCode)> GetPurchaseProductById(Guid purchaseId, Guid productId)
    {
        var (purchaseProductModel, statusCode) =
            await _purchaseProductRepository.GetPurchaseProductById(purchaseId, productId);
        return (PurchaseProductDto.FromModel(purchaseProductModel), statusCode);
    }

    public Task<HttpStatusCode> UpdatePurchaseProduct(UpdatePurchaseProductDto purchaseProductDto)
        => _purchaseProductRepository.UpdatePurchaseProduct(purchaseProductDto.ToModel());
}