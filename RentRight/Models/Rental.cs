using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RentRight.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Apartment Id")]
        public int ApartmentId { get; set; }
        public virtual Apartment? Apartment { get; set; }
        [DisplayName("Tenant")]
        public int TenantId { get; set; }
        public virtual User? Tenant { get; set; }
        [DisplayName("Rented Date")]
        public DateTime? RentedDate { get; set; } = DateTime.UtcNow.Date;
        [DisplayName("Number of months")]
        public int Months { get; set; }
    }
}
