using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cailean_Razvan_Zboruri.Data;
using Cailean_Razvan_Zboruri.Models;

namespace Cailean_Razvan_Zboruri.Pages.Booking
{
    public class EditModel : BookingAmenitiesPageModel
    {
        private readonly Cailean_Razvan_Zboruri.Data.AviationContext _context;

        public EditModel(Cailean_Razvan_Zboruri.Data.AviationContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Cailean_Razvan_Zboruri.Models.Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            // PASUL 2: Incarcam rezervarea impreuna cu facilitatile DEJA existente (Many-to-Many)
            Booking = await _context.Booking
                .Include(b => b.Flight)
                .Include(b => b.Passenger)
                .Include(b => b.BookingAmenities).ThenInclude(b => b.Amenity)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Booking == null) return NotFound();

            // PASUL 3: Dropdown-uri corecte (afiseaza Nume/Numar, nu ID)
            ViewData["PassengerID"] = new SelectList(_context.Passenger, "ID", "FullName", Booking.PassengerID);
            ViewData["FlightID"] = new SelectList(_context.Flight, "ID", "FlightNumber", Booking.FlightID);

            // PASUL 4: Aici nu vei mai avea eroare, pentru ca mostenim clasa corecta
            PopulateAssignedAmenityData(_context, Booking);

            return Page();
        }

        // PASUL 5: Primim lista de checkbox-uri bifate (selectedAmenities)
        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedAmenities)
        {
            if (id == null) return NotFound();

            // Trebuie sa incarcam entitatea din baza de date pentru a o actualiza
            // Este CRITIC sa includem BookingAmenities aici pentru a putea sterge/adauga
            var bookingToUpdate = await _context.Booking
                .Include(i => i.BookingAmenities)
                    .ThenInclude(i => i.Amenity)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (bookingToUpdate == null) return NotFound();

            // Actualizam proprietatile simple
            if (await TryUpdateModelAsync<Cailean_Razvan_Zboruri.Models.Booking>(
                bookingToUpdate,
                "Booking",
                i => i.BookingDate, i => i.SeatNumber, i => i.PassengerID, i => i.FlightID))
            {
                // PASUL 6: Apelam functia de actualizare a checkbox-urilor
                UpdateBookingAmenities(_context, selectedAmenities, bookingToUpdate);

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // Daca ceva a esuat, repopulam datele pentru a reafisa pagina
            PopulateAssignedAmenityData(_context, bookingToUpdate);
            ViewData["PassengerID"] = new SelectList(_context.Passenger, "ID", "FullName", bookingToUpdate.PassengerID);
            ViewData["FlightID"] = new SelectList(_context.Flight, "ID", "FlightNumber", bookingToUpdate.FlightID);

            return Page();
        }

        // PASUL 7: Metoda care se ocupa de logica Many-to-Many (Sterge ce e debifat, Adauga ce e bifat)
        // Aceasta este adaptata din Laborator 3 (UpdateBookCategories)
        public void UpdateBookingAmenities(AviationContext context, string[] selectedAmenities, Cailean_Razvan_Zboruri.Models.Booking bookingToUpdate)
        {
            if (selectedAmenities == null)
            {
                bookingToUpdate.BookingAmenities = new List<BookingAmenity>();
                return;
            }

            var selectedAmenitiesHS = new HashSet<string>(selectedAmenities);
            var bookingAmenities = new HashSet<int>(bookingToUpdate.BookingAmenities.Select(c => c.Amenity.ID));

            foreach (var amenity in context.Amenity)
            {
                if (selectedAmenitiesHS.Contains(amenity.ID.ToString()))
                {
                    if (!bookingAmenities.Contains(amenity.ID))
                    {
                        bookingToUpdate.BookingAmenities.Add(new BookingAmenity { BookingID = bookingToUpdate.ID, AmenityID = amenity.ID });
                    }
                }
                else
                {
                    if (bookingAmenities.Contains(amenity.ID))
                    {
                        BookingAmenity amenityToRemove = bookingToUpdate.BookingAmenities.FirstOrDefault(i => i.AmenityID == amenity.ID);
                        context.Remove(amenityToRemove);
                    }
                }
            }
        }
    }
}