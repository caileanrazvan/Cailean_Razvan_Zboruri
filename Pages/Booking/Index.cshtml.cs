using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using System.Security.Claims;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class IndexModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public IndexModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        public IList<Cailean_Razvan_Zboruri.Models.Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Construim interogarea de bază
            var bookingsQuery = _context.Booking
                .Include(b => b.Flight)
                .Include(b => b.Passengers)
                .Include(b => b.BookingAmenities)
                    .ThenInclude(ba => ba.Amenity)
                .AsQueryable();

            // FILTRARE: Dacă NU este admin, vedem doar rezervările unde UserId coincide cu cel logat
            if (!isAdmin)
            {
                bookingsQuery = bookingsQuery.Where(b => b.UserId == userId);
            }

            Booking = await bookingsQuery.ToListAsync();
        }
    }
}
