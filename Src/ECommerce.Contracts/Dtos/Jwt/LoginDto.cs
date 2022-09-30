namespace ECommerce.Contracts.Dtos.Jwt;

public readonly record struct LoginDto()
{
    public string Cpf { get; init; } = null!;
    public string Email { get; init; } = null!;
}