using EasyReading.Application.Abstractions;
using EasyReading.Application.DTOs;
using Supabase;
using System.Text.Json;

namespace EasyReading.Application.UseCases.User.Queries
{
    public class GetPagesQuery: IQuery<List<PageDTO>>
    {
        public int DocumentId { get; set; }
        public double[]? Embedding {  get; set; }
    }

    public class GetPagesQueryHandle : IQueryHandler<GetPagesQuery, List<PageDTO>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISimilarityEmbeddingService _similarityService;
        private readonly Client _supabase;

        public GetPagesQueryHandle(IApplicationDbContext dbContext, ICurrentUserService currentUserService, ISimilarityEmbeddingService similarityService, Client supabase)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _similarityService = similarityService;
            _supabase = supabase;
        }

        public async Task<List<PageDTO>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
        {
            await _supabase.InitializeAsync();  
            if (_dbContext.Documents.Any(x => x.Id == request.DocumentId && x.UserId == _currentUserService.UserId))
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict["query_embedding"] = request.Embedding!;
                dict["match_threshold"] = 0.8;
                dict["match_count"] = 5;
                dict["documentid"] = request.DocumentId;
                var response = await _supabase.Rpc("match_pages", dict);
                return JsonSerializer.Deserialize<List<PageDTO>>(response.Content ?? "") ?? new List<PageDTO>();
            }

            return new List<PageDTO>();
        }

    }
}
