using BiSaji.API.Data;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto;
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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsRepository studentsRepository;
        private readonly ILogger logger;

        public StudentsController(IStudentsRepository studentsRepository, ILogger<StudentsController> logger)
        {
            this.studentsRepository = studentsRepository;
            this.logger = logger;
        }

        // GET: api/Students
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var studentsDm = await studentsRepository.GetAllAsync(filterOn, filterQuery);
                if (studentsDm == null || !studentsDm.Any())
                {
                    return NotFound("No students found.");
                }

                logger.LogInformation($"Fetched {studentsDm.Count()} students successfully.");
                return Ok(studentsDm);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching students.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // GET: api/Students/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = await studentsRepository.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            // mapping
            StudentDto studentDto = new StudentDto
            {
                Id = id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber!,
                ParentPhoneNumber = student.ParentPhoneNumber!,
                AdditionalParentPhoneNumber = student.AdditionalParentPhoneNumber!,
                BatchId = student.BatchId ?? Guid.Empty
            };

            logger.LogInformation($"Fetched student \"{studentDto.FullName}\" with ID {id} successfully.");
            return Ok(studentDto);
        }

        // PUT: api/Students/Update/{id}
        [HttpPut]
        [Route("Update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, StudentUpdateRequestDto studentUpdateRequestDto)
        {
            try
            {
                var student = await studentsRepository.UpdateAsync(id, studentUpdateRequestDto);

                // mapping
                StudentDto studentDto = new StudentDto
                {
                    Id = id,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    PhoneNumber = student.PhoneNumber!,
                    ParentPhoneNumber = student.ParentPhoneNumber!,
                    AdditionalParentPhoneNumber = student.AdditionalParentPhoneNumber!,
                    BatchId = student.BatchId ?? Guid.Empty
                };

                logger.LogInformation($"User {studentDto.FullName} updated successfully");
                return Ok($"User updated successfully! \n\n{JsonSerializer.Serialize(studentDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, $"Student with ID {id} not found for update.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while updating student with ID {id}.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Students
        [HttpPost]
        public async Task<IActionResult> Regiester(StudentRegiesterRequestDto studentRegiesterRequestDto)
        {
            try
            {
                var createdStudent = await studentsRepository.CreateAsync(studentRegiesterRequestDto);

                // mapping
                StudentDto studentDto = new StudentDto
                {
                    Id = createdStudent.Id,
                    FullName = studentRegiesterRequestDto.FullName,
                    DateOfBirth = studentRegiesterRequestDto.DateOfBirth,
                    PhoneNumber = studentRegiesterRequestDto.PhoneNumber!,
                    ParentPhoneNumber = studentRegiesterRequestDto.ParentPhoneNumber!,
                    AdditionalParentPhoneNumber = studentRegiesterRequestDto.AdditionalParentPhoneNumber!,
                    BatchId = studentRegiesterRequestDto.BatchId ?? Guid.Empty
                };


                logger.LogInformation($"Student \"{studentDto.FullName}\" registered successfully with ID {studentDto.Id}.");
                return Ok($"Student has been created successfully.\n\n{JsonSerializer.Serialize(studentDto, new JsonSerializerOptions { WriteIndented = true })}");
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while registering a new student.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Students/Delete/{id}
        [HttpDelete]
        [Route("Delete/{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {

            var student = await studentsRepository.DeleteAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // mapping
            StudentDto studentDto = new StudentDto
            {
                Id = id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber!,
                ParentPhoneNumber = student.ParentPhoneNumber!,
                AdditionalParentPhoneNumber = student.AdditionalParentPhoneNumber!,
                BatchId = student.BatchId ?? Guid.Empty
            };


            logger.LogInformation($"Student \"{studentDto.FullName}\" with ID {id} has been deleted successfully.");
            return Ok("Student has been deleted successfully.");
        }
    }
}
