using BiSaji.API.Enums;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Batch;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Users;

namespace BiSaji.API.Models.Dto.Day
{
    public class DayDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scheduled date for this day.
        /// </summary>
        public DateOnly ScheduledDate { get; set; }

        /// <summary>
        /// Gets the timestamp of when this day was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public string CreatedByServantId { get; init; } = string.Empty;
        public string CreatedBy { get; init; } = string.Empty;

        public int DayServantsCount { get; set; } = new();

        /// <summary>
        /// Gets or sets the collection of servants designated as leaders for this day.
        /// </summary>
        public List<ServantInfoDto> DayLeaders { get; set; } = new();

        public Guid BatchId { get; set; }
        public string BatchName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of this day.
        /// </summary>
        public Status Status { get; set; }
    }
}
