using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Apartments.Response;

namespace Bookify.Application.Apartments.SearchApartments;

public record SearchApartmentsQuery(
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<IReadOnlyList<ApartmentResponse>>;