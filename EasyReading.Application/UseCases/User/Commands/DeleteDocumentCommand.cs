using EasyReading.Application.Abstractions;
using EasyReading.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Supabase;
using EasyReading.Application.Models;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class DeleteDocumentCommand : ICommand<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserServie;
        private readonly Client _supabase;


        public DeleteDocumentCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, Client supabase)
        {
            _dbContext = dbContext;
            _currentUserServie = currentUserService;
            _supabase = supabase;
        }

        public async Task<Unit> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
        {
            Document? document = await _dbContext.Documents.FirstOrDefaultAsync(x=> x.UserId == _currentUserServie.UserId && x.Id == command.Id);
            await _supabase.InitializeAsync();
            if (document is not null)
            {
                await _supabase.From<PageModel>().Where(x => x.DocumentId == command.Id).Delete();
            }

            return Unit.Value;
        }
    }
}