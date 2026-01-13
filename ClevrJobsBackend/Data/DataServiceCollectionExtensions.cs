using Data.Repositories;
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

            return services;
        }
    }
}
