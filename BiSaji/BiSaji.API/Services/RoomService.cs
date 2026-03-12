using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Day;
using BiSaji.API.Models.Dto.Room;
using BiSaji.API.Models.Dto.Servant;

namespace BiSaji.API.Services
{
    public class RoomService
    {
        private readonly IRoomRepository roomRepository;
        private readonly ILogger<RoomService> logger;

        public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
        {
            this.roomRepository = roomRepository;
            this.logger = logger;
        }
        public async Task<RoomDto> CreateAsync(CreateRoomRequestDto createRoomRequestDto)
        {
            try
            {
                bool throwException = true;
                await IsPlaceIdValidAsync(createRoomRequestDto.PlaceId, throwException);
                await IsDayIdValidAsync(createRoomRequestDto.DayId, throwException);

                var room = new Room
                {
                    Name = createRoomRequestDto.Name,
                    PlaceId = createRoomRequestDto.PlaceId,
                    DayId = createRoomRequestDto.DayId
                };

                var createdRoom = await roomRepository.CreateAsync(room);
                logger.LogInformation($"Created a new room with ID {createdRoom.Id}.");
                createdRoom = await roomRepository.GetByIdAsync(createdRoom.Id);

                if (createdRoom == null)
                    throw new Exception();

                return MapRoom(createdRoom);
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new room.");
                throw;
            }
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                var rooms = await roomRepository.GetAllAsync(filterOn, filterQuery);
                logger.LogInformation("Retrieved {RoomCount} rooms from the repository.", rooms.Count());
                return MapRooms(rooms);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving all rooms.");
                throw;
            }
        }

        public async Task<RoomDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var room = await roomRepository.GetByIdAsync(id);
                if (room == null)
                    throw new NotFoundException($"No room found with ID {id}");

                return MapRoom(room);
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while retrieving the room with ID {id}.");
                throw;
            }
        }

        public async Task<RoomDto?> UpdateAsync(Guid id, UpdateRoomRequestDto updateRoomRequestDto)
        {
            try
            {
                bool throwException = false;
                await IsPlaceIdValidAsync(updateRoomRequestDto.PlaceId, throwException);
                await IsDayIdValidAsync(updateRoomRequestDto.DayId, throwException);
                var roomToUpdate = new Room
                {
                    Name = updateRoomRequestDto.Name,
                    PlaceId = updateRoomRequestDto.PlaceId,
                    DayId = updateRoomRequestDto.DayId
                };
                var updatedRoom = await roomRepository.UpdateAsync(id, roomToUpdate);

                if (updatedRoom == null)
                    throw new NotFoundException($"No room found with ID {id} to update.");

                logger.LogInformation($"Updated the room with ID {id}.");
                return MapRoom(updatedRoom);
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while updating the room with ID {id}.");
                throw;
            }
        }

        public async Task<RoomDto> DeleteAsync(Guid id)
        {
            try
            {
                var deletedRoom = await roomRepository.DeleteAsync(id);

                if (deletedRoom == null)
                    throw new NotFoundException($"No room found with ID {id} to delete.");

                logger.LogInformation($"Deleted the room with ID {id}.");
                return MapRoom(deletedRoom);
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while deleting the room with ID {id}.");
                throw;
            }
        }


        // TODO: Async the mapping process as well if it becomes a bottleneck
        private IEnumerable<RoomDto> MapRooms(IEnumerable<Room> rooms)
        {
            var roomDtos = new List<RoomDto>();
            foreach (var room in rooms)
            {
                var roomDto = MapRoom(room);
                roomDtos.Add(roomDto);
            }
            return roomDtos;

        }
        private RoomDto MapRoom(Room room)
        {
            var roomDto = new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Place = room.Place,
                Day = new DayInfoDto
                {
                    Id = room.Day?.Id ?? Guid.Empty,
                    Name = room.Day?.Name,
                    ScheduledDate = room.Day?.ScheduledDate ?? DateOnly.MinValue,
                    DayLeaders = room.Day?.DayLeaders.Select(dl => new ServantInfoDto
                    {
                        Id = dl.Id,
                        FullName = dl.FullName ?? "Unknown"
                    }).ToList() ?? new List<ServantInfoDto>()
                }
            };


            return roomDto;
        }
        private async Task IsDayIdValidAsync(Guid? givenDayId, bool throwErrorIfNullOrEmbty)
        {
            if (givenDayId != null && givenDayId != Guid.Empty)
            {
                var isValidGuid = Guid.TryParse(givenDayId.ToString(), out Guid placeId);
                if (!isValidGuid)
                    throw new InvalidDataException($"Invalid Day ID format: {givenDayId}.");

                var isDayIdValid = await roomRepository.IsDayIdValidAsync(placeId);
                if (!isDayIdValid)
                    throw new NotFoundException($"Day with ID {givenDayId} not found.");
            }
            else if (throwErrorIfNullOrEmbty)
            {
                throw new InvalidDataException("Day ID cannot be null or empty.");
            }
        }

        private async Task IsPlaceIdValidAsync(Guid? givenPlaceId, bool throwErrorIfNullOrEmbty)
        {
            if (givenPlaceId != null && givenPlaceId != Guid.Empty)
            {
                var isValidGuid = Guid.TryParse(givenPlaceId.ToString(), out Guid placeId);
                if (!isValidGuid)
                    throw new InvalidDataException($"Invalid Place ID format: {givenPlaceId}.");

                var isPlaceIdValid = await roomRepository.IsPlaceIdValidAsync(placeId);
                if (!isPlaceIdValid)
                    throw new NotFoundException($"Place with ID {givenPlaceId} not found.");
            }
            else if (throwErrorIfNullOrEmbty)
            {
                throw new InvalidDataException("Place ID cannot be null or empty.");
            }
        }
    }
}
