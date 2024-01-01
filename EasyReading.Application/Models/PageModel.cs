using Postgrest.Attributes;
using Postgrest.Models;

namespace EasyReading.Application.Models
{
    [Table("Pages")]
    public class PageModel: BaseModel
    {
        [PrimaryKey("Id", false)]
        public long Id { get; set; }
        [Column("DocumentId")]
        public int DocumentId { get; set; }
        [Column("Body")]
        public string? Body { get; set; }
        [Column("Embedding")]
        public double[]? Embedding { get; set; }
    }
}