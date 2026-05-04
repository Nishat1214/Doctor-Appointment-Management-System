using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the doctor's name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Changed from `pass` to `Password` and from int to string

        [Required(ErrorMessage = "Please select a specialty")]
        public int SpecialtyId { get; set; }

        [ValidateNever]
        public virtual Speciality Specialty { get; set; }

        public bool IsApproved { get; set; } = false;

        [ValidateNever]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
