using System.Net;
using System.Security.Claims;
using ECommerce.Api.Utils;
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
    private readonly Guid _sellerGuid;

    public PurchaseController(
        IPurchaseService purchaseService,
        ISellerService sellerService,
        IEnumerable<Claim>? claims = null)
    {
        _purchaseService = purchaseService;
        _sellerService = sellerService;

        // The user can be null if not used by a HTTP request
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        _sellerGuid = JwtUtil.GetSellerGuid(claims ?? User?.Claims ?? Array.Empty<Claim>());
    }

    /// <summary>
    /// Creates a purchase.
    /// </summary>
    /// <response code="201">Created</response>
    [HttpPost]
    public async Task<IActionResult> CreatePurchase()
    {
        if (_sellerGuid == Guid.Empty)
            return BadRequest("Invalid seller id");

        var (sellerExists, _) = await _sellerService.GetSellerById(_sellerGuid);
        if (sellerExists is null)
            return BadRequest("Seller doesn't exists");

        var (createdPurchase, statusCode) = await _purchaseService.CreatePurchase(_sellerGuid);

        return statusCode switch
        {
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
        if (_sellerGuid == Guid.Empty)
            return BadRequest("Invalid seller id");

        var (purchaseDto, _) = await _purchaseService.GetPurchaseById(purchaseId);
        if (purchaseDto is null)
            return NotFound();

        if (purchaseDto.Value.SellerId != _sellerGuid)
            return Unauthorized();

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
        if (_sellerGuid == Guid.Empty)
            return BadRequest("Invalid seller id");

        var (purchaseDto, statusCode) = await _purchaseService.GetPurchaseById(purchaseId);
        if (purchaseDto is null)
            return NotFound();

        if (purchaseDto?.SellerId != _sellerGuid)
            return Unauthorized();

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
    /// <param name="purchaseStatus"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpPatch("{purchaseId:Guid}/purchaseStatus/{purchaseStatus}")]
    public async Task<IActionResult> UpdatePurchase(
        [FromRoute] Guid purchaseId,
        [FromRoute] PurchaseStatusEnum? purchaseStatus)
    {
        if (_sellerGuid == Guid.Empty)
            return BadRequest("Invalid seller id");

        if (purchaseStatus is null or < PurchaseStatusEnum.WaitingPayment)
            return BadRequest("Nothing to update");

        if (purchaseStatus is not null and not (PurchaseStatusEnum.WaitingPayment
            or PurchaseStatusEnum.PaymentApproved
            or PurchaseStatusEnum.Shipping
            or PurchaseStatusEnum.Delivered
            or PurchaseStatusEnum.Rejected
            or PurchaseStatusEnum.Cancelled))
            return BadRequest();

        var (purchaseModel, _) = await _purchaseService.GetPurchaseById(purchaseId);
        if (purchaseModel is null)
            return NotFound();

        var (sellerModel, _) = await _sellerService.GetSellerById(_sellerGuid);
        if (sellerModel is null)
            return NotFound();

        const string invalidPurchaseOrder = "Invalid purchase situation order";
        switch (purchaseModel.Value.PurchaseStatusId)
        {
            case PurchaseStatusEnum.WaitingPayment
                when purchaseStatus is not
                    (PurchaseStatusEnum.PaymentApproved or PurchaseStatusEnum.Cancelled or PurchaseStatusEnum.Rejected):
                return BadRequest(invalidPurchaseOrder);

            case PurchaseStatusEnum.PaymentApproved
                when purchaseStatus is not
                    (PurchaseStatusEnum.Shipping or PurchaseStatusEnum.Cancelled):
                return BadRequest(invalidPurchaseOrder);

            case PurchaseStatusEnum.Shipping
                when purchaseStatus is not PurchaseStatusEnum.Delivered:
                return BadRequest(invalidPurchaseOrder);
        }

        var statusCode = await _purchaseService.UpdatePurchase(purchaseId, purchaseStatus);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }
}