
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBookingById;
using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoomById;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLite
builder.Services.AddDbContext<RoomHubDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("RoomHub")
        ?? "Data Source=roomhub.db"));

// Unit of Work - acelasi DbContext din request
builder.Services.AddScoped<IUnitOfWork>(
    sp => sp.GetRequiredService<RoomHubDbContext>());

// Repositories
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<IRoomRepository>(
    sp => sp.GetRequiredService<RoomRepository>());

builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<IBookingRepository>(
    sp => sp.GetRequiredService<BookingRepository>());

// Command Handlers
builder.Services.AddScoped<
    ICommandHandler<CreateRoomCommand, Room>,
    CreateRoomHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateBookingCommand, Booking>,
    CreateBookingHandler>();

builder.Services.AddScoped<
    ICommandHandler<CancelBookingCommand, Unit>,
    CancelBookingHandler>();

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// Create SQLite database and tables if needed
await DatabaseInitializer.InitializeAsync(app.Services);

app.MapGet("/", () => "RoomHub API is running.");

// Vertical Slice 1 - CreateRoom
CreateRoomEndpoint.Map(
    app.MapGroup("/api/rooms").WithTags("Rooms"));

// Vertical Slice 2 - GetRoomById
GetRoomByIdEndpoint.Map(
    app.MapGroup("/api/rooms").WithTags("Rooms"));

// Vertical Slice 3 - CreateBooking
CreateBookingEndpoint.Map(
    app.MapGroup("/api/rooms").WithTags("Rooms"));

// Vertical Slice 4 - GetBookingById
GetBookingByIdEndpoint.Map(
    app.MapGroup("/api/bookings").WithTags("Bookings"));

// Vertical Slice 5 - CancelBooking
CancelBookingEndpoint.Map(
    app.MapGroup("/api/bookings").WithTags("Bookings"));

app.Run();
