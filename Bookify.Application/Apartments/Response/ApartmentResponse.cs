namespace Bookify.Application.Apartments.Response;

public sealed class ApartmentResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal PriceAmount { get; init; }
    public string Currency { get; init; }
    public AddressResponse Address { get; set; }
}