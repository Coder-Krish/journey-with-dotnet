using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Employees
    {
        public Guid EmployeeId { get; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string Password { get; set; }
        [NotMapped]
        public string Token { get; set; }

        public Employees()
        {

        }

    }
}

