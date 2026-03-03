using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Servant
{
    public class RolesRequestDto
    {
        [Required]
        public string[] Roles { get; set; }
    }
}
