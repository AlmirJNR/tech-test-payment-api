using System.Net;
using ECommerce.Contracts.Dtos.Product;

namespace ECommerce.Contracts.Interfaces.Services;

public interface IProductService
{
    public Task<(ProductDto?, HttpStatusCode)> CreateProduct(CreateProductDto productDto);
    public Task<HttpStatusCode> DeleteProduct(Guid productId);
    public Task<(ProductDto?, HttpStatusCode)> GetProductById(Guid productId);
    public Task<HttpStatusCode> UpdateProduct(Guid productId, UpdateProductDto productDto);
}