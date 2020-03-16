using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;

namespace EA.UsageTracking.Infrastructure.Features.Applications.Commands
{
    public class AddApplicationCommand: IRequest<Result<ApplicationDTO>>
    {
        public ApplicationDTO ApplicationDto { get; set; }
    }

    public class AddApplicationCommandHandler : IRequestHandler<AddApplicationCommand, Result<ApplicationDTO>>
    {
        private readonly IMapper _mapper;
        private readonly UsageTrackingContext _usageTrackingContext;

        public AddApplicationCommandHandler(UsageTrackingContext usageTrackingContext, IMapper mapper)
        {
            _usageTrackingContext = usageTrackingContext;
            _mapper = mapper;
            _usageTrackingContext.TenantId = Guid.NewGuid();
        }

        public async Task<Result<ApplicationDTO>> Handle(AddApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = _mapper.Map<Application>(request.ApplicationDto);
            _usageTrackingContext.Applications.Add(application);
            await _usageTrackingContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(_mapper.Map<ApplicationDTO>(application));
        }
    }
}
