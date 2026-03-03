using BiSaji.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto.Batch
{
    public class BatchCreateRequestDto
    {
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string LeaderId { get; set; } = string.Empty;
    }
}
