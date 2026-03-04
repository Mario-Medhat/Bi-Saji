using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Day;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Users;

namespace BiSaji.API.Services
{
    public class DayService
    {
        private readonly IDayRepository dayRepository;
        private readonly ILogger<DayService> logger;
        private readonly IServantRepository servantRepository;
        private readonly IBatchRepository batchRepository;

        public DayService(IDayRepository dayRepository, ILogger<DayService> logger, IServantRepository servantRepository, IBatchRepository batchRepository)
        {
            this.dayRepository = dayRepository;
            this.logger = logger;
            this.servantRepository = servantRepository;
            this.batchRepository = batchRepository;
        }

        public async Task<DayDto> CreateAsync(DayCreateRequestDto dayCreateRequest, string createdByServantId)
        {
            try
            {
                var batch = await batchRepository.GetByIdAsync(dayCreateRequest.BatchId);

                if (batch == null)
                    throw new NotFoundException($"No batch found with ID {dayCreateRequest.BatchId}");

                var day = new Day
                {
                    Name = dayCreateRequest.Name,
                    ScheduledDate = dayCreateRequest.ScheduledDate,
                    BatchId = dayCreateRequest.BatchId,
                    Status = dayCreateRequest.Status,
                    CreatedByServantId = createdByServantId,
                };

                await dayRepository.CreateAsync(day);
                logger.LogInformation("Day created successfully with ID: {DayId}", day.Id);

                var dayDto = await GetByIdAsync(day.Id);
                return dayDto!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a day.");
                throw;
            }
        }

        public async Task<DayDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var day = await dayRepository.GetByIdAsync(id);

                if (day == null)
                    throw new NotFoundException($"No day found with ID {id}");


                var dayleaders = new List<ServantInfoDto>();
                foreach (var dayLeader in day.DayLeaders)
                {
                    var servant = await servantRepository.GetByIdAsync(Guid.Parse(dayLeader.Id));
                    if (servant != null)
                    {
                        dayleaders.Add(new ServantInfoDto
                        {
                            Id = servant.Id,
                            FullName = servant.FullName,
                            PhoneNumber = servant.PhoneNumber ?? "",
                            Roles = await servantRepository.GetRolesAsync(servant),
                        });
                    }
                }

                DayDto dayDto = new DayDto
                {
                    Id = day.Id,
                    Name = day.Name,
                    ScheduledDate = day.ScheduledDate,
                    CreatedDate = day.CreatedDate,
                    CreatedBy = day.CreatedBy != null ? day.CreatedBy.FullName : "",
                    CreatedByServantId = day.CreatedByServantId,
                    DayServantsCount = day.DayServants.Count,
                    DayLeaders = dayleaders,
                    BatchId = day.BatchId,
                    BatchName = day.Batch != null ? day.Batch.Name : "",
                    Status = day.Status
                };
                return dayDto;

            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Day with ID {DayId} not found.", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving day with ID {DayId}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<DayDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                var days = await dayRepository.GetAllAsync(filterOn, filterQuery);
                var dayDtos = new List<DayDto>();
                foreach (var day in days)
                {
                    var dayleaders = new List<ServantInfoDto>();
                    foreach (var dayLeader in day.DayLeaders)
                    {
                        var servant = await servantRepository.GetByIdAsync(Guid.Parse(dayLeader.Id));
                        if (servant != null)
                        {
                            dayleaders.Add(new ServantInfoDto
                            {
                                Id = servant.Id,
                                FullName = servant.FullName,
                                PhoneNumber = servant.PhoneNumber ?? "",
                                Roles = await servantRepository.GetRolesAsync(servant),
                            });
                        }
                    }
                    DayDto dayDto = new DayDto
                    {
                        Id = day.Id,
                        Name = day.Name,
                        ScheduledDate = day.ScheduledDate,
                        CreatedDate = day.CreatedDate,
                        CreatedBy = day.CreatedBy != null ? day.CreatedBy.FullName : "",
                        CreatedByServantId = day.CreatedByServantId,
                        DayServantsCount = day.DayServants.Count,
                        DayLeaders = dayleaders,
                        BatchId = day.BatchId,
                        BatchName = day.Batch != null ? day.Batch.Name : "",
                        Status = day.Status
                    };
                    dayDtos.Add(dayDto);
                }
                return dayDtos;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving all days.");
                throw;
            }
        }

        public async Task<DayDto?> DeleteAsync(Guid id)
        {
            try
            {
                var deletedDay = await dayRepository.DeleteAsync(id);
                if (deletedDay == null)
                    throw new NotFoundException($"No day found with ID {id}");

                DayDto dayDto = new DayDto
                {
                    Id = deletedDay.Id,
                    ScheduledDate = deletedDay.ScheduledDate,
                    CreatedDate = deletedDay.CreatedDate,
                    CreatedByServantId = deletedDay.CreatedByServantId,
                    DayServantsCount = deletedDay.DayServants.Count,
                    BatchName = deletedDay.Batch != null ? deletedDay.Batch.Name : "",
                    Status = deletedDay.Status
                };

                return dayDto;

            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx, "Day with ID {DayId} not found for deletion.", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting day with ID {DayId}.", id);
                throw;
            }
        }

        public async Task<DayDto?> UpdateAsync(Guid id, DayUpdateRequestDto dayUpdateRequest)
        {
            try
            {
                var updatedDay = await dayRepository.GetByIdAsync(id);

                if (updatedDay == null)
                    throw new NotFoundException($"No day found with IDay with ID {id} not found for update.");

                if (!string.IsNullOrWhiteSpace(dayUpdateRequest.Name))
                    updatedDay.Name = dayUpdateRequest.Name;
                if (dayUpdateRequest.ScheduledDate != DateOnly.MinValue)
                    updatedDay.ScheduledDate = dayUpdateRequest.ScheduledDate;
                if (dayUpdateRequest.BatchId != Guid.Empty)
                    updatedDay.BatchId = dayUpdateRequest.BatchId;

                // TODO: Fix unprovided status should not be updated issue
                updatedDay.Status = dayUpdateRequest.Status;

                Batch? batch = await batchRepository.GetByIdAsync(updatedDay.BatchId);

                if (batch == null)
                    throw new NotFoundException($"No batch found with ID {dayUpdateRequest.BatchId}");

                updatedDay = await dayRepository.UpdateAsync(id, updatedDay);

                if (updatedDay == null)
                    throw new Exception("Failed to get updated day");

                DayDto dayDto = new DayDto
                {
                    Id = updatedDay.Id,
                    Name = updatedDay.Name,
                    ScheduledDate = updatedDay.ScheduledDate,
                    CreatedDate = updatedDay.CreatedDate,
                    CreatedBy = updatedDay.Batch != null ? updatedDay.CreatedBy.FullName : "",
                    CreatedByServantId = updatedDay.CreatedByServantId,
                    DayServantsCount = updatedDay.DayServants.Count,
                    BatchId = updatedDay.BatchId,
                    BatchName = updatedDay.Batch != null ? updatedDay.Batch.Name : "",
                    Status = updatedDay.Status
                };
                return dayDto;

            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating day with ID {DayId}.", id);
                throw;

            }
        }
    }
}
