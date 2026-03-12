namespace BiSaji.API.Models.Dto.Room
{
    public class CreateRoomRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid PlaceId { get; set; }
        public Guid DayId { get; set; }
    }
}
