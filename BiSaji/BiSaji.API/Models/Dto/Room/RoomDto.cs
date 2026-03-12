using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Day;

namespace BiSaji.API.Models.Dto.Room
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Place? Place { get; set; }
        public DayInfoDto? Day { get; set; }
    }
}
