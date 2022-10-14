using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Data.Context;
using ECommerce.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Xunit;

// ReSharper disable RedundantAssignment

namespace ECommerce.Test.UnitTests;

public class SellerUnitTest
{
    public SellerUnitTest() => EnvironmentUtils.SetEnvironmentVariables();

    [Theory]
    [InlineData(
        "12345656789",
        "valid@email.com",
        "John Doe",
        "+00(00)12345-6789")]
    [InlineData(
        "123.456.789-01",
        "invalidemail.com",
        "John Doe",
        "+00(00)12345-6789")]
    [InlineData(
        "123.456.789-01",
        "invalidemail.com",
        "John Doe",
        "0000123456789")]
    public async void Create_Seller_Should_Return_BadRequest(
        string sellerCpf,
        string sellerEmail,
        string sellerName,
        string sellerTelephone)
    {
        // Arrange
        var (_, _, sellerController, _, _) = SellerSetupUtils.SimpleSetupTestEnvironment();

        // Act
        var createSellerDto = new CreateSellerDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail,
            Name = sellerName,
            Telephone = sellerTelephone
        };

        var actionResult = await sellerController.CreateSeller(createSellerDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Create_Seller_Should_Return_Conflict()
    {
        // Arrange
        var (_, _, sellerController, _, _) = SellerSetupUtils.SimpleSetupTestEnvironment();

        var createFirstSellerDto = new CreateSellerDto
        {
            Cpf = "123.456.789-01",
            Email = "johndoe@email.com",
            Name = "John Doe",
            Telephone = "+00(00)12345-6789"
        };
        await sellerController.CreateSeller(createFirstSellerDto);

        var createSecondSellerDto = new CreateSellerDto
        {
            Cpf = "123.456.789-01",
            Email = "johndoe@email.com",
            Name = "John D.",
            Telephone = "+00(00)12345-6789"
        };

        // Act
        var actionResult = await sellerController.CreateSeller(createSecondSellerDto);

        // Assert
        Assert.True(actionResult is ConflictResult);
    }

    [Theory]
    [InlineData(
        "123.456.789-01",
        "johndoe@email.com",
        "John Doe",
        "+00(00)12345-6789")]
    [InlineData(
        "234.567.890-12",
        "janedoe@gmail.com",
        "Jane Doe",
        "+11(11)12345-6789")]
    public async void Create_Seller_Should_Return_Created(
        string sellerCpf,
        string sellerEmail,
        string sellerName,
        string sellerTelephone)
    {
        // Arrange
        var (_, _, sellerController, _, _) = SellerSetupUtils.SimpleSetupTestEnvironment();

        var createSellerDto = new CreateSellerDto
        {
            Cpf = sellerCpf,
            Email = sellerEmail,
            Name = sellerName,
            Telephone = sellerTelephone
        };

        // Act
        var actionResult = await sellerController.CreateSeller(createSellerDto);

        // Assert
        Assert.True(actionResult is CreatedResult);
    }

    [Fact]
    public async void Delete_Seller_Should_Return_Unauthorized()
    {
        // Arrange
        var (_, _, sellerController, _, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        // Act
        var actionResult = await sellerController.DeleteSeller(Guid.NewGuid());

        // Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public async void Delete_Seller_Should_Return_BadRequest()
    {
        // Arrange
        var (_, sellerService,
            sellerController,
            createdSeller, _, _,
            claims) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var claimsArray = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("JWT_SUBJECT")
                                             ?? throw new ArgumentException("Missing JWT_SUBJECT variable")),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
        };
        var keyAsByteArray = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
                                                    ?? throw new ArgumentException("Missing JWT_KEY variable"));
        var key = new SymmetricSecurityKey(keyAsByteArray);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var encryptedToken = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER")
                    ?? throw new ArgumentException("Missing JWT_ISSUER variable"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                      ?? throw new ArgumentException("Missing JWT_AUDIENCE variable"),
            claims: claimsArray,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        claims = new JwtSecurityTokenHandler()
            .ReadJwtToken(new JwtSecurityTokenHandler().WriteToken(encryptedToken))
            .Claims;

        sellerController = new SellerController(sellerService, claims);

        // Act
        var actionResult = await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is BadRequestResult);
    }

