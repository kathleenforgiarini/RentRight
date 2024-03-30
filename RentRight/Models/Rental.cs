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
        [DisplayName("Property")]
        public int PropertyId { get; set; }
        public virtual Property? Property { get; set; }
        [DisplayName("Apartment Number")]
        public int ApartmentNumber { get; set; }
        [DisplayName("Tenant")]
        public int TenantId { get; set; }
        public virtual User? Tenant { get; set; }
        [DisplayName("Rented Date")]
        public DateTime? RentedDate { get; set; } = DateTime.Now;
        [DisplayName("Number of months")]
        public int Months { get; set; }
    }
}
