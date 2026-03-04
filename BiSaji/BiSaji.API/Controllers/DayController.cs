using BiSaji.API.Exceptions;
using BiSaji.API.Models.Dto.Day;
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
    public class DayController : ControllerBase
    {
        private readonly DayService dayService;

        public DayController(DayService dayService)
        {
            this.dayService = dayService;
        }

        // GET: api/Day
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DayDto>>> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var daysDtos = await dayService.GetAllAsync(filterOn, filterQuery);
                return Ok(daysDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/day/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DayDto>?> GetById([FromRoute] Guid id)
        {
            try
            {
                var dayDto = await dayService.GetByIdAsync(id);
                return Ok(dayDto);
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

        // POST: api/Day
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DayDto>> Create([FromBody] DayCreateRequestDto day)
        {
            try
            {
                // Get the logged-in user's ID from the claims
                var logedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (logedInUserId == null)
                    return Unauthorized();

                var dayDto = await dayService.CreateAsync(day, logedInUserId);
                return Ok($"Day has been created successfully.\n\n{JsonSerializer.Serialize(dayDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Day/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DayDto?>> Update([FromRoute] Guid id, [FromBody] DayUpdateRequestDto dayUpdateRequestDto)
        {
            // TODO: fix name is required for Update issue
            try
            {
                var dayDto = await dayService.UpdateAsync(id, dayUpdateRequestDto);
                return Ok($"Day updated successfully! \n\n{JsonSerializer.Serialize(dayDto, new JsonSerializerOptions { WriteIndented = true })}");
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

        // DELETE: api/Day/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DayDto?>> Delete([FromRoute] Guid id)
        {

            try
            {
                var dayDto = await dayService.DeleteAsync(id);
                return Ok($"Day \"{dayDto.Name}\" has been deleted successfully.");
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
