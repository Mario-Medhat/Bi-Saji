using BiSaji.API.Data;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLRoomRepository : IRoomRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLRoomRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Room> CreateAsync(Room room)
        {
            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();
            return room;
        }

        public async Task<Room?> DeleteAsync(Guid id)
        {
            var room = await GetByIdAsyncAsNoTracking(id);
            if (room == null)
            {
                return null;
            }
            dbContext.Rooms.Remove(room);
            await dbContext.SaveChangesAsync();
            return room;
        }

        public async Task<IEnumerable<Room>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            var rooms = dbContext.Rooms
                .AsNoTracking()
                .Include(r => r.Place)
                .Include(r => r.Day)
                .AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (!string.IsNullOrWhiteSpace(filterOn) &&
                !string.IsNullOrWhiteSpace(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "name":
                        rooms = rooms.Where(r => r.Name.Contains(filterQuery));
                        break;
                }
            }

            return await rooms.ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid id)
        {
            return await dbContext.Rooms
                .Include(r => r.Place)
                .Include(r => r.Day)    
                .Include(r => r.Assignments)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room?> UpdateAsync(Guid id, Room room)
        {
            var existingRoom = await GetByIdAsync(id);
            
            if (existingRoom == null) 
                return null;

            existingRoom.Name = room.Name ?? existingRoom.Name;
            existingRoom.PlaceId = room.PlaceId != Guid.Empty ? room.PlaceId : existingRoom.PlaceId;
            existingRoom.DayId = room.DayId != Guid.Empty ? room.DayId : existingRoom.DayId;

            await dbContext.SaveChangesAsync();

            existingRoom = await GetByIdAsync(id);
            return existingRoom;
        }

        public async Task<bool> IsDayIdValidAsync(Guid dayId)
        {
            return await dbContext.Days.AnyAsync(d => d.Id == dayId);
        }
        
        public async Task<bool> IsPlaceIdValidAsync(Guid dayId)
        {
            return await dbContext.Places.AnyAsync(d => d.Id == dayId);
        }

        private async Task<Room?> GetByIdAsyncAsNoTracking(Guid id)
        {
            return await dbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
