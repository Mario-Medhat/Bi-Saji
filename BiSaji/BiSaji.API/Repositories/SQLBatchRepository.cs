using BiSaji.API.Data;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLBatchRepository : IBatchRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLBatchRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Batch> CreateAsync(Batch batch)
        {
            await dbContext.Batches.AddAsync(batch);
            await dbContext.SaveChangesAsync();

            batch = (await GetByIdAsync(batch.Id))!;
            return batch;
        }

        public async Task<Batch?> DeleteAsync(Guid id)
        {
            var existingBatch = await dbContext.Batches.FindAsync(id);

            if (existingBatch == null)
                return null;

            dbContext.Batches.Remove(existingBatch);

            await dbContext.SaveChangesAsync();
            return existingBatch;
        }

        public async Task<IEnumerable<Batch>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            var batchs = dbContext.Batches
                .AsNoTracking() // Avoid tracking for read-only operations to improve performance
                .Include(b => b.Leader) // Include the related Leader entity to avoid lazy loading issues
                .Include(b => b.CreatedBy) // Include the related Leader entity to avoid lazy loading issues
                .AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (!string.IsNullOrWhiteSpace(filterOn) &&
                !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "name":
                        batchs = batchs.Where(batch => EF.Functions.Like(batch.Name, $"%{filterQuery}%"));
                        break;
                    case "leader":
                    case "leadername":
                        batchs = batchs.Where(batch =>
                            batch.Leader != null &&
                            EF.Functions.Like(batch.Leader.FullName, $"%{filterQuery}%"));
                        break;
                    case "leaderphone":
                    case "leaderphonenumber":
                        batchs = batchs.Where(batch =>
                            batch.Leader != null &&
                            EF.Functions.Like(batch.Leader.PhoneNumber, $"%{filterQuery}%"));
                        break;
                }
            }
            return await batchs.ToListAsync();
        }

        public async Task<Batch?> GetByIdAsync(Guid id)
        {
            return await dbContext.Batches
                .AsNoTracking() // Avoid tracking for read-only operations to improve performance
                .Include(b => b.Leader) // Include the related Leader entity to avoid lazy loading issues
                .Include(b => b.CreatedBy) // Include the related Leader entity to avoid lazy loading issues
                .FirstOrDefaultAsync(batch => batch.Id == id);
        }

        public async Task<Batch?> UpdateAsync(Guid id, Batch updatedBatch)
        {
            var existingBatch = await dbContext.Batches
                .Include(b => b.Leader) // Include the related Leader entity to avoid lazy loading issues
                .Include(b => b.CreatedBy) // Include the related Leader entity to avoid lazy loading issues
                .FirstOrDefaultAsync(b => b.Id == id); // Without AsNoTracking to update data

            if (existingBatch == null)
                return null;

            if (!string.IsNullOrWhiteSpace(updatedBatch.Name))
                existingBatch.Name = updatedBatch.Name;

            if (!string.IsNullOrWhiteSpace(updatedBatch.LeaderId))
                existingBatch.LeaderId = updatedBatch.LeaderId;

            await dbContext.SaveChangesAsync();
            return existingBatch;
        }
    }
}
