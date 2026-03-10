using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Movementgroup
{
    public class CreateMovementGroupRequestDto
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid DayId { get; set; }

        public string ResponsibleServantId { get; set; } = string.Empty;

    }
}
