using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Domain.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int LastProcessedPage { get; set; }
        public int PagesCount { get; set; }
        public int UserId {  get; set; }
        public User? User { get; set; }
        public List<Page> Pages { get; } = new List<Page>();
    }
}
