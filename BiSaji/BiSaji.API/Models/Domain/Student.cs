using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BiSaji.API.Models.Domain
{
    /// <summary>
    /// Represents a student with personal details and batch assignment.
    /// </summary>
    public class Student
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the full name of this student.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth of this student.
        /// </summary>
        [Required]
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the optional phone number of this student.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the optional phone number of this student's parent.
        /// </summary>
        public string? ParentPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets an optional additional phone number of this student's parent.
        /// </summary>
        public string? AdditionalParentPhoneNumber { get; set; }

        public Guid? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch this student is assigned to.
        /// </summary>
        [ForeignKey(nameof(BatchId))]
        public Batch? Batch { get; set; }
    }
}