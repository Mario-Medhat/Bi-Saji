using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Student;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
    }
}
