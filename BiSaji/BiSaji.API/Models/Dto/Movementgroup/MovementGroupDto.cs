using BiSaji.API.Models.Dto.Day;
using BiSaji.API.Models.Dto.Room;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Student;

namespace BiSaji.API.Models.Dto.Movementgroup
{
    public class MovementGroupDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DayInfoDto? Day { get; set; }
        public List<RoomDto> Rooms { get; set; } = new List<RoomDto>();
        public ServantInfoDto? ResponsibleServant { get; set; }
        public List<StudentInfoDto> Students { get; set; } = new List<StudentInfoDto>();
    }
}
