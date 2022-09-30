using System.Net;
using ECommerce.Contracts.Dtos.Seller;

namespace ECommerce.Contracts.Interfaces.Services;

public interface ISellerService
{
    public Task<(SellerDto?, HttpStatusCode)> CreateSeller(CreateSellerDto sellerDto);
    public Task<HttpStatusCode> DeleteSeller(Guid sellerId);
    public Task<(SellerDto?, HttpStatusCode)> GetSellerById(Guid sellerId);
    public Task<HttpStatusCode> UpdateSeller(Guid sellerId, UpdateSellerDto sellerDto);
}