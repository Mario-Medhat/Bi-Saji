using BiSaji.API.Enums;
using BiSaji.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto.Day
{
    public class DayCreateRequestDto
    {
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the scheduled date for this day.
        /// </summary>
        [Required]
        public DateOnly ScheduledDate { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the current status of this day.
        /// </summary>
        public Status Status { get; set; }
    }
}
