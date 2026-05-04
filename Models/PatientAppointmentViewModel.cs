using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date <= DateTime.Now)
                {
                    return new ValidationResult("Appointment date and time must be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class PatientAppointmentViewModel
    {
        public Patient Patient { get; set; }

        [Required(ErrorMessage = "Please select an appointment date and time")]
        [FutureDate(ErrorMessage = "Appointment date and time must be in the future.")]
        public DateTime AppointmentDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a valid positive number")]
        public int DoctorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Specialty ID must be a valid positive number")]
        public int SpecialtyId { get; set; }

        [Required(ErrorMessage = "Please enter notes for the appointment")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string AppointmentNotes { get; set; }
    }
}