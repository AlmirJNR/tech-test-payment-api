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

builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
                      ?? throw new ArgumentException("Missing DATABASE_CONNECTION_STRING variable")));
// Repositories
builder.Services.AddTransient<ISellerRepository, SellerRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();

// Services
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddTransient<ISellerService, SellerService>();
builder.Services.AddTransient<IProductService, ProductService>();

// Controllers
builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_KEY")
                ?? throw new ArgumentException("Missing JWT_KEY variable")))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", configurePolicy =>
    {
        configurePolicy.AllowAnyHeader();
        configurePolicy.AllowAnyMethod();
        configurePolicy.AllowAnyOrigin();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
            securityScheme,
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();