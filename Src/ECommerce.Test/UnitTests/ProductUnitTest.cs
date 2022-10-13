using System;
using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Dtos.Product;
using ECommerce.Data.Context;
using ECommerce.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(
                $"{nameof(Create_Product_Should_Return_BadRequest)}{productName}{productAmount}{productPrice}")
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(
                $"{nameof(Create_Product_Should_Return_Conflict)}{productName}{productAmount}{productPrice}")
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(
                $"{nameof(Create_Product_Should_Return_Created)}{productName}{productAmount}{productPrice}")
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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

    [Theory]
    [InlineData("b9fcf1d2-25ba-44c9-a20d-bfecaa874fdc")]
    [InlineData("f35152e0-d9ab-4784-a944-ba69f55852c0")]
    [InlineData("354f93e2-8bcc-4cbf-9499-b850c1447db4")]
    public async void Delete_Product_Should_Return_NotFound(Guid productId)
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(
                $"{nameof(Delete_Product_Should_Return_NotFound)}{productId}")
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };

        await productController.CreateProduct(createProductDto);

        // Act
        var actionResult = await productController.DeleteProduct(productId);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Delete_Product_Should_Return_Ok()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Delete_Product_Should_Return_Ok))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Get_Product_By_Id_Should_Return_NotFound))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

        var createProductDto = new CreateProductDto
        {
            Name = "Product 1",
            Amount = 5,
            Price = 50.0M
        };

        await productController.CreateProduct(createProductDto);

        // Act
        var actionResult = await productController.GetProductById(Guid.NewGuid());

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Get_Product_By_Id_Should_Return_Ok()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Get_Product_By_Id_Should_Return_Ok))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Update_Product_By_Id_Should_Return_BadRequest))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Update_Product_By_Id_Should_Return_NotFound))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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
            Price = 250M
        };

        // Act
        var actionResult = await productController.UpdateProduct(new Guid(), updateProductDto);

        // Assert
        Assert.True(actionResult is NotFoundResult);
    }

    [Fact]
    public async void Update_Product_By_Id_Should_Return_Ok()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(nameof(Update_Product_By_Id_Should_Return_Ok))
            .Options;
        await using var dbContext = new EcommerceContext(options);

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

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