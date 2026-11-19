using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.Id)
            .ValueGeneratedNever();

        builder.Property(booking => booking.RoomId)
            .IsRequired();

        builder.Property(booking => booking.BookedBy)
            .IsRequired();

        builder.Property(booking => booking.StartUtc)
            .IsRequired();

        builder.Property(booking => booking.EndUtc)
            .IsRequired();

        builder.Property(booking => booking.Attendees)
            .IsRequired();

        builder.Property(booking => booking.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(booking => booking.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(booking => booking.RoomId);
    }
}