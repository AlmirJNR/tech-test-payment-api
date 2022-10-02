using System.Net;
using ECommerce.Api.Utils;
using ECommerce.Contracts.Dtos.Seller;
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
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;

    public SellerController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    /// <summary>
    /// Creates a seller.
    /// </summary>
    /// <param name="sellerDto"></param>
    /// <response code="409">Conflict</response>
    /// <response code="201">Created</response>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateSeller([FromBody] CreateSellerDto sellerDto)
    {
        if (string.IsNullOrWhiteSpace(sellerDto.Cpf) || !RegexHelper.IsCpfValid(sellerDto.Cpf))
            return BadRequest("Invalid Cpf");
        if (string.IsNullOrWhiteSpace(sellerDto.Name) || sellerDto.Name.Length < 3)
            return BadRequest("Invalid Name");
        if (string.IsNullOrWhiteSpace(sellerDto.Email) || !RegexHelper.IsEmailValid(sellerDto.Email))
            return BadRequest("Invalid Email");
        if (string.IsNullOrWhiteSpace(sellerDto.Telephone) || !RegexHelper.IsTelephoneValid(sellerDto.Telephone))
            return BadRequest("Invalid Telephone");

        var (createdSeller, statusCode) = await _sellerService.CreateSeller(sellerDto);

        return statusCode switch
        {
            HttpStatusCode.Conflict => Conflict(),
            HttpStatusCode.Created => Created(string.Empty, createdSeller),
            _ => Problem()
        };
    }

    /// <summary>
    /// Delete the current authenticated seller.
    /// </summary>
    /// <param name="sellerId"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpDelete("{sellerId:Guid}")]
    public async Task<IActionResult> DeleteSeller([FromRoute] Guid sellerId)
    {
        var claimsDictionary = JwtHelper.ClaimsToDictionary(User.Claims);
        var requestingSellerId = JwtHelper.GetSellerGuid(claimsDictionary);
        if (requestingSellerId is null)
            return BadRequest();

        if (requestingSellerId != sellerId)
            return Unauthorized();

        var statusCode = await _sellerService.DeleteSeller(sellerId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }

    /// <summary>
    /// Get current seller by id.
    /// </summary>
    /// <param name="sellerId"></param>
    /// <response code="404">Not Found</response>
    [HttpGet("{sellerId:Guid}")]
    [ProducesResponseType(typeof(SellerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSellerById([FromRoute] Guid sellerId)
    {
        var claimsDictionary = JwtHelper.ClaimsToDictionary(User.Claims);
        var requestingSellerId = JwtHelper.GetSellerGuid(claimsDictionary);
        if (requestingSellerId is null)
            return BadRequest();

        if (requestingSellerId != sellerId)
            return Unauthorized();

        var (sellerDto, statusCode) = await _sellerService.GetSellerById(sellerId);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(sellerDto),
            _ => Problem()
        };
    }

    /// <summary>
    /// Update almost any field from the current seller.
    /// </summary>
    /// <param name="sellerId"></param>
    /// <param name="sellerDto"></param>
    /// <response code="404">Not Found</response>
    /// <response code="200">Ok</response>
    [HttpPut("{sellerId:Guid}")]
    public async Task<IActionResult> UpdateSeller([FromRoute] Guid sellerId, [FromBody] UpdateSellerDto sellerDto)
    {
        var claimsDictionary = JwtHelper.ClaimsToDictionary(User.Claims);
        var requestingSellerId = JwtHelper.GetSellerGuid(claimsDictionary);
        if (requestingSellerId is null)
            return BadRequest();

        if (requestingSellerId != sellerId)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(sellerDto.Cpf)
            && string.IsNullOrWhiteSpace(sellerDto.Name)
            && string.IsNullOrWhiteSpace(sellerDto.Email)
            && string.IsNullOrWhiteSpace(sellerDto.Telephone))
            return BadRequest("Nothing to update");

        if (!string.IsNullOrWhiteSpace(sellerDto.Cpf) && !RegexHelper.IsCpfValid(sellerDto.Cpf))
            return BadRequest("Invalid Cpf");
        if (!string.IsNullOrWhiteSpace(sellerDto.Name) && sellerDto.Name.Length < 3)
            return BadRequest("Invalid Name");
        if (!string.IsNullOrWhiteSpace(sellerDto.Email) && !RegexHelper.IsEmailValid(sellerDto.Email))
            return BadRequest("Invalid Email");
        if (!string.IsNullOrWhiteSpace(sellerDto.Telephone) && !RegexHelper.IsTelephoneValid(sellerDto.Telephone))
            return BadRequest("Invalid Telephone");

        var statusCode = await _sellerService.UpdateSeller(sellerId, sellerDto);

        return statusCode switch
        {
            HttpStatusCode.NotFound => NotFound(),
            HttpStatusCode.OK => Ok(),
            _ => Problem()
        };
    }
}