using System.ComponentModel.DataAnnotations;

namespace Cailean_Razvan_Zboruri.Models
{
    public class ContactInfo
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "E-mailul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Format de e-mail invalid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefonul este obligatoriu.")]
        public string Phone { get; set; }

        public string Address { get; set; }

        public string WorkingHours { get; set; }

        public string Description { get; set; }
    }
}