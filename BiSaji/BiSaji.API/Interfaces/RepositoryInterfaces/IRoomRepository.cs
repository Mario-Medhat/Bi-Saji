using BiSaji.API.Models.Domain;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IRoomRepository : IRepository<Room>
    {
        public Task<bool> IsDayIdValidAsync(Guid id);
        public Task<bool> IsPlaceIdValidAsync(Guid id);
    }
}
