using ECommerce.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ECommerce.Test.UnitTests;

public class ApiUnitTest
{
    [Fact]
    public void Get_Api_Should_Return_Ok()
    {
        // Arrange
        var controller = new ApiController();

        // Act
        var actionResult = controller.GetApiStatus();

        // Assert
        Assert.True(actionResult is OkObjectResult);
    }
}