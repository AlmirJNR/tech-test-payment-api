using System.Net;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;

namespace ECommerce.Contracts.Interfaces.Services;

public interface ILoginService
{
    public Task<(SellerDto?, HttpStatusCode)> SellerLogin(LoginDto loginDto);
}