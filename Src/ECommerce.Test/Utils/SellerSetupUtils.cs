using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Test.Utils;

public static class SellerSetupUtils
{
    public static (SellerRepository,
        SellerService,
        SellerController,
        LoginService,
        TokenController)
        SimpleSetupTestEnvironment()
    {
        var dbContext = DatabaseSetupUtils.CreateTestingDatabaseContext();

        var sellerRepository = new SellerRepository(dbContext);
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        return (sellerRepository, sellerService, sellerController, loginService, tokenController);
    }

    public static async Task<(SellerRepository,
        SellerService,
        SellerController,
        SellerDto? createdSeller,
        LoginService,
        TokenController,
        IEnumerable<Claim> claims)> ExistingSellerSetupTestEnvironment()
    {
        var dbContext = DatabaseSetupUtils.CreateTestingDatabaseContext();

        var sellerRepository = new SellerRepository(dbContext);
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        var createSellerDto = new CreateSellerDto
        {
            Cpf = "123.456.789-01",
            Email = "johndoe@email.com",
            Name = "John Doe",
            Telephone = "+00(00)12345-6789"
        };
        var createdSeller = (await sellerController.CreateSeller(createSellerDto) as CreatedResult)
            ?.Value as SellerDto?;

        var loginDto = new LoginDto
        {
            Cpf = createdSeller?.Cpf ?? string.Empty,
            Email = createdSeller?.Email ?? string.Empty
        };

        var jwt = (await tokenController.GenerateJwt(loginDto) as CreatedResult)?.Value as string;
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims.ToList();

        sellerController = new SellerController(sellerService, claims);
        
        return (sellerRepository,
            sellerService,
            sellerController,
            createdSeller,
            loginService,
            tokenController,
            claims);
    }
}