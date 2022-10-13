using System.Reflection;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Data.Context;
using ECommerce.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Test.UnitTests;

public class TokenUnitTest
{
    public TokenUnitTest() => EnvironmentUtils.SetEnvironmentVariables();

    [Theory]
    [InlineData(
        "12345656789",
        "valid@email.com",
        "John Doe",
        "+55(00)12345-6789")]
    [InlineData(
        "123.456.567-89",
        "invalidemail.com",
        "John Doe",
        "+55(00)12345-6789")]
    [InlineData(
        "123.456.567-89",
        "invalidemail.com",
        "John Doe",
        "5500123456789")]
    public async void Generate_Token_Should_Return_BadRequest(
        string sellerCpf,
        string sellerEmail,
        string sellerName,
        string sellerTelephone)
    {
        // Arrange - Shared
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(MethodBase.GetCurrentMethod()!.Name)
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var sellerRepository = new SellerRepository(dbContext);

        // Arrange - Seller Exclusive
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var createSellerDto = new CreateSellerDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail,
            Name = sellerName,
            Telephone = sellerTelephone
        };

        await sellerController.CreateSeller(createSellerDto);

        // Arrange - Token Exclusive
        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        var loginDto = new LoginDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail
        };

        // Act
        var actionResult = await tokenController.GenerateJwt(loginDto);

        // Assert
        Assert.True(actionResult is BadRequestResult);
    }

    [Theory]
    [InlineData(
        "123.456.567-89",
        "valid@email.com",
        "John Doe",
        "+55(00)12345-6789")]
    public async void Generate_Token_Should_Return_Created(
        string sellerCpf,
        string sellerEmail,
        string sellerName,
        string sellerTelephone)
    {
        // Arrange - Shared
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(MethodBase.GetCurrentMethod()!.Name)
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var sellerRepository = new SellerRepository(dbContext);

        // Arrange - Seller Exclusive
        var sellerService = new SellerService(sellerRepository);
        var sellerController = new SellerController(sellerService);

        var createSellerDto = new CreateSellerDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail,
            Name = sellerName,
            Telephone = sellerTelephone
        };

        await sellerController.CreateSeller(createSellerDto);

        // Arrange - Token Exclusive
        var loginService = new LoginService(sellerRepository);
        var tokenController = new TokenController(loginService);

        var loginDto = new LoginDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail
        };

        // Act
        var actionResult = await tokenController.GenerateJwt(loginDto);

        // Assert
        Assert.True(actionResult is CreatedResult);
    }
}