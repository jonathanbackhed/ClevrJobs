using Data;
using Microsoft.EntityFrameworkCore;
using ScrapeWorker;
using ScrapeWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IScraperService, ScraperService>();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
