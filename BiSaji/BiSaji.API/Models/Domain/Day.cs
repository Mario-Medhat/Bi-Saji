using BiSaji.API.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a scheduled day within a batch, containing rooms, periods, and assigned servants.
    /// </summary>
    public class Day
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the scheduled date for this day.
        /// </summary>
        [Required]
        public DateOnly ScheduledDate { get; set; }

        /// <summary>
        /// Gets the timestamp of when this day was created.
        /// </summary>
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        [Required]
        public string CreatedByServantId { get; init; } = string.Empty;

        /// <summary>
        /// Gets the servant who created this day.
        /// </summary>
        [ForeignKey(nameof(CreatedByServantId))]
        public Servant? CreatedBy { get; init; }

        /// <summary>
        /// Gets or sets the collection of servants assigned to this day.
        /// </summary>
        public List<Servant> DayServants { get; set; } = new List<Servant>();

        /// <summary>
        /// Gets or sets the collection of servants designated as leaders for this day.
        /// </summary>
        public List<Servant> DayLeaders { get; set; } = new List<Servant>();

        /// <summary>
        /// Gets or sets the collection of rooms associated with this day.
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();

        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch this day belongs to.
        /// </summary>
        [ForeignKey(nameof(BatchId))]
        public Batch? Batch { get; set; }

        /// <summary>
        /// Gets or sets the collection of periods scheduled for this day.
        /// </summary>
        public List<Period> Periods { get; set; } = new();

        /// <summary>
        /// Gets or sets the current status of this day.
        /// </summary>
        public Status Status { get; set; }
    }
}