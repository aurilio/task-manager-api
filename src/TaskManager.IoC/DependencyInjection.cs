﻿using FluentValidation;
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
using MediatR;
using StackExchange.Redis;
using Serilog;

namespace TaskManager.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEntryPoint).Assembly));
                
        services.AddDbContext<TaskDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions => sqlOptions.EnableRetryOnFailure()));

        var redisConnectionString = configuration.GetConnectionString("RedisConnection");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnectionString);
            configuration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                Port = AmqpTcpEndpoint.UseDefaultPort // ou especifique 5672 se estiver diferente
            };

            try
            {
                var connection = factory.CreateConnection();
                Console.WriteLine("Conexão com RabbitMQ estabelecida com sucesso!");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar com RabbitMQ: {ex.Message}");
                throw;
            }
        });

        services.AddSingleton<IElasticClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var uri = configuration["ElasticSearch:Uri"];
            var defaultIndex = configuration["ElasticSearch:DefaultIndex"];

            var settings = new ConnectionSettings(new Uri(uri))
                .DefaultIndex(defaultIndex)
                .RequestTimeout(TimeSpan.FromSeconds(10))
                .EnableDebugMode()
                .PrettyJson();

            var client = new ElasticClient(settings);

            var pingResponse = client.Ping();

            if (!pingResponse.IsValid)
            {
                throw new Exception($"Erro ao conectar com Elasticsearch: {pingResponse.OriginalException.Message}");
            }

            return client;
        });

        services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<GetTaskRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateTaskRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<DeleteTaskRequestValidator>();

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddSingleton<IMessageBus, MessageBus>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IElasticSearchRepository, ElasticSearchRepository>();
        services.AddSingleton(Log.Logger);

        return services;
    }
}