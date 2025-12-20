using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("bookingsconfirmation")]
    public class BookingConfirmation
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("bookingid")]
        public long BookingId { get; set; }

        // Parameterized constructor
        public BookingConfirmation(long bookingId)
        {
            BookingId = bookingId;
        }

        // Parameterless constructor (required by EF Core)
        public BookingConfirmation()
        {
        }
    }
}