using BiSaji.API.Models.Domain;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IMovementGroupRepository : IRepository<MovementGroup>
    {
        public Task<bool> IsDayIdValidAsync(Guid dayId);
        Task<bool> IsServantIdValidAsync(string givenServantId);
    }
}
