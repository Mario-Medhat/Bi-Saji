using System.ComponentModel.DataAnnotations;

namespace BiSaji.API.Models.Dto.Servant
{
    public class ServantInfoDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
