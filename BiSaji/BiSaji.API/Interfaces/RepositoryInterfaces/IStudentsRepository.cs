using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IStudentsRepository
    {
        public Task<IEnumerable<Student>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Student?> GetByIdAsync(Guid id);
        public Task<Student> CreateAsync(StudentRegiesterRequestDto regiesterRequestDto);
        public Task<Student> UpdateAsync(Guid id, StudentUpdateRequestDto updateRequestDto);
        public Task<Student?> DeleteAsync(Guid id);

    }
}
