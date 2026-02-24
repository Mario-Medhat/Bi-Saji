using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
