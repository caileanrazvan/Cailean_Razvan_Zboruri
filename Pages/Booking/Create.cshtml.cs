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

        // Proprietăți pentru a afișa datele pe pagină
        public Models.Flight SelectedFlight { get; set; }
        public IList<Models.Amenity> AvailableAmenities { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PassengersCount { get; set; } = 1;

        // Modelul pe care îl vom salva în baza de date
        [BindProperty]
        public Models.Booking Booking { get; set; } = default!;

        // Array pentru a prinde ID-urile serviciilor bifate de utilizator
        [BindProperty]
        public int[] SelectedAmenities { get; set; }
        public List<string> OccupiedSeats { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? flightId, int passengers = 1)
        {
            if (flightId == null)
            {
                return RedirectToPage("/Flights/Index"); // Dacă nu are zbor, îl trimitem înapoi
            }

            // Aducem zborul selectat cu tot cu aeroporturi
            SelectedFlight = await _context.Flight
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefaultAsync(m => m.ID == flightId);

            if (SelectedFlight == null)
            {
                return NotFound();
            }

            PassengersCount = passengers;
            AvailableAmenities = await _context.Amenity.ToListAsync();

            // Extragem toate locurile deja rezervate pentru acest zbor
            OccupiedSeats = await _context.Booking
                .Where(b => b.FlightID == flightId)
                .SelectMany(b => b.Passengers)
                .Where(p => p.SeatNumber != null)
                .Select(p => p.SeatNumber!)
                .ToListAsync();

            return Page();
        }

        [BindProperty]
        public List<Passenger> PassengerInputs { get; set; } // Aceasta va prinde datele din formular

        [BindProperty]
        public string ContactEmail { get; set; }

        [BindProperty]
        public string ContactPhone { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Creăm obiectul principal de rezervare
            var newBooking = new Models.Booking
            {
                FlightID = Booking.FlightID,
                ContactEmail = ContactEmail,
                ContactPhone = ContactPhone,
                BookingDate = DateTime.Now,
                UserId = userId, // Salvăm cine a făcut rezervarea
                PaymentStatus = "Pending"
            };

            // 2. Adăugăm fiecare pasager din formular în lista rezervării
            foreach (var p in PassengerInputs)
            {
                var newPassenger = new Passenger
                {
                    Title = p.Title,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    SeatNumber = p.SeatNumber, // Salvăm locul din formular
                    Email = (newBooking.Passengers.Count == 0) ? ContactEmail : null
                };

                // NOU: Căutăm serviciile bifate de ACEST pasager și le adăugăm la profilul lui
                if (p.SelectedAmenitiesIds != null && p.SelectedAmenitiesIds.Any())
                {
                    var selectedAmenities = await _context.Amenity
                        .Where(a => p.SelectedAmenitiesIds.Contains(a.ID))
                        .ToListAsync();

                    newPassenger.Amenities = selectedAmenities;
                }

                newBooking.Passengers.Add(newPassenger);
            }

            _context.Booking.Add(newBooking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Payment", new { bookingId = newBooking.ID });
        }
    }
}