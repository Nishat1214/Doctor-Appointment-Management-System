using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the patient's name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select the patient's sex")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Please enter a mobile number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please enter the date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } // Note: No PastDateAttribute for now, as per the model

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Age must be a non-negative number")]
        public int Age { get; set; }

        [ValidateNever]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}