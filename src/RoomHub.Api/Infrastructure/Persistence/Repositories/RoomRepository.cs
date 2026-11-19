using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly RoomHubDbContext _context;

    public RoomRepository(RoomHubDbContext context)
    {
        _context = context;
    }

    public void Add(Room room)
    {
        _context.Rooms.Add(room);
    }

    public async Task<Room?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(
                room => room.Id == id,
                cancellationToken);
    }

    // Compatibilitate temporara cu RoomService.
    public Room? Get(Guid id)
    {
        return _context.Rooms
            .FirstOrDefault(room => room.Id == id);
    }
}