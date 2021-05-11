using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceProduct
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private CancellationTokenSource _cts;
        private Task _executingTask;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Worker service started.");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return Task.CompletedTask;
            }

            _logger.LogWarning("Worker service stopping.");

            _cts.Cancel();

            Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(true);

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogWarning("Worker service stopped.");

            return Task.CompletedTask;
        }






    }
}
