using Microsoft.Extensions.DependencyInjection;
using Task.Manager.Domain;
using FluentValidation;
using Task.Manager.Shareable.Validators;
using Task.Manager.Domain.Repositories;
using Task.Manager.Data.Repositories;
using Task.Manager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using StackExchange.Redis;
using Task.Manager.Domain.Handlers;
using Task.Manager.Domain.Interfaces;
using Task.Manager.Messaging;
using Task.Manager.Data.Cache;

namespace Task.Manager.IoC
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

            // Registro dos handlers e repositórios
            //services.AddScoped<CreateTaskHandler>();
            services.AddScoped<ITaskRepository, TaskRepository>();


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

            return services;
        }
    }
}