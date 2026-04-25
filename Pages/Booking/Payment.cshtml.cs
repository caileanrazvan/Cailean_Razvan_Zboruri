using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class PaymentModel : PageModel
    {
        private readonly AviationContext _context;

        public PaymentModel(AviationContext context)
        {
            _context = context;
        }

        public Models.Booking CurrentBooking { get; set; }
        public decimal TotalAmount { get; set; }

        public async Task<IActionResult> OnGetAsync(int bookingId)
        {
            // Aducem rezervarea cu tot cu pasageri și serviciile lor
            CurrentBooking = await _context.Booking
                .Include(b => b.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(b => b.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(b => b.Passengers).ThenInclude(p => p.Amenities)
                .FirstOrDefaultAsync(b => b.ID == bookingId);

            if (CurrentBooking == null || CurrentBooking.PaymentStatus == "Paid")
            {
                return RedirectToPage("/Index"); // Nu lăsăm să plătească de 2 ori
            }

            // Calculăm totalul: (Preț Zbor * Nr Pasageri) + Toate Serviciile Extra
            TotalAmount = (CurrentBooking.Flight.BasePrice * CurrentBooking.Passengers.Count) +
                          CurrentBooking.Passengers.SelectMany(p => p.Amenities).Sum(a => a.Price);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking == null) return NotFound();

            // Aici simulăm procesarea plății cu banca (așteptăm 2 secunde)
            await Task.Delay(2000);

            // Actualizăm statusul în baza de date
            booking.PaymentStatus = "Paid";
            booking.TransactionId = "ANGEL-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            await _context.SaveChangesAsync();

            // După ce a plătit cu succes, îl trimitem la o pagină de confirmare
            return RedirectToPage("./Confirmation", new { bookingId = booking.ID });
        }
    }
}