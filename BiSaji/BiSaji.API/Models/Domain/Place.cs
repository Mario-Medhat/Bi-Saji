using System.ComponentModel.DataAnnotations;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a physical location that can be associated with a room.
    /// </summary>
    public class Place
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name of this place.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an optional description of this place.
        /// </summary>
        public string? Description { get; set; }
    }
}