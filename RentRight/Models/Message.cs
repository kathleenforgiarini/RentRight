using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentRight.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public virtual User? Sender { get; set; }
        public int ReceivedId { get; set; }
        public virtual User? Receiver { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int ApartmentId { get; set; }
        public virtual Apartment? Apartment { get; set; }
        public DateTime SendDate { get; set; } = DateTime.UtcNow;
    }
}
