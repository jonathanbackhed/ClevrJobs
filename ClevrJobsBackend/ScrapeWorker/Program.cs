using Data;
using Microsoft.EntityFrameworkCore;
using Queue.Services;
using Workers;
using Workers.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ScrapeWorker>();
builder.Services.AddHostedService<ProcessingWorker>();

builder.Services.AddSingleton<IScraperService, ScraperService>();
builder.Services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
