using Microsoft.Extensions.DependencyInjection;
using Task.Manager.Domain;
using FluentValidation;
using Task.Manager.Shareable.Validators;

namespace Task.Manager.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IServiceCollection serviceCollection)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEntryPoint).Assembly));

            serviceCollection.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();



            // Outras injeções de serviços (como banco de dados, Redis, RabbitMQ, etc.)
            //services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
}