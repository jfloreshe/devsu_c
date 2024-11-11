namespace DevsuCustomer.Api.Models.Primitives;

public sealed record Error(string Title, string? Description = null, int? Code = null)
{
    public static readonly Error None = new(string.Empty);
}