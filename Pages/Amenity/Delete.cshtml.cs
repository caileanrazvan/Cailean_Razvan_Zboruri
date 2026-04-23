using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;
using Microsoft.AspNetCore.Authorization;

namespace Cailean_Razvan_Zboruri.Pages.Amenity
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public DeleteModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
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

            var amenity = await _context.Amenity.FirstOrDefaultAsync(m => m.ID == id);

            if (amenity == null)
            {
                return NotFound();
            }
            else
            {
                Amenity = amenity;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenity.FindAsync(id);
            if (amenity != null)
            {
                Amenity = amenity;
                _context.Amenity.Remove(Amenity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
