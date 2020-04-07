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
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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

        public AddApplicationCommandHandler(IUsageTrackingContextFactory usageTrackingContextFactory, IMapper mapper)
        {
            _usageTrackingContext = usageTrackingContextFactory.UsageTrackingContext;
            _mapper = mapper;
        }

        public async Task<Result<ApplicationDTO>> Handle(AddApplicationCommand request, CancellationToken cancellationToken)
        {
            request.ApplicationDto.ClientId = _usageTrackingContext.TenantId;

            if (_usageTrackingContext.Applications.Any())
            {
                var applicationToUpdate = await _usageTrackingContext.Applications.SingleAsync(cancellationToken);
                _mapper.Map(request.ApplicationDto, applicationToUpdate);
                _mapper.Map(applicationToUpdate, request.ApplicationDto);
            }
            else
            {
                var applicationToAdd = _mapper.Map<Application>(request.ApplicationDto);
                _usageTrackingContext.Applications.Add(applicationToAdd);
                _mapper.Map(applicationToAdd, request.ApplicationDto);
            }

            await _usageTrackingContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(request.ApplicationDto);
        }
    }
}
