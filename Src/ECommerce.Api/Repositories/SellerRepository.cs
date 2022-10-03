using System.Net;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Data.Context;
using ECommerce.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class SellerRepository : ISellerRepository
{
    private readonly EcommerceContext _dbContext;
    private readonly DbSet<Seller> _sellersEntity;

    public SellerRepository(EcommerceContext dbContext)
    {
        _dbContext = dbContext;
        _sellersEntity = _dbContext.Sellers;
    }

    public async Task<(Seller?, HttpStatusCode)> CreateSeller(Seller sellerModel)
    {
        var exists = await _sellersEntity.FirstOrDefaultAsync(s =>
            s.Cpf == sellerModel.Cpf
            || s.Email == sellerModel.Email
            || s.Telephone == sellerModel.Telephone);

        if (exists is not null)
            return (null, HttpStatusCode.Conflict);

        var addedEntity = await _sellersEntity.AddAsync(sellerModel);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? (null, HttpStatusCode.InternalServerError)
            : (addedEntity.Entity, HttpStatusCode.Created);
    }

    public async Task<HttpStatusCode> DeleteSeller(Guid sellerId)
    {
        var sellerEntity = await _sellersEntity.FirstOrDefaultAsync(s => s.Id == sellerId);
        if (sellerEntity is null)
            return HttpStatusCode.NotFound;

        sellerEntity.DeletedAt = DateTime.Now;

        _sellersEntity.Update(sellerEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;
    }

    public async Task<(Seller?, HttpStatusCode)> GetSellerById(Guid sellerId)
    {
        var sellerEntity = await _sellersEntity.FirstOrDefaultAsync(s => s.Id == sellerId);

        return sellerEntity is null
            ? (null, HttpStatusCode.NotFound)
            : (sellerEntity, HttpStatusCode.OK);
    }

    public async Task<(Seller?, HttpStatusCode)> SellerLogin(string cpf, string email)
    {
        var sellerEntity = await _sellersEntity.FirstOrDefaultAsync(s => s.Cpf == cpf && s.Email == email);

        return sellerEntity is null
            ? (null, HttpStatusCode.Forbidden)
            : (sellerEntity, HttpStatusCode.OK);
    }

    public async Task<HttpStatusCode> UpdateSeller(Seller sellerModel)
    {
        var sellerEntity = await _sellersEntity.FirstOrDefaultAsync(s => s.Id == sellerModel.Id);
        if (sellerEntity is null)
            return HttpStatusCode.NotFound;

        if (!string.IsNullOrWhiteSpace(sellerModel.Cpf))
            sellerEntity.Cpf = sellerModel.Cpf;

        if (!string.IsNullOrWhiteSpace(sellerModel.Name))
            sellerEntity.Name = sellerModel.Name;

        if (!string.IsNullOrWhiteSpace(sellerModel.Email))
            sellerEntity.Email = sellerModel.Email;

        if (!string.IsNullOrWhiteSpace(sellerModel.Telephone))
            sellerEntity.Telephone = sellerModel.Telephone;

        _sellersEntity.Update(sellerEntity);
        var savedChanges = await _dbContext.SaveChangesAsync();

        return savedChanges != 1
            ? HttpStatusCode.InternalServerError
            : HttpStatusCode.OK;
    }
}