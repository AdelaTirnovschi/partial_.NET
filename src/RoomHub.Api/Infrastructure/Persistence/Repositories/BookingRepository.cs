using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly RoomHubDbContext _context;

    public BookingRepository(RoomHubDbContext context)
    {
        _context = context;
    }

    public void Add(Booking booking)
    {
        _context.Bookings.Add(booking);
    }

    public async Task<Booking?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(
                booking => booking.Id == id,
                cancellationToken);
    }

    public async Task<List<Booking>> ListForRoomAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(booking => booking.RoomId == roomId)
            .ToListAsync(cancellationToken);
    }

    // Compatibilitate temporara cu BookingService.
    public Booking? Get(Guid id)
    {
        return _context.Bookings
            .FirstOrDefault(booking => booking.Id == id);
    }

    // Compatibilitate temporara cu BookingService.
    public List<Booking> ListForRoom(Guid roomId)
    {
        return _context.Bookings
            .Where(booking => booking.RoomId == roomId)
            .ToList();
    }
}