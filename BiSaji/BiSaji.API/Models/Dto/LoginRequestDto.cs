using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto
{
    public class LoginRequestDto
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
