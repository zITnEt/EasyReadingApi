using EasyReading.Application.Abstractions;
using EasyReading.Domain.Enums;
using System.Security.Claims;

namespace EasyReading.Application.UseCases.User.Commands
{
    public class CreateUserCommand : ICommand<int>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
    }

    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;

        public CreateUserCommandHandler(IApplicationDbContext dbContext, ITokenService tokenService, IHashService hashService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _hashService = hashService;
        }

        public async Task<int> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            if (_dbContext.Users.Any(x => x.Email == command.Email))
            {
                return -1;
            }

            EasyReading.Domain.Entities.User user = new Domain.Entities.User()
            {
                Email = command.Email,
                Name = command.Name,
                PasswordHash = _hashService.GetHash(command.Password!),
                Role = ((int)Role.User)
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();    

            return user.Id;
        }
    }
}