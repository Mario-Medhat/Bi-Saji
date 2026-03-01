using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Users
{
    public class ServantRegiesterRequestDto
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Guid? BatchId { get; set; }

        public string[] Roles { get; set; }
    }
}
