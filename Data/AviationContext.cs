using Cailean_Razvan_Zboruri.Models;
using Microsoft.EntityFrameworkCore;

namespace Cailean_Razvan_Zboruri.Data
{
    public class AviationContext : DbContext
    {
        public AviationContext(DbContextOptions<AviationContext> options)
            : base(options)
        {
        }

        public DbSet<Airport> Airport { get; set; }
        public DbSet<Flight> Flight { get; set; }
        public DbSet<Passenger> Passenger { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<BookingAmenity> BookingAmenity { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DepartureAirport)
                .WithMany(a => a.Departures) 
                .HasForeignKey(f => f.DepartureAirportID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.Arrivals)
                .HasForeignKey(f => f.ArrivalAirportID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}