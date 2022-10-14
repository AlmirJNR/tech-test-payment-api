using System.Net;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Enums;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;
using ECommerce.Data.Models;

namespace ECommerce.Api.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<(PurchaseDto?, HttpStatusCode)> CreatePurchase(Guid sellerId)
    {
        var (purchaseModel, statusCode) = await _purchaseRepository.CreatePurchase(new Purchase
        {
            SellerId = sellerId,
            PurchaseStatusId = (short)PurchaseStatusEnum.WaitingPayment
        });
        return (PurchaseDto.FromModel(purchaseModel), statusCode);
    }

    public Task<HttpStatusCode> DeletePurchase(Guid purchaseId)
        => _purchaseRepository.DeletePurchase(purchaseId);

    public async Task<(PurchaseDto?, HttpStatusCode)> GetPurchaseById(Guid purchaseId)
    {
        var (purchaseModel, statusCode) = await _purchaseRepository.GetPurchaseById(purchaseId);
        return (PurchaseDto.FromModel(purchaseModel), statusCode);
    }

    public Task<HttpStatusCode> UpdatePurchase(Guid purchaseId, PurchaseStatusEnum? purchaseStatus)
        => _purchaseRepository.UpdatePurchase(new Purchase
        {
            Id = purchaseId,
            PurchaseStatusId = (short?)purchaseStatus
        });
}