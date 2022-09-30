namespace ECommerce.Contracts.Dtos.Seller;

public readonly record struct CreateSellerDto()
{
    public string Cpf { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Telephone { get; init; } = null!;

    public Data.Models.Seller ToModel() => new()
    {
        Cpf = Cpf,
        Name = Name,
        Email = Email,
        Telephone = Telephone
    };
}