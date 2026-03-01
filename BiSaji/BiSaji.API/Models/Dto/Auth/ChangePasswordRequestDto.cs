using BiSaji.API.Models.Dto.Servant;
using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Auth
{
    public class ChangePasswordRequestDto : BasePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }
    }
}
