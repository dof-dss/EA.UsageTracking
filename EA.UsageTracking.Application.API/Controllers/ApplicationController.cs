using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Applications.Commands;
using EA.UsageTracking.Infrastructure.Features.Applications.Queries;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EA.UsageTracking.Application.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IDistributedCache _cache;

        public ApplicationController(IMediator mediator, IDistributedCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpPost("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageApp)]
        public async Task<IActionResult> OnBoard(string name) =>
            (await _mediator.Send(new AddApplicationCommand { ApplicationDto = new ApplicationDTO{Name = name} }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageApp)]
        public async Task<IActionResult> Get() =>
            (await _mediator.Send(new GetApplicationQuery())).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageUser)]
        public async Task<IActionResult> GetAll() =>
            (await _mediator.Send(new GetAllApplicationsQuery())).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageUser)]
        public async Task<IActionResult> Register(RegisterCommand registerCommand) =>
            (await _mediator.Send(registerCommand)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok() : NotFound(r.Error));


        [HttpPost("PostRedis/{key}/{value}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostRedis(string key, string value)
        {
            await _cache.SetStringAsync(key, value);
            return Ok();
        }

        [HttpGet("GetRedis")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetRedis([FromQuery] string key)
        {
            var result = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(result)) return BadRequest("Nothing in cache");
            return Ok(result);
        }
    }
}