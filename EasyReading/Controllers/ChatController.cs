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
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IMediator _mediator;

        public ChatController(ILogger<ChatController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetResponse(GetResponseQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}