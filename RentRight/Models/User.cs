using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentRight.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the first name!")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter the last name!")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter the e-mail!")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter the password!")]
        public string Password { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
