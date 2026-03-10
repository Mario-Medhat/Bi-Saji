namespace BiSaji.API.Models.Dto.Movementgroup
{
    public class UpdateMovementGroupRequestDto
    {
        public string? Name { get; set; }
        public Guid? DayId { get; set; }
        public string? ResponsibleServantId { get; set; }
    }
}
