using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Batch;
using System.Security.Claims;

namespace BiSaji.API.Services
{
    public class BatchService
    {
        private readonly IBatchRepository batchRepository;
        private readonly ILogger<BatchService> logger;
        private readonly IServantRepository servantRepository;

        public BatchService(IBatchRepository batchRepository, ILogger<BatchService> logger, IServantRepository servantRepository)
        {
            this.batchRepository = batchRepository;
            this.logger = logger;
            this.servantRepository = servantRepository;
        }


        public async Task<BatchDto> CreateAsync(BatchCreateRequestDto batchCreationDto, string createdByServantId)
        {
            try
            {
                Batch batch = new Batch
                {
                    Name = batchCreationDto.Name,
                    CreatedByServantId = createdByServantId,
                    LeaderId = batchCreationDto.LeaderId,
                };

                batch = await batchRepository.CreateAsync(batch);

                BatchDto batchDto = new BatchDto
                {
                    Id = batch.Id,
                    Name = batch.Name,
                    CreatedDate = batch.CreatedDate,
                    CreatedByServantId = batch.CreatedByServantId,
                    CreatedBy = batch.CreatedBy?.FullName,
                    LeaderId = batch.LeaderId,
                    Leader = batch.Leader?.FullName,
                };

                logger.LogInformation($"Batch {batch.Name} with ID {batch.Id} created successfully.");
                return batchDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a batch.");
                throw;
            }
        }

        public async Task<IEnumerable<BatchDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                IEnumerable<Batch> batches = await batchRepository.GetAllAsync(filterOn, filterQuery);

                logger.LogInformation($"Retrieved {batches.Count()} batches from the repository.");

                IEnumerable<BatchDto> batchDtos = batches.Select(batch => new BatchDto
                {
                    Id = batch.Id,
                    Name = batch.Name,
                    CreatedDate = batch.CreatedDate,
                    CreatedByServantId = batch.CreatedByServantId,
                    CreatedBy = batch.CreatedBy?.FullName,
                    LeaderId = batch.LeaderId,
                    Leader = batch.Leader?.FullName,
                });

                return batchDtos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving batches.");
                throw;
            }
        }

        public async Task<BatchDto> GetByIdAsync(Guid id)
        {
            try
            {
                Batch? batch = await batchRepository.GetByIdAsync(id);

                if (batch == null)
                    throw new NotFoundException($"Batch with ID {id} not found.");

                logger.LogInformation($"Batch {batch.Name} with ID {id} retrieved successfully.");

                BatchDto batchDto = new BatchDto
                {
                    Id = batch.Id,
                    Name = batch.Name,
                    CreatedDate = batch.CreatedDate,
                    CreatedByServantId = batch.CreatedByServantId,
                    CreatedBy = batch.CreatedBy?.FullName,
                    LeaderId = batch.LeaderId,
                    Leader = batch.Leader?.FullName,
                };

                return batchDto;
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while retrieving batch with ID {id}.");
                throw;
            }
        }

        public async Task<BatchDto> UpdateAsync(Guid id, BatchUpdateRequestDto batchUpdateRequestDto)
        {
            try
            {
                // Leader Validations
                NotFoundException nfEx = new NotFoundException($"Leader with ID {batchUpdateRequestDto.LeaderId} not found.");
                var isIdGuid = Guid.TryParse(batchUpdateRequestDto.LeaderId, out var existingLeaderId);

                // If the LeaderId is not a valid GUID, we can immediately throw the NotFoundException without querying the database.
                if (!isIdGuid)
                    throw nfEx;
                
                var existingLeader = await servantRepository.GetByIdAsync(Guid.Parse(batchUpdateRequestDto.LeaderId));
                var doesLeaderExist = existingLeader != null;

                // If the LeaderId is a valid GUID but does not correspond to an existing servant, we throw the NotFoundException.
                if (!doesLeaderExist)
                    throw nfEx;

                Batch updatedBatch = new Batch
                {
                    Name = batchUpdateRequestDto.Name,
                    LeaderId = batchUpdateRequestDto.LeaderId,
                };

                Batch? existingBatch = await batchRepository.UpdateAsync(id, updatedBatch);

                if (existingBatch == null)
                    throw new NotFoundException($"Batch with ID {id} not found for update.");

                logger.LogInformation($"Batch {updatedBatch.Name} with ID {id} updated successfully.");

                BatchDto batchDto = new BatchDto
                {
                    Id = existingBatch.Id,
                    Name = existingBatch.Name,
                    CreatedDate = existingBatch.CreatedDate,
                    CreatedByServantId = existingBatch.CreatedByServantId,
                    CreatedBy = existingBatch.CreatedBy?.FullName,
                    LeaderId = existingBatch.LeaderId,
                    Leader = existingBatch.Leader?.FullName,
                };

                return batchDto;
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while updating batch with ID {id}.");
                throw;
            }
        }

        public async Task<BatchDto> DeleteAsync(Guid id)
        {
            try
            {
                Batch? deletedBatch = await batchRepository.DeleteAsync(id);

                if (deletedBatch == null)
                    throw new NotFoundException($"Batch with ID {id} not found for deletion.");

                logger.LogInformation($"Batch {deletedBatch.Name} with ID {id} deleted successfully.");

                BatchDto batchDto = new BatchDto
                {
                    Id = deletedBatch.Id,
                    Name = deletedBatch.Name,
                    CreatedDate = deletedBatch.CreatedDate,
                    CreatedByServantId = deletedBatch.CreatedByServantId,
                    CreatedBy = deletedBatch.CreatedBy?.FullName,
                    LeaderId = deletedBatch.LeaderId,
                    Leader = deletedBatch.Leader?.FullName,
                };

                return batchDto;
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while deleting batch with ID {id}.");
                throw;
            }
        }
    }
}
