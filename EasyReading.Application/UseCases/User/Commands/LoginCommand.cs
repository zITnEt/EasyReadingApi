using System.Security.Claims;
using EasyReading.Application.Abstractions;
using EasyReading.Application.Exceptions;
using EasyReading.Domain.Enums;
using EasyReading.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstalmentSystem.Application.UseCases.Auth.Commands
{
    public class LoginCommand : IRequest<string>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHashService _hashService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IApplicationDbContext context, IHashService hashService, ITokenService tokenService)
        {
            _context = context;
            _hashService = hashService;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

            if (user == null)
            {
                throw new LoginException(new EntityNotFoundException(nameof(user)));
            }

            if (user.PasswordHash != _hashService.GetHash(request.Password!))
            {
                throw new LoginException();
            }

            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.Name!),
                new (ClaimTypes.Email, user.Email!),
                new (ClaimTypes.Role, ((Role)(user.Role)).ToString())
            };

            return _tokenService.GetAccessToken(claims);
        }
    }
}
