using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.UsageTracking.Subscriber
{
    public class UsageSubscriberWorker : BackgroundService
    {
        private readonly ILogger<UsageSubscriberWorker> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly  IMediator _mediator;

        public UsageSubscriberWorker(ILogger<UsageSubscriberWorker> logger, IConnectionMultiplexer connectionMultiplexer, IMediator mediator)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redisChannel = _connectionMultiplexer.GetSubscriber().Subscribe("Usage");
            redisChannel.OnMessage(async message =>
            {
                _logger.LogInformation($"{DateTime.Now:yyyyMMdd HH:mm:ss}<{message.Message.ToString()}>.");

                var command = JsonConvert.DeserializeObject<AddUsageItemSubscriberCommand>(message.Message);

                var result = await _mediator.Send(command, stoppingToken);

                if (result.IsFailure) { 
                    //TODO Handle this
                }
            });
        }
    }
}
