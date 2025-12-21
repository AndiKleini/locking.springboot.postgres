using Microsoft.EntityFrameworkCore;
using Repository;
using Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

MapOptimisticLockingApi(app);

app.Run();

static void MapOptimisticLockingApi(WebApplication app)
{
    app.MapGet(
        "/optimistic/booking",
        () =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var bookings = db.Booking;
            return !bookings?.Any() ?? true ?
                Results.NoContent() :
                Results.Ok(bookings.ToList());
        })
        .WithName("OptimisticGetAllBookings");
    
    app.MapGet(
        "/optimistic/booking/{id}",
        (long id) =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var booking = db.Booking.Find(id);
            return booking == null ?
                Results.NotFound() :
                Results.Ok(booking);
        })
        .WithName("OptimisticGetSpecificBooking");

    app.MapPost(
        "/optimistic/booking",
        (Booking booking) =>
        {
            // unfortunately we cannot guarantee that all locations which might
            // evaluate to false here are not somehow offered to our customers
            // (e.g.: shared links in social medias, clients that are working
            // with old data ... )
            // Therefore this check is crucial
            if (!CanBook(booking))
            {
                return Results.Conflict("The room is not available for the selected time frame.");
            }

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            db.Booking.Add(booking);
            db.SaveChanges();
            return Results.Created(
                $"/optimistic/booking/{booking.Id}",
                booking);
        })
        .WithName("OptimisticCreateBooking");

    app.MapDelete(
        "/optimistic/booking/{id}",
        (long id) =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var booking = db.Booking.Find(id);
            if (booking == null)
            {
                return Results.NotFound();
            }
            db.Booking.Remove(booking);
            db.SaveChanges();
            return Results.NoContent();
        })
        .WithName("OptimisticDeleteBooking");

    app.MapGet(
        "/optimistic/bookingconfirmation",
        () =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var confirmation = db.Confirmation;
            return !confirmation?.Any() ?? true ?
                Results.NoContent() :
                Results.Ok(confirmation.ToList());
        })
        .WithName("OptimisticGetAllBookingConfirmation");

    app.MapGet(
        "/optimistic/bookingconfirmation/{id}",
        (int id) =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var confirmation = db.Confirmation.Where(c => c.BookingId == id).FirstOrDefault();
            return confirmation == null ?
                Results.NotFound() :
                Results.Ok(new { Id = id, Status = "Confirmed" });
        })
        .WithName("OptimisticGetSpecificBookingConfirmation");

    app.MapPost(
        "/optimistic/bookingconfirmation",
        (BookingConfirmation bookingConfirmation) =>
        {
            // TODO: make sure that the same room cannot be booked more than once
            // intention: as it is expected that a collisions of bookings are rare
            // we believe that we will not run into this error here very often

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            db.Confirmation.Add(bookingConfirmation);
            db.SaveChanges();
            return Results.Created(
                $"/optimistic/bookingconfirmation/{bookingConfirmation.Id}",
                bookingConfirmation);
        })
        .WithName("OptimisticCreateBookingConfirmation");
}

static bool CanBook(Booking booking)
{
    // here are a couple of expensive checks  like:
        // checking if the location was already banned from the platform
        // (e.g.: as a consequence of bad feedback).
        // checking if authorities locked this location
        // (e.g.: risk of life and health)
        // checking if the location was already shut down
        // ...
    return true;
}