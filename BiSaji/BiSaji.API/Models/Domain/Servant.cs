using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a servant user account with additional domain-specific properties.
    /// </summary>
    public class Servant : IdentityUser
    {
        /// <summary>
        /// Gets or sets the full name of this servant.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date this servant joined.
        /// </summary>
        [Required]
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public Guid? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch this servant is assigned to.
        /// </summary>
        [ForeignKey(nameof(BatchId))]
        public Batch? Batch { get; set; }

        /// <summary>
        /// Gets or sets the collection of days created by this servant.
        /// </summary>
        public List<Day> CreatedDays { get; set; } = new List<Day>();
    }
}