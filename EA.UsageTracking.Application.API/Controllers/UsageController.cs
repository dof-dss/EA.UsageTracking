using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;
using EA.UsageTracking.Infrastructure.Features.UsagesPerUser.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.Policy.UsageUser)]
    //[Authorize(Policy = Constants.Policy.UsageAdmin)]
    public class UsageController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsageController(IMediator mediator, IHttpContextAccessor httpContentAccessor)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] GetUsagesQuery getUsagesQuery)
        {
            return (await _mediator.Send(getUsagesQuery)).OnBoth(r =>
                r.IsSuccess ? (IActionResult) Ok(r.Value) : BadRequest(r.Error));
        }

        [HttpGet("app")]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllApp([FromQuery] GetUsagesAppQuery getUsagesAppQuery)
        {
            return (await _mediator.Send(getUsagesAppQuery)).OnBoth(r =>
                r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));
        }

        [HttpGet("apps")]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetApps([FromQuery] GetAppsQuery getAppsQuery) =>
            (await _mediator.Send(getAppsQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

    }
}