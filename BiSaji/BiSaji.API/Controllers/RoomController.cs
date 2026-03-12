using BiSaji.API.Exceptions;
using BiSaji.API.Models.Dto.Room;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RoomService roomService;

        public RoomController(RoomService roomService)
        {
            this.roomService = roomService;
        }

        // GET: api/Room
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var batchsDto = await roomService.GetAllAsync(filterOn, filterQuery);
                return Ok(batchsDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Batch/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>?> GetById([FromRoute] Guid id)
        {
            try
            {
                var roomDto = await roomService.GetByIdAsync(id);
                return Ok(roomDto);
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

        // POST: api/Room
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomRequestDto createRoomRequestDto)
        {
            try
            {
                var createdRoomDto = await roomService.CreateAsync(createRoomRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = createdRoomDto.Id }, createdRoomDto);
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

        // PUT: api/Room/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> Update([FromRoute] Guid id, [FromBody] UpdateRoomRequestDto updateRoomRequestDto)
        {
            try
            {
                var updatedRoomDto = await roomService.UpdateAsync(id, updateRoomRequestDto);
                return Ok(updatedRoomDto);
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

        // DELETE: api/Room/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var deletedRoom = await roomService.DeleteAsync(id);
                return Ok($"Room \"{deletedRoom.Name}\" has been deleted successfully.");
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
    }
}
