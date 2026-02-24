using BiSaji.API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<IdentityUser>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<IdentityUser?> GetByIdAsync(Guid id);
        public Task<(IdentityResult, IdentityUser)> CreateAsync(RegiesterRequestDto identityUser);
        public Task<(IdentityResult, IdentityUser)> UpdateAsync(Guid id, UpdateRequestDto updateRequestDto);
        public Task<IdentityUser?> DeleteAsync(Guid id);
        public Task<IdentityResult> AddRolesToUserAsync(IdentityUser identityUser, params string[] roles);

    }
}
