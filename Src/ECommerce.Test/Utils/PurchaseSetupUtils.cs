using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Dtos.Seller;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Test.Utils;

public static class PurchaseSetupUtils
{
    public static async Task<(
        SellerService,
        SellerController,
        SellerDto? createdSeller,
        PurchaseService,
        PurchaseController,
        IEnumerable<Claim>)> SimpleSetupTestEnvironment()
    {
        var dbContext = DatabaseSetupUtils.CreateTestingDatabaseContext();

        var sellerRepository = new SellerRepository(dbContext);
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var createdSeller = (await sellerController.CreateSeller(new CreateSellerDto
        {
            Cpf = "123.456.789-01",
            Name = "John Doe",
            Email = "johndoe@email.com",
            Telephone = "+00(00)12345-6789"
        }) as CreatedResult)?.Value as SellerDto?;

        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        var jwt = (await tokenController.GenerateJwt(new LoginDto
        {
            Cpf = createdSeller?.Cpf ?? string.Empty,
            Email = createdSeller?.Email ?? string.Empty
        }) as CreatedResult)?.Value as string;

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims.ToList();

        var purchaseRepository = new PurchaseRepository(dbContext);
        var purchaseService = new PurchaseService(purchaseRepository);
        var purchaseController = new PurchaseController(purchaseService, sellerService, claims);

        return (sellerService, sellerController, createdSeller, purchaseService, purchaseController, claims);
    }

    public static async Task<(
        SellerService,
        SellerController,
        SellerDto? dummyCreatedSeller,
        SellerDto? requesterCreatedSeller,
        PurchaseService,
        PurchaseController,
        PurchaseDto? dummyCreatedPurchase,
        IEnumerable<Claim>)> CompoundSetupTestEnvironment()
    {
        var dbContext = DatabaseSetupUtils.CreateTestingDatabaseContext();

        var sellerRepository = new SellerRepository(dbContext);
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var dummyCreatedSeller = (await sellerController.CreateSeller(new CreateSellerDto
        {
            Cpf = "123.456.789-01",
            Name = "John Doe",
            Email = "johndoe@email.com",
            Telephone = "+00(00)12345-6789"
        }) as CreatedResult)?.Value as SellerDto?;

        var requesterCreatedSeller = (await sellerController.CreateSeller(new CreateSellerDto
        {
            Cpf = "234.567.890-12",
            Name = "Mary Doe",
            Email = "marydoe@email.com",
            Telephone = "+11(11)23456-7890"
        }) as CreatedResult)?.Value as SellerDto?;

        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        var dummyJwt = (await tokenController.GenerateJwt(new LoginDto
        {
            Cpf = dummyCreatedSeller?.Cpf ?? string.Empty,
            Email = dummyCreatedSeller?.Email ?? string.Empty
        }) as CreatedResult)?.Value as string;
        var dummyClaims = new JwtSecurityTokenHandler().ReadJwtToken(dummyJwt).Claims;

        var requesterJwt = (await tokenController.GenerateJwt(new LoginDto
        {
            Cpf = requesterCreatedSeller?.Cpf ?? string.Empty,
            Email = requesterCreatedSeller?.Email ?? string.Empty
        }) as CreatedResult)?.Value as string;
        var requesterClaims = new JwtSecurityTokenHandler().ReadJwtToken(requesterJwt).Claims;

        var purchaseRepository = new PurchaseRepository(dbContext);
        var purchaseService = new PurchaseService(purchaseRepository);
        var purchaseController = new PurchaseController(purchaseService, sellerService, dummyClaims);

        var dummyCreatedPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        return (sellerService,
            sellerController,
            dummyCreatedSeller,
            requesterCreatedSeller,
            purchaseService,
            purchaseController,
            dummyCreatedPurchase,
            requesterClaims);
    }
}