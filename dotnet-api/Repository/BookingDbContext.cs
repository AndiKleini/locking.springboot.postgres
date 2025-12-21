using Microsoft.EntityFrameworkCore;
using Entities;

namespace Repository
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) 
            : base(options) { }

        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingConfirmation> Confirmation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>().Property(b => b.Start).HasColumnType("timestamp");
            modelBuilder.Entity<Booking>().Property(b => b.Finish).HasColumnType("timestamp");
            base.OnModelCreating(modelBuilder);
        }
    }
}