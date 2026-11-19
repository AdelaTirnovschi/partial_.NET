using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<RoomHubDbContext>();

        await context.Database.EnsureCreatedAsync();
    }
}