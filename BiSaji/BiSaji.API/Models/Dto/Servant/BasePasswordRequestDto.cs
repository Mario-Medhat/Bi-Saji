using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Servant
{
    public class BasePasswordRequestDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
