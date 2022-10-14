using System.Net;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Data.Context;
using ECommerce.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly EcommerceContext _dbContext;
    private readonly DbSet<Purchase> _purchaseTable;

    public PurchaseRepository(EcommerceContext dbContext)
    {
        _dbContext = dbContext;
        _purchaseTable = _dbContext.Purchases;
    }

    public async Task<(Purchase?, HttpStatusCode)> CreatePurchase(Purchase purchaseModel)
    {
        var addedEntity = await _purchaseTable.AddAsync(purchaseModel);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? (null, HttpStatusCode.InternalServerError)
            : (addedEntity.Entity, HttpStatusCode.Created);
    }

    public async Task<HttpStatusCode> DeletePurchase(Guid purchaseId)
    {
        var purchaseEntity = await _purchaseTable
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseId
                && p.DeletedAt == null);
        if (purchaseEntity is null)
            return HttpStatusCode.NotFound;

        purchaseEntity.DeletedAt = DateTime.Now;

        _purchaseTable.Update(purchaseEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;
    }

    public async Task<(Purchase?, HttpStatusCode)> GetPurchaseById(Guid purchaseId)
    {
        var purchaseEntity = await _purchaseTable
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseId
                && p.DeletedAt == null);

        return purchaseEntity is null
            ? (null, HttpStatusCode.NotFound)
            : (purchaseEntity, HttpStatusCode.OK);
    }

    public async Task<HttpStatusCode> UpdatePurchase(Purchase purchaseModel)
    {
        var purchaseEntity = await _purchaseTable
            .FirstOrDefaultAsync(p =>
                p.Id == purchaseModel.Id
                && p.DeletedAt == null);
        if (purchaseEntity is null)
            return HttpStatusCode.NotFound;

        if (purchaseModel.SellerId != Guid.Empty
            && purchaseModel.SellerId != purchaseEntity.SellerId)
            purchaseEntity.SellerId = purchaseModel.SellerId;

        if (purchaseModel.PurchaseStatusId is not null)
            purchaseEntity.PurchaseStatusId = purchaseModel.PurchaseStatusId;

        _purchaseTable.Update(purchaseEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges switch
        {
            0 => HttpStatusCode.NotModified,
            1 => HttpStatusCode.OK,
            _ => HttpStatusCode.InternalServerError
        };
    }
}