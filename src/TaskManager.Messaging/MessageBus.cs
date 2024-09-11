using Polly;
using RabbitMQ.Client;
using System.Text;
using TaskManager.Domain.Interfaces;
using Serilog;

namespace TaskManager.Messaging;

public class MessageBus : IMessageBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger _logger;

    public MessageBus(IConnection connection, ILogger logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _logger = logger;
    }

    public void Publish(string queueName, string message)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3, (exception, retryCount) =>
            {
                _logger.Warning($"Tentativa {retryCount} falhou. Retentando...");
            });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(30),
            onBreak: (ex, timespan) =>
            {
                _logger.Warning("Circuito aberto devido a falhas repetidas. Circuito ficará aberto por 30 segundos.");
            },
            onReset: () =>
            {
                _logger.Information("Circuito fechado. Recuperação completa.");
            });

        var policyWrap = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

        policyWrap.Execute(() =>
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);

            _logger.Information("Mensagem publicada com sucesso na fila {QueueName}", queueName);
        });
    }
}