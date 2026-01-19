using System;
using System.Collections.Generic;
using System.Text;

namespace Workers
{
    public class ProcessingWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //throw new NotImplementedException();
            await Task.Delay(10000, stoppingToken);
        }
    }
}
