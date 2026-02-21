using System.Security.Claims;
using Api.Data;
using Api.Services;
using Asp.Versioning;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Starting API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddScoped<SavedJobsService>();
    builder.Services.AddScoped<TrackedJobService>();

    builder.Services.AddData(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    });

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

        options.AddPolicy("reportByIp", context =>
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var endpoint = context.GetEndpoint();
            var routeValues = context.GetRouteData()?.Values;
            var jobId = routeValues?["jobId"]?.ToString() ?? "no-job";

            var partitionKey = $"{ipAddress}:{jobId}";

            return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new()
            {
                PermitLimit = 2,
                Window = TimeSpan.FromHours(24),
                SegmentsPerWindow = 24
            });
        });

        options.AddPolicy("saveJobByUser", context =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? context.Connection.RemoteIpAddress?.ToString()
                         ?? "unknown";
            var jobId = context.GetRouteData()?.Values["jobId"]?.ToString() ?? "no-job";

            return RateLimitPartition.GetSlidingWindowLimiter($"{userId}:{jobId}", _ => new()
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(10),
                SegmentsPerWindow = 5
            });
        });
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Clerk:Domain"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Clerk:Domain"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Clerk:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Information("Authentication failed {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();

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
                .WithOrigins(builder.Configuration["Frontend"]!)));

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

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRateLimiter();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
