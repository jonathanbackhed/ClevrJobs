using Data;
using Microsoft.EntityFrameworkCore;
using ScrapeWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
