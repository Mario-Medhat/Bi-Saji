using BiSaji.API.Data;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLMovementGroupRepository : IMovementGroupRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLMovementGroupRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<MovementGroup> CreateAsync(MovementGroup movementGroup)
        {
            await dbContext.MovementGroups.AddAsync(movementGroup);
            await dbContext.SaveChangesAsync();
            return movementGroup;
        }

        public async Task<MovementGroup?> DeleteAsync(Guid id)
        {
            MovementGroup? deletedMovementGroup = await GetByIdAsync(id);
            if (deletedMovementGroup != null)
                dbContext.MovementGroups.Remove(deletedMovementGroup);
            await dbContext.SaveChangesAsync();
            return deletedMovementGroup;
        }

        public async Task<IEnumerable<MovementGroup>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            var movementGroups = dbContext.MovementGroups
                .AsNoTracking()
                .Include(mv => mv.Day)
                .Include(mv => mv.ResponsibleServant)
                .Include(mv => mv.Students)
                .AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (!string.IsNullOrWhiteSpace(filterOn) &&
                !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "name":
                        movementGroups = movementGroups.Where(mv =>
                            EF.Functions.Like(mv.Name, $"%{filterQuery}%"));
                        break;

                    case "responsibleservant":
                    case "servant":
                    case "responsible":
                        movementGroups = movementGroups.Where(mv =>
                            mv.ResponsibleServant != null &&
                            EF.Functions.Like(mv.ResponsibleServant.FullName, $"%{filterQuery}%"));
                        break;

                    case "student":
                        movementGroups = movementGroups.Where(mv =>
                            mv.Students != null &&
                            mv.Students.Any(s =>
                                EF.Functions.Like(s.FullName, $"%{filterQuery}%")));
                        break;
                }
            }
            return await movementGroups.ToListAsync();
        }

        public async Task<MovementGroup?> GetByIdAsync(Guid id)
        {
            return await dbContext.MovementGroups
                .AsNoTracking() // Avoid tracking for read-only operations to improve performance
                .Include(mv => mv.Day)
                .Include(mv => mv.ResponsibleServant)
                .Include(mv => mv.Students)
                .FirstOrDefaultAsync(mv => mv.Id == id);
        }

        public async Task<MovementGroup?> UpdateAsync(Guid id, MovementGroup movementGroup)
        {
            MovementGroup? existingMovementGroup = await dbContext.MovementGroups
                .FirstOrDefaultAsync(mv => mv.Id == id);

            if (existingMovementGroup == null)
                return null;

            if (!string.IsNullOrWhiteSpace(movementGroup.Name))
                existingMovementGroup.Name = movementGroup.Name;
            if (movementGroup.DayId != Guid.Empty)
                existingMovementGroup.DayId = movementGroup.DayId;
            if (!string.IsNullOrWhiteSpace(movementGroup.ResponsibleServantId))
                existingMovementGroup.ResponsibleServantId = movementGroup.ResponsibleServantId;

            await dbContext.SaveChangesAsync();
            return existingMovementGroup;
        }

        public async Task<bool> IsDayIdValidAsync(Guid dayId)
        {
            return await dbContext.Days.AnyAsync(d => d.Id == dayId);
        }

        public async Task<bool> IsServantIdValidAsync(string servantId)
        {
            return await dbContext.Users.AnyAsync(u => u.Id == servantId);
        }
    }
}