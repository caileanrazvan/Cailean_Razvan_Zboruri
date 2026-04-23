using System.ComponentModel.DataAnnotations;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Passenger
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Titlu")]
        public string Title { get; set; } // Mr, Mrs, etc.

        [Required(ErrorMessage = "Prenumele este obligatoriu.")]
        [RegularExpression(@"^[A-Z][a-zA-Z\s-]*$", ErrorMessage = "Prenumele trebuie să înceapă cu majusculă.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [RegularExpression(@"^[A-Z][a-zA-Z\s-]*$", ErrorMessage = "Numele trebuie să înceapă cu majusculă.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Facem Email-ul opțional la nivel de model pentru pasagerii secundari
        [EmailAddress(ErrorMessage = "Adresa de email nu este validă.")]
        public string? Email { get; set; }

        public string? PassportNumber { get; set; }
        public string FullName => $"{Title} {FirstName} {LastName}";

        // Legătura cu Rezervarea (Foreign Key)
        public int BookingID { get; set; }
        public Booking? Booking { get; set; }
    }
}