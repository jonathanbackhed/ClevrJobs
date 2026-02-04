using Api.Data;
using Data;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddData(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Rate limit exceeded",
            retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) 
                ? (double?)retryAfter.TotalSeconds 
                : null
        }, cancellationToken);
    };

    options.AddSlidingWindowLimiter("reportLimiter", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromHours(1);
        opt.SegmentsPerWindow = 12;
    }).AddPolicy("reportByIp", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetSlidingWindowLimiter(ipAddress, _ => new()
        {
            PermitLimit = 5,
            Window = TimeSpan.FromHours(1),
            SegmentsPerWindow = 12
        });
    });
});

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000;
    options.CompactionPercentage = 0.10;
});
builder.Services.AddSingleton<IJobCache, JobCache>();

if (builder.Configuration["Frontend"] is null)
    throw new Exception("Frontend url is null");

builder.Services.AddCors(o =>
    o.AddPolicy("CorsPolicy", b =>
        b.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(builder.Configuration["Frontend"])));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
