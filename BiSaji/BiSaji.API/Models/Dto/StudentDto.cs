using BiSaji.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string ParentPhoneNumber { get; set; } = string.Empty;

        public string AdditionalParentPhoneNumber { get; set; } = string.Empty;

        public Guid BatchId { get; set; }

    }
}
