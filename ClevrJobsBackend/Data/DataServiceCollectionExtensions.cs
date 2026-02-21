using Data.Models;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<AppDbContext>(options);

            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<ISavedJobsRepository, SavedJobsRepository>();
            services.AddScoped<ITrackedJobRepository, TrackedJobRepository>();

            return services;
        }
    }
}
