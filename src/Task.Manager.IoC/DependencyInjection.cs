using Microsoft.Extensions.DependencyInjection;
using Task.Manager.Domain;
using FluentValidation;
using Task.Manager.Shareable.Validators;
using Task.Manager.Domain.Repositories;
using Task.Manager.Data.Repositories;

namespace Task.Manager.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IServiceCollection serviceCollection)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEntryPoint).Assembly));

            serviceCollection.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
            serviceCollection.AddValidatorsFromAssemblyContaining<GetTaskRequestValidator>();
            serviceCollection.AddValidatorsFromAssemblyContaining<UpdateTaskRequestValidator>();
            serviceCollection.AddValidatorsFromAssemblyContaining<DeleteTaskRequestValidator>();

            services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
}