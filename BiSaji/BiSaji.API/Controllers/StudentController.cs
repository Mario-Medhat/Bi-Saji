using BiSaji.API.Data;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Students;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService studentService;

        public StudentController(StudentService studentService)
        {
            this.studentService = studentService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var studentsDm = await studentService.GetAllAsync(filterOn, filterQuery);
                return Ok(studentsDm);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Students/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var studentDto = await studentService.GetByIdAsync(id);
                return Ok(studentDto);
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

        // PUT: api/Students/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, StudentUpdateRequestDto studentUpdateRequestDto)
        {
            try
            {
                var studentDto = await studentService.UpdateAsync(id, studentUpdateRequestDto);
                return Ok($"Student updated successfully! \n\n{JsonSerializer.Serialize(studentDto, new JsonSerializerOptions { WriteIndented = true })}");
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

        // POST: api/Students
        [HttpPost]
        public async Task<IActionResult> Regiester(StudentRegiesterRequestDto studentRegiesterRequestDto)
        {
            try
            {
                var createdStudentDto = await studentService.CreateAsync(studentRegiesterRequestDto);
                return Ok($"Student has been created successfully.\n\n{JsonSerializer.Serialize(createdStudentDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Students/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                var studentDto = await studentService.DeleteAsync(id);
                return Ok($"Student \"{studentDto.FullName}\" has been deleted successfully.");
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
