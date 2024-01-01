using EasyReading.Application.Abstractions;
using EasyReading.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class CreateDocumentCommand: ICommand<int>
    {
        public string? Title { get; set; }
        public int PagesCount { get; set; }
    }

    public class CreateDocumentCommandHandler : ICommandHandler<CreateDocumentCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public CreateDocumentCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateDocumentCommand command, CancellationToken cancellationToken)
        {
            Document? document = await _dbContext.Documents.FirstOrDefaultAsync(x=>x.UserId == _currentUserService.UserId && x.Title == command.Title && x.PagesCount == command.PagesCount);
            
            if (document is null)
            {
                document = new Document
                {
                    UserId = _currentUserService.UserId,
                    Title = command.Title,
                    PagesCount = command.PagesCount,
                    LastProcessedPage = 0,
                };
                _dbContext.Documents.Add(document);
                await _dbContext.SaveChangesAsync();
            }

            return document.Id;
        }
    }
}