namespace ECommerce.Contracts.Dtos.Product;

public readonly record struct UpdateProductDto()
{
    public string? Name { get; init; } = null;
    public short? Amount { get; init; } = null;
    public decimal? Price { get; init; } = null;

    public Data.Models.Product ToModel() => new()
    {
        Name = Name ?? string.Empty,
        Amount = Amount,
        Price = Price ?? decimal.Zero
    };
}