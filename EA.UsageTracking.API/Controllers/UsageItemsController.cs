using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Core.Queries;
using EA.UsageTracking.Infrastructure.Commands;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Queries;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EA.UsageTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsageItemsController : BaseApiController
    {
        private readonly UsageTrackingContext _usageTrackingContext;
        private readonly IMediator _mediator;

        public UsageItemsController(UsageTrackingContext usageTrackingContext, IMediator mediator)
        {
            _usageTrackingContext = usageTrackingContext;
            _mediator = mediator;
        }

        [HttpGet("Application")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllForApplication([FromQuery] GetUsagesForApplicationQuery getUsagesForApplicationQuery) =>
            (await _mediator.Send(getUsagesForApplicationQuery)).OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : BadRequest(r.Error));

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromQuery] GetUsageDetailsQuery getUsageDetailsQuery) => 
            (await _mediator.Send(getUsageDetailsQuery))
            .OnBoth(r => r.IsSuccess ? (IActionResult)Ok(r.Value) : NotFound(r.Error));


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] UsageItemDTO item) =>
            (await _mediator.Send(new AddUsageItemCommand{UsageItemDTO = item}))
                .OnBoth(r => r.IsSuccess ? (IActionResult) Ok(r.Value): BadRequest(r.Error));
        
        [HttpPost("seed")]
        public IActionResult Seed()
        {
            SeedData.PopulateTestData(_usageTrackingContext);
            return Ok();
        }

    }
}