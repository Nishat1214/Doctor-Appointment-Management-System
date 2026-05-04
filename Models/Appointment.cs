using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Appointment
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please enter the appointment date and time")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string Notes { get; set; }

        [ValidateNever]
        public virtual Patient Patient { get; set; }

        [ValidateNever]
        public virtual Doctor Doctor { get; set; }
    }
}
