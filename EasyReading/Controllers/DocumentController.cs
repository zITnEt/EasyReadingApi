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
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMediator _mediator;

        public DocumentController(ILogger<DocumentController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpDelete]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Delete(DeleteDocumentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Create(CreateDocumentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}