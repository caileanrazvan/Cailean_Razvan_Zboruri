using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class DetailsModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public DetailsModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        public Cailean_Razvan_Zboruri.Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Booking = await _context.Booking
                .Include(b => b.Flight)        
                .Include(b => b.Passenger)     
                .Include(b => b.BookingAmenities) 
                    .ThenInclude(ba => ba.Amenity)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null) return NotFound();
            return Page();
        }
    }
}
