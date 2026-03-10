using BiSaji.API.Exceptions;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Movementgroup;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MovementGroupController : ControllerBase
    {
        private readonly MovementGroupService movementGroupService;

        public MovementGroupController(MovementGroupService movementGroupService)
        {
            this.movementGroupService = movementGroupService;
        }

        // GET: api/MovementGroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovementGroupDto>>> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                return Ok(await movementGroupService.GetAllAsync(filterOn, filterQuery));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/MovementGroup/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MovementGroupDto>> GetById([FromRoute] Guid id)
        {
            try
            {
                var movementGroup = await movementGroupService.GetByIdAsync(id);
                return Ok(movementGroup);
            }
            catch (NotFoundException nfEx)
            {
                return NotFound(nfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/MovementGroup
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MovementGroupDto>> Create([FromBody] CreateMovementGroupRequestDto createMovementGroupRequestDto)
        {
            try
            {
                var movementGroup = await movementGroupService.CreateAsync(createMovementGroupRequestDto);
                return CreatedAtAction(nameof(GetById), new { id = movementGroup.Id }, movementGroup);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // PUT: api/MovementGroup/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MovementGroupDto>> Update([FromRoute] Guid id, [FromBody] UpdateMovementGroupRequestDto updateMovementGroupRequestDto)
        {
            try
            {
                var movementGroup = await movementGroupService.UpdateAsync(id, updateMovementGroupRequestDto);
                return Ok(movementGroup);
            }
            catch (NotFoundException nfEx)
            {
                return NotFound(nfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/MovementGroup/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await movementGroupService.DeleteAsync(id);
                return Ok($"Movement group with ID {id} has been deleted");
            }
            catch (NotFoundException nfEx)
            {
                return NotFound(nfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
