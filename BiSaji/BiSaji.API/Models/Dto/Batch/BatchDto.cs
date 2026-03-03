using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto.Batch
{
    public class BatchDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the batch.
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the timestamp of when this batch was created.
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; init; }

        [Required]
        public string CreatedByServantId { get; init; } = string.Empty;

        /// <summary>
        /// Gets the servant who created this batch.
        /// </summary>
        public string? CreatedBy { get; init; }

        [Required]
        public string LeaderId { get; set; } = string.Empty;

        public string? Leader { get; set; }
    }
}
