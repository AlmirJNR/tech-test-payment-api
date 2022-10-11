using System;

namespace ECommerce.Test.Utils;

public static class EnvironmentUtils
{
    public static void SetEnvironmentVariables(
        string jwtSubject = "ECOMMERCE_SUBJECT",
        string jwtIssuer = "ECOMMERCE_ISSUER",
        string jwtAudience = "ECOMMERCE_BACKEND",
        string jwtKey = "9a5835502c69a59d42d5438316d3d43cd2fb0bdd789b3de98f1d0d58e25dbaa5")
    {
        Environment
            .SetEnvironmentVariable("JWT_SUBJECT", jwtSubject);
        Environment
            .SetEnvironmentVariable("JWT_ISSUER", jwtIssuer);
        Environment
            .SetEnvironmentVariable("JWT_AUDIENCE", jwtAudience);
        Environment
            .SetEnvironmentVariable("JWT_KEY", jwtKey);
    }
}