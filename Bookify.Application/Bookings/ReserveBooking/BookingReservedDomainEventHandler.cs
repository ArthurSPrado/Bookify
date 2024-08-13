using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Bookings.Intefaces;
using Bookify.Domain.Users.Interfaces;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;

internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IBookingRepository _boookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public BookingReservedDomainEventHandler(
        IBookingRepository boookingRepository, 
        IUserRepository userRepository, 
        IEmailService emailService)
    {
        _boookingRepository = boookingRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _boookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking is null) return;
        
        var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);
        
        if (user is null) return;

        await _emailService.SendAsync(
            user.Email,
            "Booking Reserved",
            $"Your booking for apartment {booking.ApartmentId} has been reserved, you have 10 min to confirm it.");
    }
}