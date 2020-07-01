using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.Application.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = Constants.Policy.UsageApp)]
    public class ApplicationEventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] GetEventsForApplicationQuery getEventsForApplicationQuery) =>
            (await _mediator.Send(getEventsForApplicationQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet("Details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery, Required] GetEventDetailsForApplicationQuery getEventDetailsQuery) =>
            (await _mediator.Send(getEventDetailsQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));

        [HttpPost]
        [IgnoreParameter(ParameterToIgnore = "Id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] ApplicationEventDTO eventDTO) =>
            (await _mediator.Send(new AddApplicationEventCommand { ApplicationEventDto = eventDTO }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] ApplicationEventDTO eventDTO) =>
            (await _mediator.Send(new UpdateApplicationEventCommand() { ApplicationEventDto = eventDTO }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id) =>
            (await _mediator.Send(new DeleteApplicationEventCommand() { Id = id }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok() : BadRequest(r.Error));

    }
}