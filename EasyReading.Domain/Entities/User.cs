using EasyReading.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Domain.Entities
{
    public class User
    {
        public User()
        {
            Documents = new List<Document>();
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public List<Document> Documents { get; set; }
        public int Role { get; set; }
    }
}
