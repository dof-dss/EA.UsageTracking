﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Commands;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Queries;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.UsageTracking.Application.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApplicationUsageController : BaseApiController
    {
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMediator _mediator;

        public ApplicationUsageController(IUsageTrackingContextFactory usageTrackingContextFactory, IMediator mediator)
        {
            _usageTrackingContext = usageTrackingContextFactory.UsageTrackingContext;
            _mediator = mediator;
        }

        [HttpGet]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageApp)]
        public async Task<IActionResult> GetAllForApplication([FromQuery] GetUsagesForApplicationQuery getUsagesForApplicationQuery) =>
            (await _mediator.Send(getUsagesForApplicationQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet("Total")]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCountForApplication() =>
            (await _mediator.Send(new GetTotalForApplicationQuery())).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet("Details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery, Required] GetUsageDetailsQuery getUsageDetailsQuery) => 
            (await _mediator.Send(getUsageDetailsQuery))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));

        [HttpGet("User")]
        [IgnoreParameter(ParameterToIgnore = "ApiRoute")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUser([FromQuery] GetUsagesForUserQuery getUsagesForUserQuery) =>
            (await _mediator.Send(getUsagesForUserQuery))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy.UsageApp)]
        public async Task<IActionResult> Post([FromBody] AddUsageItemPublisherCommand publisherCommand) =>
            (await _mediator.Send(publisherCommand))
                .OnBoth(r => r.IsSuccess ? (IActionResult)StatusCode(201) : BadRequest(r.Error));

        [HttpPost("seed")] 
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Seed()
        {
            new SeedData(_usageTrackingContext).PopulateTestData();
            return Ok();
        }

    }
}