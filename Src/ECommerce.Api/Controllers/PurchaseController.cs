using System.Net;
using ECommerce.Contracts.Dtos.Purchase;
using ECommerce.Contracts.Enums;
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
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ISellerService _sellerService;

    public PurchaseController(IPurchaseService purchaseService, ISellerService sellerService)
    {
        _purchaseService = purchaseService;
        _sellerService = sellerService;
    }

    /// <summary>
    /// Creates a purchase.
    /// </summary>
    /// <param name="purchaseDto"></param>
    /// <response code="409">Conflict</response>
    /// <response code="201">Created</response>
    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseDto purchaseDto)
    {
        if (purchaseDto.SellerId == Guid.Empty)
            return BadRequest("Invalid seller id");

        var (sellerExists, _) = await _sellerService.GetSellerById(purchaseDto.SellerId);
        if (sellerExists is null)
            return BadRequest("Seller doesn't exists");

        if (purchaseDto.PurchaseStatusId is null or not PurchaseStatusEnum.WaitingPayment)
            purchaseDto.PurchaseStatusId = PurchaseStatusEnum.WaitingPayment;

        var (createdPurchase, statusCode) = await _purchaseService.CreatePurchase(purchaseDto);

        return statusCode switch
        {
            HttpStatusCode.Conflict => Conflict(),
            HttpStatusCode.Created => Created(string.Empty, createdPurchase),
            _ => Problem()
        };
    }

    /// <summary>
    /// Deletes a purchase by Id.
    /// </summary>
    /// <param name="purchaseId"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpDelete("{purchaseId:Guid}")]
    public async Task<IActionResult> DeletePurchase([FromRoute] Guid purchaseId)
    {
        var statusCode = await _purchaseService.DeletePurchase(purchaseId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }

    /// <summary>
    /// Gets a purchase by Id.
    /// </summary>
    /// <param name="purchaseId"></param>
    /// <response code="404">Not Found</response>
    [HttpGet("{purchaseId:Guid}")]
    [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseById([FromRoute] Guid purchaseId)
    {
        var (purchaseDto, statusCode) = await _purchaseService.GetPurchaseById(purchaseId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(purchaseDto),
            _ => Problem()
        };
    }

    /// <summary>
    /// Update almost any field from the purchase with the informed Id.
    /// </summary>
    /// <param name="purchaseId"></param>
    /// <param name="purchaseDto"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpPut("{purchaseId:Guid}")]
    public async Task<IActionResult> UpdatePurchase(
        [FromRoute] Guid purchaseId,
        [FromBody] UpdatePurchaseDto purchaseDto)
    {
        if (purchaseDto.SellerId is null
            || (purchaseDto.SellerId == Guid.Empty
                && purchaseDto.PurchaseStatusId is null or < PurchaseStatusEnum.WaitingPayment))
            return BadRequest("Nothing to update");

        if (purchaseDto.PurchaseStatusId is not null and not (PurchaseStatusEnum.WaitingPayment
            or PurchaseStatusEnum.PaymentApproved
            or PurchaseStatusEnum.Shipping
            or PurchaseStatusEnum.Delivered
            or PurchaseStatusEnum.Rejected
            or PurchaseStatusEnum.Cancelled))
            return BadRequest();

        var (purchaseModel, _) = await _purchaseService.GetPurchaseById(purchaseId);
        if (purchaseModel is null)
            return NotFound();

        var (sellerModel, _) = await _sellerService.GetSellerById(purchaseDto.SellerId.Value);
        if (sellerModel is null)
            return NotFound();

        const string invalidPurchaseOrder = "Invalid purchase situation order";
        switch (purchaseModel.Value.PurchaseStatusId)
        {
            case PurchaseStatusEnum.WaitingPayment
                when purchaseDto.PurchaseStatusId is not
                    PurchaseStatusEnum.PaymentApproved or PurchaseStatusEnum.Cancelled:
                return BadRequest(invalidPurchaseOrder);

            case PurchaseStatusEnum.PaymentApproved
                when purchaseDto.PurchaseStatusId is not
                    PurchaseStatusEnum.Shipping or PurchaseStatusEnum.Cancelled:
                return BadRequest(invalidPurchaseOrder);

            case PurchaseStatusEnum.Shipping
                when purchaseDto.PurchaseStatusId is not PurchaseStatusEnum.Delivered:
                return BadRequest(invalidPurchaseOrder);
        }

        var statusCode = await _purchaseService.UpdatePurchase(purchaseId, purchaseDto);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }
}