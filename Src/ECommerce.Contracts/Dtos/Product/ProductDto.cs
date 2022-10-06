namespace ECommerce.Contracts.Dtos.Product;

public readonly record struct ProductDto()
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = null!;
    public short? Amount { get; init; } = null;
    public decimal Price { get; init; } = 0;
    public DateTime? CreatedAt { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;

    public static ProductDto? FromModel(Data.Models.Product? productModel)
    {
        if (productModel is null)
            return null;

        return new ProductDto
        {
            Id = productModel.Id,
            Name = productModel.Name,
            Amount = productModel.Amount,
            Price = productModel.Price,
            CreatedAt = productModel.CreatedAt,
            UpdatedAt = productModel.UpdatedAt
        };
    }
}