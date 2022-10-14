using System;
using ECommerce.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Test.Utils;

public static class DatabaseSetupUtils
{
    public static EcommerceContext CreateTestingDatabaseContext()
        => new(new DbContextOptionsBuilder<EcommerceContext>()
            .EnableSensitiveDataLogging()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}