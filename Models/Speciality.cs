using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Speciality
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the specialty name")]
        [StringLength(50, ErrorMessage = "Specialty name cannot exceed 50 characters")]
        public string Name { get; set; }

        [ValidateNever]
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
