using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Students;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BiSaji.API.Services
{
    public class StudentService
    {
        private readonly IStudentRepository studentRepository;
        private readonly ILogger<StudentService> logger;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            this.studentRepository = studentRepository;
            this.logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllAsync([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var studentsDMs = await studentRepository.GetAllAsync(filterOn, filterQuery);
                if (studentsDMs == null || !studentsDMs.Any())
                {
                    throw new Exception("No students found.");
                }

                logger.LogInformation($"Fetched {studentsDMs.Count()} students successfully.");
                return studentsDMs;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching students.");
                throw;
            }
        }

        public async Task<StudentDto> GetByIdAsync(Guid id)
        {
            try
            {
                var student = await studentRepository.GetByIdAsync(id);

                if (student == null)
                {
                    throw new NotFoundException($"Student with ID {id} not found.");
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
                return studentDto;
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx, $"Student with ID {id} not found.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while fetching student with ID {id}.");
                throw;
            }
        }

        public async Task<StudentDto> UpdateAsync(Guid id, StudentUpdateRequestDto studentUpdateRequestDto)
        {
            try
            {
                var student = await studentRepository.UpdateAsync(id, studentUpdateRequestDto);

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
                return studentDto;
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, $"Student with ID {id} not found for update.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while updating student with ID {id}.");
                throw;
            }
        }

        public async Task<StudentDto> CreateAsync(StudentRegiesterRequestDto studentRegiesterRequestDto)
        {
            try
            {
                var createdStudent = await studentRepository.CreateAsync(studentRegiesterRequestDto);

                // mapping
                StudentDto studentDto = new StudentDto
                {
                    Id = createdStudent.Id,
                    FullName = createdStudent.FullName,
                    DateOfBirth = createdStudent.DateOfBirth,
                    PhoneNumber = createdStudent.PhoneNumber!,
                    ParentPhoneNumber = createdStudent.ParentPhoneNumber!,
                    AdditionalParentPhoneNumber = createdStudent.AdditionalParentPhoneNumber!,
                    BatchId = createdStudent.BatchId ?? Guid.Empty
                };


                logger.LogInformation($"Student \"{studentDto.FullName}\" registered successfully with ID {studentDto.Id}.");
                return studentDto;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Invalid input data for student registration.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while registering a new student.");
                throw;
            }
        }

        public async Task<StudentDto> DeleteAsync(Guid id)
        {
            try
            {
                var student = await studentRepository.DeleteAsync(id);

                if (student == null)
                {
                    throw new NotFoundException($"Student with ID {id} not found for deletion.");
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
                return studentDto;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, $"Invalid ID {id} provided for student deletion.");
                throw;
            }
            catch (NotFoundException nfEx)
            {
                logger.LogWarning(nfEx, $"Student with ID {id} not found for deletion.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while deleting student with ID {id}.");
                throw;
            }
        }
    }
}
