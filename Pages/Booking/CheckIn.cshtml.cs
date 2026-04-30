using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    [Authorize]
    public class CheckInModel : PageModel
    {
        private readonly AviationContext _context;

        public CheckInModel(AviationContext context)
        {
            _context = context;
        }

        public Models.Booking Booking { get; set; }

        public class PassengerEditData
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string? PassportNumber { get; set; }
            public string SeatNumber { get; set; }
            public List<int> SelectedAmenitiesIds { get; set; } = new List<int>();
        }

        [BindProperty]
        public List<PassengerEditData> PassengersData { get; set; } = new List<PassengerEditData>();

        public List<Models.Amenity> AvailableAmenities { get; set; } = new List<Models.Amenity>();

        // Am eliminat lista hardcodată; acum o vom umple dinamic
        public List<string> OccupiedSeats { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            Booking = await _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers).ThenInclude(p => p.Amenities)
                .FirstOrDefaultAsync(b => b.ID == bookingId);

            if (Booking == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Booking.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            if (Booking.Passengers.All(p => p.IsCheckedIn))
            {
                return RedirectToPage("./Confirmation", new { bookingId = Booking.ID });
            }

            // EXTRAGEREA REALĂ A LOCURILOR OCUPATE
            // Căutăm pe același zbor, dar excludem rezervarea curentă ca să lăsăm libere locurile proprii
            OccupiedSeats = await _context.Booking
                .Where(b => b.FlightID == Booking.FlightID && b.ID != bookingId)
                .SelectMany(b => b.Passengers)
                .Where(p => p.SeatNumber != null)
                .Select(p => p.SeatNumber!)
                .ToListAsync();

            AvailableAmenities = await _context.Amenity.ToListAsync();

            foreach (var p in Booking.Passengers)
            {
                PassengersData.Add(new PassengerEditData
                {
                    ID = p.ID,
                    Title = p.Title,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    PassportNumber = p.PassportNumber,
                    SeatNumber = p.SeatNumber,
                    SelectedAmenitiesIds = p.Amenities.Select(a => a.ID).ToList()
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int bookingId)
        {
            var booking = await _context.Booking
                .Include(b => b.Passengers).ThenInclude(p => p.Amenities)
                .FirstOrDefaultAsync(b => b.ID == bookingId);

            if (booking == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (booking.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            var availableAmenities = await _context.Amenity.ToListAsync();

            foreach (var input in PassengersData)
            {
                var passengerToUpdate = booking.Passengers.FirstOrDefault(p => p.ID == input.ID);

                if (passengerToUpdate != null)
                {
                    passengerToUpdate.Title = input.Title;
                    passengerToUpdate.FirstName = input.FirstName;
                    passengerToUpdate.LastName = input.LastName;
                    passengerToUpdate.DateOfBirth = input.DateOfBirth;
                    passengerToUpdate.PassportNumber = input.PassportNumber;
                    passengerToUpdate.SeatNumber = input.SeatNumber;

                    passengerToUpdate.Amenities.Clear();
                    if (input.SelectedAmenitiesIds != null && input.SelectedAmenitiesIds.Any())
                    {
                        var selectedAm = availableAmenities.Where(a => input.SelectedAmenitiesIds.Contains(a.ID)).ToList();
                        foreach (var am in selectedAm)
                        {
                            passengerToUpdate.Amenities.Add(am);
                        }
                    }

                    passengerToUpdate.IsCheckedIn = true;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Confirmation", new { bookingId = booking.ID });
        }
    }
}