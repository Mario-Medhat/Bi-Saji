using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a time period within a day, during which movement groups are assigned to specific rooms.
    /// </summary>
    public class Period
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the start time of this period.
        /// </summary>
        [Required]
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of this period.
        /// </summary>
        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public Guid DayId { get; set; }

        /// <summary>
        /// Gets or sets the day this period belongs to.
        /// </summary>
        [ForeignKey(nameof(DayId))]
        public Day? Day { get; set; }

        /// <summary>
        /// Gets or sets the collection of assignments distributing movement groups across rooms during this period.
        /// </summary>
        public List<PeriodAssignment> Assignments { get; set; } = new();
    }
}