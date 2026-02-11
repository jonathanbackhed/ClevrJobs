using Data.Repositories;
using Queue.Messages;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Services;

namespace Workers
{
    public class ProcessRetryWorker : BackgroundService
    {
        private readonly ILogger<ProcessRetryWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProcessService _processService;

        public ProcessRetryWorker(ILogger<ProcessRetryWorker> logger, IServiceScopeFactory scopeFactory, IProcessService processService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _processService = processService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                    var processRepository = scope.ServiceProvider.GetRequiredService<IProcessRepository>();

                    _logger.LogInformation($"Scrape started at {DateTime.UtcNow}");

                    await _processService.RetryFailedProcesses(processRepository, jobRepository, stoppingToken);

                    _logger.LogInformation($"Scrape ended at {DateTime.UtcNow}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred during processing");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromHours(1));
                }
            }
        }
    }
}
