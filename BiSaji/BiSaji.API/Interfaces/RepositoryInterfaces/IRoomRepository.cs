using BiSaji.API.Models.Domain;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IRoomRepository
    {
        public Task<Room> CreateAsync(Room room);
        public Task<IEnumerable<Room>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Room?> GetByIdAsync(Guid id);
        public Task<Room?> UpdateAsync(Guid id, Room room);
        public Task<Room?> DeleteAsync(Guid id);
        public Task<bool> IsDayIdValidAsync(Guid id);
        public Task<bool> IsPlaceIdValidAsync(Guid id);

    }
}
