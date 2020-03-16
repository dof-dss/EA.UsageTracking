using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.SharedKernel.Constants;
using EA.UsageTracking.SharedKernel.Functional;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EA.UsageTracking.Infrastructure.Behaviors
{
    public class ExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public ExceptionBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Result<TResponse>> next)
        {
            try
            {
                var response = await next();
                return response;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Exception at {typeof(TResponse).Name} of {typeof(TRequest).Name} at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff}");
                return Result.Fail<TResponse>(Constants.ErrorMessages.UpdateException);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception at {typeof(TResponse).Name} of {typeof(TRequest).Name} at {DateTime.UtcNow:yyyy-MM-dd hh:mm:ss.fff}");
                throw;
            }
        }
    }
}
