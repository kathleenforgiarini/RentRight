using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using RentRight.Models.Enums;

namespace RentRight.Models
{
    public class Apartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        [DisplayName("Pets Allowed")]
        public bool Pets { get; set; }
        public decimal Size { get; set; }
        [Required(ErrorMessage = "Please select a property.")]
        public int PropertyId { get; set; }
        public virtual Property? Property { get; set; }
        [DisplayName("Rent Price")]
        public decimal RentPrice { get; set; }
        public byte[] Photo { get; set; } = new byte[0];
        [NotMapped]
        [Required(ErrorMessage = "Please select a photo.")]
        [DisplayName("Photo")]
        public IFormFile? PhotoFile { get; set; }
        public string Status {  get; set; } = ApartmentStatus.Available.ToString();

    }
}