    [Fact]
    public async void Delete_Seller_Should_Return_NotFound()
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Delete_Seller_Should_Return_Ok()
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        // Act
        var actionResult = await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Fact]
    public async void Get_Seller_By_Id_Should_Return_Unauthorized()
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await sellerController.GetSellerById(Guid.NewGuid());

        // Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public async void Get_Seller_By_Id_Should_Return_BadRequest()
    {
        // Arrange
        var (_, sellerService,
            sellerController,
            createdSeller, _, _,
            claims) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var claimsArray = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("JWT_SUBJECT")
                                             ?? throw new ArgumentException("Missing JWT_SUBJECT variable")),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
        };
        var keyAsByteArray = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
                                                    ?? throw new ArgumentException("Missing JWT_KEY variable"));
        var key = new SymmetricSecurityKey(keyAsByteArray);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var encryptedToken = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER")
                    ?? throw new ArgumentException("Missing JWT_ISSUER variable"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                      ?? throw new ArgumentException("Missing JWT_AUDIENCE variable"),
            claims: claimsArray,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        claims = new JwtSecurityTokenHandler()
            .ReadJwtToken(new JwtSecurityTokenHandler().WriteToken(encryptedToken))
            .Claims;

        sellerController = new SellerController(sellerService, claims);
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await sellerController.GetSellerById(Guid.NewGuid());

        // Assert
        Assert.True(actionResult is BadRequestResult);
    }

    [Fact]
    public async void Get_Seller_By_Id_Should_Return_NotFound()
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await sellerController.GetSellerById(createdSeller?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Get_Seller_By_Id_Should_Return_Ok()
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        // Act
        var actionResult = await sellerController.GetSellerById(createdSeller?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkObjectResult);
    }

    [Theory]
    [InlineData(
        "123.456.789-01",
        "johndoe@email.com",
        "John Doe",
        "+00(00)12345-6789")]
    public async void Update_Seller_By_Id_Should_Return_Unauthorized(
        string newSellerCpf,
        string newSellerEmail,
        string newSellerName,
        string newSellerTelephone)
    {
        // Arrange
        var (_, _, sellerController, _, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Cpf = newSellerCpf,
            Name = newSellerName,
            Email = newSellerEmail,
            Telephone = newSellerTelephone
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(Guid.NewGuid(), updateSellerDto);

        // Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Theory]
    [InlineData("123.456.56789")]
    [InlineData("123.45656789")]
    [InlineData("12345656789")]
    [InlineData("1.2.3.4.5.6.5.6.7.8.9")]
    [InlineData("123.456.567.89")]
    public async void Update_Seller_Cpf_By_Id_Should_Return_BadRequest(string newSellerCpf)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Cpf = newSellerCpf
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData("email@email.")]
    [InlineData("email.com")]
    [InlineData("email.com.com.")]
    [InlineData("email")]
    public async void Update_Seller_Email_By_Id_Should_Return_BadRequest(string newSellerEmail)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Email = newSellerEmail
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData("")]
    public async void Update_Seller_Name_By_Id_Should_Return_BadRequest(string newSellerName)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Name = newSellerName
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData("55(00)12345-6789")]
    [InlineData("+5500)12345-6789")]
    [InlineData("+55(0012345-6789")]
    [InlineData("+55(00)123456789")]
    [InlineData("+55(00)12-3456789")]
    [InlineData("+55(00)12345678-9")]
    public async void Update_Seller_Telephone_By_Id_Should_Return_BadRequest(string newSellerTelephone)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Telephone = newSellerTelephone
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData("+55(00)23456-7890")]
    public async void Update_Seller_Any_Value_By_Id_Should_Return_NotFound(string newSellerTelephone)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Telephone = newSellerTelephone
        };
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Theory]
    [InlineData(
        "123.456.789-01",
        "johndoe@email.com",
        "John Doe",
        "+00(00)12345-6789")]
    public async void Update_Seller_Values_With_Same_Existing_Values_By_Id_Should_Return_Ok(
        string newSellerCpf,
        string newSellerEmail,
        string newSellerName,
        string newSellerTelephone)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Cpf = newSellerCpf,
            Name = newSellerName,
            Email = newSellerEmail,
            Telephone = newSellerTelephone
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Theory]
    [InlineData("123.456.789-01")]
    [InlineData("234.567.890-12")]
    [InlineData("345.678.901-23")]
    public async void Update_Seller_Cpf_By_Id_Should_Return_Ok(string newSellerCpf)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Cpf = newSellerCpf
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Theory]
    [InlineData("email@email.com")]
    [InlineData("newemail@email.com")]
    [InlineData("anotherEmail@Email.com")]
    public async void Update_Seller_Email_By_Id_Should_Return_Ok(string newSellerEmail)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Email = newSellerEmail
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Theory]
    [InlineData("A Name")]
    [InlineData("Another Good Name")]
    [InlineData("A Super Good Name")]
    public async void Update_Seller_Name_By_Id_Should_Return_Ok(string newSellerName)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Name = newSellerName
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Theory]
    [InlineData("+55(00)12345-6789")]
    [InlineData("+10(58)1345-6789")]
    [InlineData("+25(90)45685-6785")]
    public async void Update_Seller_Telephone_By_Id_Should_Return_Ok(string newSellerTelephone)
    {
        // Arrange
        var (_, _,
            sellerController,
            createdSeller, _, _, _) = await SellerSetupUtils.ExistingSellerSetupTestEnvironment();

        var updateSellerDto = new UpdateSellerDto
        {
            Telephone = newSellerTelephone
        };

        // Act
        var actionResult = await sellerController.UpdateSeller(createdSeller?.Id ?? Guid.Empty, updateSellerDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }
}