using EasyReading.Application.Abstractions;
using EasyReading.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class CreatePageCommand : ICommand<int>
    {
        public Document? Document { get; set; }
        public string? Body { get; set; }
        public int PageNum { get; set; }
    }

    public class CreatePageCommandHandler : ICommandHandler<CreatePageCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreatePageCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IMediator mediator)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreatePageCommand command, CancellationToken cancellationToken)
        {
            CreateEmbeddingCommand request = new CreateEmbeddingCommand
            {
                Body = command.Body,
            };
            
            Page page = new Page
            {
                DocumentId = command.Document!.Id,
                Body = command.Body,
                Embedding = await _mediator.Send(request),
                PageNum = command.PageNum,
            };

            if (!await _dbContext.Pages.AnyAsync(x => x.PageNum == command.PageNum && x.DocumentId == command.Document!.Id))
            {
                await _dbContext.Pages.AddAsync(page);
                command.Document.LastProcessedPage = page.PageNum;
                await _dbContext.SaveChangesAsync();
            }

            return page.PageNum;
        }
    }
}