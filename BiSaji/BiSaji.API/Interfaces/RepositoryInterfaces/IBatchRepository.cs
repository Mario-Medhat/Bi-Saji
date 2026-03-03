using BiSaji.API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IBatchRepository
    {
        public Task<Batch> CreateAsync(Batch batch);
        public Task<IEnumerable<Batch>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Batch?> GetByIdAsync(Guid id);
        public Task<Batch?> UpdateAsync(Guid id, Batch batch);
        public Task<Batch?> DeleteAsync(Guid id);
    }
}
