using Data;
using Microsoft.EntityFrameworkCore;
using Queue.Services;
using Serilog;
using Workers;
using Workers.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddHostedService<ScrapeWorker>();
builder.Services.AddHostedService<ProcessingWorker>();
builder.Services.AddHostedService<RetryWorker>();

builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddSingleton<IProcessService, ProcessService>();
builder.Services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
