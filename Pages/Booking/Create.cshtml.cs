using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AviationContext _context;

        public CreateModel(AviationContext context)
        {
            _context = context;
        }

        public Models.Flight SelectedFlight { get; set; }
        public IList<Models.Amenity> AvailableAmenities { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PassengersCount { get; set; } = 1;

        [BindProperty]
        public Models.Booking Booking { get; set; } = default!;

        [BindProperty]
        public int[] SelectedAmenities { get; set; }
        public List<string> OccupiedSeats { get; set; } = new List<string>();

        [BindProperty]
        public List<Passenger> PassengerInputs { get; set; }

        [BindProperty]
        public string ContactEmail { get; set; }

        [BindProperty]
        public string ContactPhone { get; set; }

        public async Task<IActionResult> OnGetAsync(int? flightId, int passengers = 1)
        {
            if (flightId == null) return RedirectToPage("/Flight/Index");

            await LoadPageDataAsync(flightId.Value);

            if (SelectedFlight == null) return NotFound();

            PassengersCount = passengers;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. SECURITATE: Verificăm dacă zborul trimis din formular chiar există
            var flightExists = await _context.Flight.AnyAsync(f => f.ID == Booking.FlightID);
            if (!flightExists) return NotFound();

            // 2. CONCURENȚĂ: Verificăm din nou în TIMP REAL dacă locurile selectate sunt încă libere
            var currentOccupiedSeats = await _context.Booking
                .Where(b => b.FlightID == Booking.FlightID)
                .SelectMany(b => b.Passengers)
                .Where(p => p.SeatNumber != null)
                .Select(p => p.SeatNumber)
                .ToListAsync();

            var requestedSeats = PassengerInputs.Select(p => p.SeatNumber).ToList();

            // Intersectăm listele. Dacă există elemente comune, înseamnă că locul e deja luat!
            var alreadyTakenSeats = requestedSeats.Intersect(currentOccupiedSeats).ToList();

            if (alreadyTakenSeats.Any())
            {
                // Unul dintre locuri a fost luat între timp! Oprim tot și anunțăm utilizatorul.
                ModelState.AddModelError(string.Empty, $"Ne pare rău, dar locurile următoare tocmai au fost rezervate de altcineva: {string.Join(", ", alreadyTakenSeats)}. Vă rugăm să alegeți alte locuri.");

                // Reîncărcăm datele pentru ca pagina să se afișeze corect cu noile scaune ocupate
                await LoadPageDataAsync(Booking.FlightID.Value);
                return Page();
            }

            // 3. Dacă totul este valid și locurile sunt libere, creăm rezervarea
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newBooking = new Models.Booking
            {
                FlightID = Booking.FlightID,
                ContactEmail = ContactEmail,
                ContactPhone = ContactPhone,
                BookingDate = DateTime.Now,
                UserId = userId,
                PaymentStatus = "Pending"
            };

            // Preîncărcăm serviciile disponibile pentru a fi mai rapizi
            var availableAmenities = await _context.Amenity.ToListAsync();

            foreach (var p in PassengerInputs)
            {
                var newPassenger = new Passenger
                {
                    Title = p.Title,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    SeatNumber = p.SeatNumber,
                    Email = (newBooking.Passengers.Count == 0) ? ContactEmail : null
                };

                if (p.SelectedAmenitiesIds != null && p.SelectedAmenitiesIds.Any())
                {
                    newPassenger.Amenities = availableAmenities
                        .Where(a => p.SelectedAmenitiesIds.Contains(a.ID))
                        .ToList();
                }

                newBooking.Passengers.Add(newPassenger);
            }

            _context.Booking.Add(newBooking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Payment", new { bookingId = newBooking.ID });
        }

        // Am scos logica de încărcare a zborului și listelor într-o metodă separată
        // pentru a putea repopula pagina în caz de eroare (dacă locul a fost furat)
        private async Task LoadPageDataAsync(int flightId)
        {
            SelectedFlight = await _context.Flight
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefaultAsync(m => m.ID == flightId);

            AvailableAmenities = await _context.Amenity.ToListAsync();

            OccupiedSeats = await _context.Booking
                .Where(b => b.FlightID == flightId)
                .SelectMany(b => b.Passengers)
                .Where(p => p.SeatNumber != null)
                .Select(p => p.SeatNumber!)
                .ToListAsync();
        }
    }
}