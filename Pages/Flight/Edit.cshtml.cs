using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cailean_Razvan_Zboruri.Pages.Flight
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public EditModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cailean_Razvan_Zboruri.Models.Flight Flight { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight =  await _context.Flight.FirstOrDefaultAsync(m => m.ID == id);
            if (flight == null)
            {
                return NotFound();
            }
            Flight = flight;
           ViewData["ArrivalAirportID"] = new SelectList(_context.Airport, "ID", "City");
           ViewData["DepartureAirportID"] = new SelectList(_context.Airport, "ID", "City");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(Flight.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FlightExists(int id)
        {
            return _context.Flight.Any(e => e.ID == id);
        }
    }
}
