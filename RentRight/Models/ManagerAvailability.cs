using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentRight.Models
{
    public class ManagerAvailability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public virtual User? Manager { get; set; }
        public string DayOfTheWeek { get; set; } = string.Empty;
        public TimeSpan Time {  get; set; } = DateTime.UtcNow.TimeOfDay;
        public bool IsScheduled { get; set; } = false;

    }
}
