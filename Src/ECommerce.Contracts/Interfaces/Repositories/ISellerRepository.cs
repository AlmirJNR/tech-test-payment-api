using System.Net;
using ECommerce.Data.Models;

namespace ECommerce.Contracts.Interfaces.Repositories;

public interface ISellerRepository
{
    public Task<(Seller?, HttpStatusCode)> CreateSeller(Seller sellerModel);
    public Task<HttpStatusCode> DeleteSeller(Guid sellerId);
    public Task<(Seller?, HttpStatusCode)> GetSellerById(Guid sellerId);
    public Task<(Seller?, HttpStatusCode)> SellerLogin(string cpf, string email);
    public Task<HttpStatusCode> UpdateSeller(Seller sellerModel);
}