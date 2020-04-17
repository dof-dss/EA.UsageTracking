using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Data;
using EA.UsageTracking.Infrastructure.Features.Common;
using EA.UsageTracking.Infrastructure.Features.Events.Commands;
using EA.UsageTracking.Infrastructure.Features.Pagination;
using EA.UsageTracking.Infrastructure.Features.Usages.Validation;
using EA.UsageTracking.Infrastructure.Features.Users.Commands;
using EA.UsageTracking.SharedKernel;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Extensions;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace EA.UsageTracking.Infrastructure.Features.Usages.Commands
{
    public class AddUsageItemSubscriberCommand : IRequest<Result<int>>
    { 
        public string TenantId { get; set; }
        public Guid RequestId { get; set; }
        public int ApplicationEventId { get; set; }
        public string IdentityToken { get; set; }
    }

    public class AddUsageItemSubscriberCommandHandler : IRequestHandler<AddUsageItemSubscriberCommand, Result<int>>
    {
        private readonly AddUsageItemCommandValidator _validator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;
        private UsageTrackingContext _dbContext;

        public AddUsageItemSubscriberCommandHandler(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
        {
            _validator = new AddUsageItemCommandValidator();
            _serviceScopeFactory = serviceScopeFactory;
            _mediator = mediator;
        }

        public async Task<Result<int>> Handle(AddUsageItemSubscriberCommand request, CancellationToken cancellationToken)
        {
            var validationResult = Validate(request);
            if (validationResult.IsFailure) return Result.Fail<int>(validationResult.Error);

            using var scope = _serviceScopeFactory.CreateScope();
            var scopedServices = scope.ServiceProvider;
            _dbContext = scopedServices.GetRequiredService<UsageTrackingContext>();
            _dbContext.TenantId = request.TenantId;

            var applicationUserResult = await MapUser(request.IdentityToken, request.TenantId, cancellationToken);
            var applicationResult = _dbContext.Applications.AsNoTracking().SingleOrDefault().ToMaybe()
                .ToResult(Constants.ErrorMessages.NoTenantExists);
            var eventResult = _dbContext.ApplicationEvents.AsNoTracking().SingleOrDefault(e => e.Id == request.ApplicationEventId).ToMaybe()
                .ToResult(Constants.ErrorMessages.NoEventExists);

            var combinedResults = Result.Combine(applicationUserResult, applicationResult, eventResult);
            if(combinedResults.IsFailure) return Result.Fail<int>(combinedResults.Error);

            var usageItem = new UsageItem()
            {
                ApplicationId = applicationResult.Value.Id,
                ApplicationEventId = request.ApplicationEventId,
                ApplicationUserId = applicationUserResult.Value
            };

            _dbContext.UsageItems.Add(usageItem);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(usageItem.Id);
        }

        protected Result Validate(AddUsageItemSubscriberCommand request)
        {
            var validate = _validator.Validate(request);
            return !validate.IsValid ? Result.Fail(validate.ToString(",")) : Result.Ok();
        }

        private async Task<Result<Guid>> MapUser(string identityToken, string tenantId, CancellationToken cancellationToken)
        {
            var saveApplicationUserCommand = new SaveApplicationUserSubscriberCommand
            {
                TenantId = tenantId,
                IdentityToken = identityToken
            };
            var addUserResult = await _mediator.Send(saveApplicationUserCommand, cancellationToken);
            return addUserResult.IsFailure ? Result.Fail<Guid>(addUserResult.Error) : Result.Ok(addUserResult.Value.Id);
        }
    }
}
