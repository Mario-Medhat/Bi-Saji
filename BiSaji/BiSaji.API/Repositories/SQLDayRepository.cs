using BiSaji.API.Data;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLDayRepository : IDayRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLDayRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Day> CreateAsync(Day day)
        {
            await dbContext.Days.AddAsync(day);
            await dbContext.SaveChangesAsync();
            return day;
        }

        public async Task<Day?> DeleteAsync(Guid id)
        {
            Day? deletedDay = await GetByIdAsync(id);
            if (deletedDay != null)
                dbContext.Days.Remove(deletedDay);
            await dbContext.SaveChangesAsync();
            return deletedDay;
        }

        public async Task<IEnumerable<Day>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            var days = dbContext.Days
                .AsNoTracking() // Avoid tracking for read-only operations to improve performance
                .Include(b => b.CreatedBy)
                .Include(b => b.DayServants)
                .Include(b => b.DayLeaders)
                .Include(b => b.Batch)
                .AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (!string.IsNullOrWhiteSpace(filterOn) &&
                !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    // Allow filtering by the creator's full name using a case-insensitive partial match
                    case "created":
                    case "createdby":
                    case "creator":
                        days = days.Where(day =>
                            day.CreatedBy != null &&
                            EF.Functions.Like(day.CreatedBy.FullName, $"%{filterQuery}%"));
                        break;

                    // Allow filtering by the servants' full names using a case-insensitive partial match
                    case "servant":
                    case "servants":
                    case "dayservant":
                    case "dayservants":
                        days = days.Where(day => day.DayServants.Any(servant => EF.Functions.Like(servant.FullName, $"%{filterQuery}%")));
                        break;

                    // Allow filtering by the leaders' full names using a case-insensitive partial match
                    case "leader":
                    case "leaders":
                    case "dayleader":
                    case "dayleaders":
                        days = days.Where(day => day.DayLeaders.Any(leader => EF.Functions.Like(leader.FullName, $"%{filterQuery}%")));
                        break;

                    // Allow filtering by the leaders' phone numbers using a case-insensitive partial match
                    case "leaderphone":
                    case "leaderphonenumber":
                        days = days.Where(day => day.DayLeaders.Any(leader => EF.Functions.Like(leader.PhoneNumber, $"%{filterQuery}%")));
                        break;

                    // Batch filtering can be done by name or ID, so we check for both
                    case "batch":
                        days = days.Where(day =>
                            day.Batch != null &&
                                (EF.Functions.Like(day.Batch.Name, $"%{filterQuery}%") ||
                                day.Batch.Id.ToString() == $"{filterQuery}"));
                        break;

                    // Allow filtering by batch name using a case-insensitive partial match
                    case "batchname":
                        days = days.Where(day =>
                            day.Batch != null &&
                            EF.Functions.Like(day.Batch.Name, $"%{filterQuery}%"));
                        break;

                    // Allow filtering by batch ID using an exact match
                    case "batchid":
                        days = days.Where(day =>
                            day.Batch != null &&
                            day.Batch.Id.ToString() == $"{filterQuery}");
                        break;
                }
            }
            return await days.ToListAsync();
        }

        public Task<Day?> GetByIdAsync(Guid id)
        {
            return dbContext.Days
                .AsNoTracking() // Avoid tracking for read-only operations to improve performance
                .Include(b => b.CreatedBy)
                .Include(b => b.DayServants)
                .Include(b => b.DayLeaders)
                .Include(b => b.Batch)
                .FirstOrDefaultAsync(day => day.Id == id);
        }

        public async Task<Day?> UpdateAsync(Guid id, Day day)
        {
            var existingDay = await dbContext.Days
                .Include(b => b.CreatedBy)
                .Include(b => b.DayServants)
                .Include(b => b.DayLeaders)
                .Include(b => b.Batch)
                .FirstOrDefaultAsync(day => day.Id == id);

            if (existingDay == null)
                return null;

            // Update the properties of the existing day with the new values
            existingDay.Name = day.Name;
            existingDay.ScheduledDate = day.ScheduledDate;
            existingDay.DayServants = day.DayServants;
            existingDay.DayLeaders = day.DayLeaders;
            existingDay.BatchId = day.BatchId;
            existingDay.Periods = day.Periods;
            existingDay.Status = day.Status;

            await dbContext.SaveChangesAsync();
            return existingDay;
        }
    }
}
