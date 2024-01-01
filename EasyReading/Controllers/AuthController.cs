using EasyReading.Application.Abstractions;
using EasyReading.Application.DTOs;
using EasyReading.Application.UseCases.User.Commands;
using EasyReading.Application.UseCases.User.Queries;
using EasyReading.Domain.Entities;
using InstalmentSystem.Application.UseCases.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyReading.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(CreateUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}