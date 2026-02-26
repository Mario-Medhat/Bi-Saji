using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a batch that groups servants and days together.
    /// </summary>
    public class Batch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name of the batch.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the timestamp of when this batch was created.
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

        [Required]
        public string CreatedByServantId { get; init; } = string.Empty;

        /// <summary>
        /// Gets the servant who created this batch.
        /// </summary>
        [ForeignKey(nameof(CreatedByServantId))]
        public Servant? CreatedBy { get; init; }

        [Required]
        public string LeaderId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the servant assigned as the leader of this batch.
        /// </summary>
        [ForeignKey(nameof(LeaderId))]
        public Servant? Leader { get; set; }

        /// <summary>
        /// Gets or sets the collection of days associated with this batch.
        /// </summary>
        public List<Day> Days { get; set; } = new List<Day>();

        /// <summary>
        /// Gets or sets the collection of servants assigned to this batch.
        /// </summary>
        public List<Servant> Servants { get; set; } = new List<Servant>();
    }
}