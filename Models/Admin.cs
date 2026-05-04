using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Admin
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the admin's name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
