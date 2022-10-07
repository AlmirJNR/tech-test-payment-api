using System;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Test.UnitTests;

public class TokenUnitTest
{
    public TokenUnitTest()
    {
        Environment.SetEnvironmentVariable("JWT_SUBJECT", "ECOMMERCE_SUBJECT");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "ECOMMERCE_ISSUER");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "ECOMMERCE_BACKEND");
        Environment.SetEnvironmentVariable("JWT_KEY", "9a5835502c69a59d42d5438316d3d43cd2fb0bdd789b3de98f1d0d58e25dbaa5");
    }

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
            .UseInMemoryDatabase("GenerateTokenBadRequest")
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
            .UseInMemoryDatabase("GenerateTokenCreated")
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