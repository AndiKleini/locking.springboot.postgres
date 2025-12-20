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
            Results.Ok(new { Id = id, Status = "Confirmed"});  
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}