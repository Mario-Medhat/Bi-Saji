using BiSaji.API.Models.Domain;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IMovementGroupRepository
    {
        public Task<MovementGroup> CreateAsync(MovementGroup day);
        public Task<IEnumerable<MovementGroup>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<MovementGroup?> GetByIdAsync(Guid id);
        public Task<MovementGroup?> UpdateAsync(Guid id, MovementGroup day);
        public Task<MovementGroup?> DeleteAsync(Guid id);
        public Task<bool> IsDayIdValidAsync(Guid dayId);
        Task<bool> IsServantIdValidAsync(string givenServantId);
    }
}
