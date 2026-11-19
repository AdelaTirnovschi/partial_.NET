using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<Booking>;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider time)
    : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        // 404 pentru sala inexistenta, inaintea validarilor.
        var room = await rooms.GetByIdAsync(
            command.RoomId,
            cancellationToken);

        if (room is null)
            return RoomErrors.NotFound;

        // Regulile R2-R6.
        var result = Booking.Create(
            command.RoomId,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            room.Capacity,
            time.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
            return result.Error;

        // Verificarea intervalului pentru R7.
        var rangeResult = DateRange.Create(
            command.StartUtc,
            command.EndUtc);

        if (rangeResult.IsFailure)
            return rangeResult.Error;

        var existingBookings = await bookings.ListForRoomAsync(
            command.RoomId,
            cancellationToken);

        foreach (var existing in existingBookings)
        {
            if (existing.Overlaps(rangeResult.Value))
                return BookingErrors.RoomOccupied;
        }

        bookings.Add(result.Value);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return result;
    }
}