using EasyReading.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InstalmentSystem.Infrastructure.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        public int UserId { get; set; }

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            var claims = contextAccessor.HttpContext?.User.Claims;

            var idClaim = claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (idClaim != null && int.TryParse(idClaim.Value, out var value))
            {
                UserId = value;
            }
            else
            {
                UserId = 4;
            }
        }
    }
}
