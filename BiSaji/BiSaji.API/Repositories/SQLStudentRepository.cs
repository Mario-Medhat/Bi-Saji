using BiSaji.API.Data;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Student;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    // TODO: Use DMs instead of DTOs 
    public class SQLStudentRepository : IStudentRepository
    {
        private readonly BiSajiDbContext dbContext;

        public SQLStudentRepository(BiSajiDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            try
            {
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

        public async Task<Student?> UpdateAsync(Guid id, Student updatedStudent)
        {
            Student? student = await dbContext.Students.FindAsync(id);

            if (student == null)
                return null;
            

            student.FullName = updatedStudent.FullName ?? student.FullName;
            student.PhoneNumber = updatedStudent.PhoneNumber ?? student.PhoneNumber;
            student.ParentPhoneNumber = updatedStudent.ParentPhoneNumber ?? student.ParentPhoneNumber;
            student.AdditionalParentPhoneNumber = updatedStudent.AdditionalParentPhoneNumber ?? student.AdditionalParentPhoneNumber;
            student.BatchId = updatedStudent.BatchId ?? student.BatchId;
            
            if (updatedStudent.DateOfBirth != DateOnly.MinValue)
                student.DateOfBirth = updatedStudent.DateOfBirth;

            await dbContext.SaveChangesAsync();

            return student;
        }
    }
}
