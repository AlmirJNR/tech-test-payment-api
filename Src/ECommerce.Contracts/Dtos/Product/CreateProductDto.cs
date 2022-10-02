namespace ECommerce.Contracts.Dtos.Product;

public readonly record struct CreateProductDto()
{
    public string Name { get; init; } = null!;
    public short? Amount { get; init; } = null;
    public decimal Price { get; init; } = 0;

    public Data.Models.Product ToModel() => new()
    {
        Name = Name,
        Amount = Amount,
        Price = Price
    };
}