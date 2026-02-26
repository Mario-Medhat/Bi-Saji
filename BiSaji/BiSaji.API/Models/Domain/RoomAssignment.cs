using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents the assignment of a servant to a room with a specific role.
    /// </summary>
    public class RoomAssignment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RoomId { get; set; }

        /// <summary>
        /// Gets or sets the room associated with this assignment.
        /// </summary>
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        [Required]
        public string ServantId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the servant assigned to the room.
        /// </summary>
        [ForeignKey(nameof(ServantId))]
        public Servant? Servant { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the servant in this room.
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public RoomRole? Role { get; set; }
    }
}