using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    private Booking() { }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }

    public string BookedBy { get; private set; } = string.Empty;

    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }

    public int Attendees { get; private set; }

    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Guid roomId,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        int roomCapacity,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        var rangeResult = DateRange.Create(startUtc, endUtc);

        if (rangeResult.IsFailure)
            return rangeResult.Error;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > roomCapacity)
            return BookingErrors.AttendeesExceedCapacity;

        if (startUtc < nowUtc)
            return BookingErrors.StartInPast;

        if (endUtc - startUtc > TimeSpan.FromHours(4))
            return BookingErrors.DurationTooLong;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = bookedBy.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;

        return Unit.Value;
    }

    public bool Overlaps(DateRange other)
    {
        if (Status != BookingStatus.Active)
            return false;

        var currentRange = DateRange.Create(StartUtc, EndUtc);

        return currentRange.IsSuccess &&
               currentRange.Value.Overlaps(other);
    }
}