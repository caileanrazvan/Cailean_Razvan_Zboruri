using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Data
{
    public class AviationContext : IdentityDbContext<IdentityUser>
    {
        public AviationContext(DbContextOptions<AviationContext> options)
            : base(options)
        {
        }

        public DbSet<Cailean_Razvan_Zboruri.Models.Flight> Flight { get; set; } = default!;
        public DbSet<Cailean_Razvan_Zboruri.Models.Passenger> Passenger { get; set; } = default!;
        public DbSet<Cailean_Razvan_Zboruri.Models.Airport> Airport { get; set; } = default!;
        public DbSet<Cailean_Razvan_Zboruri.Models.Amenity> Amenity { get; set; } = default!;
        public DbSet<Cailean_Razvan_Zboruri.Models.Booking> Booking { get; set; } = default!;

        // ADAUGA ACEASTA METODA:
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // IMPORTANT: Prima linie trebuie sa fie asta cand folosim Identity!
            base.OnModelCreating(builder);

            // 1. Configuram relatia pentru Aeroportul de Plecare (Fara stergere in cascada)
            builder.Entity<Flight>()
                .HasOne(f => f.DepartureAirport)
                .WithMany(a => a.Departures)
                .HasForeignKey(f => f.DepartureAirportID)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Configuram relatia pentru Aeroportul de Sosire (Fara stergere in cascada)
            builder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.Arrivals)
                .HasForeignKey(f => f.ArrivalAirportID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}