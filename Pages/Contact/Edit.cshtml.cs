using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Contact
{
    [Authorize(Roles = "Admin")] // RESTRICȚIE STRICTĂ DOAR PENTRU ADMINI
    public class EditModel : PageModel
    {
        private readonly AviationContext _context;

        public EditModel(AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ContactInfo ContactInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            ContactInfo = await _context.ContactInfo.FirstOrDefaultAsync(m => m.ID == id);

            if (ContactInfo == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(ContactInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactInfoExists(ContactInfo.ID)) return NotFound();
                else throw;
            }

            // Ne întoarcem la pagina de vizualizare după salvare
            return RedirectToPage("./Index");
        }

        private bool ContactInfoExists(int id)
        {
            return _context.ContactInfo.Any(e => e.ID == id);
        }
    }
}