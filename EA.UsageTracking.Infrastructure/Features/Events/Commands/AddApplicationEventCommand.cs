using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Events.Commands
{
    public class AddApplicationEventCommand: IRequest<Result<ApplicationEventDTO>>
    {
        public ApplicationEventDTO ApplicationEventDto { get; set; }
    }

    public class AddApplicationEventCommandHandler : IRequestHandler<AddApplicationEventCommand, Result<ApplicationEventDTO>>
    {
        private readonly UsageTrackingContext _dbContext;
        private readonly IMapper _mapper;

        public AddApplicationEventCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            _dbContext = usageTrackingContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        public async Task<Result<ApplicationEventDTO>> Handle(AddApplicationEventCommand request, CancellationToken cancellationToken)
        {
            var applicationResult = _dbContext.Applications.SingleOrDefault().ToMaybe().ToResult(Constants.ErrorMessages.NoTenantExists);
            if (applicationResult.IsFailure) 
                return Result.Fail<ApplicationEventDTO>(applicationResult.Error);

            var applicationEvent = _mapper.Map<ApplicationEvent>(request.ApplicationEventDto);
            applicationEvent.Application = applicationResult.Value;

            _dbContext.ApplicationEvents.Add(applicationEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ApplicationEventDTO>(applicationEvent));
        }
    }
}
