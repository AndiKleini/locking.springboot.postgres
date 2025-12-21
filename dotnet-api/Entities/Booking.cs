using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("bookings")]
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("roomid")]
        public long RoomId { get; set; }

        [Column("start")]
        public DateTime Start { get; set; }

        [Column("finish")]
        public DateTime Finish { get; set; }

        // Parameterized constructor
        public Booking(string name, long roomId, DateTime start, DateTime finish)
        {
            Name = name;
            RoomId = roomId;
            Start = start;
            Finish = finish;
        }

        // Parameterless constructor (required by EF Core)
        public Booking()
        {
        }
    }
}