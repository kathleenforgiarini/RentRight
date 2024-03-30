using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RentRight.Models.Enums;

namespace RentRight.Models
{
    public class Appointments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public virtual User? Tenant { get; set; }
        public int ManagerId { get; set; }
        public virtual User? Manager { get; set;}
        public int ApartmentId { get; set; }
        public virtual Apartment? Apartment { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = AppointmentStatus.Pending.ToString();
    }
}
