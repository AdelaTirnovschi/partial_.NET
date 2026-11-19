using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record DateRange
{
    private DateRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }

    public static Result<DateRange> Create(
        DateTime startUtc,
        DateTime endUtc)
    {
        if (endUtc <= startUtc)
            return BookingErrors.InvalidRange;

        return new DateRange(startUtc, endUtc);
    }

    public bool Overlaps(DateRange other)
    {
        return StartUtc < other.EndUtc &&
               other.StartUtc < EndUtc;
    }
}