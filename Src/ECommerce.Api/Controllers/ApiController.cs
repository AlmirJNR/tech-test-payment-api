using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]/v1")]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Checks if Api is up and running.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns when Api is up and running.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetApiStatus()
        => Ok("ECommerce api is up and running");
}