using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a room within a specific place, associated with a day and assigned servants.
    /// </summary>
    public class Room
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name of this room.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid PlaceId { get; set; }

        /// <summary>
        /// Gets or sets the place where this room is located.
        /// </summary>
        [ForeignKey(nameof(PlaceId))]
        public Place? Place { get; set; }

        [Required]
        public Guid DayId { get; set; }

        /// <summary>
        /// Gets or sets the day this room is scheduled for.
        /// </summary>
        [ForeignKey(nameof(DayId))]
        public Day? Day { get; set; }

        /// <summary>
        /// Gets or sets the collection of servant assignments for this room, including their roles.
        /// </summary>
        public List<RoomAssignment> Assignments { get; set; } = new();
    }
}