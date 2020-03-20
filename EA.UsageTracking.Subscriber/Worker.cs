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
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly  IMediator _mediator;

        public Worker(ILogger<Worker> logger, IConnectionMultiplexer connectionMultiplexer, IMediator mediator)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redisChannel = _connectionMultiplexer.GetSubscriber().Subscribe("Usage");
            redisChannel.OnMessage(message =>
            {
                _logger.LogInformation($"{DateTime.Now:yyyyMMdd HH:mm:ss}<{message.Message.ToString()}>.");

                var command = JsonConvert.DeserializeObject<AddUsageItemSubscriberCommand>(message.Message);

                _mediator.Send(command, stoppingToken);
            });
        }
    }
}
