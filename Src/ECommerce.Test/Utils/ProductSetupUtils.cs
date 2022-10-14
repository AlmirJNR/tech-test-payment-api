using ECommerce.Api.Controllers;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;

namespace ECommerce.Test.Utils;

public static class ProductSetupUtils
{
    public static (ProductRepository, ProductService, ProductController) SimpleSetupTestEnvironment()
    {
        var dbContext = DatabaseSetupUtils.CreateTestingDatabaseContext();

        var productRepository = new ProductRepository(dbContext);
        var productService = new ProductService(productRepository);
        var productController = new ProductController(productService);

        return (productRepository, productService, productController);
    }
}