namespace ECommerce.Contracts.Dtos.Seller;

public readonly record struct UpdateSellerDto()
{
    public string? Cpf { get; init; } = null;
    public string? Name { get; init; } = null;
    public string? Email { get; init; } = null;
    public string? Telephone { get; init; } = null;

    public Data.Models.Seller ToModel() => new()
    {
        Cpf = Cpf ?? string.Empty,
        Name = Name ?? string.Empty,
        Email = Email ?? string.Empty,
        Telephone = Telephone ?? string.Empty
    };
}