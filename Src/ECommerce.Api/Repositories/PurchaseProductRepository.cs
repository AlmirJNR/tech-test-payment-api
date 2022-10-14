using System.Net;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Data.Context;
using ECommerce.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class PurchaseProductRepository : IPurchaseProductRepository
{
    private readonly EcommerceContext _dbContext;
    private readonly DbSet<PurchaseProduct> _purchaseProductTable;

    public PurchaseProductRepository(EcommerceContext dbContext)
    {
        _dbContext = dbContext;
        _purchaseProductTable = _dbContext.PurchaseProducts;
    }

    public async Task<(PurchaseProduct?, HttpStatusCode)> CreatePurchaseProduct(PurchaseProduct purchaseProductModel)
    {
        var exists = await _purchaseProductTable
            .FirstOrDefaultAsync(pp =>
                pp.PurchaseId == purchaseProductModel.PurchaseId
                && pp.ProductId == purchaseProductModel.ProductId
                && pp.DeletedAt == null);
        if (exists is not null)
            return (null, HttpStatusCode.Conflict);

        var addedEntity = _purchaseProductTable.Add(purchaseProductModel);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? (null, HttpStatusCode.InternalServerError)
            : (addedEntity.Entity, HttpStatusCode.Created);
    }

    public async Task<HttpStatusCode> DeletePurchaseProduct(Guid purchaseId, Guid productId)
    {
        var purchaseProductEntity = await _purchaseProductTable
            .FirstOrDefaultAsync(pp =>
                pp.PurchaseId == purchaseId
                && pp.ProductId == productId
                && pp.DeletedAt == null);
        if (purchaseProductEntity is null)
            return HttpStatusCode.NotFound;

        purchaseProductEntity.DeletedAt = DateTime.Now;

        _purchaseProductTable.Update(purchaseProductEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;
    }

    public async Task<(PurchaseProduct?, HttpStatusCode)> GetPurchaseProductById(Guid purchaseId, Guid productId)
    {
        var purchaseProductEntity = await _purchaseProductTable
            .FirstOrDefaultAsync(pp =>
                pp.PurchaseId == purchaseId
                && pp.ProductId == productId
                && pp.DeletedAt == null);

        return purchaseProductEntity is null
            ? (null, HttpStatusCode.NotFound)
            : (purchaseProductEntity, HttpStatusCode.OK);
    }

    public async Task<HttpStatusCode> UpdatePurchaseProduct(PurchaseProduct purchaseProductModel)
    {
        var purchaseProductEntity = await _purchaseProductTable
            .FirstOrDefaultAsync(pp =>
                pp.PurchaseId == purchaseProductModel.PurchaseId
                && pp.ProductId == purchaseProductModel.ProductId
                && pp.DeletedAt == null);
        if (purchaseProductEntity is null)
            return HttpStatusCode.NotFound;

        if (purchaseProductEntity.PurchaseId != Guid.Empty)
            purchaseProductEntity.PurchaseId = purchaseProductModel.PurchaseId;

        if (purchaseProductEntity.ProductId != Guid.Empty)
            purchaseProductEntity.ProductId = purchaseProductModel.ProductId;

        if (purchaseProductEntity.ProductAmount != 0)
            purchaseProductEntity.ProductAmount = purchaseProductModel.ProductAmount;

        _purchaseProductTable.Update(purchaseProductEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges switch
        {
            0 => HttpStatusCode.NotModified,
            1 => HttpStatusCode.OK,
            _ => HttpStatusCode.InternalServerError
        };
    }
}