using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents the assignment of a movement group to a specific room during a period.
    /// </summary>
    public class PeriodAssignment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PeriodId { get; set; }

        /// <summary>
        /// Gets or sets the period associated with this assignment.
        /// </summary>
        [ForeignKey(nameof(PeriodId))]
        public Period? Period { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        /// <summary>
        /// Gets or sets the room associated with this assignment.
        /// </summary>
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        [Required]
        public Guid MovementGroupId { get; set; }

        /// <summary>
        /// Gets or sets the movement group assigned to the room during this period.
        /// </summary>
        [ForeignKey(nameof(MovementGroupId))]
        public MovementGroup? MovementGroup { get; set; }
    }
}