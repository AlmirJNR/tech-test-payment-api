using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class ApiController : ControllerBase
{
    /// <summary>
    /// Checks if Api is up and running.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetApiStatus()
        => Ok("ECommerce api is up and running");
}