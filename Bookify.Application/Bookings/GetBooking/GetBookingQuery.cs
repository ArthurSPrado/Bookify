using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Bookings.GetBooking.Response;

namespace Bookify.Application.Bookings.GetBooking;

public record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;