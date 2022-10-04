using System.Net;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;

namespace ECommerce.Api.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<(PurchaseDto?, HttpStatusCode)> CreatePurchase(CreatePurchaseDto purchaseDto)
    {
        var (purchaseModel, statusCode) = await _purchaseRepository.CreatePurchase(purchaseDto.ToModel());
        if (purchaseModel is null)
            return (null, statusCode);

        return (PurchaseDto.FromModel(purchaseModel), statusCode);
    }

    public Task<HttpStatusCode> DeletePurchase(Guid purchaseId)
        => _purchaseRepository.DeletePurchase(purchaseId);

    public async Task<(PurchaseDto?, HttpStatusCode)> GetPurchaseById(Guid purchaseId)
    {
        var (purchaseModel, statusCode) = await _purchaseRepository.GetPurchaseById(purchaseId);
        if (purchaseModel is null)
            return (null, statusCode);

        return (PurchaseDto.FromModel(purchaseModel), statusCode);
    }

    public Task<HttpStatusCode> UpdatePurchase(Guid purchaseId, UpdatePurchaseDto purchaseDto)
    {
        var purchaseModel = purchaseDto.ToModel();
        purchaseModel.Id = purchaseId;

        return _purchaseRepository.UpdatePurchase(purchaseModel);
    }
}