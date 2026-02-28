using BiSaji.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto
{
    public class StudentRegiesterRequestDto
    {

        [Required]
        public string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string? ParentPhoneNumber { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string? AdditionalParentPhoneNumber { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public Guid? BatchId { get; set; }

    }
}
