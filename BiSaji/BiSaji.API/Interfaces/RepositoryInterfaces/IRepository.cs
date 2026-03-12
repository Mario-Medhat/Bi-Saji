namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IRepository<T>
    {
        public Task<T> CreateAsync(T dm);
        public Task<IEnumerable<T>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<T?> GetByIdAsync(Guid id);
        public Task<T?> UpdateAsync(Guid id, T dm);
        public Task<T?> DeleteAsync(Guid id);
    }
}
