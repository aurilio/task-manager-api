using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.Api.Endpoints;
using TaskManager.Data;
using TaskManager.IoC;

namespace TaskManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configura o Serilog antes de construir o host
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfiguration()) // Lê configuração do appsettings.json
                .WriteTo.Console() // Adicione outros sinks conforme necessário
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Adiciona o Serilog ao builder
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddAuthorization();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddApplicationServices(builder.Configuration);
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
                    dbContext.Database.Migrate(); // Aplica as migrações pendentes
                }

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapEndpoints();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration;
        }
    }
}
