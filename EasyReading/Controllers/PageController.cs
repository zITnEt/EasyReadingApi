using EasyReading.Application.Abstractions;
using EasyReading.Application.DTOs;
using EasyReading.Application.UseCases.User.Commands;
using EasyReading.Application.UseCases.User.Queries;
using EasyReading.Domain.Entities;
using InstalmentSystem.Application.UseCases.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EasyReading.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly ILogger<PageController> _logger;
        private readonly IMediator _mediator;

        public PageController(ILogger<PageController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("CreatePages")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Create(CreatePagesCommand command)
        {
             return Ok(await _mediator.Send(command));
        }

        [HttpPost("GetPages")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetPages(GetPagesQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

    }
}