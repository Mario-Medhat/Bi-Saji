using BiSaji.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Day
{
    public class DayUpdateRequestDto
    {
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the scheduled date for this day.
        /// </summary>
        [Required]
        public DateOnly ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the batch.
        /// </summary>
        /// <remarks>The BatchId is required for identifying the batch in processing workflows. Ensure
        /// that a valid GUID is provided.</remarks>
        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the current status of this day.
        /// </summary>
        public Status Status { get; set; }
    }
}
