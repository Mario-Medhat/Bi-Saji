using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Day;
using BiSaji.API.Models.Dto.Movementgroup;
using BiSaji.API.Models.Dto.Room;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Student;

namespace BiSaji.API.Services
{
    public class MovementGroupService
    {
        private readonly IMovementGroupRepository movementGroupRepository;
        private readonly ILogger<MovementGroupService> logger;

        public MovementGroupService(IMovementGroupRepository movementGroupRepository, ILogger<MovementGroupService> logger)
        {
            this.movementGroupRepository = movementGroupRepository;
            this.logger = logger;
        }

        public async Task<MovementGroupDto> CreateAsync(CreateMovementGroupRequestDto createMovementGroupRequestDto)
        {
            try
            {
                bool throwErrorIfNullOrEmbty = true;
                await IsDayIdValidAsync(createMovementGroupRequestDto.DayId, throwErrorIfNullOrEmbty);
                await CheckServantIdValidation(createMovementGroupRequestDto.ResponsibleServantId, throwErrorIfNullOrEmbty);

                MovementGroup movementGroup = new MovementGroup
                {
                    Name = createMovementGroupRequestDto.Name,
                    DayId = createMovementGroupRequestDto.DayId,
                    ResponsibleServantId = createMovementGroupRequestDto.ResponsibleServantId,
                };

                movementGroup = await movementGroupRepository.CreateAsync(movementGroup);
                logger.LogInformation($"A new movement group with ID {movementGroup.Id} has been created.");
                var movementGroupDtos = await MapDmToDto(movementGroup);
                return movementGroupDtos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a new movement group.");
                throw;
            }
        }

