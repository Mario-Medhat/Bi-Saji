using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Users
{
    public class LoginRequestDto
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
