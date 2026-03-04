using BiSaji.API.Exceptions;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Batch;
using BiSaji.API.Models.Dto.Students;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BatchController : ControllerBase
    {
        private readonly BatchService batchService;

        public BatchController(BatchService batchService)
        {
            this.batchService = batchService;
        }

        // GET: api/Batches
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BatchDto>>> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var batchDm = await batchService.GetAllAsync(filterOn, filterQuery);
                return Ok(batchDm);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Batches/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BatchDto>?> GetById([FromRoute] Guid id)
        {
            try
            {
                var batchDto = await batchService.GetByIdAsync(id);
                return Ok(batchDto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Batches
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BatchDto>> Create([FromBody] BatchCreateRequestDto batch)
        {
            try
            {
                // Get the logged-in user's ID from the claims
                var logedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (logedInUserId == null)
                    return Unauthorized();

                var batchDto = await batchService.CreateAsync(batch, logedInUserId);
                return Ok($"Batch has been created successfully.\n\n{JsonSerializer.Serialize(batchDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Batches/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BatchDto?>> Update([FromRoute] Guid id, [FromBody] BatchUpdateRequestDto batchUpdateRequestDto)
        {
            try
            {
                var batchDto = await batchService.UpdateAsync(id, batchUpdateRequestDto);
                return Ok($"Batch updated successfully! \n\n{JsonSerializer.Serialize(batchDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Batches/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BatchDto?>> Delete([FromRoute] Guid id)
        {

            try
            {
                var batchDto = await batchService.DeleteAsync(id);
                return Ok($"Batch \"{batchDto.Name}\" has been deleted successfully.");
            }
            catch (NotFoundException nfEx)
            {
                return NotFound(nfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
    }
}
