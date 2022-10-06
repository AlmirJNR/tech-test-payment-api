using System.Net;
using ECommerce.Contracts.Dtos.Product;
using ECommerce.Contracts.Dtos.PurchaseProduct;
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
public class PurchaseProductController : ControllerBase
{
    private readonly IPurchaseProductService _purchaseProductService;
    private readonly IPurchaseService _purchaseService;
    private readonly IProductService _productService;

    public PurchaseProductController(
        IPurchaseProductService purchaseProductService,
        IPurchaseService purchaseService,
        IProductService productService)
    {
        _purchaseProductService = purchaseProductService;
        _purchaseService = purchaseService;
        _productService = productService;
    }

    /// <summary>
    /// Creates a purchase product many to many relation.
    /// </summary>
    /// <param name="purchaseProductDto"></param>
    /// <response code="409">Conflict</response>
    /// <response code="201">Created</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreatePurchaseProductDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePurchaseProduct([FromBody] CreatePurchaseProductDto purchaseProductDto)
    {
        var (purchaseExists, _) = await _purchaseService.GetPurchaseById(purchaseProductDto.PurchaseId);
        if (purchaseExists is null || purchaseProductDto.PurchaseId == Guid.Empty)
            return BadRequest("Purchase id doesn't exists");

        var (productExists, _) = await _productService.GetProductById(purchaseProductDto.ProductId);
        if (productExists is null || purchaseProductDto.ProductId == Guid.Empty)
            return BadRequest("Product id doesn't exists");

        if (purchaseProductDto.ProductAmount < 1)
            return BadRequest("Invalid product amount");

        var updateProductStatusCode = await _productService.UpdateProduct(purchaseProductDto.ProductId,
            new UpdateProductDto
            {
                Amount = (short)(productExists.Value.Amount! - purchaseProductDto.ProductAmount)
            });
        if (updateProductStatusCode != HttpStatusCode.OK)
            return BadRequest();

        var (createdPurchaseProduct, statusCode) = await _purchaseProductService
            .CreatePurchaseProduct(purchaseProductDto);

        return statusCode switch
        {
            HttpStatusCode.Conflict => Conflict(),
            HttpStatusCode.Created => Created(string.Empty, createdPurchaseProduct),
            _ => Problem()
        };
    }

    /// <summary>
    /// Deletes a purchase product many to many relation.
    /// </summary>
    /// <param name="deletePurchaseProductDto"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpDelete]
    public async Task<IActionResult> DeletePurchaseProduct([FromBody] DeletePurchaseProductDto deletePurchaseProductDto)
    {
        var statusCode = await _purchaseProductService
            .DeletePurchaseProduct(deletePurchaseProductDto.PurchaseId, deletePurchaseProductDto.ProductId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }

    /// <summary>
    /// Get a purchase product many to many relation by relation Ids.
    /// </summary>
    /// <param name="getPurchaseProductDto"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpGet]
    public async Task<IActionResult> GetPurchaseProductById([FromBody] GetPurchaseProductDto getPurchaseProductDto)
    {
        var (purchaseProductDto, statusCode) = await _purchaseProductService
            .GetPurchaseProductById(getPurchaseProductDto.PurchaseId, getPurchaseProductDto.ProductId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(purchaseProductDto),
            _ => Problem()
        };
    }

    /// <summary>
    /// Updates a purchase product many to many relation by relation Ids.
    /// </summary>
    /// <param name="updatePurchaseProductDto"></param>
    /// <response code="304">Not Modified</response>
    /// <response code="200">Ok</response>
    [HttpPut]
    public async Task<IActionResult> UpdatePurchaseProduct([FromBody] UpdatePurchaseProductDto updatePurchaseProductDto)
    {
        var statusCode = await _purchaseProductService.UpdatePurchaseProduct(updatePurchaseProductDto);

        return statusCode switch
        {
            HttpStatusCode.NotModified => new StatusCodeResult(StatusCodes.Status304NotModified),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }
}