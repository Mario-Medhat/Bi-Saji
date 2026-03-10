using BiSaji.API.Models.Dto.Servant;

namespace BiSaji.API.Models.Dto.Day
{
    public class DayInfoDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public DateOnly ScheduledDate { get; set; }
        public List<ServantInfoDto> DayLeaders { get; set; } = new();
    }
}
