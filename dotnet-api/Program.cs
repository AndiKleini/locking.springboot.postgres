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

MapPessemisticLockingApi(app);

app.Run();

static void MapOptimisticLockingApi(WebApplication app)
{
    const string optimisticUrlPath = "optimistic";

    MapBookingApi(app, optimisticUrlPath);

    app.MapPost(
    $"/{optimisticUrlPath}/booking",
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
    .WithName($"{optimisticUrlPath}CreateBooking");

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

static void MapPessemisticLockingApi(WebApplication app)
{
    const string pessemisticUrlPath = "pessemistic";

    MapBookingApi(app, pessemisticUrlPath);

    app.MapPost(
    $"/{pessemisticUrlPath}/booking",
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

        // TODO: assure that the same room cannot be booked twice
        // intention: it might happen for some locations that booking attempts to the same rooms are
        // made in parallel. Consequently it is better when we prevent this very early in the proce

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
        db.Booking.Add(booking);
        db.SaveChanges();
        return Results.Created(
            $"/{pessemisticUrlPath}/booking/{booking.Id}",
            booking);
    })
    .WithName($"{pessemisticUrlPath}CreateBooking");

    app.MapGet(
        "/pessemistic/bookingconfirmation",
        () =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var confirmation = db.Confirmation;
            return !confirmation?.Any() ?? true ?
                Results.NoContent() :
                Results.Ok(confirmation.ToList());
        })
        .WithName("PessemisticGetAllBookingConfirmation");

    app.MapGet(
        "/pessemistic/bookingconfirmation/{id}",
        (int id) =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var confirmation = db.Confirmation.Where(c => c.BookingId == id).FirstOrDefault();
            return confirmation == null ?
                Results.NotFound() :
                Results.Ok(new { Id = id, Status = "Confirmed" });
        })
        .WithName("PessemisticGetSpecificBookingConfirmation");

    app.MapPost(
        "/pessemistic/bookingconfirmation",
        (BookingConfirmation bookingConfirmation) =>
        {
            // This can usually not fail as we are usip/booking/ng pessimistic locking
            // When this crashes it means that some infratsructure problem occurred
            // When the confirmation is not written it is indiacating that the transaction
            // flow was abortzed unexpectedly
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            db.Confirmation.Add(bookingConfirmation);
            db.SaveChanges();
            return Results.Created(
                $"/optimistic/bookingconfirmation/{bookingConfirmation.Id}",
                bookingConfirmation);
        })
        .WithName("PessemisticCreateBookingConfirmation");
}

static void MapBookingApi(WebApplication app, string apiSubpath)
{
    app.MapGet(
        $"/{apiSubpath}/booking",
        () =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var bookings = db.Booking;
            return !bookings?.Any() ?? true ?
                Results.NoContent() :
                Results.Ok(bookings.ToList());
        })
        .WithName($"{apiSubpath}GetAllBookings");

    app.MapGet(
        $"/{apiSubpath}/booking/{{id}}",
        (long id) =>
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var booking = db.Booking.Find(id);
            return booking == null ?
                Results.NotFound() :
                Results.Ok(booking);
        })
        .WithName($"{apiSubpath}GetSpecificBooking");

    app.MapDelete(
        $"/{apiSubpath}/booking/{{id}}",
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
        .WithName($"{apiSubpath}DeleteBooking");
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