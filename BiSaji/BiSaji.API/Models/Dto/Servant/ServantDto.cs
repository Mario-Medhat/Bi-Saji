using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiSaji.API.Models.Dto.Users
{
    public class ServantDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public IEnumerable<string> Roles { get; set; }

         public Guid? BatchId { get; set; }

        public string? BatchName { get; set; }

    }
}
