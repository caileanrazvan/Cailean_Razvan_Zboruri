using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cailean_Razvan_Zboruri.Pages.Amenity
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
        public Cailean_Razvan_Zboruri.Models.Amenity Amenity { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity =  await _context.Amenity.FirstOrDefaultAsync(m => m.ID == id);
            if (amenity == null)
            {
                return NotFound();
            }
            Amenity = amenity;
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

            _context.Attach(Amenity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AmenityExists(Amenity.ID))
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

        private bool AmenityExists(int id)
        {
            return _context.Amenity.Any(e => e.ID == id);
        }
    }
}
