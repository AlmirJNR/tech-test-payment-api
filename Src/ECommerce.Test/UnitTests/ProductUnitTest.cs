using System;
using ECommerce.Contracts.Dtos.Product;
using ECommerce.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ECommerce.Test.UnitTests;

public class ProductUnitTest
{
    public ProductUnitTest() => EnvironmentUtils.SetEnvironmentVariables();

    [Theory]
    [InlineData("Product 1", 0, 0)]
    [InlineData("Product 2", 1, 0)]
    [InlineData("Product 3", -1, 1)]
    public async void Create_Product_Should_Return_BadRequest(
        string productName,
        short productAmount,
        decimal productPrice)
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = productName,
            Amount = productAmount,
            Price = productPrice
        };

        // Act
        var actionResult = await productController.CreateProduct(createProductDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Theory]
    [InlineData("Product 1", 15, 250)]
    [InlineData("Product 2", 30, 30)]
    [InlineData("Product 3", 10, 15)]
    public async void Create_Product_Should_Return_Conflict(
        string productName,
        short productAmount,
        decimal productPrice)
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = productName,
            Amount = productAmount,
            Price = productPrice
        };
        await productController.CreateProduct(createProductDto);

        // Act
        var actionResult = await productController.CreateProduct(createProductDto);

        // Assert
        Assert.True(actionResult is ConflictResult);
    }

    [Theory]
    [InlineData("Product 1", 15, 250)]
    [InlineData("Product 2", 30, 30)]
    [InlineData("Product 3", 10, 15)]
    public async void Create_Product_Should_Return_Created(
        string productName,
        short productAmount,
        decimal productPrice)
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = productName,
            Amount = productAmount,
            Price = productPrice
        };

        // Act
        var actionResult = await productController.CreateProduct(createProductDto);

        // Assert
        Assert.True(actionResult is CreatedResult);
    }

    [Fact]
    public async void Delete_Product_Should_Return_NotFound()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        // Act
        var actionResult = await productController.DeleteProduct(Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Delete_Product_Should_Return_Ok()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };
        var createdProductDto = (await productController.CreateProduct(createProductDto) as CreatedResult)
            ?.Value as ProductDto?;

        // Act
        var actionResult = await productController.DeleteProduct(createdProductDto?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkResult);
    }

    [Fact]
    public async void Get_Product_By_Id_Should_Return_NotFound()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        // Act
        var actionResult = await productController.GetProductById(Guid.Empty);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Get_Product_By_Id_Should_Return_Ok()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };
        var createdProductDto = (await productController.CreateProduct(createProductDto) as CreatedResult)
            ?.Value as ProductDto?;

        // Act
        var actionResult = await productController.GetProductById(createdProductDto?.Id ?? Guid.Empty);

        // Assert
        Assert.True(actionResult is OkObjectResult);
    }

    [Fact]
    public async void Update_Product_By_Id_Should_Return_BadRequest()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };
        var createdProductDto = (await productController.CreateProduct(createProductDto) as CreatedResult)
            ?.Value as ProductDto?;

        var updateProductDto = new UpdateProductDto
        {
            Name = "",
            Amount = -1,
            Price = 0
        };

        // Act
        var actionResult = await productController.UpdateProduct(createdProductDto?.Id ?? Guid.Empty, updateProductDto);

        // Assert
        Assert.True(actionResult is BadRequestObjectResult);
    }

    [Fact]
    public async void Update_Product_By_Id_Should_Return_NotFound()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };
        await productController.CreateProduct(createProductDto);

        var updateProductDto = new UpdateProductDto
        {
            Name = "product 2",
            Amount = 50,
            Price = 250.0M
        };

        // Act
        var actionResult = await productController.UpdateProduct(Guid.NewGuid(), updateProductDto);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Update_Product_By_Id_Should_Return_Ok()
    {
        // Arrange
        var (_, _, productController) = ProductSetupUtils.SimpleSetupTestEnvironment();

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };

        var createdProductDto = (await productController.CreateProduct(createProductDto) as CreatedResult)
            ?.Value as ProductDto?;

        var updateProductDto = new UpdateProductDto
        {
            Name = "product 2",
            Amount = 50,
            Price = 250M
        };

        // Act
        var actionResult = await productController.UpdateProduct(createdProductDto?.Id ?? Guid.Empty, updateProductDto);

        // Assert
        Assert.True(actionResult is OkResult);
    }
}