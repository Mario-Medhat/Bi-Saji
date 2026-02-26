using System.ComponentModel.DataAnnotations;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a customizable role that can be assigned to a servant within a room.
    /// </summary>
    public class RoomRole
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name of the role (e.g. "Leader", "Assistant").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}