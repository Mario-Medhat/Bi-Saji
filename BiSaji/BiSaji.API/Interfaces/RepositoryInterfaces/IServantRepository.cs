using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IServantRepository
    {
        public Task<IEnumerable<Servant>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Servant?> GetByIdAsync(Guid id);
        public Task<(IdentityResult, Servant)> CreateAsync(ServantRegiesterRequestDto regiesterRequestDto);
        public Task<(IdentityResult, Servant)> UpdateAsync(Guid id, SetvantUpdateRequestDto updateRequestDto);
        public Task<(IdentityResult, Servant?)> DeleteAsync(Guid id);
        public Task<IdentityResult> AddRolesToUserAsync(Servant servant, params string[] roles);

    }
}
