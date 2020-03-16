﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.UsageTracking.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsageController : BaseApiController
    {
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMediator _mediator;
        private  readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly HttpContext _httpContext;

        public ApplicationUsageController(IUsageTrackingContextFactory usageTrackingContextFactory, IMediator mediator, 
            IConnectionMultiplexer connectionMultiplexer, IHttpContextAccessor httpContextAccessor)
        {
            _usageTrackingContext = usageTrackingContextFactory.UsageTrackingContext;
            _mediator = mediator;
            _connectionMultiplexer = connectionMultiplexer;
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllForApplication([FromQuery] GetUsagesForApplicationQuery getUsagesForApplicationQuery) =>
            (await _mediator.Send(getUsagesForApplicationQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet("Details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery, Required] GetUsageDetailsQuery getUsageDetailsQuery) => 
            (await _mediator.Send(getUsageDetailsQuery))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));

        [HttpGet("User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUser([FromQuery] GetUsagesForUserQuery getUsagesForUserQuery) =>
            (await _mediator.Send(getUsagesForUserQuery))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] AddUsageItemCommand command)
        {
            command.TenantId = Guid.Parse(_httpContext.Request.Headers[Constants.Tenant.TenantId].ToString());
            var publisher = _connectionMultiplexer.GetSubscriber();
            await publisher.PublishAsync("Usage", JsonConvert.SerializeObject(command), CommandFlags.FireAndForget);

            return StatusCode(201);
        }

        [HttpPost("seed")] 
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Seed()
        {
            new SeedData(_usageTrackingContext).PopulateTestData();
            return Ok();
        }

    }
}