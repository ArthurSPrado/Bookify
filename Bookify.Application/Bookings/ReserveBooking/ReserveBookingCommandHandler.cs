using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging.Command;
using Bookify.Application.Bookings.ReserveBooking;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments.Interfaces;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Intefaces;
using Bookify.Domain.Bookings.Services;
using Bookify.Domain.Bookings.ValueObjects;
using Bookify.Domain.Users.Interfaces;

namespace Bookify.Application.Bookings;

public class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly PricingService _pricingService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReserveBookingCommandHandler(
        IBookingRepository bookingRepository, 
        IApartmentRepository apartmentRepository, 
        IUserRepository userRepository, 
        IUnityOfWork unityOfWork, 
        PricingService pricingService, 
        IDateTimeProvider dateTimeProvider)
    {
        _bookingRepository = bookingRepository;
        _apartmentRepository = apartmentRepository;
        _userRepository = userRepository;
        _unityOfWork = unityOfWork;
        _pricingService = pricingService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.UserNotFound);
        }
        
        var apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);
        
        if (apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.ApartmentNotFound);
        }
        
        var duration = DateRange.Create(request.StartDate, request.EndDate);

        if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
        
        var booking = Booking.Reserve(
            apartment,
            user.Id,
            duration,
            _dateTimeProvider.UtcNow, 
            _pricingService
        );
        
        _bookingRepository.Add(booking);
        
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return booking.Id;
    }
    
    
}