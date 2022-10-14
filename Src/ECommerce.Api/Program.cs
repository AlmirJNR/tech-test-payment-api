using System.Reflection;
using System.Text;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using ECommerce.Contracts.Interfaces.Repositories;
using ECommerce.Contracts.Interfaces.Services;
using ECommerce.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services
    .AddDbContext<EcommerceContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
                          ?? throw new ArgumentException("Missing DATABASE_CONNECTION_STRING variable")));
// Repositories
builder.Services
    .AddTransient<ISellerRepository, SellerRepository>()
    .AddTransient<IProductRepository, ProductRepository>()
    .AddTransient<IPurchaseRepository, PurchaseRepository>()
    .AddTransient<IPurchaseProductRepository, PurchaseProductRepository>();

// Services
builder.Services
    .AddTransient<ILoginService, LoginService>()
    .AddTransient<ISellerService, SellerService>()
    .AddTransient<IProductService, ProductService>()
    .AddTransient<IPurchaseService, PurchaseService>()
    .AddTransient<IPurchaseProductService, PurchaseProductService>();

// Controllers
builder.Services.AddControllers();

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                          ?? throw new ArgumentException("Missing JWT_ISSUER variable"),

            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                            ?? throw new ArgumentException("Missing JWT_AUDIENCE variable"),

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_KEY")
                    ?? throw new ArgumentException("Missing JWT_KEY variable")))
        };
    });

//Cors
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()));

// OpenApi/Swagger
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "E-Commerce",
            Description = "An ASP.NET Core Web API for managing a fictitious E-Commerce",
            Contact = new OpenApiContact
            {
                Name = "Almir Junior",
                Url = new Uri("https://www.linkedin.com/in/almir-j%C3%BAnior/")
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://github.com/AlmirJNR/tech-test-payment-api/blob/main/LICENSE")
            }
        });

        var securityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid JWT",
            Name = "JWT Authentication",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                securityScheme, Array.Empty<string>()
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

var app = builder.Build();

// Enable Swagger when in development mode.
if (app.Environment.IsDevelopment())
    app
        .UseSwagger()
        .UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

app
    .UseCors()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();
app.Run();