using System.Net;
using ECommerce.Data.Models;

namespace ECommerce.Contracts.Interfaces.Repositories;

public interface IProductRepository
{
    public Task<(Product?, HttpStatusCode)> CreateProduct(Product productModel);
    public Task<HttpStatusCode> DeleteProduct(Guid productId);
    public Task<(Product?, HttpStatusCode)> GetProductById(Guid productId);
    public Task<HttpStatusCode> UpdateProduct(Product productModel);
}