using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using RabbitMQ.Client;
using TaskManager.Data;
using TaskManager.Data.Cache;
using TaskManager.Data.Repositories;
using TaskManager.Domain;
using TaskManager.Domain.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Messaging;
using TaskManager.Shareable.Validators;

namespace TaskManager.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TaskDbContext>(options =>
                options.UseSqlServer(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetConnectionString("RedisConnection");
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEntryPoint).Assembly));

            services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<GetTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<DeleteTaskRequestValidator>();

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddScoped<ICacheService, CacheService>();

            services.AddSingleton(sp =>
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:HostName"],
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]
                };
                return factory.CreateConnection();
            });

            services.AddSingleton<IElasticClient>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var uri = configuration["ElasticSearch:Uri"];
                var defaultIndex = configuration["ElasticSearch:DefaultIndex"];

                var settings = new ConnectionSettings(new Uri(uri))
                    .DefaultIndex(defaultIndex);

                return new ElasticClient(settings);
            });

            return services;
        }
    }
}