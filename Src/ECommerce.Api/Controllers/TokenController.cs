using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Api.Utils;
using ECommerce.Contracts.Dtos.Jwt;
using ECommerce.Contracts.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILoginService _loginService;

    public TokenController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    /// <summary>
    /// Creates a Jwt with seller id.
    /// </summary>
    /// <response code="400">Bad request</response>
    /// <exception cref="ArgumentException"></exception>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> GenerateJwt([FromBody] LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Cpf) ||
            string.IsNullOrWhiteSpace(loginDto.Email) ||
            !RegexUtil.IsCpfValid(loginDto.Cpf) ||
            !RegexUtil.IsEmailValid(loginDto.Email))
            return BadRequest();

        var (loggedSeller, _) = await _loginService.SellerLogin(loginDto);
        if (loggedSeller is null)
            return BadRequest();

        var claimsArray = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("JWT_SUBJECT")
                                             ?? throw new ArgumentException("Missing JWT_SUBJECT variable")),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new("sellerId", loggedSeller.Value.Id.ToString())
        };

        var keyAsByteArray = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
                                                    ?? throw new ArgumentException("Missing JWT_KEY variable"));
        var key = new SymmetricSecurityKey(keyAsByteArray);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER")
                    ?? throw new ArgumentException("Missing JWT_ISSUER variable"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                      ?? throw new ArgumentException("Missing JWT_AUDIENCE variable"),
            claims: claimsArray,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return Created(string.Empty, serializedToken);
    }
}