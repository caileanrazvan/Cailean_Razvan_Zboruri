using System.ComponentModel.DataAnnotations;

namespace Cailean_Razvan_Zboruri.Models
{
    public class Passenger
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Prenumele este obligatoriu.")]
        [RegularExpression(@"^[A-Z][a-zA-Z\s-]*$", ErrorMessage = "Prenumele trebuie să înceapă cu majusculă.")]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        [RegularExpression(@"^[A-Z][a-zA-Z\s-]*$", ErrorMessage = "Numele trebuie să înceapă cu majusculă.")]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Adresa de email nu este validă.")]
        public string Email { get; set; }

        [RegularExpression(@"^[A-Z]{2}[0-9]{6,8}$", ErrorMessage = "Pașaport invalid (Format: AA123456).")]
        public string PassportNumber { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public ICollection<Booking>? Bookings { get; set; }
    }
}