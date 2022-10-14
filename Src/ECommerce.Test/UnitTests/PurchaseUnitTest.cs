using System;
using ECommerce.Api.Controllers;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Dtos.Seller;
using ECommerce.Contracts.Enums;
using ECommerce.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

// ReSharper disable RedundantAssignment

namespace ECommerce.Test.UnitTests;

public class PurchaseUnitTest
{
    public PurchaseUnitTest() => EnvironmentUtils.SetEnvironmentVariables();

    [Fact]
    public async void Create_Purchase_Should_Return_BadRequest()
    {
        // Arrange
        var (sellerService,
            sellerController,
            createdSeller,
            _,
            purchaseController,
            claims) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        sellerController = new SellerController(sellerService, claims);
        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await purchaseController.CreatePurchase();

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Create_Purchase_Should_Return_Created()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) =
            await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        // Act
        var actionResult = await purchaseController.CreatePurchase();

        // Assert
        Assert.True(actionResult is CreatedResult);
    }

    [Fact]
    public async void Delete_Purchase_Should_Return_BadRequest()
    {
        // Arrange
        var (sellerService,
            _, _,
            purchaseService,
            purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        purchaseController = new PurchaseController(purchaseService, sellerService);

        // Act
        var actionResult = await purchaseController.DeletePurchase(Guid.Empty);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Delete_Purchase_Should_Return_Unauthorized()
    {
        // Arrange
        var (sellerService,
            _, _, _,
            purchaseService,
            purchaseController,
            dummyCreatedPurchase,
            requesterClaims) = await PurchaseSetupUtils.CompoundSetupTestEnvironment();

        purchaseController = new PurchaseController(purchaseService, sellerService, requesterClaims);

        // Act
        var actionResult = await purchaseController.DeletePurchase(dummyCreatedPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public async void Delete_Purchase_Should_Return_NotFound()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;
        await purchaseController.DeletePurchase(createdPurchase?.Id ?? Guid.Empty);

        // Act
        var actionResult = await purchaseController.DeletePurchase(createdPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Delete_Purchase_Should_Return_Ok()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        // Act
        var actionResult = await purchaseController.DeletePurchase(createdPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Fact]
    public async void Get_Purchase_By_Id_Should_Return_BadRequest()
    {
        // Arrange
        var (sellerService, _, _,
            purchaseService, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();
        purchaseController = new PurchaseController(purchaseService, sellerService);

        // Act
        var actionResult = await purchaseController.GetPurchaseById(Guid.Empty);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Get_Purchase_By_Id_Should_Return_Unauthorized()
    {
        // Arrange
        var (sellerService,
            _, _, _,
            purchaseService,
            purchaseController,
            dummyCreatedPurchase,
            requesterClaims) = await PurchaseSetupUtils.CompoundSetupTestEnvironment();
        purchaseController = new PurchaseController(purchaseService, sellerService, requesterClaims);

        // Act
        var actionResult = await purchaseController.GetPurchaseById(dummyCreatedPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is UnauthorizedResult);
    }

    [Fact]
    public async void Get_Purchase_By_Id_Should_Return_NotFound()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;
        await purchaseController.DeletePurchase(createdPurchase?.Id ?? Guid.Empty);

        // Act
        var actionResult = await purchaseController.GetPurchaseById(createdPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Get_Purchase_By_Id_Should_Return_Ok()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        // Act
        var actionResult = await purchaseController.GetPurchaseById(createdPurchase?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkObjectResult);
    }

    [Fact]
    public async void Update_Purchase_By_Id_Should_Return_BadRequest_1()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        // Act
        var actionResult = await purchaseController.UpdatePurchase(Guid.Empty, null);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Update_Purchase_By_Id_Should_Return_BadRequest_2()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as SellerDto?;

        // Act
        var actionResult = await purchaseController.UpdatePurchase(createdPurchase?.Id ?? Guid.Empty, null);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Update_Purchase_By_Id_Should_Return_BadRequest_3()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as SellerDto?;

        // Act
        var actionResult = await purchaseController.UpdatePurchase(
            createdPurchase?.Id ?? Guid.Empty,
            (short)800 as PurchaseStatusEnum?);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData(PurchaseStatusEnum.Shipping)]
    [InlineData(PurchaseStatusEnum.Delivered)]
    public async void Update_Purchase_By_Id_When_Invalid_Purchase_Order_Should_Return_BadRequest(
        PurchaseStatusEnum newPurchaseStatus)
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        // Act
        var actionResult = await purchaseController.UpdatePurchase(
            createdPurchase?.Id ?? Guid.Empty,
            newPurchaseStatus);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Update_Purchase_By_Id_Should_Return_NotFound_1()
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();
        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        await purchaseController.DeletePurchase(createdPurchase?.Id ?? Guid.Empty);

        // Act
        var actionResult = await purchaseController.UpdatePurchase(
            createdPurchase?.Id ?? Guid.Empty,
            PurchaseStatusEnum.Rejected);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Update_Purchase_By_Id_Should_Return_NotFound_2()
    {
        // Arrange
        var (sellerService,
            sellerController,
            createdSeller, _,
            purchaseController,
            claims) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();
        sellerController = new SellerController(sellerService, claims);

        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        await sellerController.DeleteSeller(createdSeller?.Id ?? Guid.Empty);

        // Act
        var actionResult = await purchaseController.UpdatePurchase(
            createdPurchase?.Id ?? Guid.Empty,
            PurchaseStatusEnum.Rejected);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Theory]
    [InlineData(PurchaseStatusEnum.Rejected)]
    [InlineData(PurchaseStatusEnum.Cancelled)]
    public async void Update_Purchase_By_Id_Should_Return_Ok(PurchaseStatusEnum newPurchaseStatus)
    {
        // Arrange
        var (_, _, _, _, purchaseController, _) = await PurchaseSetupUtils.SimpleSetupTestEnvironment();
        var createdPurchase = (await purchaseController.CreatePurchase() as CreatedResult)?.Value as PurchaseDto?;

        // Act
        var actionResult = await purchaseController.UpdatePurchase(
            createdPurchase?.Id
            ?? Guid.Empty, newPurchaseStatus);

        // Assert
        Assert.True(actionResult is OkResult);
    }
}