using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Features.Applications.Commands;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EA.UsageTracking.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreTenant]
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
        public async Task<IActionResult> OnBoard(string name) =>
            (await _mediator.Send(new AddApplicationCommand { ApplicationDto = new ApplicationDTO{Name = name} }))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

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