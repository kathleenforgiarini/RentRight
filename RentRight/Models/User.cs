using Microsoft.AspNetCore.Mvc;
using RentRight.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentRight.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;
        [DisplayName("E-mail")]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Type { get; set; } = TypeUsers.Tenant.ToString();
        [DisplayName("Active")]
        public bool IsActive { get; set; } = true;
    }
}
