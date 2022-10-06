using System.Net;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Data.Context;
using ECommerce.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly EcommerceContext _dbContext;
    private readonly DbSet<Product> _productsEntity;

    public ProductRepository(EcommerceContext dbContext)
    {
        _dbContext = dbContext;
        _productsEntity = _dbContext.Products;
    }

    public async Task<(Product?, HttpStatusCode)> CreateProduct(Product productModel)
    {
        var exists = await _productsEntity
            .FirstOrDefaultAsync(p => p.Name == productModel.Name);
        if (exists is not null)
            return (null, HttpStatusCode.Conflict);

        var addedEntity = await _productsEntity.AddAsync(productModel);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? (null, HttpStatusCode.InternalServerError)
            : (addedEntity.Entity, HttpStatusCode.Created);
    }

    public async Task<HttpStatusCode> DeleteProduct(Guid productId)
    {
        var productEntity = await _productsEntity
            .FirstOrDefaultAsync(p =>
                p.Id == productId
                && p.DeletedAt == null);
        if (productEntity is null)
            return HttpStatusCode.NotFound;

        productEntity.DeletedAt = DateTime.Now;

        _productsEntity.Update(productEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;
    }

    public async Task<(Product?, HttpStatusCode)> GetProductById(Guid productId)
    {
        var productEntity = await _productsEntity
            .FirstOrDefaultAsync(p =>
                p.Id == productId
                && p.DeletedAt == null);

        return productEntity is null
            ? (null, HttpStatusCode.NotFound)
            : (productEntity, HttpStatusCode.OK);
    }

    public async Task<HttpStatusCode> UpdateProduct(Product productModel)
    {
        var productEntity = await _productsEntity
            .FirstOrDefaultAsync(p =>
                p.Id == productModel.Id
                && p.DeletedAt == null);
        if (productEntity is null)
            return HttpStatusCode.NotFound;

        if (!string.IsNullOrWhiteSpace(productModel.Name))
            productEntity.Name = productModel.Name;

        if (productModel.Amount is not null)
            productEntity.Amount = productModel.Amount;

        if (productModel.Price > 0)
            productEntity.Price = productModel.Price;

        _productsEntity.Update(productEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges switch
        {
            0 => HttpStatusCode.NotModified,
            1 => HttpStatusCode.OK,
            _ => HttpStatusCode.InternalServerError
        };
    }
}