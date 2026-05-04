using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class AppointmentViewModel
    {

        public int DoctorId { get; set; }
        public string PatientName { get; set; }
        [Required(ErrorMessage = "Please enter a mobile number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string Mobile { get; set; }
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
    }
}
