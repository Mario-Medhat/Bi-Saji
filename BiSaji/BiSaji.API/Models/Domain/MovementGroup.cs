using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a group of students that moves between rooms throughout the day.
    /// </summary>
    public class MovementGroup
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name of this movement group.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid DayId { get; set; }

        /// <summary>
        /// Gets or sets the day this movement group belongs to.
        /// </summary>
        [ForeignKey(nameof(DayId))]
        public Day? Day { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of rooms this movement group will visit throughout the day.
        /// </summary>
        public List<Room> Rooms { get; set; } = new List<Room>();

        [Required]
        public string ResponsibleServantId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the servant responsible for this movement group.
        /// </summary>
        [ForeignKey(nameof(ResponsibleServantId))]
        public Servant? ResponsibleServant { get; set; }

        /// <summary>
        /// Gets or sets the collection of students assigned to this movement group.
        /// </summary>
        public List<Student> Students { get; set; } = new List<Student>();
    }
}