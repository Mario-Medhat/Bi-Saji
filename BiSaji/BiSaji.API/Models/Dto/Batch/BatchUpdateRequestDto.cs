using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Batch
{
    public class BatchUpdateRequestDto
    {      
        [StringLength(255)]
        public string? Name { get; set; } = string.Empty;

        public string? LeaderId { get; set; } = string.Empty;
    }
}
