using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EA.UsageTracking.Application.API.Attributes;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.Infrastructure.Features.Usages.Queries;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UsageItemDTO item) =>
            (await _mediator.Send(new AddUsageItemCommand{UsageItemDTO = item}))
                .OnBoth(r => r.IsSuccess ? (IActionResult) Ok(r.Value): BadRequest(r.Error));
        
        [HttpPost("seed")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Seed()
        {
            SeedData.PopulateTestData(_usageTrackingContext);
            return Ok();
        }

    }
}