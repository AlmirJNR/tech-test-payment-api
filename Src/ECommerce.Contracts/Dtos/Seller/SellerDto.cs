namespace ECommerce.Contracts.Dtos.Seller;

public readonly record struct SellerDto()
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Cpf { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Telephone { get; init; } = null!;
    public DateTime? CreatedAt { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;

    public static SellerDto? FromModel(Data.Models.Seller? seller)
    {
        if (seller is null)
            return null;

        return new SellerDto
        {
            Id = seller.Id,
            Cpf = seller.Cpf,
            Name = seller.Name,
            Email = seller.Email,
            Telephone = seller.Telephone,
            CreatedAt = seller.CreatedAt,
            UpdatedAt = seller.UpdatedAt
        };
    }
};