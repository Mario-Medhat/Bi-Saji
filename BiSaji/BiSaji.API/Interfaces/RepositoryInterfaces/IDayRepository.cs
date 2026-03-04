using BiSaji.API.Models.Domain;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IDayRepository
    {
        public Task<Day> CreateAsync(Day day);
        public Task<IEnumerable<Day>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Day?> GetByIdAsync(Guid id);
        public Task<Day?> UpdateAsync(Guid id, Day day);
        public Task<Day?> DeleteAsync(Guid id);
    }
}
