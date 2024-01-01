namespace EasyReading.Domain.Entities
{
    public class Page
    {
        public long Id { get; set; }
        public int DocumentId { get; set; }
        public string? Body { get; set; }
        public int PageNum {  get; set; }
        public double[]? Embedding { get; set; }
        public Document? Document { get; set; }
    }
}