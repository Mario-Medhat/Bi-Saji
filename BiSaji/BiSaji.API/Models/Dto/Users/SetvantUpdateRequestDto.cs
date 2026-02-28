using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Users
{
    public class SetvantUpdateRequestDto
    {
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
