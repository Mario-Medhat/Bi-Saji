using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<Servant>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Servant?> GetByIdAsync(Guid id);
        public Task<(IdentityResult, Servant)> CreateAsync(RegiesterRequestDto regiesterRequestDto);
        public Task<(IdentityResult, Servant)> UpdateAsync(Guid id, UpdateRequestDto updateRequestDto);
        public Task<Servant?> DeleteAsync(Guid id);
        public Task<IdentityResult> AddRolesToUserAsync(Servant servant, params string[] roles);

    }
}
