using System.Net;
using ECommerce.Data.Models;

namespace ECommerce.Contracts.Interfaces.Repositories;

public interface IPurchaseRepository
{
    public Task<(Purchase?, HttpStatusCode)> CreatePurchase(Purchase purchaseModel);
    public Task<HttpStatusCode> DeletePurchase(Guid purchaseId);
    public Task<(Purchase?, HttpStatusCode)> GetPurchaseById(Guid purchaseId);
    public Task<HttpStatusCode> UpdatePurchase(Purchase purchaseModel);
}