using System.Net;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;

namespace ECommerce.Api.Services;

public class LoginService : ILoginService
{
    private readonly ISellerRepository _sellerRepository;

    public LoginService(ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
    }

    public async Task<(SellerDto?, HttpStatusCode)> SellerLogin(LoginDto loginDto)
    {
        var (sellerEntity, statusCode) = await _sellerRepository.SellerLogin(loginDto.Cpf, loginDto.Email);
        if (sellerEntity is null)
            return (null, statusCode);

        return (SellerDto.FromModel(sellerEntity), statusCode);
    }
}