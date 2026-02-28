using BiSaji.API.Data;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Students;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLStudentsRepository : IStudentsRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLStudentsRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Student> CreateAsync(StudentRegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                // mapping
                Student student = new Student
                {
                    FullName = regiesterRequestDto.FullName,
                    PhoneNumber = regiesterRequestDto.PhoneNumber,
                    ParentPhoneNumber = regiesterRequestDto.ParentPhoneNumber,
                    AdditionalParentPhoneNumber = regiesterRequestDto.ParentPhoneNumber,
                    DateOfBirth = regiesterRequestDto.DateOfBirth,
                    BatchId = regiesterRequestDto.BatchId,
                };

                await dbContext.Students.AddAsync(student);
                await dbContext.SaveChangesAsync();

                return student;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Student?> DeleteAsync(Guid id)
        {
            // mapping
            Student? student = await GetByIdAsync(id);

            if (student == null)
            {
                throw new NotFoundException($"Student with id: {id} not found");
            }

            dbContext.Students.Remove(student);
            await dbContext.SaveChangesAsync();

            return student;
        }

        public async Task<IEnumerable<Student>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                return await dbContext.Students.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            Student? student = await dbContext.Students.FindAsync(id);
            return student;
        }

        public async Task<Student> UpdateAsync(Guid id, StudentUpdateRequestDto updateRequestDto)
        {
            Student? student = await dbContext.Students.FindAsync(id);

            if (student == null)
            {
                throw new NotFoundException($"Student with id: {id} not found");
            }

            student.FullName = updateRequestDto.FullName ?? student.FullName;
            student.PhoneNumber = updateRequestDto.PhoneNumber ?? student.PhoneNumber;
            student.ParentPhoneNumber = updateRequestDto.ParentPhoneNumber ?? student.ParentPhoneNumber;
            student.AdditionalParentPhoneNumber = updateRequestDto.AdditionalParentPhoneNumber ?? student.AdditionalParentPhoneNumber;
            student.BatchId = updateRequestDto.BatchId ?? student.BatchId;
            if (updateRequestDto.DateOfBirth.HasValue)
                student.DateOfBirth = updateRequestDto.DateOfBirth.Value;

            await dbContext.SaveChangesAsync();

            return student;
        }
    }
}
