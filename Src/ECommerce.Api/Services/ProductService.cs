using System.Net;
using ECommerce.Contracts.Dtos.Product;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;

namespace ECommerce.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<(ProductDto?, HttpStatusCode)> CreateProduct(CreateProductDto productDto)
    {
        var (productModel, statusCode) = await _productRepository.CreateProduct(productDto.ToModel());
        return (ProductDto.FromModel(productModel), statusCode);
    }

    public Task<HttpStatusCode> DeleteProduct(Guid productId)
        => _productRepository.DeleteProduct(productId);

    public async Task<(ProductDto?, HttpStatusCode)> GetProductById(Guid productId)
    {
        var (productModel, statusCode) = await _productRepository.GetProductById(productId);
        return (ProductDto.FromModel(productModel), statusCode);
    }

    public Task<HttpStatusCode> UpdateProduct(Guid productId, UpdateProductDto productDto)
    {
        var productModel = productDto.ToModel();
        productModel.Id = productId;

        return _productRepository.UpdateProduct(productModel);
    }
}