        public async Task<IEnumerable<MovementGroupDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                var movementGroups = await movementGroupRepository.GetAllAsync(filterOn, filterQuery);
                logger.LogInformation($"Retrieved {movementGroups.Count()} movement groups from the database.");
                var movementGroupDtos = await MapDmToDto(movementGroups);
                return movementGroupDtos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving all movement groups.");
                throw;
            }
        }

        public async Task<MovementGroupDto> GetByIdAsync(Guid id)
        {
            try
            {
                var movementGroup = await movementGroupRepository.GetByIdAsync(id);
                if (movementGroup == null)
                    throw new NotFoundException($"Movement group with ID {id} not found.");

                logger.LogInformation($"Retrieved movement group with ID {id} from the database.");
                var movementGroupDtos = await MapDmToDto(movementGroup);
                return movementGroupDtos;
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning($"Movement group with ID {id} not found.");
                throw; // Rethrow the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the movement group with ID {Id}.", id);
                throw;
            }
        }

        public async Task<MovementGroupDto> UpdateAsync(Guid id, UpdateMovementGroupRequestDto updateMovementGroupRequestDto)
        {
            try
            {
                bool throwErrorIfNullOrEmbty = false;
                await IsDayIdValidAsync(updateMovementGroupRequestDto.DayId, throwErrorIfNullOrEmbty);
                await CheckServantIdValidation(updateMovementGroupRequestDto.ResponsibleServantId, throwErrorIfNullOrEmbty);

                MovementGroup movementGroup = new MovementGroup
                {
                    Name = updateMovementGroupRequestDto.Name ?? string.Empty,
                    DayId = updateMovementGroupRequestDto.DayId ?? Guid.Empty,
                    ResponsibleServantId = updateMovementGroupRequestDto.ResponsibleServantId ?? string.Empty,
                };

                var updatedMovementGroup = await movementGroupRepository.UpdateAsync(id, movementGroup);
                if (updatedMovementGroup == null)
                    throw new NotFoundException($"Movement group with ID {id} not found.");

                logger.LogInformation($"Movement group with ID {id} has been updated.");
                var updatedMovementGroupDtos = await MapDmToDto(updatedMovementGroup);
                return updatedMovementGroupDtos;
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning($"Movement group with ID {id} not found for update.");
                throw; // Rethrow the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the movement group with ID {Id}.", id);
                throw;
            }
        }

        public async Task<MovementGroupDto> DeleteAsync(Guid id)
        {
            try
            {
                var deletedMovementGroup = await movementGroupRepository.DeleteAsync(id);
                if (deletedMovementGroup == null)
                    throw new NotFoundException($"Movement group with ID {id} not found.");

                logger.LogInformation($"Movement group with ID {id} has been deleted.");
                var deletedMovementGroupDtos = await MapDmToDto(deletedMovementGroup);
                return deletedMovementGroupDtos;
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning($"Movement group with ID {id} not found for deletion.");
                throw; // Rethrow the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting the movement group with ID {Id}.", id);
                throw;
            }
        }

        private async Task CheckServantIdValidation(string? givenServantId, bool throwErrorIfNullOrEmbty)
        {
            if (!string.IsNullOrWhiteSpace(givenServantId))
            {
                var isServantIdValid = await movementGroupRepository.IsServantIdValidAsync(givenServantId);
                if (!isServantIdValid)
                    throw new NotFoundException($"No servant with ID {givenServantId} found.");
            }
            else if (throwErrorIfNullOrEmbty)
            {
                throw new InvalidDataException("Responsible Servant ID cannot be null or empty.");
            }
        }
        private async Task IsDayIdValidAsync(Guid? givenDayId, bool throwErrorIfNullOrEmbty)
        {
            if (givenDayId != null && givenDayId != Guid.Empty)
            {
                var isValidGuid = Guid.TryParse(givenDayId.ToString(), out Guid dayId);
                if (!isValidGuid)
                    throw new InvalidDataException($"Invalid Day ID format: {givenDayId}.");

                var isDayIdValid = await movementGroupRepository.IsDayIdValidAsync(dayId);
                if (!isDayIdValid)
                    throw new NotFoundException($"Day with ID {givenDayId} not found.");
            }
            else if (throwErrorIfNullOrEmbty)
            {
                throw new InvalidDataException("Day ID cannot be null or empty.");
            }
        }

        private async Task<MovementGroupDto> MapDmToDto(MovementGroup movementGroup)
        {
            var movementGroupDto = new MovementGroupDto
            {
                Id = movementGroup.Id,
                Name = movementGroup.Name,
                Day = movementGroup.Day != null ? new DayInfoDto
                {
                    Id = movementGroup.Day.Id,
                    Name = movementGroup.Day.Name,
                    ScheduledDate = movementGroup.Day.ScheduledDate,
                    DayLeaders = movementGroup.Day.DayLeaders?.Select(dl => new ServantInfoDto
                    {
                        Id = dl.Id,
                        FullName = dl.FullName,
                        PhoneNumber = dl.PhoneNumber
                    }).ToList() ?? new List<ServantInfoDto>()
                } : null,
                ResponsibleServant = movementGroup.ResponsibleServant != null ? new ServantInfoDto
                {
                    Id = movementGroup.ResponsibleServant.Id,
                    FullName = movementGroup.ResponsibleServant.FullName,
                    PhoneNumber = movementGroup.ResponsibleServant.PhoneNumber
                } : null,
                Rooms = movementGroup.Rooms?.Select(r => new RoomDto
                {
                    // Assuming Room has properties Id and Name,
                    // TODO: replace with actual properties
                }).ToList() ?? new List<RoomDto>(),
                Students = movementGroup.Students?.Select(s => new StudentInfoDto
                {
                    Id = s.Id,
                    FullName = s.FullName,
                }).ToList() ?? new List<StudentInfoDto>()
            };
            return movementGroupDto;
        }
        private async Task<IEnumerable<MovementGroupDto>> MapDmToDto(IEnumerable<MovementGroup> movementGroups)
        {
            var movementGroupDtos = new List<MovementGroupDto>();

            foreach (var movementGroup in movementGroups)
                movementGroupDtos.Add(await MapDmToDto(movementGroup));

            return movementGroupDtos;
        }
    }
}
