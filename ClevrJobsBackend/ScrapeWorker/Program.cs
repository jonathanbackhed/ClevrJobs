using Data;
using Microsoft.EntityFrameworkCore;
using Queue.Services;
using Serilog;
using Serilog.Events;
using Workers;
using Workers.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddHostedService<ScrapeWorker>();
builder.Services.AddHostedService<ScrapeRetryWorker>();

builder.Services.AddHostedService<ProcessingWorker>();
builder.Services.AddHostedService<ProcessingWorker>();

builder.Services.AddHostedService<ProcessRetryWorker>();


builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddSingleton<IProcessService, ProcessService>();
builder.Services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
