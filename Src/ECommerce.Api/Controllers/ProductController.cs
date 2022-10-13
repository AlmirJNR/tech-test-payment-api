using System.Net;
using ECommerce.Contracts.Dtos.Product;
using ECommerce.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

/// <response code="500">Internal Server Error</response>
/// <response code="400">Bad request</response>
/// <response code="401">Unauthorized</response>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Creates a product.
    /// </summary>
    /// <param name="productDto"></param>
    /// <response code="409">Conflict</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.Name))
            return BadRequest("Missing product name");
        if (productDto.Amount is null or < 0)
            return BadRequest("Invalid product amount");
        if (productDto.Price <= 0)
            return BadRequest("Invalid product price");

        var (createdProduct, statusCode) = await _productService.CreateProduct(productDto);

        return statusCode switch
        {
            HttpStatusCode.Conflict => Conflict(),
            HttpStatusCode.Created => Created(string.Empty, createdProduct),
            _ => Problem()
        };
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="productId"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpDelete("{productId:Guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
    {
        var statusCode = await _productService.DeleteProduct(productId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }

    /// <summary>
    /// Get product by Id.
    /// </summary>
    /// <param name="productId"></param>
    /// <response code="404">Not Found</response>
    [HttpGet("{productId:Guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductById([FromRoute] Guid productId)
    {
        var (productDto, statusCode) = await _productService.GetProductById(productId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(productDto),
            _ => Problem()
        };
    }

    /// <summary>
    /// Update almost any field from the product with the informed Id.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="productDto"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpPut("{productId:Guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid productId, [FromBody] UpdateProductDto productDto)
    {
        if (string.IsNullOrWhiteSpace(productDto.Name)
            && productDto.Amount is null or < 0
            && productDto.Price is null or <= 0)
            return BadRequest("Nothing to update");

        var statusCode = await _productService.UpdateProduct(productId, productDto);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }
}